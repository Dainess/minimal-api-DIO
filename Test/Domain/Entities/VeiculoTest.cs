using minimal_api.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class VeiculoTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        //Arrange
        var veiculo = new Veiculo();

        //Act
        veiculo.Id = 1;
        veiculo.Nome = "Creta";
        veiculo.Ano = 2022;
        veiculo.Marca = "Hyundai";

        //Assert
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("Creta", veiculo.Nome);
        Assert.AreEqual(2022, veiculo.Ano);
        Assert.AreEqual("Hyundai", veiculo.Marca);
    }
}
