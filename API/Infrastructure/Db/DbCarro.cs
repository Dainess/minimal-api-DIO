using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.Db;
public class DbCarro : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;
    public DbCarro(IConfiguration configuracaoAppSettings) 
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }
    public DbSet<Administrador> Administradores { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador 
            {
                Id = 1, // só no seed
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("MySql")?.ToString();
            {
                Console.WriteLine(stringConexao);
                optionsBuilder.UseMySql(stringConexao, ServerVersion.AutoDetect(stringConexao));
            }
    }
}
