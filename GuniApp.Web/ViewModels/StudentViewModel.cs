using System;
using System.ComponentModel.DataAnnotations;

namespace GuniApp.Web.ViewModels
{
    public class StudentViewModel
    {
        [Key]
        [Display(Name = "Student User ID")]
        [Required]
        public Guid StudentUserId { get; set; }

        [Display(Name = "Date of Birth")]
        [Required]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Student Enrollment ID")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [StringLength(10, ErrorMessage = "{0} should contain {1} characters.")]
        [MinLength(10, ErrorMessage = "{0} should contain {1} characters.")]
        public string EnrollmentId { get; set; }

        [Display(Name = "Name of the Parent / Guardian")]
        [MinLength(2, ErrorMessage = "{0} should have at least {1} characters.")]
        [StringLength(60, ErrorMessage = "{0} should not contain more than {1} characters.")]
        public string ParentName { get; set; }
    }
}
