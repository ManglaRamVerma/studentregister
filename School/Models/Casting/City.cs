using Microsoft.AspNetCore.Mvc;
using School.Data;
using School.Models.Casting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Models.Casting
{
    public class City
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [ForeignKey("State")]
        public int StateId { get; set; }

        public virtual State? State { get; set; }
    }
}


