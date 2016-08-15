using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Chamber.Domain.DomainModel;

namespace Chamber.Web.ViewModels
{
    public class MemberAddViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int MinPasswordLength { get; set; }

        public string[] Roles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public StatesViewModel _stateViewModel { get; set; }

        public IList<MembershipRole> AllRoles { get; set; }
        public string ReturnUrl { get; set; }
        public string UserAccessToken { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public Guid? Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; }
    }

    public class MemberFrontEndEditViewModel
    {
        [Required]
        public Guid Id { get; set; }
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string DisplayName { get; set; }

        //[EmailAddress]
        //[Required]
        //public string Email { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }
        public StatesViewModel _stateViewModel { get; set; }
        //[Url]
        //[StringLength(100)]
        //public string Website { get; set; }
        public HttpPostedFileBase[] Files { get; set; }


        public string Avatar { get; set; }
        public string role { get; set; }

        //added this for side details.
        public MembershipUser user { get; set; }

    }


    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}