using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Korisnik
    {

        [Required]
        public string Id { get; set; } // ovo je mail a samim tim i id jer je unique

        //[Required]
        // public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public bool Procitano {get; set;}
    }
}