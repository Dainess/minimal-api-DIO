using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;

namespace Test.Mock;

public class AdministradorServiceMock : IAdministradorService
{
    private static List<Administrador> administradores =
    [
        new Administrador { 
            Id = 1,
            Email = "adm@teste.com",
            Senha = "123456",
            Perfil = "Adm",
        },
        new Administrador {
            Id = 2,
            Email = "editor@teste.com",
            Senha = "abcdef",
            Perfil = "Editor"
        }
    ];
    public Administrador? BuscaPorId(int id)
    {
        return administradores.Find(a => a.Id == id);
    }

    public void Incluir(Administrador administrador)
    {
        administrador.Id = administradores.Count + 1;
        administradores.Add(administrador);
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return administradores.Find(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
    }

    public List<Administrador> Todos(int pagina, int itensPorPagina = 10)
    {
        return administradores;
    }
}
