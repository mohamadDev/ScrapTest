FROM mcr.microsoft.com/dotnet/core/runtime:2.1
COPY KlimaScrap/KlimaScrap/bin/Release/netcoreapp2.1/publish/ app/

ENTRYPOINT ["dotnet", "app/KlimaScrap.dll"]
