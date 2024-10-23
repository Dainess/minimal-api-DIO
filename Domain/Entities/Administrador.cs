using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Domain.Entities;

public class Administrador
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [StringLength(255)]
    public string Email { get; set;} = string.Empty;
    [Required]
    [StringLength(50)]
    public string Senha { get; set;} = string.Empty;
    [Required]
    [StringLength(10)]
    public string Perfil { get; set;} = string.Empty;
}
