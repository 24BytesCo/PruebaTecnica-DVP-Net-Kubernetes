using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace PruebaTecnica_DVP_Net_Kubernetes.Services
{
    public class EncryptionService
    {
        private readonly string _secretKey;
        private readonly bool _encryptionEnabled;

        public EncryptionService(string secretKey, bool encryptionEnabled)
        {
            _secretKey = secretKey;
            _encryptionEnabled = encryptionEnabled;
        }

        public string Encrypt(object payload)
        {
            if (!_encryptionEnabled)
                return JsonConvert.SerializeObject(payload);

            // Generar un token JWT con los datos
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim("data", JsonConvert.SerializeObject(payload))
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Establecer la expiración a 1 minuto
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // El método Decrypt ya no es necesario
        public string Decrypt(string cipherText)
        {
            if (!_encryptionEnabled)
                return cipherText;

            throw new NotImplementedException("La desencriptación no es necesaria cuando se utiliza JWT firmado.");
        }

        internal bool IsEncryptionEnabled()
        {
            return _encryptionEnabled;
        }
    }
}
