using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IdentityServer.Models;
using System.Linq;
using IdentityServer.Data;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;


[Route("users")]
public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UserController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _context = context;
        _roleManager = roleManager;
    }

    // GET: Users list
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var view_users = new List<UserWithClaimnsModel>();
        foreach (var user in users){

            var claims = await _userManager.GetClaimsAsync(user);
            var nameClaim = claims.FirstOrDefault(c => c.Type == "name");
            var name = nameClaim.Value;

            var userWithClaimnsModel = new UserWithClaimnsModel(){
                user = user,
                Name = name
            };


            view_users.Add(userWithClaimnsModel);
        }


        var vm = new UserListViewModel(){
            users = view_users
        };

        return View(vm);
    }

    // GET: User/Create
    [Route("create")]
    public IActionResult Create()
    {
        var vm = new UserCreateViewModel();
        return View(vm);
    }

    // POST: User/Create
    [Route("create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateViewModel vm)
    {
        if (ModelState.IsValid)
        {

            var user = new ApplicationUser
            {
                UserName = vm.UserName,
                Email = vm.Email,
                // Additional user properties, if any
            };

            IdentityResult result;

            if (vm.SetPassword){
                result = await _userManager.CreateAsync(user, vm.Password);
            } else {
                result = await _userManager.CreateAsync(user, "Pass123$123456");
            }

            if (result.Succeeded)
            {

                var name = vm.FirstName + " " + vm.LastName;

                await _userManager.AddClaimsAsync(user, new Claim[]{
                    new Claim(JwtClaimTypes.Name, name),
                    new Claim(JwtClaimTypes.GivenName, vm.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, vm.LastName),
                });

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(vm);
    }

    // GET: User/Edit/5
    [Route("edit")]
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: User/Edit/5
    [Route("edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, ApplicationUser user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_userManager.Users.Any(u => u.Id == user.Id))
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

        return View(user);
    }

    // GET: User/Delete/5
    [Route("delete")]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: User/Delete/5
    [HttpPost, ActionName("Delete")]
    [Route("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
    }


    [Route("details")]
    [HttpGet]
    public async Task<IActionResult> Details(string id){

        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        var roleIds = GetRolesForUser(user.Id).Select(role => role.Id).ToList();
        var allRoles = _context.Roles.ToList();

        var vm = new UserDetailsViewModel{
            User = user,
            RoleIds = roleIds,
            Roles = allRoles,
        };

        return View(vm);
    }


    private List<ApplicationRole> GetRolesForUser(string userId)
    {
        // Assuming you have a DbSet<RoleClientGrant> in your ApplicationDbContext
        var userRoles = _context.UserRoles
            .Where(rcg => rcg.UserId == userId)
            .ToList();

        // Extract the role IDs from the RoleClientGrants
        var roleIds = userRoles.Select(rcg => rcg.RoleId).ToList();

        // Retrieve the roles based on the extracted role IDs
        var roles = _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .ToList();

        return roles;
    }


     [HttpPost]
    [Route("details")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Details(UserDetailsViewModel vm)
    {
        if (ModelState.IsValid)
        {
            // Retrieve the existing client from the database
            var user = _context.Users.
                FirstOrDefault(c => c.Id == vm.User.Id);

            var vmUser = vm.User;

            if (user != null)
            {
                // Update the properties of the existing client
                user.UserName = vmUser.UserName;
                user.Email = vmUser.Email;


                //Read user group
                var userRoles = _context.UserRoles
                    .Where(rcg => rcg.UserId == user.Id)
                    .ToList();
                
                // Remove existing roleClientGrants from the database
                _context.UserRoles.RemoveRange(userRoles);
                
                if (vm.RoleIds != null){
                    foreach (var roleId in vm.RoleIds)
                    {
                        // Find the role by role ID
                        var role = await _roleManager.FindByIdAsync(roleId);

                        if (role != null)
                        {
                            // Add the user to the role
                            var result = await _userManager.AddToRoleAsync(user, role.Name);

                            if (!result.Succeeded)
                            {
                                // Handle the case where adding the user to the role failed
                                // You may choose to display an error message or take other actions
                                return View("Error");
                            }
                        }
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
