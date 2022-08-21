using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secret;
        public UserRepository( ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            secret = config.GetValue<string>("ApiConfiguration:SecurityKey");
        }
        public bool IsUserUnique(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.Username == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequest)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.Username.ToLower() == loginRequest.Username.ToLower() && 
                                                        x.Password == loginRequest.Password);
            if (user == null)
            {
                return null;
            }
            // if user found generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                    }
                ),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var loginResponse = new LoginResponseDto()
            {
                User = user,
                Token = tokenHandler.WriteToken(token)
            };
            return loginResponse;

        }

        public async Task<LocalUser> Register(RegistrationRequestDto registrationRequestDto)
        {
            
            LocalUser user = new LocalUser()
            {
                Username = registrationRequestDto.Username,
                Password = registrationRequestDto.Password,
                Name = registrationRequestDto.Name,
                Role = registrationRequestDto.Role
            };
            _db.LocalUsers.Add(user);
            await _db.SaveChangesAsync();
            user.Password = "";
            return user;
        }
    }
}
