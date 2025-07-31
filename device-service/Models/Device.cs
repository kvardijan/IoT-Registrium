using System;
using System.Collections.Generic;

namespace device_service.Models;

public partial class Device
{
    public int Id { get; set; }

    public string SerialNumber { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string? Manufacturer { get; set; }

    public int Type { get; set; }

    public int Status { get; set; }

    public string FirmwareVersion { get; set; } = null!;

    public string? Location { get; set; }

    public DateTime? LastSeen { get; set; }

    public virtual Status StatusNavigation { get; set; } = null!;

    public virtual Type TypeNavigation { get; set; } = null!;
}
