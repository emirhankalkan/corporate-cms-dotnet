using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models.ViewModels;

public class MenuViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Menü başlığı zorunludur.")]
    [StringLength(100, ErrorMessage = "Başlık maksimum 100 karakter olabilir.")]
    [Display(Name = "Menü Başlığı")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "URL maksimum 500 karakter olabilir.")]
    [Display(Name = "URL")]
    public string Url { get; set; } = string.Empty;
    
    [Display(Name = "Üst Menü")]
    public int? ParentId { get; set; }
    
    [Display(Name = "Sıralama")]
    public int Order { get; set; } = 0;
    
    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
    
    [Display(Name = "Dış Link")]
    public bool IsExternal { get; set; } = false;
    
    [Display(Name = "CSS Sınıfı")]
    public string? CssClass { get; set; }
    
    [Display(Name = "İkon")]
    public string? Icon { get; set; }
    
    public List<SelectListItem> ParentMenus { get; set; } = new();
}

public class SelectListItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; } = false;
}
