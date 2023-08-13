using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoUnitTests
{
    public class CategoriasControllerUnitTests
    {
        private readonly CategoriasController _controller;

        public CategoriasControllerUnitTests()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection);
            var context = new AppDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();

            DbUnitTestsMockInitializer.Seed(context);

            var repository = new UnitOfWork(context);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ControllerActionDescriptor());

            _controller = new CategoriasController(repository, mapper)
            {
                ControllerContext = new ControllerContext(actionContext)
            };
        }

        [Fact]
        public async Task Get_GetActionWithParameters_ShouldReturnOkObjectResultWithCorrectAmountOfCategorias()
        {
            // Arrange
            var parameters = new CategoriasParameters();

            // Act
            var result = await _controller.Get(parameters);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CategoriaDto>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<CategoriaDto>>(okObjectResult.Value);
            Assert.Equal(7, dtos.Count());
            var firstCategoria = dtos.First();
            Assert.Equal(999, firstCategoria.CategoriaId);
            Assert.Equal("Bebidas999", firstCategoria.Nome);
            Assert.Equal("bebidas999.jpg", firstCategoria.ImagemUrl);
        }

        [Fact]
        public async Task Get_GetActionWithExistingId_ShouldReturnOkObjectResultWithCorrectCategoria()
        {
            // Arrange
            var id = 2;

            // Act
            var result = await _controller.Get(id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CategoriaDto>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsAssignableFrom<CategoriaDto>(okObjectResult.Value);
            Assert.Equal(id, dto.CategoriaId);
            Assert.Equal("Sucos", dto.Nome);
            Assert.Equal("suco1.jpg", dto.ImagemUrl);
        }

        [Fact]
        public async Task Get_GetActionWithNonExistingId_ShouldReturnNotFoundObjectResult()
        {
            // Arrange
            var id = 8;

            // Act
            var result = await _controller.Get(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Post_PostAction_ShouldReturnCreatedAtRouteResult()
        {
            // Arrange
            var requestDto = new CategoriaDto() { Nome = "TestName", ImagemUrl = "TestImage.jpg" };

            // Act
            var result = await _controller.Post(requestDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CategoriaDto>>(result);
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
            var dto = Assert.IsAssignableFrom<CategoriaDto>(createdAtRouteResult.Value);
            Assert.IsType<int>(dto.CategoriaId);
            Assert.Equal(requestDto.Nome, dto.Nome);
            Assert.Equal(requestDto.ImagemUrl, dto.ImagemUrl);
        }

        [Fact]
        public async Task Put_PutAction_ShouldReturnOkObjectResult()
        {
            // Arrange
            var id = 2;
            var requestDto = new CategoriaDto() { CategoriaId = id, Nome = "UpdatedName", ImagemUrl = "UpdatedImage.jpg" };

            // Act
            var result = await _controller.Put(id, requestDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CategoriaDto>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsAssignableFrom<CategoriaDto>(okObjectResult.Value);
            Assert.Equal(id, dto.CategoriaId);
            Assert.Equal(requestDto.Nome, dto.Nome);
            Assert.Equal(requestDto.ImagemUrl, dto.ImagemUrl);
        }

        [Fact]
        public async Task Delete_DeleteAction_ShouldReturnOkObjectResult()
        {
            // Arrange
            var id = 2;

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var actionResult = Assert.IsType<ActionResult<CategoriaDto>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var dto = Assert.IsAssignableFrom<CategoriaDto>(okObjectResult.Value);
            Assert.Equal(id, dto.CategoriaId);
            Assert.Equal("Sucos", dto.Nome);
            Assert.Equal("suco1.jpg", dto.ImagemUrl);
        }
    }
}