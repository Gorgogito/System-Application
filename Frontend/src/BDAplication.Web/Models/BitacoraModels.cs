namespace BDAplication.Web.Models;

public class BitacoraEvidenciaModel
{
    public int Id { get; set; }
    public int BitacoraActividadId { get; set; }
    public string NombreOriginal { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public string Tipo { get; set; } = string.Empty; // "Imagen" | "Video"
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }

    public bool IsImagen => Tipo == "Imagen";
    public bool IsVideo => Tipo == "Video";

    public string TamanoDisplay => TamanoBytes switch
    {
        < 1024 => $"{TamanoBytes} B",
        < 1_048_576 => $"{TamanoBytes / 1024.0:N1} KB",
        _ => $"{TamanoBytes / 1_048_576.0:N1} MB"
    };
}

public class BitacoraActividadModel
{
    public int Id { get; set; }
    public int BitacoraId { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string? UserModified { get; set; }
    public DateTime? DateModified { get; set; }
    public List<BitacoraEvidenciaModel> Evidencias { get; set; } = new();

    public string RangoHorario => $"{HoraInicio:HH:mm} — {HoraFin:HH:mm}";
    public int TotalImagenes => Evidencias.Count(e => e.IsImagen);
    public int TotalVideos => Evidencias.Count(e => e.IsVideo);
}

public class BitacoraModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Fecha { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public string UserCreated { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string? UserModified { get; set; }
    public DateTime? DateModified { get; set; }
    public List<BitacoraActividadModel> Actividades { get; set; } = new();
}

public class BitacoraResumenModel
{
    public DateTime Fecha { get; set; }
    public int TotalActividades { get; set; }
    public int TotalImagenes { get; set; }
    public int TotalVideos { get; set; }
}

public class UpdateBitacoraRequest
{
    public string? Observacion { get; set; }
}

public class CreateActividadRequest
{
    public int BitacoraId { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public class UpdateActividadRequest
{
    public int Id { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public class SasUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long DeclaredSizeBytes { get; set; }
}

public class SasUploadResponse
{
    public string UploadUrl { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class ConfirmEvidenciaRequest
{
    public int BitacoraActividadId { get; set; }
    public string BlobPath { get; set; } = string.Empty;
    public string NombreOriginal { get; set; } = string.Empty;
    public long DeclaredSizeBytes { get; set; }
}
