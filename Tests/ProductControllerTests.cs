using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NegotiationsApi.Controllers;
using NegotiationsApi.Data;
using NegotiationsApi.Models;

namespace Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task GetAllProducts_ReturnsOkResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.ProductModel.Add(new ProductModel { Name = "TestProduct", BasePrice = 10.0m });
                context.SaveChanges();
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var controller = new ProductsController(context);
                var result = await controller.GetAllProducts();

                // Assert
                var okResult = Assert.IsType<ActionResult<IEnumerable<ProductModel>>>(result);
                var products = Assert.IsAssignableFrom<IEnumerable<ProductModel>>(okResult.Value);
                Assert.NotEmpty(products);
            }
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResultForValidId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Act
            using (var context = new AppDbContext(options))
            {
                var controller = new ProductsController(context);
                var result = await controller.GetProduct(1);

                // Assert
                var okResult = Assert.IsType<ActionResult<ProductModel>>(result);
                var products = Assert.IsAssignableFrom<ProductModel>(okResult.Value);
            }
        }

        [Fact]
        public async Task PostProduct_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Act
            using (var context = new AppDbContext(options))
            {
                var controller = new ProductsController(context);
                var newProduct = new ProductModel { Name = "NewProduct", BasePrice = 15.0m };
                var result = await controller.PostProduct(newProduct);

                // Assert
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);

                var createdProduct = Assert.IsType<ProductModel>(createdAtActionResult.Value);
                Assert.Equal(newProduct.Id, createdProduct.Id);
            }
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContentResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Stworzenie nowego produktu do usuniêcia
            var productToDelete = new ProductModel { Name = "ProductToDelete", BasePrice = 20.0m };

            // Dodanie produktu do bazy danych
            using (var context = new AppDbContext(options))
            {
                context.ProductModel.Add(productToDelete);
                context.SaveChanges();
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var controller = new ProductsController(context);
                var result = await controller.DeleteProduct(productToDelete.Id);

                // Assert
                Assert.IsType<NoContentResult>(result);

                var deletedProduct = await context.ProductModel.FindAsync(productToDelete.Id);
                Assert.Null(deletedProduct);
            }
        }
    }
}
