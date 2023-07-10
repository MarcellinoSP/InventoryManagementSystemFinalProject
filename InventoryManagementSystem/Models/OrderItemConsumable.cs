using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Models
{
    public enum OrderItemConsumableStatus
    {
        WaitingPickUp,
		DonePickUp,
		CancelledBySystem,
    }

    public class OrderItemConsumable
    {
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OrderConsumableId { get; set; }
		public int? RequestId { get; set; }
		public virtual RequestItemConsumable? RequestItemConsumable { get; set; }
		public int? ConsumableId {get; set;}
		public virtual ConsumedItem? ConsumableItem {get; set;}

        public int ItemConsumableId {get; set;}
        public virtual ItemConsumable? ItemConsumable {get; set;}

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
		public virtual User? User { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime CreateAt { get; set; }

		[Required]
		[DataType(DataType.Date)]
		public DateTime ConsumeDateApproved { get; set; }
		public string NoteDonePickUp { get; set; } = "";
		public string NoteWaitingPickUp { get; set; } = "";

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number !")]
    	public int Quantity {get; set;}
		public OrderItemConsumableStatus Status { get; set; } = OrderItemConsumableStatus.WaitingPickUp;

    }
}