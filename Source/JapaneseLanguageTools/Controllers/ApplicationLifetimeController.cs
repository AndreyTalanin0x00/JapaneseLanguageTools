using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace JapaneseLanguageTools.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApplicationLifetimeController : ControllerBase
{
    private readonly IWebHostEnvironment m_webHostEnvironment;
    private readonly IHostApplicationLifetime m_hostApplicationLifetime;

    public ApplicationLifetimeController(IWebHostEnvironment webHostEnvironment, IHostApplicationLifetime hostApplicationLifetime)
    {
        m_webHostEnvironment = webHostEnvironment;
        m_hostApplicationLifetime = hostApplicationLifetime;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult Shutdown()
    {
        if (!m_webHostEnvironment.IsDevelopment())
        {
            // Do not allow to shut down the application outside of the Development environment.
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        m_hostApplicationLifetime.StopApplication();

        return Ok();
    }
}
