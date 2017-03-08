﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoRenter.Api.Features.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AutoRenter.Api.Authorization
{
    public class TokenManager : ITokenManager
    {
        private readonly AppSettings _appSettings;

        public DateTime UtcTime { get; set; }

        public TokenManager(IOptions<AppSettings> appSettings, DateTime utcTime)
        {
            _appSettings = appSettings.Value;
            UtcTime = utcTime;
        }

        public TokenManager(IOptions<AppSettings> appSettings)
            : this(appSettings, DateTime.UtcNow)
        {
        }

        public virtual string CreateToken(UserModel userModel)
        {
            return $"Bearer {new JwtSecurityTokenHandler().WriteToken(CreateJsonWebToken(userModel))}";
        }

        public virtual JwtSecurityToken CreateJsonWebToken(UserModel userModel)
        {
            return new JwtSecurityToken(
                _appSettings.TokenSettings.Issuer,
                _appSettings.TokenSettings.Audience,
                GetClaims(userModel),
                UtcTime,
                UtcTime.AddMinutes(_appSettings.TokenSettings.ExpirationMinutes),
                GetSigningCredentials());
        }

        public virtual SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.TokenSettings.Secret)),
                SecurityAlgorithms.HmacSha256);
        }

        public virtual Claim[] GetClaims(UserModel userModel)
        {
            return new[]
            {
                new Claim(AutoRenterClaimNames.Username, userModel.Username),
                new Claim(AutoRenterClaimNames.Email, userModel.Email),
                new Claim(AutoRenterClaimNames.FirstName, userModel.FirstName),
                new Claim(AutoRenterClaimNames.LastName, userModel.LastName),
                new Claim(AutoRenterClaimNames.IsAdministrator, userModel.IsAdministrator.ToString()),
            };
        }
    }
}