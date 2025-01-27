using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;
public enum ReStockConsumableItemStatus
{
	Requested,
	Received,
	Canceled
}

public class ReStockConsumableItem
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int IdItemConsumable { get; set; }
	public string? KodeItemConsumable { get; set; }

	[Required]
	public string? Name { get; set; }
	public string? PicturePath { get; set; }
	public string? Description { get; set; }
	public bool Availability { get; set; } = true;
	public int CategoryId { get; set; }
	public int SubCategoryId { get; set; }

	[Required]
	[DataType(DataType.DateTime)]
	public DateTime CreateAt { get; set; } = DateTime.Now;
	public virtual Category? Category { get; set; }
	public virtual SubCategory? SubCategory { get; set; }
	public int SupplierId { get; set; }
	public virtual Supplier? Supplier { get; set; }

	[Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number !")]
	public int Quantity { get; set; }
	public ReStockConsumableItemStatus Status { get; set; } = ReStockConsumableItemStatus.Requested;
}