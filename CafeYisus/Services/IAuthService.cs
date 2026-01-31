using CafeYisus.Models;

namespace CafeYisus.Services
{
    public interface IAuthService
    {
        string GenerateToken(User user);
    }
}
