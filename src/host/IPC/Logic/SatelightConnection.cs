using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface;
using Comms.Common.Interface.Models;
using Common.Utility.Models;
using Comms.Common.Implementation.Services;
using Satelight.Protos.Core;
using Piped;
using ProtoBuf;
using RequestType = Piped.RequestType;

namespace Comms.Common.Implementation;

public abstract class SatelightConnection(NamedPipeServerStream stream)
    : ISatelightConnection
{
    public void Dispose() => stream.Dispose();

    public ValueTask DisposeAsync()
    {
        stream.Dispose();
        return new();
    }

    public async Task<SatelightRequest> ReadRequestAsync(CancellationToken token)
    {
        Body request;
        using (var memoryStream = new MemoryStream())
        {
            var buffer = new byte[1024];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
            memoryStream.Write(buffer, 0, bytesRead);
            memoryStream.Position = 0;
            request = Serializer.Deserialize<Body>(memoryStream);
        }
        return GetRequest(request);
    }

    private SatelightRequest GetRequest(Body request)
    {
        using MemoryStream paramStream = new(request.Parameters ?? []);
        return Map(request.Type, paramStream);
    }

    protected virtual SatelightRequest Map(RequestType type, MemoryStream paramStream)
    {
        ModelMapper mapper = new();
        return type switch
        {
            RequestType.Init          => mapper.Map(Serializer.Deserialize         <InitBody>(paramStream)),
            RequestType.GetCache      => mapper.Map(Serializer.Deserialize     <GetCacheBody>(paramStream)),
            RequestType.GetOp         => mapper.Map(Serializer.Deserialize        <GetOpBody>(paramStream)),
            RequestType.ListOps       => mapper.Map(Serializer.Deserialize      <ListOpsBody>(paramStream)),
            RequestType.ListTags      => mapper.Map(Serializer.Deserialize     <ListTagsBody>(paramStream)),
            RequestType.ListPlatforms => mapper.Map(Serializer.Deserialize<ListPlatformsBody>(paramStream)),
            RequestType.ListGenres    => mapper.Map(Serializer.Deserialize   <ListGenresBody>(paramStream)),
            RequestType.ListLibraries => mapper.Map(Serializer.Deserialize<ListLibrariesBody>(paramStream)),
            RequestType.ListSeries    => mapper.Map(Serializer.Deserialize   <ListSeriesBody>(paramStream)),
            RequestType.ListFeatures  => mapper.Map(Serializer.Deserialize <ListFeaturesBody>(paramStream)),
            RequestType.ListCompanies => mapper.Map(Serializer.Deserialize<ListCompaniesBody>(paramStream)),
            _                         => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task SendResponseAsync<T>(T response, CancellationToken token) where T : SatelightResponse
    {
        var reply = Map(response);
        byte[] data;
        using (MemoryStream memoryStream = new())
        {
            Serializer.Serialize(memoryStream, reply);
            data = memoryStream.ToArray();
        }

        if (response is StreamGamesResponse) 
            await stream.WriteAsync(BitConverter.GetBytes(data.Length), 0, 4, token);
        await stream.WriteAsync(data, 0, data.Length, token);
    }

    protected virtual object Map<T>(T response) where T : SatelightResponse
    {
        ModelMapper mapper = new();
        return response switch
        {
            InitializeResponse           initResponse => mapper.Map        (initResponse),
            GetCacheResponse         getCacheResponse => mapper.Map    (getCacheResponse),
            GetOpResponse               getOpResponse => mapper.Map       (getOpResponse),
            GetOpsResponse             getOpsResponse => mapper.Map      (getOpsResponse),
            GetTagsResponse           getTagsResponse => mapper.Map     (getTagsResponse),
            GetPlatformsResponse getPlatformsResponse => mapper.Map(getPlatformsResponse),
            GetGenresResponse       getGenresResponse => mapper.Map   (getGenresResponse),
            GetLibrariesResponse getLibrariesResponse => mapper.Map(getLibrariesResponse),
            GetSeriesResponse       getSeriesResponse => mapper.Map   (getSeriesResponse),
            GetFeaturesResponse   getFeaturesResponse => mapper.Map (getFeaturesResponse),
            GetCompaniesResponse getCompaniesResponse => mapper.Map(getCompaniesResponse),
            _                                         => throw new ArgumentOutOfRangeException()
        };
    }

    public Conditional IsConnected { get; } = new FuncConditional(() => stream.IsConnected);
}