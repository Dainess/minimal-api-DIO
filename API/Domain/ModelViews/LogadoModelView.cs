namespace minimal_api.Domain.ModelViews;

public record LogadoModelView
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
}
