using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuniApp.Web.Models
{
    [Table("Faculty")]
    public class Faculty
    {
        /// <summary>
        ///     Mapped to the ID column of the Identity User
        /// </summary>
        [Display(Name = "User ID")]
        [Key]
        [ForeignKey(nameof(Faculty.User))]
        public Guid UserId { get; set; }

        [Display(Name = "Type of Faculty")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [MinLength(3, ErrorMessage = "{0} should have more than {1} characters.")]
        [StringLength(25, ErrorMessage = "{0} cannot contain more than {1} characters.")]
        public string FacultyType { get; set; }


        #region Navigational Properties to the MyIdentityUser model (1:1 mapping)

        public MyIdentityUser User { get; set; }

        #endregion
    }
}
