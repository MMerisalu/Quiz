
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.props .
COPY *.sln .
#COPY ITaxi/ITaxi/*.sln .
# copy all the projects from host to image
# Base
COPY Base.Contracts/*.csproj ./Base.Contracts/
COPY Base.Contracts.BLL/*.csproj ./Base.Contracts.BLL/
COPY Base.Contracts.DAL/*.csproj ./Base.Contracts.DAL/
COPY Base.Contracts.Domain/*.csproj ./Base.Contracts.Domain/
#---
COPY Base.Resources/*.csproj ./Base.Resources/
COPY Base.Extensions/*.csproj ./Base.Extensions/
COPY Base.Domain/*.csproj ./Base.Domain/
COPY Base.BLL/*.csproj ./Base.BLL/
COPY Base.DAL/*.csproj ./Base.DAL/
COPY Base.DAL.EF/*.csproj ./Base.DAL.EF/
COPY WebApp.Helpers/*.csproj ./WebApp.Helpers/
#App
COPY App.Resources/*.csproj ./App.Resources/
COPY App.Enum/*.csproj ./App.Enum/
COPY App.DAL.DTO/*.csproj ./App.DAL.DTO/
COPY App.Domain/*.csproj ./App.Domain/
COPY App.Contracts.DAL/*.csproj ./App.Contracts.DAL/
COPY App.BLL/*.csproj ./App.BLL/
COPY App.BLL.DTO/*.csproj ./App.BLL.DTO/
COPY App.Contracts.BLL/*.csproj ./App.Contracts.BLL/
COPY App.DAL.EF/*.csproj ./App.DAL.EF/
COPY App.Public.DTO/*.csproj ./App.Public.DTO/
#---
COPY WebApp/*.csproj ./WebApp/
COPY Tests/*.csproj ./Tests/

RUN dotnet restore

# copy everything else and build app
# Base
COPY Base.Contracts/. ./Base.Contracts/
COPY Base.Contracts.Domain/. ./Base.Contracts.Domain/
COPY Base.Contracts.DAL/. ./Base.Contracts.DAL/
COPY Base.Contracts.BLL/. ./Base.Contracts.BLL/
#---
COPY Base.Resources/. ./Base.Resources/
COPY Base.Extensions/. ./Base.Extensions/
COPY Base.Domain/. ./Base.Domain/
COPY Base.BLL/. ./Base.BLL/
COPY Base.DAL/. ./Base.DAL/
COPY Base.DAL.EF/. ./Base.DAL.EF/
COPY WebApp.Helpers/. ./WebApp.Helpers/
# App
COPY App.Resources/. ./App.Resources/
COPY App.Enum/. ./App.Enum/
COPY App.DAL.DTO/. ./App.DAL.DTO/
COPY App.Domain/. ./App.Domain/
COPY App.Contracts.DAL/. ./App.Contracts.DAL/
COPY App.BLL/. ./App.BLL/
COPY App.BLL.DTO/. ./App.BLL.DTO/
COPY App.Contracts.BLL/. ./App.Contracts.BLL/
COPY App.DAL.EF/. ./App.DAL.EF/
COPY App.Public.DTO/. ./App.Public.DTO/
# App
COPY WebApp/. ./WebApp/
##
COPY Tests/. ./Tests/


# Copy everything

# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /src
ENV ConnectionStrings:DefaultConnection="Server=172.27.64.1,1433;Database=ITaxiDb;User=sa;Password=Hobujaama10;Encrypt=true;TrustServerCertificate=True"
COPY --from=build-env /src/out .
ENTRYPOINT ["dotnet", "WebApp.dll"]
