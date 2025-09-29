using System;
using System.Collections.Generic;
using System.IO;

namespace Model.Domain.ValueObjects;

public class ManagedStream : IDisposable
{
    public Stream Stream { get; set; }
    public ICollection<IDisposable> ManagedResources { get; } = new List<IDisposable>();

    public void Dispose()
    {
        foreach (var resource in ManagedResources)
            resource.Dispose();
    }
}