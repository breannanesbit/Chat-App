using System;
using System.Collections.Generic;

namespace Shared.Data;

public partial class MessageContainerLocation
{
    public int Id { get; set; }

    public int? MessageId { get; set; }

    public int? ContainerLocationId { get; set; }

    public virtual ContainerLocation? ContainerLocation { get; set; }

    public virtual Message? Message { get; set; }
}
