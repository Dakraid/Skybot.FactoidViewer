// Skybot.FactoidViewer - Factoid.cs
// Created on 2023.02.17
// Last modified at 2023.02.17 17:45

namespace Skybot.FactoidViewer.Models;

public class Factoid
{
    public string Key { get; set; } = null!;

    public string? CreatedBy { get; set; }

    public long? CreatedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public long? ModifiedAt { get; set; }

    public long? LockedAt { get; set; }

    public string? LockedBy { get; set; }

    public string? Fact { get; set; }

    public long? RequestedCount { get; set; }
}
