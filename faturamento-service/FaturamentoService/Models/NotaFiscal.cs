namespace FaturamentoService.Models;

public class NotaFiscal
{
    /// <summary>Identificador único da nota fiscal.</summary>
    public int Id { get; set; }

    /// <summary>Número da nota fiscal.</summary>
    public int Numero { get; set; }

    /// <summary>Status atual da nota fiscal (ex: Pendente, Emitida, Cancelada).</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Data e hora de criação da nota fiscal.</summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>Lista de itens vinculados a esta nota fiscal.</summary>
    public List<ItemNota> Itens { get; set; } = [];
}
