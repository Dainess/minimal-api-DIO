using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enums;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

builder.Services.AddDbContext<DbCarro>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
ErrosDeValidacao ValidaAdministradoresDTO(AdministradoresDTO administradoresDTO)
{
    var validacao = new ErrosDeValidacao();

    if (string.IsNullOrEmpty(administradoresDTO.Email))
        validacao.Mensagens.Add("O email não pode ser vazio.");

    if (string.IsNullOrEmpty(administradoresDTO.Senha))
        validacao.Mensagens.Add("A senha não pode estar ausente.");

    if (string.IsNullOrEmpty(administradoresDTO.Perfil))
        validacao.Mensagens.Add("O perfil não foi enviado");
    else
        if (Enum.TryParse(administradoresDTO.Perfil, out Perfil perfil) == false)
            validacao.Mensagens.Add("O perfil enviado é inválido");
    
    return validacao;
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorService administradorService) => {
    if (administradorService.Login(loginDTO) != null)
        return Results.Ok("Login com sucesso");
    else 
        return Results.Unauthorized();
}).WithTags("Administradores");

app.MapPost("/administradores/", ([FromBody] AdministradoresDTO administradoresDTO, IAdministradorService administradorService) => {
    var validacao = ValidaAdministradoresDTO(administradoresDTO);

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao.Mensagens);

    var administrador = new Administrador {
        Email = administradoresDTO.Email,
        Senha = administradoresDTO.Senha,
        Perfil = administradoresDTO.Perfil
    };
    
    administradorService.Incluir(administrador);

    return Results.Created($"/veiculo/{administrador.Id}", new AdministradorModelView {
        Email = administrador.Email,
        Id = administrador.Id,
        Perfil = administrador.Perfil.ToString()
    });
}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorService administradorService) => {
    var administrador = administradorService.BuscaPorId(id);

    if (administrador == null)  
        return Results.NotFound();
    
    return Results.Ok(administrador);
}).WithTags("Administradores");

app.MapGet("/administradores/", (IAdministradorService administradorService, [FromQuery] int pagina = 1) => {
    return Results.Ok(administradorService.Todos(pagina));
}).WithTags("Administradores");
#endregion

#region Veiculos
ErrosDeValidacao ValidaVeiculosDTO(VeiculosDTO veiculosDTO)
{
    var validacao = new ErrosDeValidacao();

    if (string.IsNullOrEmpty(veiculosDTO.Nome))
        validacao.Mensagens.Add("O nome não pode ser vazio.");

    if (string.IsNullOrEmpty(veiculosDTO.Marca))
        validacao.Mensagens.Add("A marca não pode estar ausente.");

    if (veiculosDTO.Ano < 1950)
        validacao.Mensagens.Add("Apenas são aceitos carros posteriores a 1950.");
    
    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculosDTO veiculoDTO, IVeiculoService veiculoService) => {

    var validacao = ValidaVeiculosDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao.Mensagens);

    var veiculo = new Veiculo {
        Ano = veiculoDTO.Ano,
        Marca = veiculoDTO.Marca,
        Nome = veiculoDTO.Nome
    };
    
    veiculoService.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veículos");

app.MapGet("/veiculos", (IVeiculoService veiculoService, [FromQuery] int pagina = 1) => {
    return Results.Ok(veiculoService.Todos(pagina));
}).WithTags("Veículos");

app.MapGet("/veiculos/{id}", (IVeiculoService veiculoService, [FromRoute] int id) => {
    var veiculo = veiculoService.BuscaPorId(id);

    if (veiculo == null)  
        return Results.NotFound();
    
    return Results.Ok(veiculo);
}).WithTags("Veículos");

app.MapPut("/veiculos/{id}", (IVeiculoService veiculoService, [FromRoute] int id, VeiculosDTO veiculoDTO) => {

    var validacao = ValidaVeiculosDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao.Mensagens);

    var veiculo = veiculoService.BuscaPorId(id);

    if (veiculo == null)  
        return Results.NotFound();
    
    veiculo.Ano = veiculoDTO.Ano;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Nome = veiculoDTO.Nome;

    veiculoService.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veículos");

app.MapDelete("/veiculos/{id}", (IVeiculoService veiculoService, [FromRoute] int id) => {
    var veiculo = veiculoService.BuscaPorId(id);

    if (veiculo == null)  
        return Results.NotFound();

    veiculoService.Apagar(veiculo);
    return Results.NoContent();

}).WithTags("Veículos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion