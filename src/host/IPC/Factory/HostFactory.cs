using Comms.Host.Implementation;
using Comms.Host.Interface;

namespace Comms.Host.Factory;

public static class HostFactory
{
    public static IHostListener CreateListener() => new HostListener();
}
