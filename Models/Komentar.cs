using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Komentar
    {
        [Required]
        public string Id { get; set; } = $"kometar:{Guid.NewGuid().ToString()}";
        [Required]
        public string Tekst { get; set; }

        [Required]
        public string KorisnikId { get; set; }
    }
}