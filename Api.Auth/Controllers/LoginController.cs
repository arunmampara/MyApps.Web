using Api.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Api.Auth.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService, ILogger<LoginController> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        [HttpPost]
        [Route("login/")]
        public async Task<ActionResult<bool>> LoginUser(Models.Account userAccount)
        {
            _logger.LogInformation("Login attempt for user: {UserName}", userAccount.UserName);
            _logger.LogDebug("Debug logs");
            var result = await _loginService.Login(userAccount);
            return Ok(result);
        }

        [HttpPost]
        [Route("register/")]
        public async Task<ActionResult<bool>> RegisterUser(Models.Account userAccount)
        {
            var result = await _loginService.Register(userAccount);
            return Ok(result);
        }
    }
}
