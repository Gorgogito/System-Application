namespace BDAplication.Domain.Entities.Bitacora;

public class BitacoraActividad
{
    public int Id { get; set; }
    public int BitacoraId { get; set; }
    public Bitacora Bitacora { get; set; } = null!;
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public string? UserModified { get; set; }
    public DateTime? DateModified { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<BitacoraEvidencia> Evidencias { get; set; } = new List<BitacoraEvidencia>();
}
