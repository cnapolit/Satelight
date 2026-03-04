using Comms.Host.Implementation;
using Comms.Host.Interface;

namespace Comms.Host.Factory;

public static class HostFactory
{
    public static IHostClient   CreateClient(string ipAddress, int port) => new HostClient(ipAddress, port);
    public static IHostListener CreateListener()                         => new HostListener();
}
