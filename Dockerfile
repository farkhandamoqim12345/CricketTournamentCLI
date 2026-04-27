# =============================================
# Cricket Tournament CLI - Dockerfile
# =============================================

# Step 1: Build stage - .NET SDK use karo compile karne ke liye
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Project file copy karo aur dependencies restore karo
COPY CricketTournamentCLI.csproj .
RUN dotnet restore

# Baaki saara code copy karo
COPY . .

# App publish karo (release mode)
RUN dotnet publish -c Release -o /app/publish

# =============================================
# Step 2: Runtime stage - sirf runtime use karo (image choti hogi)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

# Published files copy karo
COPY --from=build /app/publish .

# App run karo
ENTRYPOINT ["dotnet", "CricketTournamentCLI.dll"]