using System.IO.Pipes;
using Comms.Common.Implementation;
using Comms.Host.Interface;

namespace Comms.Host.Implementation;

public class HostListener : SatelightListener<IHostConnection>, IHostListener
{
    protected override string PipeName => "SatelightHost";

    protected override IHostConnection CreateConnection(NamedPipeServerStream stream) => new HostConnection(stream);
}