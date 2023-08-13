using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APICatalogo.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetProdutosPrecos()
        {
            var produtos = await _uof.ProdutoRepository
                .GetProdutosPorPreco();

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);

            return Ok(produtosDto);
        }

        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <param name="produtosParameters"></param>
        /// <returns>Retorna uma lista de objetos Produto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> Get(
            [FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository
                .GetProdutos(produtosParameters);

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDto>>(produtos);

            return Ok(produtosDto);
        }

        /// <summary>
        /// Obtém um produto pelo seu identificador produtoId
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Um objeto Produto</returns>
        [HttpGet("{id}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDto>> Get(int id)
        {
            var produto = await _uof.ProdutoRepository
                .GetById(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado...");

            var produtoDto = _mapper.Map<ProdutoDto>(produto);

            return Ok(produtoDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDto>> Post(ProdutoDto produtoDto)
        {
            var produto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Add(produto);
            await _uof.Commit();

            var produtoParaRetornar = _mapper.Map<ProdutoDto>(produto);

            return new CreatedAtRouteResult(
                "ObterProduto", new { id = produtoParaRetornar.ProdutoId }, produtoParaRetornar);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProdutoDto>> Put(int id, ProdutoDto produtoDto)
        {
            if (id != produtoDto.ProdutoId)
                return BadRequest("Dados inválidos");

            var produtoDb = await _uof.ProdutoRepository
                .GetById(p => p.ProdutoId == id);

            if (produtoDb is null)
                return NotFound();

            var produto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Update(produto);
            await _uof.Commit();

            return Ok(produtoDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdutoDto>> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository
                .GetById(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não localizado...");

            _uof.ProdutoRepository.Delete(produto);
            await _uof.Commit();

            var produtoDto = _mapper.Map<ProdutoDto>(produto);

            return Ok(produtoDto);
        }
    }
}
