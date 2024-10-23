using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infrastructure.Db;

namespace minimal_api.Domain.Services;

public class VeiculoService(DbCarro dbCarro) : IVeiculoService
{
    private readonly DbCarro _dbCarro = dbCarro;
    public void Apagar(Veiculo veiculo)
    {
        _dbCarro.Veiculos.Remove( veiculo );
        _dbCarro.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _dbCarro.Veiculos.Update(veiculo);
        _dbCarro.SaveChanges();
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _dbCarro.Veiculos.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Incluir(Veiculo veiculo)
    {
        _dbCarro.Veiculos.Add(veiculo);
        _dbCarro.SaveChanges();
    }

    public List<Veiculo>? Todos(int pagina, int itensPorPagina = 10, string? nome = null, string? marca = null)
    {
        var query = _dbCarro.Veiculos.AsQueryable();
        if (string.IsNullOrEmpty(nome) == false)
            query = query.Where(v => v.Nome.Contains(nome, StringComparison.CurrentCultureIgnoreCase));
        if (string.IsNullOrEmpty(marca) == false) 
            query = query.Where(v => v.Marca.Contains(marca, StringComparison.CurrentCultureIgnoreCase));
        
        return [.. query.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina)];
    }
}
