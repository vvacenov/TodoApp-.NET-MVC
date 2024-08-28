using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.VisualBasic;

namespace TodoApp.Models.Todos
{
    public class Todo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [ForeignKey("UserAccount")]
        public Guid OwnerId { get; set; }

        [Required(ErrorMessage = titleRequiredMessage)]
        [MaxLength(60, ErrorMessage = titleMaxLengthMessage)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = descriptionRequiredMessage)]
        [MaxLength(600, ErrorMessage = descriptionMaxLengthMessage)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = dueDateMessage)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }

        [Required]
        public string StatusId { get; set; } = "new";

        [ValidateNever]
        public Status Status { get; set; } = null!;

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        public bool Overdue => StatusId != "finished" && DueDate < DateTime.Today;

        private const string titleRequiredMessage = "Please enter a Title.";
        private const string titleMaxLengthMessage = "Title cannot exceed 60 characters.";
        private const string descriptionRequiredMessage = "Please enter a Description.";
        private const string descriptionMaxLengthMessage = "Description cannot exceed 600 characters.";
        private const string dueDateMessage = "Please enter a valid Due Date.";
    }
}