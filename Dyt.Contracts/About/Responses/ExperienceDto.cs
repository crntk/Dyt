using System;

namespace Dyt.Contracts.About.Responses
{
    /// <summary>
    /// Deneyim/Özgeçmiþ için Response DTO
    /// </summary>
    public class ExperienceDto
    {
        public int Id { get; set; }
public string Position { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
   public string? Description { get; set; }
   public DateTime StartDate { get; set; }
 public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
public string Type { get; set; } = "Work"; // Work, Education, Internship
  public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
  public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
