using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;
public enum ReStockStatus
{
	Requested,
	Received,
	WaitingAdminApproval,
	Canceled
}

public class ReStockItem
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ReStockID { get; set; }
	public int ItemConsumableId { get; set; }
	public virtual ItemConsumable? ItemConsumable { get; set; }
	public string? KodeItemConsumable { get; set; }


	[Required]
	[DataType(DataType.Date)]
	public DateTime RequestStockDate { get; set; } = DateTime.Now;

	[Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number !")]
	public int Quantity { get; set; }
	
	[Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number !")]
	public int? AcceptedQuantity {get; set;}
	public ReStockStatus Status { get; set; } = ReStockStatus.Requested;
}

public class ReStockViewModel : ReStockItem
{
	public List <ReStockItem> Items {get; set;}
	public int? VendorId {get; set;}
}