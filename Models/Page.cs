using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models;

public class Page
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(160)]
    public string? MetaDescription { get; set; }
    
    [StringLength(100)]
    public string? MetaKeywords { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsHomePage { get; set; } = false;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public string? UpdatedBy { get; set; }
    
    public int ViewCount { get; set; } = 0;
}
