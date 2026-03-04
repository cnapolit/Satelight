using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Web;

namespace Server.Services.Images.WebProcessors;

public class SaturationProcessor : FloatProcessor
{

    protected override void Process(FormattedImage image, float value) => image.Image.Mutate(x => x.Saturate(value));

    protected override string Cmd => "saturate";
}