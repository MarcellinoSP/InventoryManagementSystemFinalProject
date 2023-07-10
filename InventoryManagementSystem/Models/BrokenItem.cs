using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Models
{
    public enum BrokenItemStatus
    {
        Active,
        Resolve
    }

    public class BrokenItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BrokenId { get; set; }

        public int ItemId { get; set; }
        public virtual Item? Item { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual User? User { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateAt { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BrokenDate { get; set; }
        public string? NoteItemBroken { get; set; }
        public string? NoteItemFound { get; set; }

        public int? BorrowedId { get; set; }
        public virtual BorrowedItem? BorrowedItem { get; set; }

        public BrokenItemStatus Status { get; set; }

    }
}