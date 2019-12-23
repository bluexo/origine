using System;
using System.Linq;
using System.Resources;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using Orleans;

using Origine.Models;
using Origine.WebApi.Hubs;
using Origine.WebApi.Services;
using Origine.WebService.Controllers;

namespace Origine.WebService.Api
{
    [Route("api/account")]
    [Authorize]
    [ApiController]
    public class AccountApi : GrainApi<AccountApi>
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly ITokenGenerator _tokenGenerator;

        public AccountApi(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenGenerator tokenGenerator,
            IClusterClient clusterClient,
            ILogger<AccountApi> logger)
            : base(clusterClient, logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
        }

        [AllowAnonymous]
        [HttpGet(nameof(Register))]
        public async Task<ActionResult> Register(string userName, string password)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userName) };

            var user = new ApplicationUser
            {
                UserName = userName,
                AccessToken = _tokenGenerator.GetToken(claims)
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status403Forbidden, result.Errors.ToArray());
            await _signInManager.SignInAsync(user, true);
            return Ok(user.AccessToken);
        }

        [AllowAnonymous]
        [HttpGet(nameof(Login))]
        public async Task<ActionResult> Login(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var sessionManager = _clusterClient.GetGrain<ISessionManager>(0);
                if (await sessionManager.IsOnline(userName))
                    return StatusCode(StatusCodes.Status409Conflict, "该账号已经登录,请稍后再试!");
                var result = await _signInManager.PasswordSignInAsync(user, password, true, false);
                if (result.Succeeded)
                    return Ok(user.AccessToken);
                else
                    return StatusCode(StatusCodes.Status401Unauthorized, "密码错误,请重新输入!");
            }

            return StatusCode(StatusCodes.Status401Unauthorized, "账号不存在,请先注册账号!");
        }
    }
}