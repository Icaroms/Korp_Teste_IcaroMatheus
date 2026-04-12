using FaturamentoService.Models;
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>Tabela de notas fiscais.</summary>
    public DbSet<NotaFiscal> NotasFiscais { get; set; }

    /// <summary>Tabela de itens de nota fiscal.</summary>
    public DbSet<ItemNota> ItensNota { get; set; }
}
