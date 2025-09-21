using CourseEnrollment.Models.DTO;
using CourseEnrollment.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseEnrollment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        this._authService = authService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterEmployeeDto registerEmployeeDto)
    {
        if (ModelState.IsValid)
        {
            var authResult = await _authService.Register(registerEmployeeDto);
            if (!authResult.Success)
                return BadRequest(authResult);
            return Ok(authResult);
        }
        return BadRequest(ModelState);
    }
    [HttpPost("Confirm-Email")]
    public async Task<IActionResult> EmailConfirm([FromBody] string newEmployeeEmail, string confirmationCode)
    {
        var authResult = await _authService.ConfirmEmail(newEmployeeEmail, confirmationCode);
        if (authResult.Success)
            return Ok(authResult);
        return BadRequest();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginEmployeeDto loginEmployeeDto)
    {
        if (ModelState.IsValid)
        {
            var authResult = await _authService.Login(loginEmployeeDto);
            if (!authResult.Success)
                return Unauthorized("Invalid Authentication");
            return Ok(authResult);
        }
        return BadRequest(ModelState);
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest rawToken)
    {
        if (ModelState.IsValid)
        {
            var authResult = await _authService.Refresh(rawToken);
            if (!authResult.Success)
                return Unauthorized("Invalid Authentication");
            return Ok(authResult);
        }
        return BadRequest(ModelState);
    }
    [HttpPost("logout")]
    public async Task<IActionResult> LogOut([FromBody] LogoutRequest logoutRequest)
    {
        if (ModelState.IsValid && await _authService.Logout(logoutRequest.UserId))
        {
            return Ok();
        }
        return BadRequest();
    }
}
