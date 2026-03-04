using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class LibrariesService(ILogger<LibrariesService> logger) : Libraries.LibrariesBase
{
    public override Task<ListLibrariesReply> List(ListLibrariesBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<ListLibrariesReply>(
            logger, RequestType.ListLibraries, request, ListLibrariesReply.Parser, context.CancellationToken);
}