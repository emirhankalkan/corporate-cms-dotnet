using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.ViewModels
{
    public class UserDashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public string DisplayName { get; set; } = string.Empty;
        public System.DateTime? LastLoginAt { get; set; }
    }

    public class UserProfileViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = string.Empty;
        
        [MaxLength(50, ErrorMessage = "En fazla 50 karakter")]
        [Display(Name = "Görünen İsim")]
        public string? DisplayName { get; set; }
        
        [MaxLength(500, ErrorMessage = "En fazla 500 karakter")]
        [Display(Name = "Biyografi")]
        [DataType(DataType.MultilineText)]
        public string? Bio { get; set; }
        
        [MaxLength(256, ErrorMessage = "En fazla 256 karakter")]
        [Display(Name = "Avatar URL")]
        [Url(ErrorMessage = "Geçerli bir URL girin")]
        public string? AvatarUrl { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string? CurrentPassword { get; set; }
        
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string? NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
        public string? ConfirmPassword { get; set; }
    }
}
