using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

using App.DAL.EF;
using App.Domain;
using App.Domain.Identity;

using Base.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.DTO.Identity;

namespace WebApp.ApiControllers.Identity;

/// <summary>
/// Account controller
/// </summary>

[Route("api/identity/[controller]/[action]")]
[ApiController]

public class AccountController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly Random _rand = new Random();
    private readonly AppDbContext _context;
    

    
    /// <summary>
    /// Account controller constructor
    /// </summary>
    /// <param name="signInManager">Manager sign in</param>
    /// <param name="userManager">Manager for user's</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="logger">Logger</param>
    /// <param name="context">Context</param>
    
    public AccountController(SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IConfiguration configuration,
        ILogger<AccountController> logger, AppDbContext context
        )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _context = context;
        
    }

    /// <summary>
    /// Customer registration api endpoint
    /// </summary>
    /// <param name="customerRegistrationDTO">Customer registration DTO which holds data for the registration</param>
    /// <returns>Jwt response customer register</returns>
    [HttpPost]
    public async Task<ActionResult<JwtResponse>> RegisterRegularUserDTO
        (UserRegistrationDto customerRegistrationDTO)
    {
         var appUser = await _userManager.FindByEmailAsync(customerRegistrationDTO.Email);
        if (appUser != null)
        {
            _logger.LogWarning("Webapi user registration failed! User with an email {} already exist!",
                customerRegistrationDTO.Email);
            var errorResponse = new RestErrorResponse
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Title = "App error",
                Status = HttpStatusCode.BadRequest,
                TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Errors =
                {
                    ["Email"] = new List<string>()
                    {
                        "Email already registered!"
                    }
                }
            };
            return BadRequest(errorResponse);
        }

        var refreshToken = new RefreshToken
        {
            TokenExpirationDateAndTime = DateTime.Now.AddMinutes(_configuration.GetValue<int>("JWT:ExpireInMinutes")).ToUniversalTime()
        };
        appUser = new AppUser()
        {
            Id = Guid.NewGuid(),
            FirstName = customerRegistrationDTO.FirstName,
            LastName = customerRegistrationDTO.LastName,
            Email = customerRegistrationDTO.Email,
            UserName = customerRegistrationDTO.Email,
            EmailConfirmed = true,
            RefreshTokens = new Collection<RefreshToken>()
            {
                refreshToken
            }

        };

        var result = await _userManager.CreateAsync(appUser, customerRegistrationDTO.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        result = await _userManager.AddClaimAsync(appUser, new Claim("aspnet.firstname",
            appUser.FirstName));
        
        result = await _userManager.AddClaimAsync(appUser, new Claim("aspnet.lastname", appUser.LastName));
        appUser = await _userManager.FindByEmailAsync(appUser.Email);

        if (appUser == null)
        {
            _logger.LogWarning("User with email {} is not found after registration", customerRegistrationDTO.Email);
            return BadRequest($"User with email {customerRegistrationDTO.Email} is not found after registration");

        }

        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
        if (claimsPrincipal == null)
        {
            _logger.LogWarning("Could not get claims for user {}!", customerRegistrationDTO.Email);
            await Task.Delay(_rand.Next(100, 1000));
            return NotFound("Username / password problem!");
        }
        await _userManager.AddToRoleAsync(appUser, "RegularUser");

        var jwt = IdentityExtension.GenerateJwt(
            claimsPrincipal.Claims,
            key: _configuration["JWT:Key"]!,
            issuer: _configuration["JWT:Issuer"]!,
            audience: _configuration["JWT:Issuer"]!,
            expirationDateTime: refreshToken.TokenExpirationDateAndTime
        );

        var regularUser = new RegularUser()
        {
            AppUserId = appUser.Id,
            
        };
         
        _context.RegularUsers.Add(regularUser);
        await _context.SaveChangesAsync();

        var roles =  await _userManager.GetRolesAsync(appUser);
        var res = new JwtResponse()
        {
            Token = jwt,
            RefreshToken = refreshToken.Token,
            
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            RoleNames = roles.ToArray()
        };
            
        return Ok(res);
    }

    /// <summary>
    /// Admin registration api endpoint
    /// </summary>
    /// <param name="userRegistrationDTO">Admin registration DTO which holds data for the registration</param>
    /// <returns>Jwt response admin register</returns>
    [HttpPost]
    public async Task<ActionResult<JwtResponse>> RegisterAdminDTO(UserRegistrationDto userRegistrationDTO)
    {
        var appUser = await _userManager.FindByEmailAsync(userRegistrationDTO.Email);
        if (appUser != null)
        {
            _logger.LogWarning("Webapi user registration failed! User with an email {} already exist!",
                userRegistrationDTO.Email);
            var errorResponse = new RestErrorResponse
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                Title = "App error",
                Status = HttpStatusCode.BadRequest,
                TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Errors =
                {
                    ["Email"] = new List<string>()
                    {
                        "Email already registered!"
                    }
                }
            };
            return BadRequest(errorResponse);
        }

        var refreshToken = new RefreshToken
        {
            TokenExpirationDateAndTime = DateTime.Now.AddMinutes(_configuration.GetValue<int>("JWT:ExpireInMinutes")).ToUniversalTime()
        };
        
        appUser = new AppUser()
        {
            Id = Guid.NewGuid(),
            FirstName = userRegistrationDTO.FirstName,
            LastName = userRegistrationDTO.LastName,
            Email = userRegistrationDTO.Email,
            UserName = userRegistrationDTO.Email,
            RefreshTokens = new Collection<RefreshToken>()
            {
                refreshToken
            }
        };

        var result = await _userManager.CreateAsync(appUser, userRegistrationDTO.Password );
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        
        appUser = await _userManager.FindByEmailAsync(appUser.Email);

        if (appUser == null)
        {
            _logger.LogWarning("User with email {} is not found after registration", userRegistrationDTO.Email);
            return BadRequest($"User with email {userRegistrationDTO.Email} is not found after registration");

        }

        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
        if (claimsPrincipal == null)
        {
            _logger.LogWarning("Could not get claims for user {}!", userRegistrationDTO.Email);
            await Task.Delay(_rand.Next(100, 1000));
            return NotFound("Username / password problem!");
        }

        var jwt = IdentityExtension.GenerateJwt(
            claimsPrincipal.Claims,
            key: _configuration["JWT:Key"]!,
            issuer: _configuration["JWT:Issuer"]!,
            audience: _configuration["JWT:Issuer"]!,
            expirationDateTime: refreshToken.TokenExpirationDateAndTime
        );
        
        await _userManager.AddToRoleAsync(appUser, "Admin");
      
        var admin = new App.Domain.Admin()
        {
            Id = new Guid(),
            AppUserId = appUser.Id,
            
        };
        
        _context.Admins.Add(admin);
        await _context.SaveChangesAsync();

        
        
        var roles = await _userManager.GetRolesAsync(appUser);
        
        var res = new JwtResponse()
        {
            Token = jwt,
            FirstName = userRegistrationDTO.FirstName,
            LastName = userRegistrationDTO.LastName,
            RoleNames = roles.ToArray(),
            RefreshToken = refreshToken.Token,
        };
            
        return Ok(res);
    }
    


