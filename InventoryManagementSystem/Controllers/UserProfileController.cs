using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
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

	public async Task<IActionResult> Index()
	{
		var userID = _userManager.GetUserId(this.User);
		User currentUser = _userManager.FindByIdAsync(userID).Result;
		return View(currentUser);
	}
	
	[HttpGet]
	public async Task<IActionResult> Edit()
	{
		var userID = _userManager.GetUserId(HttpContext.User);
		User currentUser = _userManager.FindByIdAsync(userID).Result;
		return View(currentUser);
	}
	
	[HttpPost]
	public async Task<IActionResult> Edit(User editUser)
	{
		var userID = _userManager.GetUserId(HttpContext.User);
		User currentUser = _userManager.FindByIdAsync(userID).Result;
		
		if(currentUser.FirstName != editUser.FirstName)
		{
			currentUser.FirstName = editUser.FirstName;
		}
		if(currentUser.LastName != editUser.LastName)
		{
			currentUser.LastName = editUser.LastName;
		}
		if(currentUser.IdEmployee != editUser.IdEmployee)
		{
			currentUser.IdEmployee = editUser.IdEmployee;
		}
		if(currentUser.PhoneNumber != editUser.PhoneNumber)
		{
			currentUser.PhoneNumber = editUser.PhoneNumber;
		}
		
		IdentityResult edit = await _userManager.UpdateAsync(currentUser);
		if(edit.Succeeded)
		{
			return RedirectToAction(nameof(Index));
		}
		return View(editUser);
	}
}