# STAGE 1: Build Frontend (Vue)
FROM node:22-alpine AS build-node
WORKDIR /app/ClientApp
COPY ClientApp/package*.json ./
RUN npm install
COPY ClientApp/ ./
RUN npm run build

# STAGE 2: Build Backend (.NET)
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build-dotnet
WORKDIR /src
COPY ["src/RestroApp.Api/RestroApp.Api.csproj", "src/RestroApp.Api/"]
COPY ["src/RestroApp.Core/RestroApp.Core.csproj", "src/RestroApp.Core/"]
COPY ["src/RestroApp.Infrastructure/RestroApp.Infrastructure.csproj", "src/RestroApp.Infrastructure/"]
RUN dotnet restore "src/RestroApp.Api/RestroApp.Api.csproj"
COPY src/ ./
RUN dotnet publish "src/RestroApp.Api/RestroApp.Api.csproj" -c Release -o /app/publish

# STAGE 3: Final Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build-dotnet /app/publish .

# Copy Frontend build to wwwroot
# Note: Vite builds into 'dist' folder by default
RUN mkdir -p wwwroot
COPY --from=build-node /app/ClientApp/dist ./wwwroot

# Railway/Render usually provides PORT environment variable
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "RestroApp.Api.dll"]
