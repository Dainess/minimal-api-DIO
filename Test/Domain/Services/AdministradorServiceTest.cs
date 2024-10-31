using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

namespace Test.Domain.Services;

[TestClass]
public class AdministradorServiceTest
{
    private DbCarro CriandoContextoDeTeste()
    {
        var path = Directory.GetCurrentDirectory();
        string upperDirectory = path.Replace("bin\\Debug\\net8.0", "");
        string settingsPath = Path.Combine(upperDirectory, "appsettings.Development.json");

        /* var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location*/

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile(settingsPath, optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        var configuration = builder.Build();
        
        return new DbCarro(configuration);
    }
    [TestMethod]
    public void TestMethod1()
    {
        var context = CriandoContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        //Arrange
        var adm = new Administrador
        {
            Id = 1,
            Email = "teste@teste.com",
            Senha = "meuTeste",
            Perfil = "Adm"
        };

        var administradorService = new AdministradorService(context);

        //Act
        administradorService.Incluir(adm);
        var admRecuperado = administradorService.BuscaPorId(adm.Id) ?? new Administrador{Id = -1};

        //Assert
        Assert.AreEqual(1, administradorService.Todos(1, 10).Count);
        Assert.AreEqual(1, admRecuperado.Id);
    }
}
