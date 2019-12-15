using System;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

namespace Origine.WebApi.Services
{
    public interface ITokenGenerator
    {
        string GetToken(Claim[] claims);
    }

    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly JwtSecurityTokenHandler _jwtTokenHandler = new JwtSecurityTokenHandler();
        private readonly SecurityKey _securityKey;
        private readonly string _issuer, _audience;

        public JwtTokenGenerator(SecurityKey securityKey, string issuer = null, string audience = null)
        {
            _securityKey = securityKey;
            _audience = audience;
            _issuer = issuer;
        }

        public string GetToken(Claim[] claims)
        {
            var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_issuer, _audience, claims, expires: DateTime.Now.AddHours(1), signingCredentials: credentials);
            return _jwtTokenHandler.WriteToken(token);
        }
    }
}
