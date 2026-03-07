using Comms.Common.Interface.Models;
using System.IO;

namespace HostPlugin.Services.RequestHandlers;

public abstract class ImageHandler<TReq> : RequestHandler<TReq> where TReq : SatelightRequest
{

    protected static readonly string BackgroundChangerJsonPath;
    protected static readonly string BackgroundChangerImagePath;

    static ImageHandler()
    {
        var backgroundChangerSubPath = Path.Combine("ExtensionsData", "3afdd02b-db6c-4b60-8faa-2971d6dfad2a");
        BackgroundChangerJsonPath = Path.Combine(backgroundChangerSubPath, "BackgroundChanger");
        BackgroundChangerImagePath = Path.Combine(backgroundChangerSubPath, "Images");
    }
}