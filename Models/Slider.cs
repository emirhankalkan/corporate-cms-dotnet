using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models;

public class Slider
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(500)]
    public string ImagePath { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Link { get; set; }
    
    public int Order { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public bool OpenInNewTab { get; set; } = false;
    
    [StringLength(100)]
    public string? ButtonText { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; }
}
