using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models.ViewModels;

public class SliderViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Slider başlığı zorunludur.")]
    [StringLength(200, ErrorMessage = "Başlık maksimum 200 karakter olabilir.")]
    [Display(Name = "Slider Başlığı")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Açıklama maksimum 500 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Resim seçimi zorunludur.")]
    [Display(Name = "Slider Resmi")]
    public IFormFile? ImageFile { get; set; }
    
    public string? CurrentImagePath { get; set; }
    
    [StringLength(500, ErrorMessage = "Link maksimum 500 karakter olabilir.")]
    [Display(Name = "Yönlendirilecek Link")]
    public string? Link { get; set; }
    
    [Display(Name = "Sıralama")]
    public int Order { get; set; } = 0;
    
    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
    
    [Display(Name = "Yeni Sekmede Aç")]
    public bool OpenInNewTab { get; set; } = false;
    
    [StringLength(100, ErrorMessage = "Buton metni maksimum 100 karakter olabilir.")]
    [Display(Name = "Buton Metni")]
    public string? ButtonText { get; set; }
}
