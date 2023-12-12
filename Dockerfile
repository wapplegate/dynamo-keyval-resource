FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS publish

RUN apk add clang build-base zlib-dev

COPY ["DynamoKeyvalResource/Common/Common.csproj", "DynamoKeyvalResource/Common/"]
COPY ["DynamoKeyvalResource/Check/Check.csproj", "DynamoKeyvalResource/Check/"]
COPY ["DynamoKeyvalResource/In/In.csproj", "DynamoKeyvalResource/In/"]
COPY ["DynamoKeyvalResource/Out/Out.csproj", "DynamoKeyvalResource/Out/"]

RUN dotnet restore "DynamoKeyvalResource/Check/Check.csproj" --runtime linux-musl-x64
RUN dotnet restore "DynamoKeyvalResource/In/In.csproj" --runtime linux-musl-x64
RUN dotnet restore "DynamoKeyvalResource/Out/Out.csproj" --runtime linux-musl-x64

COPY . .

RUN dotnet publish ./DynamoKeyvalResource/Check/Check.csproj -c Release -p:DebugType=None --runtime linux-musl-x64 -o /opt/resource
RUN dotnet publish ./DynamoKeyvalResource/In/In.csproj -c Release -p:DebugType=None --runtime linux-musl-x64 -o /opt/resource
RUN dotnet publish ./DynamoKeyvalResource/Out/Out.csproj -c Release -p:DebugType=None --runtime linux-musl-x64 -o /opt/resource

FROM alpine AS final

RUN apk add --no-cache libstdc++ icu-libs

COPY --from=publish /opt /opt

RUN chmod +x /opt/resource/check /opt/resource/in /opt/resource/out