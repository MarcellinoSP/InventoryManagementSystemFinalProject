using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.Data;

public class SetVendorOnDatabase
{
	public static async void CreateVendorDataOnDatabase(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
			await SetVendorAccount(userManager);
			await SetNipseaVendor(userManager);
			await SetTriviaVendor(userManager);
		}
		static async Task SetVendorAccount(UserManager<User> userManager)
		{
			//membaut instance admin ke databse. permanen set
			var vendorEmail = "vendor@formulatrixbootcamp.com";
			var vendorPassword = "Vendor123@";
			var vendorUser = await userManager.FindByEmailAsync(vendorEmail);
			if (vendorUser == null)
			{
				vendorUser = new User
				{
					Email = vendorEmail,
					UserName = vendorEmail,
					PhoneNumber="088982928",
					IdEmployee = "Vendor01",
					FirstName = "Vendor",
					LastName = "Supplier",
					EmailConfirmed = true
				};
				//pembuatan akun agar memiliki role admin
				await userManager.CreateAsync(vendorUser, vendorPassword);
				await userManager.AddToRoleAsync(vendorUser, "Vendor");
			}
		}
		
		static async Task SetNipseaVendor(UserManager<User> userManager)
		{
			//membaut instance admin ke databse. permanen set
			var vendorEmail = "vendornipsea@formulatrixbootcamp.com";
			var vendorPassword = "Vendor123@";
			var vendorUser = await userManager.FindByEmailAsync(vendorEmail);
			if (vendorUser == null)
			{
				vendorUser = new User
				{
					Email = vendorEmail,
					UserName = vendorEmail,
					PhoneNumber="088982928",
					IdEmployee = "Vendor04",
					FirstName = "Vendor",
					LastName = "Nipsea",
					EmailConfirmed = true,
					HandleSupplierId = 4
				};
				//pembuatan akun agar memiliki role admin
				await userManager.CreateAsync(vendorUser, vendorPassword);
				await userManager.AddToRoleAsync(vendorUser, "Vendor");
			}
		}
		static async Task SetTriviaVendor(UserManager<User> userManager)
		{
			//membaut instance admin ke databse. permanen set
			var vendorEmail = "vendortrivia@formulatrixbootcamp.com";
			var vendorPassword = "Vendor123@";
			var vendorUser = await userManager.FindByEmailAsync(vendorEmail);
			if (vendorUser == null)
			{
				vendorUser = new User
				{
					Email = vendorEmail,
					UserName = vendorEmail,
					PhoneNumber="088982928",
					IdEmployee = "Vendor01",
					FirstName = "Vendor",
					LastName = "Trivia Nusantara",
					EmailConfirmed = true,
					HandleSupplierId = 1
				};
				//pembuatan akun agar memiliki role admin
				await userManager.CreateAsync(vendorUser, vendorPassword);
				await userManager.AddToRoleAsync(vendorUser, "Vendor");
			}
		}
}
