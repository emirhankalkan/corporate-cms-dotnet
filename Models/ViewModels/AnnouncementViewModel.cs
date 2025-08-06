using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models.ViewModels;

public class AnnouncementViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Duyuru başlığı zorunludur.")]
    [StringLength(200, ErrorMessage = "Başlık maksimum 200 karakter olabilir.")]
    [Display(Name = "Duyuru Başlığı")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Özet maksimum 500 karakter olabilir.")]
    [Display(Name = "Kısa Özet")]
    public string? Summary { get; set; }
    
    [Required(ErrorMessage = "İçerik alanı zorunludur.")]
    [Display(Name = "Duyuru İçeriği")]
    public string Content { get; set; } = string.Empty;
    
    [Display(Name = "Duyuru Resmi")]
    public IFormFile? ImageFile { get; set; }
    
    public string? CurrentImagePath { get; set; }
    
    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
    
    [Display(Name = "Sabitlenmiş")]
    public bool IsPinned { get; set; } = false;
    
    [Display(Name = "Yayınlanma Tarihi")]
    [DataType(DataType.DateTime)]
    public DateTime? PublishDate { get; set; }
    
    [StringLength(200, ErrorMessage = "Etiketler maksimum 200 karakter olabilir.")]
    [Display(Name = "Etiketler (virgül ile ayırın)")]
    public string? Tags { get; set; }
}
