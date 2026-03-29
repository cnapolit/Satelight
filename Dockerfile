FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore
COPY src/server/Server.csproj server/
COPY src/Common/Utility.Core/Utility.Core.csproj Common/Utility.Core/
COPY src/Protos/ Protos/
RUN dotnet restore server/Server.csproj

# Copy source files
COPY src/server/ server/
COPY src/Common/Utility/ Common/Utility/
COPY src/Common/Utility.Core/ Common/Utility.Core/
# Recreate symlinks (Docker copies them as broken absolute paths)
RUN rm -f Common/Utility.Core/Functions Common/Utility.Core/Classes \
         Common/Utility.Core/Models Common/Utility.Core/Extensions \
 && ln -s /src/Common/Utility/Functions Common/Utility.Core/Functions \
 && ln -s /src/Common/Utility/Classes Common/Utility.Core/Classes \
 && ln -s /src/Common/Utility/Models Common/Utility.Core/Models \
 && ln -s /src/Common/Utility/Extensions Common/Utility.Core/Extensions
# Publish using pre-generated protobuf files (skip protoc)
RUN dotnet publish server/Server.csproj -c Release -o /app/publish --no-restore \
    -p:SkipProtobuf=true

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p Build-out
RUN mkdir -p wwwroot/media
EXPOSE 8080
ENTRYPOINT ["dotnet", "Server.dll"]
