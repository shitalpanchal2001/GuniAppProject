
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GuniApp.Web.Models.Enums
{
    public enum MyIdentityRoleTypes
    {
        [Display(Name = "Student")]
        Student,

        [Display(Name = "Faculty")]
        Faculty
    }
}
