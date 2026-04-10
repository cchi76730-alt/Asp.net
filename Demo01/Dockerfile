# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy file project trước để restore nhanh hơn
COPY *.csproj ./
RUN dotnet restore

# copy toàn bộ source
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

# expose port (Render dùng 8080)
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Demo01.dll"]