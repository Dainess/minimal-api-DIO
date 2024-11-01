using System.Net;
using System.Text;
using System.Text.Json;
using minimal_api.Domain.DTO;
using minimal_api.Domain.ModelViews;
using Test.Helpers;

namespace Test.Domain.Requests;

[TestClass]
public class AdministradorRequestTest
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        Setup.ClassInit(context);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestMethod1()
    {
        //Arrange
        var loginDTO = new LoginDTO{
            Email = "adm@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");

        //Act
        var response = await Setup.client.PostAsync("/administradores/login", content);

        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<LogadoModelView>(result, new JsonSerializerOptions{
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado);
        Assert.IsNotNull(admLogado.Perfil);
        Assert.IsNotNull(admLogado.Email);
        Assert.IsNotNull(admLogado.Token);
    }
}
