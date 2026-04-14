using FaturamentoService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registra os controllers da aplicação
builder.Services.AddControllers();

// Configura o OpenAPI (Swagger) para documentação dos endpoints
builder.Services.AddOpenApi();

// Permite requisições do frontend Angular (CORS)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Registra o HttpClient nomeado para comunicação com o EstoqueService
// Atenção: dentro do Docker, o nome do host é "estoque-service", não "localhost"
builder.Services.AddHttpClient("EstoqueService", client =>
{
    client.BaseAddress = new Uri("http://estoque-service:8080/");
});

// Registra o AppDbContext usando a connection string "DefaultConnection" do appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Aplica as migrations automaticamente ao iniciar a aplicação
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Habilita o OpenAPI apenas no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Habilita o CORS
app.UseCors();

// Redireciona requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Mapeia as rotas dos controllers
app.MapControllers();

app.Run();