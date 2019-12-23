using System;
using System.IO;
using System.Net.Http;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Origine.WebApi.Options;
using Origine.WebApi.Services;

namespace Origine
{
    public static class JwtTokenGeneratorExtensions
    {
        public static void AddJwtTokenGenerator(this IServiceCollection services, SecurityKey key, string issuer = null, string audience = null)
        {
            services.AddSingleton<ITokenGenerator>(new JwtTokenGenerator(key, issuer, audience));
        }
    }
}
