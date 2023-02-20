using System;
using System.Collections.Generic;

namespace Skybot.FactoidViewer.Models;

public partial class Validator
{
    public string ValidKey { get; set; } = null!;

    public byte[]? ValidValue { get; set; }
}
