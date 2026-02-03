using System.Threading.Tasks;
using aws_dotnet_demo.Api.Models;
using aws_dotnet_demo.Api.Repositories;
using aws_dotnet_demo.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace aws_dotnet_demo.Tests
{
    public class ItemServiceTests
    {
        [Fact]
        public async Task CreateAndGetItem_Succeeds()
        {
            var repo = new Mock<IItemRepository>();
            var logger = new Mock<ILogger<ItemService>>();
            var svc = new ItemService(repo.Object, logger.Object);
            var table = "test_table";

            var item = new Item { Id = "abc", Name = "Hello" };

            repo.Setup(r => r.CreateAsync(item, table, default)).Returns(Task.CompletedTask);
            repo.Setup(r => r.GetAsync("abc", table, default)).ReturnsAsync(item);

            await svc.CreateAsync(item, table);
            var loaded = await svc.GetAsync("abc", table);

            Assert.NotNull(loaded);
            Assert.Equal("abc", loaded!.Id);
            Assert.Equal("Hello", loaded.Name);
        }
    }
}
