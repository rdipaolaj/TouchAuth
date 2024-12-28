using System.ComponentModel.DataAnnotations;

namespace auth.entities;

public class AuthUser
{
    [Key]
    public Guid AuthUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty; // Token JWT generado tras el login
    public DateTime TokenExpiry { get; set; } // Fecha de expiración del token
    public string RefreshToken { get; set; } = string.Empty; // Token de refresco para obtener un nuevo JWT
    public DateTime RefreshTokenExpiry { get; set; } // Fecha de expiración del refresh token
    public DateTime LastLogin { get; set; } // Fecha y hora del último acceso
}