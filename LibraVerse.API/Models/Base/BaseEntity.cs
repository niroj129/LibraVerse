using System.ComponentModel.DataAnnotations.Schema;

namespace LibraVerse.Models.Base;

public class BaseEntity
{
    public Guid Id { get; set; }

    public bool IsDeleted { get; set; } = false;
    
    public DateTime CreatedDate { get; set; }
    
    public Guid CreatedBy { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
    
    public Guid? UpdatedBy { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    
    public Guid? DeletedBy { get; set; }
    
    [ForeignKey(nameof(CreatedBy))]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey(nameof(UpdatedBy))]
    public virtual User? UpdatedByUser { get; set; }

    [ForeignKey(nameof(DeletedBy))]
    public virtual User? DeletedByUser { get; set; }
}