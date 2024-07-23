using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Kategorija
    {
        [Required]
        public string Id { get; set; } = $"kategorija:{Guid.NewGuid().ToString()}";

        [Required]
        public string Naziv { get; set; }
    }
}