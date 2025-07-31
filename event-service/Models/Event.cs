using System;
using System.Collections.Generic;

namespace event_service.Models;

public partial class Event
{
    public int Id { get; set; }

    public string? Device { get; set; }

    public int Type { get; set; }

    public string? Data { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Type TypeNavigation { get; set; } = null!;
}
