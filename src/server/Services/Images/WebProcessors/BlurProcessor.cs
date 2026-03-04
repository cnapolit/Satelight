using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Web;

namespace Server.Services.Images.WebProcessors;

public class BlurProcessor : FloatProcessor
{
    protected override void Process(FormattedImage image, float value) => image.Image.Mutate(x => x.GaussianBlur(value));

    protected override string Cmd => "blur";
}