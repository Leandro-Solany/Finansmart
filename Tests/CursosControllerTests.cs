//namespace Finansmart.Tests
//{
//    using Finansmart.Api.Controllers;
//    using Finansmart.Api.Models;
//    using Microsoft.AspNetCore.Mvc;
//    using Microsoft.EntityFrameworkCore;
//    using Moq;
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Threading.Tasks;
//    using Xunit;

//    public class CursosControllerTests
//    {
//        [Fact]
//        public async Task GetCursos_DeveRetornarCursosPaginados()
//        {
//            // Arrange
//            var cursos = new List<Curso>
//        {
//            new Curso { Id = 1, Nome = "Finanças Pessoais", Descricao = "Aprenda a organizar sua vida financeira", AvaliacaoMedia = 4.8M },
//            new Curso { Id = 2, Nome = "Investimentos", Descricao = "Noções básicas para iniciantes", AvaliacaoMedia = 4.6M }
//        }.AsQueryable();

//            var mockSet = new Mock<DbSet<Curso>>();
//            mockSet.As<IQueryable<Curso>>().Setup(m => m.Provider).Returns(cursos.Provider);
//            mockSet.As<IQueryable<Curso>>().Setup(m => m.Expression).Returns(cursos.Expression);
//            mockSet.As<IQueryable<Curso>>().Setup(m => m.ElementType).Returns(cursos.ElementType);
//            mockSet.As<IQueryable<Curso>>().Setup(m => m.GetEnumerator()).Returns(cursos.GetEnumerator());

//            var mockContext = new Mock<AppDbContext>();
//            mockContext.Setup(c => c.Cursos).Returns(mockSet.Object);

//            var controller = new CursosController(mockContext.Object);

//            // Act
//            var result = await controller.GetCursos(1, 10);

//            // Assert
//            var okResult = Assert.IsType<OkObjectResult>(result);
//            dynamic data = okResult.Value;

//            Assert.Equal(2, (int)data.Total);
//            Assert.Equal(2, ((IEnumerable<object>)data.Cursos).Count());
//        }
//    }

//}
