namespace minimal_api.Domain.DTO;

public record AdministradoresDTO
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
}
