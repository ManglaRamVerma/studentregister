using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Models.Casting
{
    public class StudentDetails
    {
        internal object? phone;

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required]
        [Phone]
        [StringLength(10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please select a country")]
        [ForeignKey("Country")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "Please select a state")]
        [ForeignKey("State")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "Please select a city")]
        [ForeignKey("City")]
        public int CityId { get; set; }

        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
        [Display(Name = "Profile Photo")]
        public IFormFile? ImageFile { get; set; }
        

        public string? ImagePath { get; set; }

        public virtual Country? Country { get; set; }
        public virtual State? State { get; set; }
        public virtual City? City { get; set; }
    }
}