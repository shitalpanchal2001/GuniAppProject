using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using GuniApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using GuniApp.Web.Data;

namespace GuniApp.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterStudentModel : PageModel
    {
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public RegisterStudentModel(
            ApplicationDbContext context,
            UserManager<MyIdentityUser> userManager,
            SignInManager<MyIdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Display Name")]
            [Required(ErrorMessage = "{0} cannot be empty.")]
            [MinLength(2, ErrorMessage = "{0} should have at least {1} characters.")]
            [StringLength(60, ErrorMessage = "{0} cannot have more than {1} characters.")]
            public string DisplayName { get; set; }

            [Display(Name = "Date of Birth")]
            [Required]
            public DateTime DateOfBirth { get; set; }

            [Display(Name = "Is Admin User?")]
            [Required]
            public bool IsAdminUser { get; set; }


            [Display(Name = "Student Enrollment ID")]
            [Required(ErrorMessage = "{0} cannot be empty.")]
            [StringLength(10, ErrorMessage = "{0} should contain {1} characters.")]
            [MinLength(10, ErrorMessage = "{0} should contain {1} characters.")]
            public string EnrollmentID { get; set; }


            [Display(Name = "Name of the Faculty")]
            [ForeignKey(nameof(Faculty.User))]
            public string FacultyName { get; set; }

            [Display(Name = "Description")]
            [Required(ErrorMessage = "{0} cannot be empty.")]
            public string ProjectDescription { get; set; }

            [Display(Name = "Document")]
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

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new MyIdentityUser { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    DisplayName = Input.DisplayName,
                    DateOfBirth = Input.DateOfBirth,
                    IsAdminUser = Input.IsAdminUser
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // insert the student information into the Students Table
                    var student = new Student
                    {
                        User = user,
                        UserId = user.Id,
                        EnrollmentID = Input.EnrollmentID,
                    };
                    _context.Students.Add(student);
                    _context.SaveChanges();

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
