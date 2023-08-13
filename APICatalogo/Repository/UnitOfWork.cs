using APICatalogo.Context;

namespace APICatalogo.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly Lazy<ProdutoRepository> _produtoRepository;
        private readonly Lazy<CategoriaRepository> _categoriaRepository;
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            _produtoRepository = new Lazy<ProdutoRepository>(() => new ProdutoRepository(_context));
            _categoriaRepository = new Lazy<CategoriaRepository>(() => new CategoriaRepository(_context));
        }

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepository.Value;
            }
        }

        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                return _categoriaRepository.Value;
            }
        }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _context.Dispose();
        }
    }
}
