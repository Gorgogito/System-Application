namespace BDAplication.Domain.Entities.Bitacora;

public class Bitacora
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Fecha { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public string? UserModified { get; set; }
    public DateTime? DateModified { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<BitacoraActividad> Actividades { get; set; } = new List<BitacoraActividad>();
}
