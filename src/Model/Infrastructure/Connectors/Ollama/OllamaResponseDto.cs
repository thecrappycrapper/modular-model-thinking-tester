using System.Text.Json.Serialization;

namespace Model.Infrastructure.Connectors.Ollama;

public class OllamaResponseDto
{
    [JsonPropertyName("model")]
    public string Model { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("response")]
    public string Text { get; set; }
    [JsonPropertyName("done")]
    public bool IsDone { get; set; }
    [JsonPropertyName("done_reason")]
    public string? DoneReason { get; set; }
    [JsonPropertyName("context")]
    public int[] Context { get; set; }
    //missing duration properties
}