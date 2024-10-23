using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces;

public interface IAdministradorService
{
    Administrador? Login(LoginDTO loginDTO);
    void Incluir(Administrador administrador);
    Administrador? BuscaPorId(int id);
    List<Administrador> Todos(int pagina, int itensPorPagina = 10);
}
