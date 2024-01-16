using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Stores;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("apiscopes")]
[SecurityHeaders]
[Authorize(Roles = "admin")]
public class ApiScopeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IClientStore _clients;

    public ApiScopeController(ApplicationDbContext context, IClientStore clients)
    {
        _context = context;
        _clients = clients;
    }

    // GET: Client
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        // Assuming _context is your DbContext
        var totalItems = await _context.ApiScopes.CountAsync();
        
        var apiScopes = await _context.ApiScopes
            .OrderBy(c => c.Id)  // Order by a property to ensure consistent results
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new PaginatedListViewModel<ApiScope>
        {
            Items = apiScopes,
            PageIndex = page,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
        };

        return View(model);
    }


    [Route("details")]
    public async Task<IActionResult> Details(int id){

        var apiScope = await _context.ApiScopes.FindAsync(id);
        return View(apiScope);
    }


    [Route("create")]
    public IActionResult Create(){

        var vm = new ApiScopeCreateViewModel();
        return View(vm);
    }


    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(ApiScopeCreateViewModel vm){

        if (_context.ApiScopes.Any(apiScope => apiScope.Name == vm.Name))
        {
            ModelState.AddModelError(nameof(vm.Name), "Name must be unique.");
        }
        else
        {
            var apiScope = new ApiScope
            {
                Name = vm.Name,
                DisplayName = vm.DisplayName,
                Description = vm.Description,
            };

            _context.ApiScopes.Add(apiScope);
            await _context.SaveChangesAsync();
        }

        return View(vm);
    }


    // POST: Role/Edit/5
    [HttpPost]
    [Route("details")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Details(ClientDetailsViewModel vm)
    {
        if (ModelState.IsValid)
        {
        
        }

        // If ModelState is not valid, return to the edit view with the ViewModel
        return View(vm);
    }


    // GET: User/Delete/5
    [Route("delete")]
    public async Task<IActionResult> Delete(int id)
    {

        var apiScope = await _context.ApiScopes.FindAsync(id);
        if (apiScope == null)
        {
            return NotFound();
        }

        return View(apiScope);
    }

    // POST: User/Delete/5
    [HttpPost, ActionName("Delete")]
    [Route("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var apiScope = await _context.ApiScopes.FindAsync(id);
        _context.ApiScopes.Remove(apiScope);
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
}
