using System.Threading.Tasks;
using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginDto loginDto);
        Task<AuthResponseDto> Register(RegisterDto registerDto);
    }
}