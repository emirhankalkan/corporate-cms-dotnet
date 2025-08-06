using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models.ViewModels;

public class PageViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Başlık alanı zorunludur.")]
    [StringLength(200, ErrorMessage = "Başlık maksimum 200 karakter olabilir.")]
    [Display(Name = "Sayfa Başlığı")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(200, ErrorMessage = "URL maksimum 200 karakter olabilir.")]
    [Display(Name = "URL (Slug)")]
    public string Slug { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "İçerik alanı zorunludur.")]
    [Display(Name = "Sayfa İçeriği")]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(160, ErrorMessage = "Meta açıklama maksimum 160 karakter olabilir.")]
    [Display(Name = "Meta Açıklama")]
    public string? MetaDescription { get; set; }
    
    [StringLength(100, ErrorMessage = "Meta anahtar kelimeler maksimum 100 karakter olabilir.")]
    [Display(Name = "Meta Anahtar Kelimeler")]
    public string? MetaKeywords { get; set; }
    
    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
    
    [Display(Name = "Ana Sayfa")]
    public bool IsHomePage { get; set; } = false;
}
