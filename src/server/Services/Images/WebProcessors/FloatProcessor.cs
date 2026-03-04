namespace Server.Services.Images.WebProcessors;

public abstract class FloatProcessor : SingleCmdProcessor<float>
{
    protected override bool TryParse(string str, out float value) => float.TryParse(str, out value);
}