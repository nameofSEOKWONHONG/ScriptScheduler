using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScriptScheduler.Entity;

[Table(nameof(SchedulerInfo), Schema = "test")]
public class SchedulerInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    /// <summary>
    /// ENUM_REPEAT_TYPE
    /// </summary>
    [Required, MaxLength(10)]
    public string Mode { get; set; }
    
    [Required, MaxLength(4)]
    public string ExecuteTime { get; set; }
    
    [Required]
    public string RoleName { get; set; }
    
    [Required, MaxLength(3)]
    public string Extension { get; set; }
    
    [Required]
    public string FullPath { get; set; }
    
    [Required]
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime ModifyDate { get; set; }
}