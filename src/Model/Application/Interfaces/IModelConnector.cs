using System.IO;
using Model.Domain.ValueObjects;

namespace Model.Application.Interfaces;

public interface IModelConnector
{
    Stream Generate(Prompt prompt);
    Response GetResponse(Prompt prompt);
}