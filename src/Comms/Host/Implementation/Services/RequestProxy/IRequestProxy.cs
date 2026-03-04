using System.Threading;
using System.Threading.Tasks;

namespace Comms.Host.Implementation.Services.RequestProxy;

public interface IRequestProxy<in TIn, TRes>
{
    Task<TRes> SendAsync(TIn input, CancellationToken token);
}