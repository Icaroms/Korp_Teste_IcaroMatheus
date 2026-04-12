namespace FaturamentoService.Models;

public class ItemNota
{
    /// <summary>Identificador único do item.</summary>
    public int Id { get; set; }

    /// <summary>Identificador da nota fiscal à qual este item pertence.</summary>
    public int NotaFiscalId { get; set; }

    /// <summary>Identificador do produto referenciado neste item.</summary>
    public int ProdutoId { get; set; }

    /// <summary>Quantidade do produto neste item da nota.</summary>
    public int Quantidade { get; set; }
}
