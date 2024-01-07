using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("roles")]
[SecurityHeaders]
[Authorize]
public class RoleController : Controller
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public RoleController(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _context = context;
    }

    // GET: Role
    public IActionResult Index()
    {
        var roles = _roleManager.Roles;
        return View(roles);
    }

    // GET: Role/Create
    [Route("create")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Role/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("create")]
    public async Task<IActionResult> Create(ApplicationRole role)
    {
        if (ModelState.IsValid)
        {
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(role);
    }

    // GET: Role/Edit/5
    [Route("edit")]
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return View(role);
    }

    // POST: Role/Edit/5
    [HttpPost]
    [Route("edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, ApplicationRole role)
    {
        if (id != role.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _roleManager.UpdateAsync(role);
            }
            catch (Exception)
            {
                if (!_roleManager.RoleExistsAsync(role.Name).Result)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        return View(role);
    }

    // GET: Role/Delete/5
    [Route("delete")]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return View(role);
    }

    // POST: Role/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        await _roleManager.DeleteAsync(role);
        return RedirectToAction(nameof(Index));
    }


    // GET: Role/Details/5
    [Route("details")]
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var role = await _roleManager.FindByIdAsync(id);
        var userIds = GetUsersForRole(role.Id).Select(user => user.Id).ToList();
        var allUsers = _context.Users.ToList();

        var vm = new RoleDetailsViewModel{
            Role = role,
            UserIds = userIds,
            Users = allUsers,
        };

        return View(vm);
    }

    private List<ApplicationUser> GetUsersForRole(string roleId)
    {
        // Assuming you have a DbSet<RoleClientGrant> in your ApplicationDbContext
        var userRoles = _context.UserRoles
            .Where(rcg => rcg.RoleId == roleId)
            .ToList();

        // Extract the role IDs from the RoleClientGrants
        var userIds = userRoles.Select(rcg => rcg.UserId).ToList();

        // Retrieve the roles based on the extracted role IDs
        var users = _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToList();

        return users;
    }

    
}
