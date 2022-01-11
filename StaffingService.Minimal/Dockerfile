#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StaffingService.Minimal/StaffingService.Minimal.csproj", "StaffingService.Minimal/"]
RUN dotnet restore "StaffingService.Minimal/StaffingService.Minimal.csproj"
COPY . .
WORKDIR "/src/StaffingService.Minimal"
RUN dotnet build "StaffingService.Minimal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StaffingService.Minimal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StaffingService.Minimal.dll"]