using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Infrastructure.Db;

namespace minimal_api.Domain.Services;

public class AdministradorService(DbCarro dbCarro) : IAdministradorService
{
    private readonly DbCarro _dbCarro = dbCarro;
    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _dbCarro.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
}
