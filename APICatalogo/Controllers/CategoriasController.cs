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
    [Produces("application/json")]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategoriasProdutos()
        {
            var categorias = await _uof.CategoriaRepository
                .GetCategoriasProdutos();

            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDto>>(categorias);

            return Ok(categoriasDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> Get(
            [FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository
                .GetCategorias(categoriasParameters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDto>>(categorias);

            return Ok(categoriasDto);
        }

        /// <summary>
        /// Obtém uma Categoria pelo seu Id
        /// </summary>
        /// <param name="id">Código da categoria</param>
        /// <returns>Objetos Categoria</returns>
        [HttpGet("{id}", Name = "ObterCategoria")]
        [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaDto>> Get(int id)
        {
            var categoria = await _uof.CategoriaRepository
                .GetById(c => c.CategoriaId == id);

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);

            return categoria is null ?
                NotFound($"Categoria com id {id} não encontrada...") :
                Ok(categoriaDto);
        }

        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        ///     POST api/categorias
        ///     {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "http://teste.net/1.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoriaDto">Objeto Categoria</param>
        /// <returns>O objeto Categoria incluído</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoriaDto>> Post(CategoriaDto categoriaDto)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Add(categoria);
            await _uof.Commit();

            var categoriaParaRetornar = _mapper.Map<CategoriaDto>(categoria);

            return new CreatedAtRouteResult(
                "ObterCategoria", new { id = categoriaParaRetornar.CategoriaId }, categoriaParaRetornar);
        }

        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult<CategoriaDto>> Put(int id, CategoriaDto categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
                return BadRequest("Dados inválidos");

            var categoriaDb = await _uof.CategoriaRepository
                .GetById(c => c.CategoriaId == id);

            if (categoriaDb is null)
                return NotFound();

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Update(categoria);
            await _uof.Commit();

            return Ok(categoriaDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoriaDto>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository
                .GetById(c => c.CategoriaId == id);

            if (categoria is null)
                return NotFound($"Categoria com id {id} não encontrada...");

            _uof.CategoriaRepository.Delete(categoria);
            await _uof.Commit();

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);

            return Ok(categoriaDto);
        }
    }
}
