using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using Microsoft.AspNetCore.Identity;

namespace PruebaTecnica_DVP_Net_Kubernetes.Token
{
    /// <summary>
    /// This class is responsible for generating JWT tokens based on user information.
    /// </summary>
    public class JwtGenerator : IJwtGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor that injects the JWT settings from the configuration.
        /// </summary>
        /// <param name="jwtSettings">An instance of JwtSettings containing the JWT configuration options such as the secret key, issuer, and audience.</param>
        /// <param name="userManager">The UserManager to retrieve user roles.</param>
        public JwtGenerator(IOptions<JwtSettings> jwtSettings, UserManager<User> userManager)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is being generated.</param>
        /// <returns>A string representation of the generated JWT token.</returns>
        public async Task<string> GenerateJwtToken(User user)
        {
            //Creating the data that the token will carry
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, user.Email ?? string.Empty),
                new("UserId", user.Id),
                new("Email", user.Email?? ""),
                new("NameCompleted", $"{user.FirstName ?? string.Empty} {user.LastName ?? string.Empty}")
            };

            // Get the roles associated with the user and add them as claims
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            // Get the secret key from the configuration (appsettings.json)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret ?? string.Empty));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(_jwtSettings.Expires),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //Writing token
            return tokenHandler.WriteToken(token);
        }
    }
}
