using FaturamentoService.Data;
using FaturamentoService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Controllers;

// DTO para receber os itens na criação de uma nota fiscal
public record ItemNotaRequest(int ProdutoId, int Quantidade);

// DTO para receber o corpo da requisição de criação de nota fiscal
public record CriarNotaRequest(List<ItemNotaRequest> Itens);

// DTO para enviar a quantidade ao serviço de estoque
public record DescontarEstoqueRequest(int Quantidade);

[ApiController]
[Route("notas")]
public class NotaFiscaisController(AppDbContext db, IHttpClientFactory httpClientFactory) : ControllerBase
{
    // GET /notas
    // Retorna todas as notas fiscais com seus respectivos itens
    [HttpGet]
    public async Task<IActionResult> ListarNotas()
    {
        var notas = await db.NotasFiscais
            .Include(n => n.Itens) // carrega os itens de cada nota
            .ToListAsync();

        return Ok(notas);
    }

    // GET /notas/{id}
    // Retorna uma nota fiscal específica pelo seu Id, incluindo os itens
    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarNota(int id)
    {
        var nota = await db.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);

        // Retorna 404 caso a nota não seja encontrada
        if (nota is null)
            return NotFound($"Nota fiscal com Id {id} não encontrada.");

        return Ok(nota);
    }

    // POST /notas
    // Cria uma nova nota fiscal com status "Aberta" e numeração sequencial automática.
    // Recebe uma lista de itens contendo ProdutoId e Quantidade.
    [HttpPost]
    public async Task<IActionResult> CriarNota([FromBody] CriarNotaRequest request)
    {
        // Calcula o próximo número sequencial com base no maior número existente
        var proximoNumero = await db.NotasFiscais.AnyAsync()
            ? await db.NotasFiscais.MaxAsync(n => n.Numero) + 1
            : 1;

        var nota = new NotaFiscal
        {
            Numero = proximoNumero,
            Status = "Aberta",
            DataCriacao = DateTime.UtcNow,
            // Mapeia cada item da requisição para a entidade ItemNota
            Itens = request.Itens.Select(i => new ItemNota
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList()
        };

        db.NotasFiscais.Add(nota);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarNota), new { id = nota.Id }, nota);
    }

    // POST /notas/{id}/imprimir
    // Fecha a nota fiscal alterando seu status para "Fechada".
    // Para cada item da nota, desconta a quantidade no serviço de estoque via HTTP.
    // Retorna 400 se a nota não estiver com status "Aberta".
    // Retorna 503 se o serviço de estoque estiver indisponível ou retornar erro.
    [HttpPost("{id}/imprimir")]
    public async Task<IActionResult> ImprimirNota(int id)
    {
        var nota = await db.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);

        // Retorna 404 caso a nota não seja encontrada
        if (nota is null)
            return NotFound($"Nota fiscal com Id {id} não encontrada.");

        // Somente notas com status "Aberta" podem ser fechadas/impressas
        if (nota.Status != "Aberta")
            return BadRequest($"A nota fiscal {nota.Numero} não pode ser impressa pois seu status atual é \"{nota.Status}\". Apenas notas com status \"Aberta\" podem ser impressas.");

        var httpClient = httpClientFactory.CreateClient("EstoqueService");

        // Para cada item da nota, realiza a requisição de desconto no estoque
        foreach (var item in nota.Itens)
        {
            var url = $"produtos/{item.ProdutoId}/descontar";
            var body = new DescontarEstoqueRequest(item.Quantidade);

            HttpResponseMessage resposta;

            try
            {
                // Envia POST ao EstoqueService para descontar a quantidade do produto
                resposta = await httpClient.PostAsJsonAsync(url, body);
            }
            catch (HttpRequestException ex)
            {
                // Falha de conectividade com o serviço de estoque
                return StatusCode(503, $"Falha ao comunicar com o serviço de estoque ao descontar o produto {item.ProdutoId}: {ex.Message}");
            }

            if (!resposta.IsSuccessStatusCode)
            {
                // O serviço de estoque retornou um erro (ex: produto não encontrado, estoque insuficiente)
                var detalhes = await resposta.Content.ReadAsStringAsync();
                return StatusCode(503, $"O serviço de estoque retornou erro ao descontar o produto {item.ProdutoId} " +
                                       $"(HTTP {(int)resposta.StatusCode}): {detalhes}");
            }
        }

        // Atualiza o status da nota para "Fechada" após todos os descontos realizados com sucesso
        nota.Status = "Fechada";
        await db.SaveChangesAsync();

        return Ok(nota);
    }
}