/// <summary>
/// Log in api endpoint
/// </summary>
/// <param name="loginData">Supply email and password</param>
/// <returns>Status 200 OK response</returns>
    [HttpPost]
    public async Task<ActionResult<JwtResponse>> LogIn([FromBody] LoginDTO loginData)
    {
        var appUser = await _userManager.FindByEmailAsync(loginData.Email);
        if (appUser == null)
        {
            _logger.LogWarning("Webapi login failed! Email {} not found!", loginData.Email);
            await Task.Delay(_rand.Next(100, 1000));
            return NotFound("Username / password problem");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginData.Password,
            false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Webapi login failed! Password problem for user {}", loginData.Email);
            await Task.Delay(_rand.Next(100, 1000));
            return NotFound("Username / password problem");
        }

        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
        if (claimsPrincipal == null)
        {
            _logger.LogWarning("Could not get claims for user {}!", loginData.Email);
            await Task.Delay(_rand.Next(100, 1000));
            return NotFound("Username / password problem!");
        }

        var jwt = IdentityExtension.GenerateJwt(
            claimsPrincipal.Claims,
            key: _configuration["JWT:Key"]!,
            issuer:_configuration["JWT:Issuer"]!,
            audience: _configuration["JWT:Issuer"]!,
            expirationDateTime: DateTime.Now.AddMinutes(_configuration.GetValue<int>("JWT:ExpireInMinutes")));
        
        var refreshToken = new RefreshToken
        {
            TokenExpirationDateAndTime = DateTime.Now.AddMinutes(_configuration.GetValue<int>("JWT:ExpireInMinutes"))
                .ToUniversalTime()
            
        };
        
        await _context.Entry(appUser)
            .Collection(a => a.RefreshTokens!).Query().ToListAsync();
        appUser.RefreshTokens!.Add(refreshToken);

        var roles = await _userManager.GetRolesAsync(appUser);
        await _context.SaveChangesAsync();
        var res = new JwtResponse()
        {
            Token = jwt,
            RefreshToken = refreshToken.Token,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            RoleNames = roles.ToArray(),
        };
        return Ok(res);
    }

