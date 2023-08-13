namespace APICatalogo.DTOs
{
    public class CategoriaDto
    {
        public int CategoriaId { get; set; }
        public string Nome { get; set; } = string.Empty!;
        public string ImagemUrl { get; set; } = string.Empty!;
        public ICollection<ProdutoDto> Produtos { get; set; } = new List<ProdutoDto>();
    }
}
