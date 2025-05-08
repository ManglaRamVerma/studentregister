using System.ComponentModel.DataAnnotations;

namespace School.Models.Casting
{
    public class Country
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        public virtual ICollection<State>? States { get; set; }
    }
}
