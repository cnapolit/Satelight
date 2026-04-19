using Playnite;
using Plugin.Models;

namespace Plugin.Common.Extensions;

internal static class GameMediaFileExtensions
{
    public static MediaFileType GetMediaFileType(this GameMediaFile file) => file.Type switch
    {
        "Playnite.DesktopCover"      => MediaFileType.DesktopCover,
        "Playnite.DesktopBackground" => MediaFileType.DesktopBackground,
        "Playnite.DesktopIcon"       => MediaFileType.DesktopIcon,
        _ => default
    };
}
