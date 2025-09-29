using System.Text.Json.Serialization;

namespace Model.Infrastructure.Connectors.Ollama;

public class OllamaGenerateDto
{
    /// <summary>
    /// (required) the model name
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }
    /// <summary>
    /// the prompt to generate a response for
    /// </summary>
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }
    /// <summary>
    /// the text after the model response
    /// </summary>
    [JsonPropertyName("suffix")]
    public string Suffix { get; set; }
    /// <summary>
    /// (optional) a list of base64-encoded images (for multimodal models such as llava)
    /// </summary>
    [JsonPropertyName("images")]
    public string[] Images { get; set; }
    /// <summary>
    /// the format to return a response in. Format can be json or a JSON schema
    /// </summary>
    [JsonPropertyName("format")]
    public object Format { get; set; }
    /// <summary>
    /// additional model parameters listed in the documentation for the Modelfile such as temperature
    /// </summary>
    [JsonPropertyName("options")]
    public string Options { get; set; }
    /// <summary>
    /// system message to (overrides what is defined in the Modelfile)
    /// </summary>
    [JsonPropertyName("system")]
    public string System { get; set; }
    /// <summary>
    /// the prompt template to use (overrides what is defined in the Modelfile)
    /// </summary>
    [JsonPropertyName("template")]
    public string Template { get; set; }
    /// <summary>
    /// if false the response will be returned as a single response object, rather than a stream of objects
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = true;
    /// <summary>
    /// if true no formatting will be applied to the prompt. You may choose to use the raw parameter if you are specifying a full templated prompt in your request to the API
    /// </summary>
    [JsonPropertyName("raw")]
    public bool Raw { get; set; } = false;
    /// <summary>
    /// controls how long the model will stay loaded into memory following the request (default: 5m)
    /// </summary>
    [JsonPropertyName("keep_alive")]
    public bool KeepAlive { get; set; }
    /// <summary>
    /// (deprecated): the context parameter returned from a previous request to /generate, this can be used to keep a short conversational memory
    /// </summary>
    [JsonPropertyName("context")]
    public int[] Context { get; set; }
}