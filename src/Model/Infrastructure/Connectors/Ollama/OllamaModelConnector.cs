using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Model.Application.Interfaces;
using Model.Domain.Enums;
using Model.Domain.ValueObjects;

namespace Model.Infrastructure.Connectors.Ollama;

public class OllamaModelConnector(OllamaModels model) : IModelConnector
{
    public Stream Generate(Prompt prompt)
    {
        var content = PromptToStringContent(prompt);
        return TryPostAsyncStream(content).GetAwaiter().GetResult().Stream; //this does not work
    }

    public Response GetResponse(Prompt prompt)
    {
        var content = PromptToStringContent(prompt);
        var response = GetCompleteResponseAsync(content).GetAwaiter().GetResult();
        return response;
    }

    private static async Task<Response> GetCompleteResponseAsync(StringContent content)
    {
        var responses = new List<OllamaResponseDto>();
        var builder = new StringBuilder();
        var options = new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
        using (var managedStream = await TryPostAsyncStream(content))
        {
            using (var reader = new StreamReader(managedStream.Stream))
            {
                string nextLine;
                while ((nextLine = await reader.ReadLineAsync()) != null)
                {
                    var jsonDocument = JsonDocument.Parse(nextLine);
                    var response = JsonSerializer.Deserialize<OllamaResponseDto>(jsonDocument, options);
                    responses.Add(response);
                    Debug.Write(response.Text);
                    builder.Append(response.Text);
                }
            }
        }

        return new Response()
        {
            Text = builder.ToString(),
            Context = responses.Last().Context
        };
    }

    private static async Task<ManagedStream> TryPostAsyncStream(StringContent content)
    {
        ManagedStream stream = new();
        var client = new HttpClient();
        stream.ManagedResources.Add(client);
        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/generate");
        stream.ManagedResources.Add(request);
        request.Content = content;
        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead); //why not use PostAsync?
        stream.ManagedResources.Add(response);

        if (!response.IsSuccessStatusCode)
            throw new(); //TODO: throw designated exception

        stream.Stream = await response.Content.ReadAsStreamAsync();
        return stream;
    }
    
    private static async Task HandleStream(Stream stream)
    {
        using (StreamReader reader = new StreamReader(stream))
        {
            string nextLine;
            while ((nextLine = await reader.ReadLineAsync()) != null)
            {
                //Console.WriteLine(nextLine);
                var jsonDocument = JsonDocument.Parse(nextLine);
                Console.Write(jsonDocument.RootElement.GetProperty("response").GetString());
            }
        }
    }

    private StringContent PromptToStringContent(Prompt prompt)
    {
        var dto = new OllamaGenerateDto()
        {
            Prompt = prompt.Text,
            Stream = false,
            Model = "mistral" //TODO: use enum
        };

        if (prompt.Context != null && prompt.Context.Count != 0)
            dto.Context = prompt.Context.ToArray();
        if (prompt.Images != null && prompt.Images.Length != 0)
            dto.Images = prompt.Images;

        var options = new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
        return new(JsonSerializer.Serialize(dto, options), Encoding.UTF8, "application/json");
    }
}