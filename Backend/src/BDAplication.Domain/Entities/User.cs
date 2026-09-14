using BDAplication.Domain.Common;

namespace BDAplication.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }

    /// <summary>Preferencia de tema de UI del usuario: "Light" | "Dark" | "System". Null = no configurado (System).</summary>
    public string? ThemePreference { get; set; }

    public Role Role { get; set; } = null!;
}
