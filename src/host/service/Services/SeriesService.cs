using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class SeriesService(ILogger<DatabaseService> logger) : Series.SeriesBase
{
    public override Task<ListSeriesReply> List(ListSeriesBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<ListSeriesReply>(
            logger, RequestType.ListSeries, request, ListSeriesReply.Parser, context.CancellationToken);
}