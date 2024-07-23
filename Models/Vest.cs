using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Vest
    {
        [Required]
        public string Id { get; set; } = $"vest:{Guid.NewGuid().ToString()}";

        [Required]
        public string Naslov { get; set; }
        [Required]
        public string KratakTekst { get; set; }
        [Required]
        public string DuziTekst { get; set; }
        public string Slika { get; set; }
        [Required]
        public DateTime DatumObjavljivanja { get; set; }

        [Required]
        public string KategorijaID { get; set; }

    }
}
