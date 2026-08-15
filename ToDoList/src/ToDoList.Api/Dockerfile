# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files
COPY src/ToDoList.Api/ToDoList.Api.csproj src/ToDoList.Api/
COPY src/ToDoList.Application/ToDoList.Application.csproj src/ToDoList.Application/
COPY src/ToDoList.Domain/ToDoList.Domain.csproj src/ToDoList.Domain/
COPY src/ToDoList.Persistence/ToDoList.Infrastructure.csproj src/ToDoList.Persistence/

# Restore
RUN dotnet restore src/ToDoList.Api/ToDoList.Api.csproj

# Copy source code
COPY src/ src/

# Publish
RUN dotnet publish src/ToDoList.Api/ToDoList.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ToDoList.Api.dll"]