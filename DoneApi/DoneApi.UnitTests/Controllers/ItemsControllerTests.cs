using DoneApi.Controllers;
using DoneApi.Data;
using DoneApi.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace DoneApi.UnitTests.Controllers
{
    public class ItemsControllerTests : IDisposable
    {
        private readonly DoneDbContext _context;
        private readonly ItemsController _controller;

        public ItemsControllerTests()
        {
            var serviceProvider = new ServiceCollection()
               .AddEntityFrameworkInMemoryDatabase()
               .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<DoneDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider);

            var context = new DoneDbContext(builder.Options);
            context.Database.EnsureCreated();

            var tasks = Enumerable.Range(1, 10).Select(t => new Item { Description = "Item " + t });

            context.Items.AddRange(tasks);
            context.SaveChanges();

            _context = context;
            _controller = new ItemsController(_context);
        }

        [Fact]
        public async Task GetAll_ReturnsItems()
        {
            var result = await _controller.GetAll();

            var model = Assert.IsAssignableFrom<IEnumerable<Item>>(result);

            var expectedCount = 10;

            Assert.Equal(expectedCount, model.Count());
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_GivenInvalidId()
        {
            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsItem_GivenValidId()
        {
            string expectedName = "Item 2";

            var result = await _controller.GetById(2);

            var objectResult = Assert.IsType<ObjectResult>(result);

            var task = Assert.IsAssignableFrom<Item>(objectResult.Value);

            Assert.Equal(expectedName, task.Description);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_GivenNullItem()
        {
            var result = await _controller.Create(null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Create(new Item());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsNewlyCreatedTodoItem()
        {
            var result = await _controller.Create(new Item { Description = "This is a new task" });

            Assert.IsType<CreatedAtRouteResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdIsInvalid()
        {
            var result = await _controller.Update(99, new Item { Id = 1, Description = "Task 1" });

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequestWhenItemIsInvalid()
        {
            var result = await _controller.Update(1, null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Update(1, new Item { Id = 1, Description = null });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenIdIsInvalid()
        {
            var result = await _controller.Update(99, new Item { Id = 99, Description = "Task 99" });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenItemUpdated()
        {
            var result = await _controller.Update(1, new Item { Id = 1, Description = "Task 1", Done = true });

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIsIsInvalid()
        {
            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenItemDeleted()
        {
            var result = await _controller.Delete(2);

            Assert.IsType<NoContentResult>(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
