using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enums;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

namespace minimal_api;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; set; } = configuration;
    private readonly string key = configuration.GetSection("Jwt").ToString() ?? "123456";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });

        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement{{    
                new OpenApiSecurityScheme{
                    Reference = new OpenApiReference{
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    }
                },
                new string[] {}
            }});
        });

        services.AddScoped<IAdministradorService, AdministradorService>();
        services.AddScoped<IVeiculoService, VeiculoService>();

        services.AddDbContext<DbCarro>(options => {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
            );
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => {
            
        #region Home
        endpoints.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
        #endregion

        #region Administradores
        string GerarTokenJwt(Administrador administrador)
        {
            if (string.IsNullOrEmpty(key)) return String.Empty;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim("Email", administrador.Email),
                new(ClaimTypes.Role, administrador.Perfil),
                new Claim("Perfil", administrador.Perfil)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

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

        endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorService administradorService) => {
            var administrador = administradorService.Login(loginDTO);
            if (administrador != null)
            {
                string token = GerarTokenJwt(administrador);
                return Results.Ok(new LogadoModelView {
                    Email = administrador.Email,
                    Perfil = administrador.Perfil,
                    Token = token
                });
            }  
            else 
                return Results.Unauthorized();
        }).WithTags("Administradores");

        endpoints.MapPost("/administradores/", ([FromBody] AdministradoresDTO administradoresDTO, IAdministradorService administradorService) => {
            var validacao = ValidaAdministradoresDTO(administradoresDTO);

            if (validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao.Mensagens);

            var administrador = new Administrador {
                Email = administradoresDTO.Email,
                Senha = administradoresDTO.Senha,
                Perfil = administradoresDTO.Perfil
            };
            
            administradorService.Incluir(administrador);

            return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView {
                Email = administrador.Email,
                Id = administrador.Id,
                Perfil = administrador.Perfil
            });
        }).RequireAuthorization(new AuthorizeAttribute{
            Roles = "Adm"
        }).WithTags("Administradores");

        endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorService administradorService) => {
            var administrador = administradorService.BuscaPorId(id);

            if (administrador == null)  
                return Results.NotFound();
            
            return Results.Ok(new AdministradorModelView {
                Email = administrador.Email,
                Id = administrador.Id,
                Perfil = administrador.Perfil
            });
        }).RequireAuthorization(new AuthorizeAttribute{
            Roles = "Adm"
        }).WithTags("Administradores");

        endpoints.MapGet("/administradores/", (IAdministradorService administradorService, [FromQuery] int pagina = 1) => {
            List<AdministradorModelView> administradores = [];
            foreach (var item in administradorService.Todos(pagina))
            {
                administradores.Add(new AdministradorModelView{
                    Email = item.Email,
                    Id = item.Id,
                    Perfil = item.Perfil
                });
            } 
            return Results.Ok(administradores);
        }).RequireAuthorization(new AuthorizeAttribute{
            Roles = "Adm"
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

        endpoints.MapPost("/veiculos", ([FromBody] VeiculosDTO veiculoDTO, IVeiculoService veiculoService) => {

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
        }).RequireAuthorization(new AuthorizeAttribute{
            Roles = "Adm, Editor"
        }).WithTags("Veículos");

        endpoints.MapGet("/veiculos", (IVeiculoService veiculoService, [FromQuery] int pagina = 1) => {
            return Results.Ok(veiculoService.Todos(pagina));
        }).RequireAuthorization(new AuthorizeAttribute{
            Roles = "Adm, Editor"
        }).WithTags("Veículos");

        endpoints.MapGet("/veiculos/{id}", (IVeiculoService veiculoService, [FromRoute] int id) => {
            var veiculo = veiculoService.BuscaPorId(id);

            if (veiculo == null)  
                return Results.NotFound();
            
            return Results.Ok(veiculo);
        }).RequireAuthorization(new AuthorizeAttribute{
            Roles = "Adm, Editor"
        }).WithTags("Veículos");

        endpoints.MapPut("/veiculos/{id}", (IVeiculoService veiculoService, [FromRoute] int id, VeiculosDTO veiculoDTO) => {

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
        }).RequireAuthorization(new AuthorizeAttribute{
            Roles = "Adm"
        }).WithTags("Veículos");

        endpoints.MapDelete("/veiculos/{id}", (IVeiculoService veiculoService, [FromRoute] int id) => {
            var veiculo = veiculoService.BuscaPorId(id);

            if (veiculo == null)  
                return Results.NotFound();

            veiculoService.Apagar(veiculo);
            return Results.NoContent();

        }).RequireAuthorization(new AuthorizeAttribute{
            Roles = "Adm"
        }).WithTags("Veículos");
        #endregion

        });
    } 
}
