using Domain.Models;

namespace Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
