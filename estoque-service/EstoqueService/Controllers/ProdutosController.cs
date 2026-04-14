using EstoqueService.Data;
using EstoqueService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// Corpo da requisição para o endpoint de desconto de saldo
public record DescontarRequest(int Quantidade);

namespace EstoqueService.Controllers;

[ApiController]
[Route("produtos")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    // GET /produtos
    // Retorna a lista de todos os produtos cadastrados
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var produtos = await _context.Produtos.ToListAsync();
        return Ok(produtos);
    }

    // GET /produtos/{id}
    // Retorna um produto específico pelo seu Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto is null)
            return NotFound();

        return Ok(produto);
    }

    // POST /produtos
    // Cria um novo produto com os dados enviados no corpo da requisição
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    // POST /produtos/{id}/descontar
    // Subtrai uma quantidade do saldo do produto informado
    // Retorna 404 se o produto não for encontrado
    // Retorna 400 se o saldo atual for insuficiente para a quantidade solicitada
    [HttpPost("{id}/descontar")]
    public async Task<IActionResult> Descontar(int id, [FromBody] DescontarRequest request)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto is null)
            return NotFound();

        // Verifica se o saldo disponível é suficiente para realizar o desconto
        if (produto.Saldo < request.Quantidade)
            return BadRequest($"Saldo insuficiente. Saldo atual: {produto.Saldo}, quantidade solicitada: {request.Quantidade}.");

        produto.Saldo -= request.Quantidade;
        await _context.SaveChangesAsync();

        return Ok(produto);
    }

    // DELETE /produtos/{id}
    // Remove um produto pelo seu Id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto is null)
            return NotFound();

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
