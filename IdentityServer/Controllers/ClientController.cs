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


[Route("clients")]
[SecurityHeaders]
[Authorize(Roles = "admin")]
public class ClientController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IClientStore _clients;

    public ClientController(ApplicationDbContext context, IClientStore clients)
    {
        _context = context;
        _clients = clients;
    }

    // GET: Client
    public IActionResult Index(int page = 1, int pageSize = 10)
    {
        // Assuming _context is your DbContext
        var totalItems = _context.Clients.Count();
        
        var clients = _context.Clients
            .OrderBy(c => c.Id)  // Order by a property to ensure consistent results
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new PaginatedListViewModel<Client>
        {
            Items = clients,
            PageIndex = page,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
        };

        return View(model);
    }

    [Route("details")]
    public async Task<IActionResult> Details(int id){

        var client = await _context.Clients.FindAsync(id);

        if (client != null)
        {
            _context.Entry(client).Collection(c => c.RedirectUris).Load();
            _context.Entry(client).Collection(c => c.ClientSecrets).Load();
        }

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


    [Route("create")]
    public IActionResult Create(){

        var vm = new ClientCreateViewModel();
        return View(vm);
    }


    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(ClientCreateViewModel vm){

        if (ModelState.IsValid){

            var clientId = GenerateClientId();
            var client = new IdentityServer4.Models.Client{
                ClientName = vm.ClientName,
                Description = vm.ClientDescription,
                ClientId = clientId,
                ClientUri = vm.ClientUri,
                BackChannelLogoutUri = vm.ClientUri + "/signout-callback-oidc",
                FrontChannelLogoutUri = vm.ClientUri + "/signout-oidc",
                AllowedGrantTypes = IdentityServer4.Models.GrantTypes.Code,
                AllowedScopes = {
                    "openid",
                    "profile",
                },
                ClientSecrets = { new IdentityServer4.Models.Secret(IdentityServer4.Models.HashExtensions.Sha256(generateClientSecret()))},
                RedirectUris = {vm.ClientUri + "/signin-oidc"}
            };

            var client_entity = client.ToEntity();
            await _context.Clients.AddAsync(client_entity);
            await _context.SaveChangesAsync();

            //Save change
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new {id = client_entity.Id});
        }
        

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
                existingClient.ClientUri = vm.Client.ClientUri;
                existingClient.Enabled = true;

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


    static string GenerateClientId()
    {
        // You can customize the format and length of the client ID as needed
        string prefix = "client_";
        string randomPart = Guid.NewGuid().ToString("N").Substring(0, 8); // Using the first 8 characters of a GUID for randomness

        return $"{prefix}{randomPart}";
    }


    static string generateClientSecret(){
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789~-";
        const string format = "xxxxxxxxxxxxxxxxxxxxxxxxxxxx~xxxxxxxxxxxxGQ4x66oMYYeNSP-8";

        StringBuilder result = new StringBuilder(format.Length);

        Random random = new Random();

        foreach (char c in format)
        {
            if (c == 'x')
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }


    [HttpPost]
    [Route("generate-secret")]
    public IActionResult RegenerateClientSecret(int ClientId)
    {
        // Assuming a method in your service to regenerate the client secret
        var newClientSecret = generateClientSecret();
        var Secret = new ClientSecret()
        {
            ClientId = ClientId,
            Value = IdentityServer4.Models.HashExtensions.Sha256(newClientSecret)
        };

        _context.ClientSecrets.Add(Secret);
        _context.SaveChanges();

        // Returning the new client secret as JSON
        return Json(new { ClientSecret = newClientSecret });
    }


    // GET: User/Delete/5
    [Route("delete")]
    public async Task<IActionResult> Delete(int id)
    {

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        if (client == null)
        {
            return NotFound();
        }

        return View(client);
    }

    // POST: User/Delete/5
    [HttpPost, ActionName("Delete")]
    [Route("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
}
