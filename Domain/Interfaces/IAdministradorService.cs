using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces;

public interface IAdministradorService
{
    Administrador? Login(LoginDTO loginDTO);
}
