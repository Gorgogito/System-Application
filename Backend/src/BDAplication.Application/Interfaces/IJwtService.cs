using BDAplication.Domain.Entities;

namespace BDAplication.Application.Interfaces;

public interface IJwtService
{
    (string Token, DateTime Expiration) GenerateToken(User user);
}
