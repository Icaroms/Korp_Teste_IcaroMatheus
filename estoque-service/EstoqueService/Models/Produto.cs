using System.ComponentModel.DataAnnotations;

namespace EstoqueService.Models;

// Model que representa um produto no estoque
public class Produto
{
    // Identificador único do produto (chave primária)
    public int Id { get; set; }

    // Código de identificação do produto — campo obrigatório
    [Required]
    public string Codigo { get; set; } = string.Empty;

    // Descrição textual do produto — campo obrigatório
    [Required]
    public string Descricao { get; set; } = string.Empty;

    // Quantidade disponível em estoque
    public int Saldo { get; set; }
}
