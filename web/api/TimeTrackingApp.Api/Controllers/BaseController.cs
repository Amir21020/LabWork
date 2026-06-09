using Microsoft.AspNetCore.Mvc;

namespace TimeTrackingApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseController : ControllerBase;