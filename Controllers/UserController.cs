using Microsoft.AspNetCore.Mvc;
using api.Context;
using api.Models;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserContext _userContext;
        private readonly SaltContext _saltContext;
        private readonly UserAuthService _userAuthService;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private const string _LOGINSUCCESSFUL = "Successfully Login!";
        private const string _LOGINFAIL = "Username or Password incorrect!";
        public UserController(ILogger<UserController> logger, UserContext context, SaltContext saltContext, UserAuthService userAuthService, IConfiguration configuration, JwtTokenService jwtTokenService)
        {
            _logger = logger;
            _userContext = context;
            _saltContext = saltContext;
            _userAuthService = userAuthService;
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("GetUsers/")]
        public ActionResult<User> Get()
        {
            try
            {
                List<User> users = _userContext.Users.ToList();
                return Ok(users);
            }
            catch (Exception error)
            {
                _logger.LogInformation($"GetUsers Error: {error}");
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpGet("GetClaims/")]
        public IActionResult GetClaims()
        {
            return Ok(User.Claims.Select(p => new { p.Type, p.Value }));
        }

        [Authorize]
        [HttpGet("IsAuth/")]
        public IActionResult GetIsAuth()
        {
            return Ok(User.Identity.IsAuthenticated);
        }

        [HttpPost("Login/")]
        public ActionResult Login(UserLoginRequest userLoginRequest)
        {
            if (_userContext.Users.Where(u => u.Mail == userLoginRequest.Mail).Count() == 0)
            {
                Response response = new Response { Status = false, Message = _LOGINFAIL };
                return BadRequest(response);
            }
            else
            {
                User user = _userContext.Users.Where(u => u.Mail == userLoginRequest.Mail).FirstOrDefault();

                String salt = _saltContext.Salts.Where(x => x.user.Mail == userLoginRequest.Mail).Select(x => x.Salt1).FirstOrDefault();

                if (_userAuthService.VertifyUserCreds(userLoginRequest, user, salt))
                {
                    String token = _jwtTokenService.CreateToken(user);

                    Response.Cookies.Append(key: "jwt", value: token, new CookieOptions
                    {
                        HttpOnly = true
                    });

                    Response response = new Response { Status = true, Message = _LOGINSUCCESSFUL };
                    return Ok(response);
                }
                else
                {
                    Response response = new Response { Status = false, Message = _LOGINFAIL };
                    return BadRequest(response);
                }
            }
        }

        [HttpPost("Register/")]
        public ActionResult Register(Register register)
        {
            Guid user_guid = Guid.NewGuid();
            Guid salt_guid = Guid.NewGuid();

            var user = new User { Guid = user_guid, Mail = register.Mail, Password = _userAuthService.CreatePasswordHashByte(register.Password, salt_guid.ToString().ToUpper()), UserName = register.UserName };
            user.Role = "User";
            _userContext.Users.Add(user);
            _userContext.SaveChanges();


            var salt = new Salt { Guid = Guid.NewGuid(), Salt1 = salt_guid.ToString().ToUpper(), UserGuid = user_guid };
            _saltContext.Salts.Add(salt);
            _saltContext.SaveChanges();

            return Ok();
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            var userName = User.Identity.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName); // can be more specific to ip, user agent, device name, etc.
            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }
    }
}