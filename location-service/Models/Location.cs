using System;
using System.Collections.Generic;

namespace location_service.Models;

public partial class Location
{
    public int Id { get; set; }

    public string Latitude { get; set; } = null!;

    public string Longitude { get; set; } = null!;

    public string? Address { get; set; }

    public string? Description { get; set; }
}
