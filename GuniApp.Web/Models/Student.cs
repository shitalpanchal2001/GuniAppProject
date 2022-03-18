using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuniApp.Web.Models
{
    [Table("Students")]
    public class Student
    {
        /// <summary>
        ///     Mapped to the ID column of the Identity User
        /// </summary>
        [Display(Name = "User ID")]
        [Key]
        [ForeignKey(nameof(Student.User))]
        public Guid UserId { get; set; }


        [Display(Name = "Student Enrollment ID")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [StringLength(10, ErrorMessage = "{0} should contain {1} characters.")]
        [MinLength(10, ErrorMessage = "{0} should contain {1} characters.")]
        public string EnrollmentID { get; set; }
        

        [Display(Name = "Description")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string ProjectDescription { get; set; }

        [Display(Name ="Document")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string document { get; set; }

        [Display(Name = "StartDate")]
        [Required]                            
        [Column(TypeName = "smalldatetime")]
        public DateTime Startdate { get; set; }

        [Display(Name = "EndDate")]
        [Required]
        [Column(TypeName = "smalldatetime")]
        public DateTime Enddate { get; set; }

        [Display(Name = "GroupDetails")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [StringLength(10, ErrorMessage = "{0} should contain {1} characters.")]
        [MinLength(10, ErrorMessage = "{0} should contain {1} characters.")]
        public string GroupDetails { get; set; }

        [Display(Name = "Comments")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [StringLength(1000, ErrorMessage = "{0} should contain {1} characters.")]
        public string Comment { get; set; }


        #region Navigational Properties to the MyIdentityUser model (1:1 mapping)

        public MyIdentityUser User { get; set; }

        #endregion

    }
}
