FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore

WORKDIR /src

# Copy project files for restore
COPY backend/AngularNetBase.API/AngularNetBase.API.csproj backend/AngularNetBase.API/
COPY backend/Modules/Identity/Identity.csproj backend/Modules/Identity/
COPY backend/Shared/Shared.Core/Shared.Core.csproj backend/Shared/Shared.Core/

# Create a script to strip frontend/SpaProxy references from the csproj.
# These are only needed for local development and must be removed for the container build.
RUN cat <<'SEDSCRIPT' > /tmp/strip-frontend.sh
#!/bin/sh
set -e
FILE="$1"
# Remove the multi-line frontend ProjectReference block
sed -i '/<ProjectReference.*frontend\.esproj/,/<\/ProjectReference>/d' "$FILE"
# Remove SpaProxy-related properties
sed -i '/<SpaRoot>/d; /<SpaProxyLaunchCommand>/d; /<SpaProxyServerUrl>/d' "$FILE"
# Remove SpaProxy package reference (multi-line with child elements)
sed -i '/<PackageReference Include="Microsoft.AspNetCore.SpaProxy"/,/<\/PackageReference>/d' "$FILE"
# Remove SpaProxy package reference (self-closing variant)
sed -i '/<PackageReference Include="Microsoft.AspNetCore.SpaProxy"[^>]*\/>/d' "$FILE"
SEDSCRIPT
RUN chmod +x /tmp/strip-frontend.sh

RUN /tmp/strip-frontend.sh backend/AngularNetBase.API/AngularNetBase.API.csproj
RUN dotnet restore backend/AngularNetBase.API/AngularNetBase.API.csproj

# Build stage
FROM restore AS build

COPY backend/ backend/

# Re-apply stripping (COPY overwrites the modified csproj)
RUN /tmp/strip-frontend.sh backend/AngularNetBase.API/AngularNetBase.API.csproj

RUN dotnet publish backend/AngularNetBase.API/AngularNetBase.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "AngularNetBase.API.dll"]
