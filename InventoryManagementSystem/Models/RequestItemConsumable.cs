using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
	public enum RequestItemConsumableStatus
	{
		WaitingApproval,
		Rejected,
		Approved,
		Cancel,
	}

	public class RequestItemConsumable
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int RequestConsumableId {get; set;}
		public int? OrderItemConsumableId {get; set;}
		public virtual OrderItemConsumable? OrderItemConsumable { get; set; }
		public int ItemConsumableId { get; set; }
		public virtual ItemConsumable? ItemConsumable { get; set; }

		[ForeignKey(nameof(User))]
		public string UserId { get; set; } = null!;
		public virtual User? User { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime CreateAt { get; set; } = DateTime.Now;

		[Required]
		[DataType(DataType.Date)]
		public DateTime RequestConsumeDate { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive number !")]
		public int Quantity {get; set;}
		public string NoteRequest { get; set; } = ""; //di isi ketika user ngajukan request
		public string? NoteActionRequest { get; set; }  //di isi ketika admin melakukan aksi reject atau approved
		public RequestItemConsumableStatus Status { get; set; } = RequestItemConsumableStatus.WaitingApproval;

	}
}