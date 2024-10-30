using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ModelViews;

public record AdministradorModelView
{
    public string Email { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Perfil { get; set; } = string.Empty;
}
