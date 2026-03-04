using Microsoft.AspNetCore.Components;

namespace Server.Common.Extensions;

public static class EnumerableExtensions
{
    public static RenderFragment Render(this IEnumerable<RenderFragment> fragments) => builder =>
    {
        var i = 0;
        foreach (var fragment in fragments)
        {
            builder.AddContent(i++, fragment);
        }
    };
}
