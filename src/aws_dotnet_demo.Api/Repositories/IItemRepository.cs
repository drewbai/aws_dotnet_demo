using aws_dotnet_demo.Api.Models;

namespace aws_dotnet_demo.Api.Repositories
{
    /// <summary>
    /// Abstraction for CRUD operations on <see cref="Item"/>.
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// Retrieves a list of all items.
        /// </summary>
        Task<IReadOnlyList<Item>> ListAsync(string tableName, CancellationToken ct = default);

        /// <summary>
        /// Gets a single item by identifier.
        /// </summary>
        Task<Item?> GetAsync(string id, string tableName, CancellationToken ct = default);

        /// <summary>
        /// Creates a new item.
        /// </summary>
        Task CreateAsync(Item item, string tableName, CancellationToken ct = default);

        /// <summary>
        /// Updates an existing item.
        /// </summary>
        Task UpdateAsync(Item item, string tableName, CancellationToken ct = default);

        /// <summary>
        /// Deletes an item by identifier.
        /// </summary>
        Task<bool> DeleteAsync(string id, string tableName, CancellationToken ct = default);
    }
}
