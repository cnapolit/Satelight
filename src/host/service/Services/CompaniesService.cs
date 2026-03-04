using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class CompaniesService(ILogger<CompaniesService> logger) : Companies.CompaniesBase
{
    public override Task<ListCompaniesReply> List(ListCompaniesBody request, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.ListCompanies, request, ListCompaniesReply.Parser, context.CancellationToken);
}