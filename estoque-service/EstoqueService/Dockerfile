# Etapa 1: build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura as dependências
COPY *.csproj .
RUN dotnet restore

# Copia o restante do código e publica
COPY . .
RUN dotnet publish -c Release -o /out

# Etapa 2: imagem final menor, só com o runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copia o resultado do build
COPY --from=build /out .

# Porta que a aplicação vai escutar
EXPOSE 8080

ENTRYPOINT ["dotnet", "EstoqueService.dll"]