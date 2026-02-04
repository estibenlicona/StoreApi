using AppStore.Core.Entities;
using AppStore.Core.Interfaces;
using AppStore.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppStore.Core.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IUniOfWork> _mockUniOfWork;
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockUniOfWork = new Mock<IUniOfWork>();
            _mockProductRepository = new Mock<IRepository<Product>>();
            
            _mockUniOfWork
                .Setup(u => u.ProductRespository)
                .Returns(_mockProductRepository.Object);

            _productService = new ProductService(_mockUniOfWork.Object);
        }

        #region List Tests

        [Fact]
        public void List_WhenCalled_ReturnProductList()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100, Stock = 10 },
                new Product { Id = 2, Name = "Product 2", Price = 200, Stock = 20 }
            };

            _mockProductRepository
                .Setup(r => r.SelectAll())
                .Returns(expectedProducts);

            // Act
            var result = _productService.List();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(expectedProducts, result);
            _mockProductRepository.Verify(r => r.SelectAll(), Times.Once);
        }

        [Fact]
        public void List_WhenNoProducts_ReturnEmptyList()
        {
            // Arrange
            _mockProductRepository
                .Setup(r => r.SelectAll())
                .Returns(new List<Product>());

            // Act
            var result = _productService.List();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockProductRepository.Verify(r => r.SelectAll(), Times.Once);
        }

        #endregion

        #region Get Tests

        [Fact]
        public async Task Get_WithValidId_ReturnProduct()
        {
            // Arrange
            var productId = 1;
            var expectedProduct = new Product 
            { 
                Id = productId, 
                Name = "Test Product", 
                Price = 100,
                Stock = 5
            };

            _mockProductRepository
                .Setup(r => r.Select(productId))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productService.Get(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
            _mockProductRepository.Verify(r => r.Select(productId), Times.Once);
        }

        [Fact]
        public async Task Get_WithInvalidId_ReturnNull()
        {
            // Arrange
            var productId = 999;
            _mockProductRepository
                .Setup(r => r.Select(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.Get(productId);

            // Assert
            Assert.Null(result);
            _mockProductRepository.Verify(r => r.Select(productId), Times.Once);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidProduct_CallInsertAndSaveChanges()
        {
            // Arrange
            var product = new Product 
            { 
                Name = "New Product", 
                Price = 150,
                Stock = 10
            };

            _mockProductRepository
                .Setup(r => r.Insert(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _mockUniOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _productService.Create(product);

            // Assert
            _mockProductRepository.Verify(r => r.Insert(product), Times.Once);
            _mockUniOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_WithNullProduct_ThrowException()
        {
            // Arrange
            Product nullProduct = null;

            _mockProductRepository
                .Setup(r => r.Insert(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _mockUniOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productService.Create(nullProduct));
        }

        #endregion

        #region Edit Tests

        [Fact]
        public async Task Edit_WithValidProduct_CallUpdateAndSaveChanges()
        {
            // Arrange
            var product = new Product 
            { 
                Id = 1,
                Name = "Updated Product", 
                Price = 200,
                Stock = 15
            };

            _mockProductRepository
                .Setup(r => r.Update(It.IsAny<Product>()));

            _mockUniOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.Edit(product);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(r => r.Update(product), Times.Once);
            _mockUniOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Edit_WithNullProduct_ThrowException()
        {
            // Arrange
            Product nullProduct = null;

            _mockProductRepository
                .Setup(r => r.Update(It.IsAny<Product>()));

            _mockUniOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productService.Edit(nullProduct));
        }

        #endregion

        #region Remove Tests

        [Fact]
        public async Task Remove_WithValidId_CallDeleteAndSaveChanges()
        {
            // Arrange
            var productId = 1;

            _mockProductRepository
                .Setup(r => r.Delete(productId))
                .Returns(Task.CompletedTask);

            _mockUniOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.Remove(productId);

            // Assert
            Assert.True(result);
            _mockProductRepository.Verify(r => r.Delete(productId), Times.Once);
            _mockUniOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Remove_WhenCalled_AlwaysReturnTrue()
        {
            // Arrange
            var productId = 5;

            _mockProductRepository
                .Setup(r => r.Delete(productId))
                .Returns(Task.CompletedTask);

            _mockUniOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.Remove(productId);

            // Assert
            Assert.True(result);
        }

        #endregion
    }
}
