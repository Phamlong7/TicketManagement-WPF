using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fstore.DAL.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; } = string.Empty; // Make it non-nullable and initialize

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; } = string.Empty; // Make it non-nullable and initialize

        [Required(ErrorMessage = "Priority is required.")]
        [StringLength(20, ErrorMessage = "Priority cannot be longer than 20 characters.")]
        public string Priority { get; set; } = string.Empty; // Make it non-nullable and initialize

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot be longer than 20 characters.")]
        public string Status { get; set; } = string.Empty; // Make it non-nullable and initialize

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Set default to current UTC time

        // Foreign Key
        public int UserId { get; set; }

        // Navigation property
        public User? User { get; set; } // Make it non-nullable unless there's a specific reason for it to be nullable

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
