using AppStore.Core.Entities;
using AppStore.Core.Interfaces;
using AppStore.Infrastructure.Repositories;
using AppStore.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppStore.Core.Tests.Integration
{
    public class ProductRepositoryIntegrationTests : IDisposable
    {
        private readonly AppStoreDbContext _context;
        private readonly IRepository<Product> _productRepository;

        public ProductRepositoryIntegrationTests()
        {
            // Configurar DbContext en memoria
            var options = new DbContextOptionsBuilder<AppStoreDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppStoreDbContext(options);
            _productRepository = new BaseRepository<Product>(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region SelectAll Tests

        [Fact]
        public void SelectAll_WhenProductsExist_ReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Price = 100, Stock = 10, IsActive = true },
                new Product { Name = "Product 2", Price = 200, Stock = 20, IsActive = true }
            };
            _context.Products.AddRange(products);
            _context.SaveChanges();

            // Act
            var result = _productRepository.SelectAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void SelectAll_WhenNoProducts_ReturnEmptyList()
        {
            // Act
            var result = _productRepository.SelectAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region Select Tests

        [Fact]
        public async Task Select_WithValidId_ReturnProduct()
        {
            // Arrange
            var product = new Product 
            { 
                Name = "Test Product", 
                Price = 150,
                Stock = 5,
                IsActive = true
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            var productId = product.Id;

            // Act
            var result = await _productRepository.Select(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
            Assert.Equal(150, result.Price);
        }

        [Fact]
        public async Task Select_WithInvalidId_ReturnNull()
        {
            // Act
            var result = await _productRepository.Select(999);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Insert Tests

        [Fact]
        public async Task Insert_WithValidProduct_AddProductToDatabase()
        {
            // Arrange
            var product = new Product 
            { 
                Name = "New Product", 
                Price = 250,
                Stock = 15,
                IsActive = true,
                Date = DateTime.Now
            };

            // Act
            await _productRepository.Insert(product);
            _context.SaveChanges();

            // Assert
            var savedProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal("New Product", savedProduct.Name);
            Assert.Equal(250, savedProduct.Price);
        }

        [Fact]
        public async Task Insert_MultipleProducts_AllInserted()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Price = 100, Stock = 10, IsActive = true, Date = DateTime.Now },
                new Product { Name = "Product 2", Price = 200, Stock = 20, IsActive = true, Date = DateTime.Now },
                new Product { Name = "Product 3", Price = 300, Stock = 30, IsActive = true, Date = DateTime.Now }
            };

            // Act
            foreach (var product in products)
            {
                await _productRepository.Insert(product);
            }
            _context.SaveChanges();

            // Assert
            var allProducts = _productRepository.SelectAll();
            Assert.Equal(3, allProducts.Count());
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithExistingProduct_UpdateProductInDatabase()
        {
            // Arrange
            var product = new Product 
            { 
                Name = "Original Name", 
                Price = 100,
                Stock = 10,
                IsActive = true,
                Date = DateTime.Now
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            var productId = product.Id;
            product.Name = "Updated Name";
            product.Price = 200;
            product.Stock = 20;

            // Act
            _productRepository.Update(product);
            _context.SaveChanges();

            // Assert
            var updatedProduct = _context.Products.Find(productId);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Name", updatedProduct.Name);
            Assert.Equal(200, updatedProduct.Price);
            Assert.Equal(20, updatedProduct.Stock);
        }

        [Fact]
        public void Update_MultipleProperties_AllUpdated()
        {
            // Arrange
            var product = new Product 
            { 
                Name = "Test", 
                Price = 100,
                Stock = 10,
                IsActive = true,
                Date = DateTime.Now
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            product.Name = "Updated";
            product.Price = 500;
            product.Stock = 50;
            product.IsActive = false;

            // Act
            _productRepository.Update(product);
            _context.SaveChanges();

            // Assert
            var updated = _context.Products.Find(product.Id);
            Assert.Equal("Updated", updated.Name);
            Assert.Equal(500, updated.Price);
            Assert.Equal(50, updated.Stock);
            Assert.False(updated.IsActive);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithValidId_RemoveProductFromDatabase()
        {
            // Arrange
            var product = new Product 
            { 
                Name = "To Delete", 
                Price = 100,
                Stock = 10,
                IsActive = true,
                Date = DateTime.Now
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            var productId = product.Id;

            // Act
            await _productRepository.Delete(productId);
            _context.SaveChanges();

            // Assert
            var deletedProduct = await _context.Products.FindAsync(productId);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productRepository.Delete(999));
        }

        #endregion

        #region Complex Integration Scenarios

        [Fact]
        public async Task CompleteWorkflow_CreateUpdateAndDelete()
        {
            // Arrange - Create
            var product = new Product 
            { 
                Name = "Workflow Product", 
                Price = 100,
                Stock = 10,
                IsActive = true,
                Date = DateTime.Now
            };

            // Act - Create
            await _productRepository.Insert(product);
            _context.SaveChanges();
            var createdId = product.Id;

            // Assert - Created
            var createdProduct = await _productRepository.Select(createdId);
            Assert.NotNull(createdProduct);
            Assert.Equal("Workflow Product", createdProduct.Name);

            // Act - Update
            createdProduct.Name = "Updated Workflow Product";
            createdProduct.Price = 200;
            _productRepository.Update(createdProduct);
            _context.SaveChanges();

            // Assert - Updated
            var updatedProduct = await _productRepository.Select(createdId);
            Assert.Equal("Updated Workflow Product", updatedProduct.Name);
            Assert.Equal(200, updatedProduct.Price);

            // Act - Delete
            await _productRepository.Delete(createdId);
            _context.SaveChanges();

            // Assert - Deleted
            var deletedProduct = await _productRepository.Select(createdId);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task SelectAll_AfterMultipleOperations_ReturnCorrectState()
        {
            // Arrange - Create multiple products
            var products = new List<Product>
            {
                new Product { Name = "P1", Price = 100, Stock = 10, IsActive = true, Date = DateTime.Now },
                new Product { Name = "P2", Price = 200, Stock = 20, IsActive = true, Date = DateTime.Now },
                new Product { Name = "P3", Price = 300, Stock = 30, IsActive = true, Date = DateTime.Now }
            };

            foreach (var p in products)
            {
                await _productRepository.Insert(p);
            }
            _context.SaveChanges();

            // Act - Delete one
            await _productRepository.Delete(products[1].Id);
            _context.SaveChanges();

            // Assert - Verify remaining
            var remaining = _productRepository.SelectAll();
            Assert.Equal(2, remaining.Count());
            Assert.DoesNotContain(remaining, p => p.Id == products[1].Id);
        }

        #endregion
    }
}
