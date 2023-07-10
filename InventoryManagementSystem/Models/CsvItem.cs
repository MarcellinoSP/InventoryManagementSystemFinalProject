using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Models;
public class CsvItem
{
    public string KodeItem { get; set; }
    public string Name { get; set; }
    public string PicturePath { get; set; }
    public string Description { get; set; }
    public bool Availability { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreateAt { get; set; }
    public string CategoryCode { get; set; }
    public string CategoryName { get; set; }
    public string SubCategoryCode { get; set; }
    public string SubCategoryName { get; set; }
    public string CompanyName { get; set; }
    public string ContactNumber { get; set; }
    public string Address { get; set; }
    public string EmailCompany { get; set; }
}
public class CsvItemViewModel : Item
{
    public IFormFile? Picture { get; set; }
}