using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.DTO;

public record VeiculosDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public int Ano { get; set;}
}
