using System;
using System.Collections.Generic;

namespace Shared.Data;

public partial class ContainerLocation
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<MessageContainerLocation> MessageContainerLocations { get; set; } = new List<MessageContainerLocation>();
}
