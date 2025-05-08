using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Models.Casting
{
    public class State
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }

        public virtual Country? Country { get; set; }

        public virtual ICollection<City>? Cities { get; set; }
    }
}
