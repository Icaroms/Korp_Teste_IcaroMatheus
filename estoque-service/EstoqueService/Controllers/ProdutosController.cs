using EstoqueService.Data;
using EstoqueService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
