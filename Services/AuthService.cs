using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserAuthentication.Models;
using UserAuthentication.Models.DTOs;

namespace UserAuthentication.Services
{ 
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration){
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }



        public async Task<(int, string)> Login(LoginDTO model)
        {
            var user = await userManager.FindByNameAsync(model.Name);
            if (user == null)
                return (0, "Invalid Username");
            
            if (!await userManager.CheckPasswordAsync(user, model.Password))
                return (0, "Invalid Password");
            
            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Name),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
                authClaims.Add(new(ClaimTypes.Role, userRole));
            
            string token = GenerateToken(authClaims);
            return (1, token);
        }

        public async Task<(int, string)> Registration(RegistrationDTO model, string role)
        {
            var userExists = await userManager.FindByNameAsync(model.Name);

            if(userExists != null){
                return (0, "User already exists");
            }

            User user = new()
            {
                Email = model.Email,
                Name = model.Name,
                UserName = "",
                Age = model.Age,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var createUserResult =  await userManager.CreateAsync(user, model.Password);

            if(!createUserResult.Succeeded){
                return (1, "User creation faild! Please check your details and try again.");
            }

            if(!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

            if(await roleManager.RoleExistsAsync(role))
                await userManager.AddToRoleAsync(user, role);
            
            return (1, "User created successfully");
        }

        public string GenerateToken(IEnumerable<Claim> claims){
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]!));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWTKey:ValidIssuer"],
                Audience = _configuration["JWTKey:ValidAudience"],
                // Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }
    }
}