
using System.ComponentModel.DataAnnotations.Schema;

namespace Timetracker.Models;

public sealed class TimeSlot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TrackedTaskId { get; set; }
    public TrackedTask? Task { get; set; }
    public DateTime StartedAtUtc { get; set; } 
    public DateTime? EndedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAtUtc { get; set; }

    [NotMapped]
    public TimeSpan? Duration => EndedAtUtc is null ? null : EndedAtUtc - StartedAtUtc;
}