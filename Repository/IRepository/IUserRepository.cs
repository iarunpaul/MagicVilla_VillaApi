
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;

namespace MagicVilla_VillaApi.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUserUnique(string username);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequest);
        Task<LocalUser> Register(RegistrationRequestDto registrationRequestDto);
    }
}
