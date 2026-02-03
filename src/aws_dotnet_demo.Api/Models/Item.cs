using System.ComponentModel.DataAnnotations;

namespace aws_dotnet_demo.Api.Models
{
    /// <summary>
    /// Represents a simple item stored in DynamoDB.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Primary identifier (partition key).
        /// </summary>
        [Required]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable item name.
        /// </summary>
        [MaxLength(256)]
        public string? Name { get; set; }

        /// <summary>
        /// Optional description.
        /// </summary>
        [MaxLength(1024)]
        public string? Description { get; set; }

        /// <summary>
        /// UTC timestamp set when the item is created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// UTC timestamp set when the item is updated.
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Configuration settings for DynamoDB.
    /// </summary>
    public class DynamoSettings
    {
        /// <summary>
        /// DynamoDB table name for storing items.
        /// </summary>
        public string TableName { get; set; } = "aws_dotnet_demo_items";
    }
}
