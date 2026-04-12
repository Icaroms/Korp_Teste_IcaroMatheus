using EstoqueService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra os controllers da aplicação
builder.Services.AddControllers();

// Configura o OpenAPI (Swagger) para documentação dos endpoints
builder.Services.AddOpenApi();

// Registra o AppDbContext usando a connection string "DefaultConnection" do appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Habilita o OpenAPI apenas no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redireciona requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Mapeia as rotas dos controllers
app.MapControllers();

app.Run();
