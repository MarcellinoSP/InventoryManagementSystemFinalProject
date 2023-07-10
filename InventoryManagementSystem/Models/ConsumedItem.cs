using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{

	public enum ConsumedItemStatus
	{
		Consumed
	}


	public class ConsumedItem
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]

		public int ConsumedId {get; set;}

		public int? OrderId {get; set;}
		public virtual OrderItemConsumable? OrderItemConsumable {get; set;}

		// public int? ReceiptId {get; set;}
		// public virtual GoodReceipt? GoodReceipt {get; set;}
		
		public int ItemConsumableId {get; set;}
		public virtual ItemConsumable? ItemConsumable {get; set;}

		[ForeignKey(nameof(User))]

		//null!, null forgiving operator
		public string UserId {get; set;} = null!;
		public virtual User? User {get; set;}
		[Required]
		[DataType(DataType.DateTime)]
		public DateTime CreateAt {get; set;}

		[Required]
        [DataType(DataType.Date)]
        public DateTime ConsumedDate { get; set; }


		// [Required]
		// [DataType(DataType.Date)]

		// public DateTime DueDate {get; set;}
		public string NoteConsumed{get; set;} = "";
		public string? PicturePath{get; set;}

		[Required]
		public ConsumedItemStatus Status {get; set;} = ConsumedItemStatus.Consumed;

		public int Quantity {get; set;}
	}

	public class ConsumedItemViewModel : ConsumedItem
	{
		public IFormFile? Picture {get; set;}
	}
}