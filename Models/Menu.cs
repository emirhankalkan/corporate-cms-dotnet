using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models;

public class Menu
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Url { get; set; } = string.Empty;
    
    public int? ParentId { get; set; }
    
    public int Order { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public bool IsExternal { get; set; } = false;
    
    public string? CssClass { get; set; }
    
    public string? Icon { get; set; }
    
    // Navigation properties
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
