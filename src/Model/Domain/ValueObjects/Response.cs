using System.Collections.Generic;

namespace Model.Domain.ValueObjects;

public class Response
{
    public string Text { get; init; }
    public ICollection<int> Context { get; init; }
}