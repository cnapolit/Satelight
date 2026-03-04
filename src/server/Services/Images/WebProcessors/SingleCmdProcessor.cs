using System.Globalization;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Processors;

namespace Server.Services.Images.WebProcessors;

public abstract class SingleCmdProcessor<TVal> : IImageWebProcessor
{
    public FormattedImage Process(
        FormattedImage image, ILogger logger, CommandCollection commands, CommandParser parser, CultureInfo culture)
    {
        if (commands.TryGetValue(Cmd, out var strValue) && TryParse(strValue, out var value))
        {
            Process(image, value);
        }

        return image;
    }

    protected abstract bool TryParse(string str, out TVal value);
    protected abstract void Process(FormattedImage image, TVal value);

    public virtual bool RequiresTrueColorPixelFormat(CommandCollection commands, CommandParser parser, CultureInfo culture)
        => false;

    protected abstract string              Cmd      { get; }
    public             IEnumerable<string> Commands => [Cmd];
}