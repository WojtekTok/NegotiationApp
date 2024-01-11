using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NegotiationsApi.Controllers;
using NegotiationsApi.Data;
using NegotiationsApi.Models;
using NegotiationsApi.Repositories;
using NegotiationsApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class NegotiationsControllerTests
    {
        [Fact]
        public async Task GetAllNegotiations_ReturnsNegotiations()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase1")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.NegotiationModel.Add(new NegotiationModel
                {
                    ProductId = 1,
                    CustomerId = 1,
                    ProposedPrice = 15.0m,
                    AttemptsLeft = 2,
                    Status = NegotiationModel.NegotiationStatus.Pending
                });

                context.NegotiationModel.Add(new NegotiationModel
                {
                    ProductId = 2,
                    CustomerId = 2,
                    ProposedPrice = 20.0m,
                    AttemptsLeft = 1,
                    Status = NegotiationModel.NegotiationStatus.Accepted
                });

                context.SaveChanges();
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var negotiationServiceMock = new Mock<INegotiationService>();
                var controller = new NegotiationsController(context, negotiationServiceMock.Object);
                var result = await controller.GetAllNegotiations();

                // Assert
                var negotiations = Assert.IsAssignableFrom<IEnumerable<NegotiationModel>>(result.Value);
                Assert.NotEmpty(negotiations);

                var firstNegotiation = negotiations.First();
                var secondNegotiation = negotiations.Last();
                Assert.Equal(1, firstNegotiation.Id);
                Assert.Equal(1, firstNegotiation.ProductId);
                Assert.Equal(1, firstNegotiation.CustomerId);
                Assert.Equal(15.0m, firstNegotiation.ProposedPrice);
                Assert.Equal(2, firstNegotiation.AttemptsLeft);
                Assert.Equal(NegotiationModel.NegotiationStatus.Pending, firstNegotiation.Status);
                Assert.Equal(2, secondNegotiation.Id);
                Assert.Equal(2, secondNegotiation.ProductId);
                Assert.Equal(2, secondNegotiation.CustomerId);
                Assert.Equal(20.0m, secondNegotiation.ProposedPrice);
                Assert.Equal(1, secondNegotiation.AttemptsLeft);
                Assert.Equal(NegotiationModel.NegotiationStatus.Accepted, secondNegotiation.Status);
            }
        }

        [Fact]
        public async Task GetNegotiation_ReturnsNegotiationForValidId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.NegotiationModel.Add(new NegotiationModel
                {
                    ProductId = 1,
                    CustomerId = 1,
                    ProposedPrice = 15.0m,
                    AttemptsLeft = 2,
                    Status = NegotiationModel.NegotiationStatus.Pending
                });

                context.NegotiationModel.Add(new NegotiationModel
                {
                    ProductId = 2,
                    CustomerId = 2,
                    ProposedPrice = 20.0m,
                    AttemptsLeft = 1,
                    Status = NegotiationModel.NegotiationStatus.Accepted
                });

                context.SaveChanges();
            }

            // Act
            using (var context = new AppDbContext(options))
            {
                var negotiationServiceMock = new Mock<INegotiationService>();
                var controller = new NegotiationsController(context, negotiationServiceMock.Object);
                var result = await controller.GetNegotiation(1);

                // Assert
                var negotiation = Assert.IsType<NegotiationModel>(result.Value);
                Assert.Equal(1, negotiation.Id);
                Assert.Equal(1, negotiation.ProductId);
                Assert.Equal(1, negotiation.CustomerId);
                Assert.Equal(15.0m, negotiation.ProposedPrice);
                Assert.Equal(2, negotiation.AttemptsLeft);
                Assert.Equal(NegotiationModel.NegotiationStatus.Pending, negotiation.Status);
            }
        }

        [Fact]
        public async Task DeleteNegotiation_ReturnsNoContentForValidId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.NegotiationModel.Add(new NegotiationModel
                {
                    ProductId = 1,
                    CustomerId = 1,
                    ProposedPrice = 15.0m,
                    AttemptsLeft = 2,
                    Status = NegotiationModel.NegotiationStatus.Pending
                });

                context.SaveChanges();
            }

            var negotiationServiceMock = new Mock<INegotiationService>();

            // Act
            using (var context = new AppDbContext(options))
            {
                var controller = new NegotiationsController(context, negotiationServiceMock.Object);
                var result = await controller.DeleteNegotiation(1);

                // Assert
                Assert.IsType<NoContentResult>(result);

                var deletedNegotiation = await context.NegotiationModel.FindAsync(1);
                Assert.Null(deletedNegotiation);
            }
        }

        [Fact]
        public async Task CustomerPostNegotiation_ReturnsCreatedResultForValidNegotiation()
        {
            // Arrange
            var negotiationRepositoryMock = new Mock<INegotiationRepository>();
            var productRepositoryMock = new Mock<IProductRepository>();

            negotiationRepositoryMock.Setup(repo => repo.AddNegotiationAsync(It.IsAny<NegotiationModel>()))
                .Returns((NegotiationModel negotiation) => Task.FromResult(negotiation));

            var negotiationService = new NegotiationService(negotiationRepositoryMock.Object, productRepositoryMock.Object);
            var controller = new NegotiationsController(negotiationService);

            var productId = 1;
            var customerId = 1;
            var proposedPrice = 15.0m;

            // Act
            var result = await controller.CustomerPostNegotiation(productId, customerId, proposedPrice);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);

            var negotiation = Assert.IsType<NegotiationModel>(createdAtActionResult.Value);

            Assert.Equal(productId, negotiation.ProductId);
            Assert.Equal(customerId, negotiation.CustomerId);
            Assert.Equal(proposedPrice, negotiation.ProposedPrice);
            Assert.Equal(customerId, negotiation.CustomerId);
            Assert.Equal(NegotiationModel.NegotiationStatus.Pending, negotiation.Status);

            negotiationRepositoryMock.Verify(repo => repo.AddNegotiationAsync(It.IsAny<NegotiationModel>()), Times.Once);
        }
        }//TODO: ma sprawdzić: czy dla produktu o cenie bazowej x dla propozycji 2 razy większej dostaniemy odrzucenie a dla normalnej pending
        // Potem czy jeśli dodam dla tych samych id czy mi wyskoczy że już istnieje taka negocjacja

        [Fact]
        public async Task CustomerPostNegotiation_ReturnsBadRequestIfNegotiationExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase5")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.NegotiationModel.Add(new NegotiationModel
                {
                    ProductId = 1,
                    CustomerId = 1,
                    ProposedPrice = 15.0m,
                    AttemptsLeft = 2,
                    Status = NegotiationModel.NegotiationStatus.Pending
                });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var negotiationService = new NegotiationService(context);
                var controller = new NegotiationsController(context, negotiationService);

                var productId = 1;
                var customerId = 1;
                var proposedPrice = 15.0m;

                // Act
                var result = await controller.CustomerPostNegotiation(productId, customerId, proposedPrice);

                // Assert
                var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
                Assert.Equal("Negocjacja o ten produkt już istnieje", badRequestObjectResult.Value);
            }
        }

        [Fact]
        public async Task CustomerPostNegotiation_RejectsTooHighProposition()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase6")
                .Options;

            using (var context = new AppDbContext(options))
            {
                context.ProductModel.Add(new ProductModel { Name = "TestProduct", BasePrice = 10.0m });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var negotiationService = new NegotiationService(context);
                var controller = new NegotiationsController(context, negotiationService);

                var productId = 1;
                var customerId = 1;
                var proposedPrice = 25.0m;

                // Act
                var result = await controller.CustomerPostNegotiation(productId, customerId, proposedPrice);

                // Assert
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);

                var negotiation = Assert.IsType<NegotiationModel>(createdAtActionResult.Value);
                Assert.Equal(NegotiationModel.NegotiationStatus.Rejected, negotiation.Status);

                var savedNegotiation = await context.NegotiationModel.FindAsync(negotiation.Id);
                Assert.NotNull(savedNegotiation);
                Assert.Equal(NegotiationModel.NegotiationStatus.Rejected, savedNegotiation.Status);
            }
        }

        [Fact]
        public async Task CustomerPutNegotiation_ReturnsBadRequestWhenNegotiationNotExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase7")
                .Options;

            var context = new AppDbContext(options);
            var negotiationService = new NegotiationService(context);
            var controller = new NegotiationsController(context, negotiationService);

            var productId = 1;
            var customerId = 1;
            var proposedPrice = 25.0m;

            // Act
            var result = await controller.CustomerPutNegotiation(productId, customerId, proposedPrice);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CustomerPutNegotiation_ReturnsBadRequestWhenPendingNegotiation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase8")
                .Options;

            var context = new AppDbContext(options);
            var negotiationService = new NegotiationService(context);
            var controller = new NegotiationsController(context, negotiationService);

            var productId = 1;
            var customerId = 1;
            var proposedPrice = 25.0m;

            // Add a negotiation with Pending status
            var negotiation = new NegotiationModel
            {
                ProductId = productId,
                CustomerId = customerId,
                ProposedPrice = 20.0m,
                Status = NegotiationModel.NegotiationStatus.Pending,
                AttemptsLeft = 3
            };
            context.NegotiationModel.Add(negotiation);
            context.SaveChanges();

            // Act
            var result = await controller.CustomerPutNegotiation(productId, customerId, proposedPrice);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Fact]
        public async Task EmployeePutNegotiation_ReturnsOkResultForValidNegotiation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase9")
                .Options;

            var context = new AppDbContext(options);
            var negotiationService = new NegotiationService(context);

            // Dodaj przykładową negocjację do bazy danych
            var negotiation = new NegotiationModel
            {
                ProductId = 1,
                CustomerId = 1,
                ProposedPrice = 20.0m,
                Status = NegotiationModel.NegotiationStatus.Pending
            };

            context.NegotiationModel.Add(negotiation);
            context.SaveChanges();

            var controller = new NegotiationsController(context, negotiationService);

            // Act
            var result = await controller.EmployeePutNegotiation(negotiation.Id, NegotiationModel.NegotiationStatus.Accepted);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var updatedNegotiation = Assert.IsType<NegotiationModel>(okObjectResult.Value);

            Assert.Equal(NegotiationModel.NegotiationStatus.Accepted, updatedNegotiation.Status);
        }

        [Fact]
        public async Task EmployeePutNegotiation_ReturnsNotFoundForInvalidNegotiation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase10")
                .Options;

            var context = new AppDbContext(options);
            var negotiationService = new NegotiationService(context);

            var controller = new NegotiationsController(context, negotiationService);

            // Act
            var result = await controller.EmployeePutNegotiation(999, NegotiationModel.NegotiationStatus.Accepted);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

