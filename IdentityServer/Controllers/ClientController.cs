using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("clients")]
public class ClientController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClientController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Client
    public IActionResult Index()
    {
        var clients = _context.Clients.ToList();
        return View(clients);
    }

    [Route("details")]
    public IActionResult Details(int? id){

        if (id == null)
        {
            return NotFound();
        }

        var client = _context.Clients.Include(c => c.RedirectUris).FirstOrDefault(c => c.Id == id);
        var roleIds = GetRolesForClient(client.Id).Select(role => role.Id).ToList();
        var redirectUris = client.RedirectUris.Select(uri => uri.RedirectUri).ToList();
        var allRoles = _context.Roles.ToList();

        if (redirectUris == null){
            redirectUris = new List<string>();
        }

        var vm = new ClientDetailsViewModel{
            Client = client,
            RoleIds = roleIds,
            RedirectUris = redirectUris,
            Roles = allRoles,
        };

        return View(vm);
    }

    private List<ApplicationRole> GetRolesForClient(int clientId)
    {
        // Assuming you have a DbSet<RoleClientGrant> in your ApplicationDbContext
        var roleClientGrants = _context.RoleClientGrants
            .Where(rcg => rcg.ClientId == clientId)
            .ToList();

        // Extract the role IDs from the RoleClientGrants
        var roleIds = roleClientGrants.Select(rcg => rcg.RoleId).ToList();

        // Retrieve the roles based on the extracted role IDs
        var roles = _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .ToList();

        return roles;
    }


    // POST: Role/Edit/5
    [HttpPost]
    [Route("details")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Details(ClientDetailsViewModel vm)
    {
        if (ModelState.IsValid)
        {
            // Retrieve the existing client from the database
            var existingClient = _context.Clients.
                Include(c => c.RedirectUris).
                FirstOrDefault(c => c.Id == vm.Client.Id);

            var vmClient = vm.Client;

            if (existingClient != null)
            {
                // Update the properties of the existing client
                existingClient.ClientName = vm.Client.ClientName;
                existingClient.Description = vm.Client.Description;

                if (vmClient.AllowOfflineAccess){
                    existingClient.AllowOfflineAccess = true;
                }

                
                //Read RedirectUris
                existingClient.RedirectUris.Clear();

                // Create and add new RedirectUris based on the provided list
                foreach (var redirectUri in vm.RedirectUris)
                {
                    existingClient.RedirectUris.Add(new ClientRedirectUri { RedirectUri = redirectUri});
                }

                //Read user group
                var roleClientGrants = _context.RoleClientGrants
                    .Where(rcg => rcg.ClientId == existingClient.Id)
                    .ToList();
                
                // Remove existing roleClientGrants from the database
                _context.RoleClientGrants.RemoveRange(roleClientGrants);
                
                if (vm.RoleIds != null){
                    foreach (var id in vm.RoleIds){

                        var newRoleClientGrant = new RoleClientGrant
                        {
                            ClientId = existingClient.Id,
                            RoleId = id
                        };

                        // Add the new RoleClientGrant to the context
                       _context.RoleClientGrants.Add(newRoleClientGrant);
                    }
                }
                
                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Redirect to a success page or return a view with updated data
                return RedirectToAction("Index");
            }
            else
            {
                // Handle the case where the client with the specified ID is not found
                return NotFound();
            }
        }

        // If ModelState is not valid, return to the edit view with the ViewModel
        return View(vm);
    }
}
