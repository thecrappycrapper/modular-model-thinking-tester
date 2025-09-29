using System.IO;

namespace Model.Application.Interfaces;

public interface IModelController
{
    Stream Think(string prompt, params string[] base64images);
    Stream SearchInternet(string prompt, params string[] base64images);
    Stream SearchKnowledgeGraph(string prompt, params string[] base64images);
}