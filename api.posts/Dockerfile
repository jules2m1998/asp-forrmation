FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["api.posts/api.posts.csproj", "api.posts/"]
RUN dotnet restore "api.posts/api.posts.csproj"
COPY . .
WORKDIR "/src/api.posts"
RUN dotnet build "api.posts.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api.posts.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.posts.dll"]
