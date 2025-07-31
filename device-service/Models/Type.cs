using System;
using System.Collections.Generic;

namespace device_service.Models;

public partial class Type
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
