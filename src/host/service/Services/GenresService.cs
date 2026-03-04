using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class GenresService(ILogger<GenresService> logger) : Genres.GenresBase
{
    public override Task<ListGenresReply> List(ListGenresBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<ListGenresReply>(
            logger, RequestType.ListGenres, request, ListGenresReply.Parser, context.CancellationToken);
}