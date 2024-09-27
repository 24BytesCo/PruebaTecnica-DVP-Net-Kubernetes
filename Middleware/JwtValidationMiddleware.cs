using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PruebaTecnica_DVP_Net_Kubernetes.Models;

namespace PruebaTecnica_DVP_Net_Kubernetes.Middleware
{
    /// <summary>
    /// Custom middleware to validate the JWT token on each incoming request.
    /// </summary>
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Constructor that receives the next middleware in the processing chain and the JWT configuration.
        /// </summary>
        /// <param name="next">The next middleware in the processing chain.</param>
        /// <param name="jwtSettings">JWT settings injected from appsettings.json.</param>
        public JwtValidationMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Method that runs to validate the JWT token on each incoming request.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <returns>A task representing the asynchronous execution of the middleware.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    ValidateJwtToken(token);
                }
                catch (SecurityTokenException ex)
                {
                    // If the token is invalid or cannot be validated, return a 401 Unauthorized status
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync($"Unauthorized: {ex.Message}");
                    return;
                }
            }

            // If the token is valid, continue to the next middleware
            await _next(context);
        }

        /// <summary>
        /// Validates the JWT token using the same parameters as during generation.
        /// </summary>
        /// <param name="token">The JWT token that needs to be validated.</param>
        private void ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret!);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);
        }
    }
}
