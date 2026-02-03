using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using aws_dotnet_demo.Api.Models;
using Microsoft.Extensions.Logging;

namespace aws_dotnet_demo.Api.Repositories
{
    /// <summary>
    /// DynamoDB implementation of <see cref="IItemRepository"/> using <see cref="DynamoDBContext"/>.
    /// </summary>
    public class DynamoDbItemRepository : IItemRepository
    {
        private readonly IDynamoDBContext _context;
        private readonly ILogger<DynamoDbItemRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the repository.
        /// </summary>
        public DynamoDbItemRepository(IDynamoDBContext context, ILogger<DynamoDbItemRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        private static DynamoDBOperationConfig Op(string tableName) => new()
        {
            OverrideTableName = tableName,
        };

        /// <inheritdoc />
        public async Task<IReadOnlyList<Item>> ListAsync(string tableName, CancellationToken ct = default)
        {
            var conditions = new List<ScanCondition>();
            var search = _context.ScanAsync<Item>(conditions, Op(tableName));
            var results = await search.GetRemainingAsync(ct);
            return results;
        }

        /// <inheritdoc />
        public async Task<Item?> GetAsync(string id, string tableName, CancellationToken ct = default)
        {
            return await _context.LoadAsync<Item>(id, Op(tableName), ct);
        }

        /// <inheritdoc />
        public async Task CreateAsync(Item item, string tableName, CancellationToken ct = default)
        {
            await _context.SaveAsync(item, Op(tableName), ct);
            _logger.LogInformation("Created item {Id}", item.Id);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Item item, string tableName, CancellationToken ct = default)
        {
            await _context.SaveAsync(item, Op(tableName), ct);
            _logger.LogInformation("Updated item {Id}", item.Id);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(string id, string tableName, CancellationToken ct = default)
        {
            var existing = await _context.LoadAsync<Item>(id, Op(tableName), ct);
            if (existing is null)
            {
                return false;
            }

            await _context.DeleteAsync(existing, Op(tableName), ct);
            _logger.LogInformation("Deleted item {Id}", id);
            return true;
        }
    }
}
