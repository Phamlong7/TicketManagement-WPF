using System;
using System.ComponentModel.DataAnnotations;

namespace Fstore.DAL.Models
{
    public class Title
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title name is required.")]
        [StringLength(100, ErrorMessage = "Title name cannot be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty; // Make it non-nullable and initialize
    }
}
