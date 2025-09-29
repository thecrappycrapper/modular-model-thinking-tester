using System.Collections.Generic;

namespace Model.Domain.ValueObjects;

public class Prompt
{
    public string Text { get; set; }
    public string[] Images { get; set; }
    public ICollection<int> Context { get; set; }
}