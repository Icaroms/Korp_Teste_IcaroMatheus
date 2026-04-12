using EstoqueService.Models;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Data;

// Contexto principal do banco de dados da aplicação.
// Herda de DbContext e é configurado via injeção de dependência no Program.cs.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Representa a tabela "Produtos" no banco de dados
    public DbSet<Produto> Produtos { get; set; }
}
