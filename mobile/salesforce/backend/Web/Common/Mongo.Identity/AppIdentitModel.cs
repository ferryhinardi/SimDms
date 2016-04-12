using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Common.Mongo.Identity
{
     public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
             var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
             return userIdentity;
        }
    }

     public class RoleViewModel
     {
         public string Id { get; set; }
         [Required(AllowEmptyStrings = false)]
         [Display(Name = "RoleName")]
         public string Name { get; set; }
     }

     public class EditUserViewModel
     {
         public string Id { get; set; }

         [Required(AllowEmptyStrings = false)]
         [Display(Name = "Email")]
         [EmailAddress]
         public string Email { get; set; }

         public IEnumerable<SelectListItem> RolesList { get; set; }
     }

     public class ExternalLoginConfirmationViewModel
     {
         [Required]
         [Display(Name = "Email")]
         public string Email { get; set; }
     }

     public class ExternalLoginListViewModel
     {
         public string ReturnUrl { get; set; }
     }

     public class SendCodeViewModel
     {
         public string SelectedProvider { get; set; }
         public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
         public string ReturnUrl { get; set; }
     }

     public class VerifyCodeViewModel
     {
         [Required]
         public string Provider { get; set; }

         [Required]
         [Display(Name = "Code")]
         public string Code { get; set; }
         public string ReturnUrl { get; set; }

         [Display(Name = "Remember this browser?")]
         public bool RememberBrowser { get; set; }
     }

     public class ForgotViewModel
     {
         [Required]
         [Display(Name = "Email")]
         public string Email { get; set; }
     }

     public class LoginViewModel
     {
         [Required]
         [Display(Name = "Email")]
         [EmailAddress]
         public string Email { get; set; }

         [Required]
         [DataType(DataType.Password)]
         [Display(Name = "Password")]
         public string Password { get; set; }

         [Display(Name = "Remember me?")]
         public bool RememberMe { get; set; }
     }

     public class RegisterViewModel
     {
         [Required]
         [EmailAddress]
         [Display(Name = "Email")]
         public string Email { get; set; }

         [Required]
         [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
         [DataType(DataType.Password)]
         [Display(Name = "Password")]
         public string Password { get; set; }

         [DataType(DataType.Password)]
         [Display(Name = "Confirm password")]
         [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
         public string ConfirmPassword { get; set; }
     }

     public class ResetPasswordViewModel
     {
         [Required]
         [EmailAddress]
         [Display(Name = "Email")]
         public string Email { get; set; }

         [Required]
         [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
         [DataType(DataType.Password)]
         [Display(Name = "Password")]
         public string Password { get; set; }

         [DataType(DataType.Password)]
         [Display(Name = "Confirm password")]
         [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
         public string ConfirmPassword { get; set; }

         public string Code { get; set; }
     }

     public class ForgotPasswordViewModel
     {
         [Required]
         [EmailAddress]
         [Display(Name = "Email")]
         public string Email { get; set; }
     }
}