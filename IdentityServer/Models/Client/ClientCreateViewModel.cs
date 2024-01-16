using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models{

    public class ClientCreateViewModel
    {
        [Required(ErrorMessage = "Application is required.")]
        public string ClientName {get; set;}
        public string ClientDescription {get; set; }

        [Url(ErrorMessage = "Must be a valid URI.")]
        [Required(ErrorMessage = "Application uri is required.")]
        public string ClientUri{get; set;}
    }


    public class ClientIdViewModel
{
    public int Id { get; set; }
}
}

