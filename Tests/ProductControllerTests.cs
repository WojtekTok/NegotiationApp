using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NegotiationsApi.Controllers;
using NegotiationsApi.Data;
using NegotiationsApi.Models;
using NegotiationsApi.Repositories;

namespace Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task GetAllProducts_ReturnsOkResult()
        {
            // Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var testProduct = new ProductModel { Name = "TestProduct", BasePrice = 10.0m };
            productRepositoryMock.Setup(repo => repo.GetAllProductsAsync()).ReturnsAsync(new List<ProductModel> { testProduct });

            var controller = new ProductsController(productRepositoryMock.Object);

            // Act
            var result = await controller.GetAllProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductModel>>(okResult.Value);
            Assert.NotEmpty(products);
            Assert.Contains(testProduct, products);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResultForValidId()
        {
            // Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var testProduct = new ProductModel { Id = 1, Name = "TestProduct", BasePrice = 10.0m };
            productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync(testProduct);

            var controller = new ProductsController(productRepositoryMock.Object);

            // Act
            var result = await controller.GetProduct(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var product = Assert.IsType<ProductModel>(okResult.Value);
            Assert.Equal(testProduct, product);
        }

        [Fact]
        public async Task PostProduct_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            productRepositoryMock.Setup(repo => repo.AddProductAsync(It.IsAny<ProductModel>())).Returns(Task.CompletedTask);

            var controller = new ProductsController(productRepositoryMock.Object);

            // Act
            var newProduct = new ProductModel { Name = "NewProduct", BasePrice = 15.0m };
            var result = await controller.PostProduct(newProduct);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);

            var createdProduct = Assert.IsType<ProductModel>(createdAtActionResult.Value);
            Assert.Equal(newProduct, createdProduct);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContentResult()
        {
            // Arrange
            var productRepositoryMock = new Mock<IProductRepository>();
            var testProduct = new ProductModel { Id = 1, Name = "TestProduct", BasePrice = 10.0m };
            productRepositoryMock.Setup(repo => repo.GetProductByIdAsync(testProduct.Id)).ReturnsAsync(testProduct);

            var controller = new ProductsController(productRepositoryMock.Object);

            // Act
            var result = await controller.DeleteProduct(testProduct.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
