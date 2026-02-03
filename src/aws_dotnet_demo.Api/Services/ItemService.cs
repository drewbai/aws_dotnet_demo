using aws_dotnet_demo.Api.Models;
using aws_dotnet_demo.Api.Repositories;
using Microsoft.Extensions.Logging;

namespace aws_dotnet_demo.Api.Services
{
    /// <summary>
    /// Business logic layer for items.
    /// </summary>
    public class ItemService
    {
        private readonly IItemRepository _repo;
        private readonly ILogger<ItemService> _logger;

        /// <summary>
        /// Initializes the service.
        /// </summary>
        public ItemService(IItemRepository repo, ILogger<ItemService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        /// <summary>
        /// Lists all items.
        /// </summary>
        public Task<IReadOnlyList<Item>> ListAsync(string tableName, string? correlationId = null)
        {
            _logger.LogInformation("List items (table: {Table}, corr: {Corr})", tableName, correlationId);
            return _repo.ListAsync(tableName);
        }

        /// <summary>
        /// Gets an item by id.
        /// </summary>
        public Task<Item?> GetAsync(string id, string tableName, string? correlationId = null)
        {
            _logger.LogInformation("Get item {Id} (table: {Table}, corr: {Corr})", id, tableName, correlationId);
            return _repo.GetAsync(id, tableName);
        }

        /// <summary>
        /// Creates an item.
        /// </summary>
        public async Task CreateAsync(Item item, string tableName, string? correlationId = null)
        {
            _logger.LogInformation("Create item {Id} (table: {Table}, corr: {Corr})", item.Id, tableName, correlationId);
            await _repo.CreateAsync(item, tableName);
        }

        /// <summary>
        /// Updates an item.
        /// </summary>
        public async Task UpdateAsync(Item item, string tableName, string? correlationId = null)
        {
            _logger.LogInformation("Update item {Id} (table: {Table}, corr: {Corr})", item.Id, tableName, correlationId);
            await _repo.UpdateAsync(item, tableName);
        }

        /// <summary>
        /// Deletes an item.
        /// </summary>
        public Task<bool> DeleteAsync(string id, string tableName, string? correlationId = null)
        {
            _logger.LogInformation("Delete item {Id} (table: {Table}, corr: {Corr})", id, tableName, correlationId);
            return _repo.DeleteAsync(id, tableName);
        }
    }
}
