# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files
COPY src/ToDoList.BlazorUI/ToDoList.BlazorUI.csproj src/ToDoList.BlazorUI/
COPY src/ToDoList.Domain/ToDoList.Domain.csproj src/ToDoList.Domain/

# Restore
RUN dotnet restore src/ToDoList.BlazorUI/ToDoList.BlazorUI.csproj

# Copy source
COPY src/ src/

# Publish Blazor WASM
RUN dotnet publish src/ToDoList.BlazorUI/ToDoList.BlazorUI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# =========================
# Runtime stage
# =========================
FROM nginx:alpine

# Remove default nginx files
RUN rm -rf /usr/share/nginx/html/*

# Copy Blazor WASM files
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]