/// <summary>
/// Generating a refresh token to grant access without reentering credentials
/// </summary>
/// <param name="refreshTokenModel">Refresh token model DTO which holds data for the refresh token</param>
/// <returns>Action result OK</returns>
    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModelDTO refreshTokenModel)
    {
        JwtSecurityToken jwtToken;
        try
        {
             jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(refreshTokenModel.Token);
            if (jwtToken == null)
            {
                return BadRequest("No token"); 
            }
        }
        catch (Exception e)
        {
            return BadRequest($"Cannot parse the token {e.Message}");
        }
        
        var userEmail = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (userEmail == null)
        {
            return BadRequest("No email in jwt");
        }

        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser == null)
        {
            return BadRequest($"User with email ${userEmail} was not found!");
        }

        List<RefreshToken> tokens = await GetRefreshTokens(appUser, refreshTokenModel.Token);
   
        if (appUser.RefreshTokens == null)
        {
            return Problem("Refresh token collection is null");
        }

        if (appUser.RefreshTokens!.Count == 0)
        {
            return Problem("Refresh token collection is empty! No valid refresh tokens found!");
        }
        
        if (appUser.RefreshTokens!.Count != 1)
        {
            return Problem("More than one valid token found");
        }
        
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);

        if (claimsPrincipal == null)
        {
            _logger.LogWarning("Could not get claims for user {}!", userEmail);
            await Task.Delay(_rand.Next(100, 1000));
            return NotFound("Username / password problem!");
        }

        var jwt = IdentityExtension.GenerateJwt(
            claimsPrincipal.Claims,
            key: _configuration["JWT:Key"]!,
            issuer: _configuration["JWT:Issuer"]!,
            audience: _configuration["JWT:Issuer"]!,
            expirationDateTime: DateTime.Now.AddMinutes(_configuration.GetValue<int>("JWT:ExpireInMinutes")));

        var refreshToken = appUser.RefreshTokens.First();

        if (refreshToken.Token == refreshTokenModel.RefreshToken)
        {
            refreshToken.PreviousToken = refreshToken.Token;
            refreshToken.PreviousTokenExpirationDateAndTime = DateTime.UtcNow.AddMinutes(1);

            refreshToken.Token = Guid.NewGuid().ToString();
            refreshToken.TokenExpirationDateAndTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();
        }
        
        var res = new JwtResponse()
        {
            Token = jwt,
            RefreshToken = refreshToken.Token,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName
        };
        
        return Ok(res);
    }

    /// <summary>
    /// Get refresh tokens method
    /// </summary>
    /// <param name="appUser">App user</param>
    /// <param name="token">Token</param>
    /// <returns>Refresh token</returns>
    protected async Task<List<RefreshToken>> GetRefreshTokens(AppUser appUser, string token)
    {
        return await _context.Entry(appUser).Collection(u => u.RefreshTokens!)                               
            .Query().Where(x => (x.Token == token &&                         
                                 x.TokenExpirationDateAndTime > DateTime.UtcNow) ||                   
                                x.PreviousToken == token &&                  
                                x.PreviousTokenExpirationDateAndTime > DateTime.UtcNow).ToListAsync();
    }
}