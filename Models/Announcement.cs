using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models;

public class Announcement
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Slug { get; set; } // new slug for SEO friendly URLs
    
    [StringLength(500)]
    public string? Summary { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? ImagePath { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool IsPinned { get; set; } = false;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; }
    
    public DateTime? PublishDate { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public string? UpdatedBy { get; set; }
    
    public int ViewCount { get; set; } = 0;
    
    [StringLength(200)]
    public string? Tags { get; set; }
}
