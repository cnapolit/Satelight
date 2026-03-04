using Grpc.Core;
using Piped;
using Satelight.Protos.Host;
using Service.Common;

namespace Service.Services;

public class ImagesService(ILogger<DatabaseService> logger) : Media.MediaBase
{
    public override Task<UpdateBackgroundReply> UpdateBackground(UpdateBackgroundBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.GetCache, body, UpdateBackgroundReply.Parser, context.CancellationToken);

    public override Task<UpdateIconReply> UpdateIcon(UpdateIconBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.GetCache, body, UpdateIconReply.Parser, context.CancellationToken);

    public override Task<UpdateCoverReply> UpdateCover(UpdateCoverBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.UpdateGameCover, body, UpdateCoverReply.Parser, context.CancellationToken);

    public override Task<UpdateLogoReply> UpdateLogo(UpdateLogoBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.GetCache, body, UpdateLogoReply.Parser, context.CancellationToken);

    public override Task<UpdateTrailerReply> UpdateTrailer(UpdateTrailerBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.GetCache, body, UpdateTrailerReply.Parser, context.CancellationToken);

    public override Task<UpdateMicroTrailerReply> UpdateMicroTrailer(UpdateMicroTrailerBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.GetCache, body, UpdateMicroTrailerReply.Parser, context.CancellationToken);
}
