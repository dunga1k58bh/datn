using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


[Route("portal")]
[SecurityHeaders]
[Authorize]
public class PortalController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public PortalController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager
    )
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {

        var id = _userManager.GetUserId(User); 
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            // Handle the case where the user is not found
            return NotFound();
        }

        // Get the roles associated with the user
        var roleNames = await _userManager.GetRolesAsync(user);
        var roleIds = new List<string>();
        foreach (var roleName in roleNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            
            if (role != null)
            {
                roleIds.Add(role.Id);
            }
        }

        // Get the clients associated with the user's roles through RoleClientGrant
        var clientIds = _context.RoleClientGrants
            .Where(rcg => roleIds.Contains(rcg.RoleId))
            .Select(rcg => rcg.ClientId)
            .Distinct()
            .ToList();

        // Get the actual Client objects
        var clients = _context.Clients
            .Where(client => clientIds.Contains(client.Id))
            .ToList();

        var vm = new PortalViewModel{
            Clients = clients
        };

        return View(vm);
    }



}
