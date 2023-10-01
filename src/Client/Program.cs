using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;


var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}