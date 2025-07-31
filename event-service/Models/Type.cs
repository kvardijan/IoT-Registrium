using System;
using System.Collections.Generic;

namespace event_service.Models;

public partial class Type
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
