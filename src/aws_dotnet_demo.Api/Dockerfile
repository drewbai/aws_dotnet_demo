# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY aws_dotnet_demo.Api.csproj ./
RUN dotnet restore aws_dotnet_demo.Api.csproj
COPY . ./
RUN dotnet publish aws_dotnet_demo.Api.csproj -c Release -o /out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "aws_dotnet_demo.Api.dll"]
