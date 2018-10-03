using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace Allegro.Framework.Jwt
{
    public sealed class JwtTokenBuilder
    {
        private readonly SecurityKey _securityKey;
        private readonly string _subject, _issuer, _audience;
        private readonly Dictionary<string, string> _claims;
        private readonly int _expiry;
        private readonly TokenType _tokenType;

        public JwtTokenBuilder(TokenType tokenType, SecurityKey securityKey, string subject, string issuer, string audience, int expiry = 5)
        {
            _tokenType = tokenType;
            _securityKey = securityKey;
            _subject = subject;
            _issuer = issuer;
            _audience = audience;
            _expiry = expiry;
            _claims = new Dictionary<string, string>();
        }

        public JwtTokenBuilder AddClaims<T>(T claims)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.GetValue(claims) != null)
                {
                    _claims.Add(property.Name, property.GetValue(claims).ToString());
                }
            }

            return this;
        }

        public JwtTokenBuilder AddScope(List<string> scopes)
        {
            _claims.Add("scope", JsonConvert.SerializeObject(scopes));
            return this;
        }

        public JwtTokenBuilder AddScope(string scope)
        {
            _claims.Add("scope", scope);
            return this;
        }

        public JwtToken Build()
        {
            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Typ, _tokenType.ToString()),
              new Claim(JwtRegisteredClaimNames.Sub, _subject),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }
            .Union(_claims.Select(item => new Claim(item.Key, item.Value)));

            var token = new JwtSecurityToken(
                              issuer: _issuer,
                              audience: _audience,
                              claims: claims,
                              expires: DateTime.UtcNow.AddMinutes(_expiry),
                              signingCredentials: new SigningCredentials(
                                                        _securityKey,
                                                        SecurityAlgorithms.HmacSha256));

            return new JwtToken(token);
        }

        private bool IsGenericList(object o)
        {
            var oType = o.GetType();
            return oType.GetTypeInfo().IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
