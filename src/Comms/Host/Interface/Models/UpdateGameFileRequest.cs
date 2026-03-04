using System;
using Comms.Common.Interface.Models;

namespace Comms.Host.Interface.Models;

public abstract class UpdateGameFileRequest : GameRequest
{
    public string FileName { get; set; } = string.Empty;
    public string NewPath  { get; set; } = string.Empty;
}