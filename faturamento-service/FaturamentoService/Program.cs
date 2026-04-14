using FaturamentoService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra os controllers da aplicação
builder.Services.AddControllers();

// Configura o OpenAPI (Swagger) para documentação dos endpoints
builder.Services.AddOpenApi();

// Registra o HttpClient nomeado para comunicação com o EstoqueService
builder.Services.AddHttpClient("EstoqueService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});

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

// Aplica as migrations automaticamente ao iniciar a aplicação
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
