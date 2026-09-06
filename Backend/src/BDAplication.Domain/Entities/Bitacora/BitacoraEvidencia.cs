using BDAplication.Domain.Enums;

namespace BDAplication.Domain.Entities.Bitacora;

public class BitacoraEvidencia
{
    public int Id { get; set; }
    public int BitacoraActividadId { get; set; }
    public BitacoraActividad Actividad { get; set; } = null!;
    public string NombreOriginal { get; set; } = string.Empty;
    public string NombreAlmacenado { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public TipoEvidencia Tipo { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
