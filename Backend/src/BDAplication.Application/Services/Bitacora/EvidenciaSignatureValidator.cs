using BDAplication.Domain.Enums;

namespace BDAplication.Application.Services.Bitacora;

/// <summary>
/// Valida evidencias multimedia por firma binaria (magic bytes), no solo por extensión o
/// Content-Type declarado por el cliente — ambos son fácilmente falsificables.
/// </summary>
public static class EvidenciaSignatureValidator
{
    public static readonly IReadOnlyDictionary<string, string> AllowedContentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".gif"] = "image/gif",
            [".webp"] = "image/webp",
            [".mp4"] = "video/mp4",
            [".mov"] = "video/quicktime",
            [".webm"] = "video/webm"
        };

    public static TipoEvidencia? TipoFromExtension(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" => TipoEvidencia.Imagen,
        ".mp4" or ".mov" or ".webm" => TipoEvidencia.Video,
        _ => null
    };

    /// <summary>True si los primeros bytes del archivo corresponden a la firma esperada para la extensión.</summary>
    public static bool MatchesSignature(string extension, byte[] header)
    {
        bool At(int offset, params byte[] expected)
        {
            if (header.Length < offset + expected.Length) return false;
            for (var i = 0; i < expected.Length; i++)
                if (header[offset + i] != expected[i]) return false;
            return true;
        }

        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => At(0, 0xFF, 0xD8, 0xFF),
            ".png" => At(0, 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A),
            ".gif" => At(0, (byte)'G', (byte)'I', (byte)'F', (byte)'8'),
            ".webp" => At(0, (byte)'R', (byte)'I', (byte)'F', (byte)'F') && At(8, (byte)'W', (byte)'E', (byte)'B', (byte)'P'),
            // MP4/MOV (formatos ISO Base Media): caja "ftyp" en el offset 4
            ".mp4" or ".mov" => At(4, (byte)'f', (byte)'t', (byte)'y', (byte)'p'),
            // WebM/Matroska: firma EBML
            ".webm" => At(0, 0x1A, 0x45, 0xDF, 0xA3),
            _ => false
        };
    }
}
