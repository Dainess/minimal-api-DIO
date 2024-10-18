using minimal_api.Domain.DTO;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if ((loginDTO.Senha == "123456") && (loginDTO.Email == "adm@teste.com"))
        return Results.Ok("Login com sucesso");
    else 
        return Results.Unauthorized();
});

app.Run();