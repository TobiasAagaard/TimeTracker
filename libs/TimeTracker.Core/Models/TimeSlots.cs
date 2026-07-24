using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTracker.Core.Models;

public sealed class TimeSlots
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TrackedTaskId { get; set; }
    public TrackedTasks? Task { get; set; }
    public DateTime StartedAt { get; set; } 
    public DateTime? EndedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    [NotMapped]
    public TimeSpan? Duration => EndedAt is null ? null : EndedAt - StartedAt;
}