using APICatalogo.Context;

namespace ApiCatalogoUnitTests
{
    public static class DbUnitTestsMockInitializer
    {
        public static void Seed(AppDbContext context)
        {
            context.Categorias.Add(
                new APICatalogo.Models.Categoria { CategoriaId = 999, Nome = "Bebidas999", ImagemUrl = "bebidas999.jpg" });
            context.Categorias.Add(
                new APICatalogo.Models.Categoria { CategoriaId = 2, Nome = "Sucos", ImagemUrl = "suco1.jpg" });
            context.Categorias.Add(
                new APICatalogo.Models.Categoria { CategoriaId = 3, Nome = "Doces", ImagemUrl = "doces1.jpg" });
            context.Categorias.Add(
                new APICatalogo.Models.Categoria { CategoriaId = 4, Nome = "Salgados", ImagemUrl = "salgado1.jpg" });
            context.Categorias.Add(
                new APICatalogo.Models.Categoria { CategoriaId = 5, Nome = "Tortas", ImagemUrl = "tortas1.jpg" });
            context.Categorias.Add(
                new APICatalogo.Models.Categoria { CategoriaId = 6, Nome = "Bolos", ImagemUrl = "bolos1.jpg" });
            context.Categorias.Add(
                new APICatalogo.Models.Categoria { CategoriaId = 7, Nome = "Lanches", ImagemUrl = "lanches1.jpg" });

            context.SaveChanges();
            context.ChangeTracker.Clear();
        }
    }
}
