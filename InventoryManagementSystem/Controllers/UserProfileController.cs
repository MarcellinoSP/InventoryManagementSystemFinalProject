using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystem.Controllers;

[Authorize]
public class UserProfile : Controller
{
	private readonly UserManager<User> _userManager;
	public UserProfile(UserManager<User> userManager)
	{
		_userManager = userManager;
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var userID = _userManager.GetUserId(this.User);
		User currentUser = _userManager.FindByIdAsync(userID).Result;
		return View(currentUser);
	}
	
	public async Task<IActionResult> Edit()
	{
		var userID = _userManager.GetUserId(this.User);
		User currentUser = _userManager.FindByIdAsync(userID).Result;
		return View(currentUser);
	}
	
	// [HttpPost]
	// public async Task<IActionResult> Edit(User editUser)
	// {
	// 	IdentityResult edit = await _userManager.UpdateAsync(editUser);
	// 	if(edit.Succeeded)
	// 	{
	// 		return RedirectToAction(nameof(Index));
	// 	}
	// 	return View(editUser);
	// }
}