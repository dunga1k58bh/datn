using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("clients")]
public class PortalController : Controller
{
    private readonly ApplicationDbContext _context;

    public PortalController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Client
    public IActionResult Index()
    {
        var clients = _context.Clients.ToList();
        return View(clients);
    }
}
