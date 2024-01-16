using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;

public class RoleClientGrant
{
    public int Id { get; set; }
    public string RoleId { get; set; }
    public int ClientId { get; set; }

    public ApplicationRole Role { get; set; }
    public Client Client {get; set; }
}