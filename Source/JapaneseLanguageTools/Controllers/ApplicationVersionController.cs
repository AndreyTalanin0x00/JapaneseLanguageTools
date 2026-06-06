using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;

using AndreyTalanin0x00.Extensions.Hosting.Services.Abstractions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace JapaneseLanguageTools.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ApplicationVersionController : ControllerBase
{
    private const string c_gitVersionInformationTypeName = "GitVersionInformation";

    private const string c_informationalVersionFieldName = "InformationalVersion";
    private const string c_commitDateFieldName = "CommitDate";

    private readonly IMemoryCache m_memoryCache;
    private readonly IStartupAssemblyProvider m_startupAssemblyProvider;
    private readonly IWebHostEnvironment m_webHostEnvironment;

    public ApplicationVersionController(IMemoryCache memoryCache, IStartupAssemblyProvider startupAssemblyProvider, IWebHostEnvironment webHostEnvironment)
    {
        m_memoryCache = memoryCache;
        m_startupAssemblyProvider = startupAssemblyProvider;
        m_webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Text.Plain)]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<string> GetInformationalVersion()
    {
        if (!m_webHostEnvironment.IsDevelopment())
        {
            // Do not allow to query the application version outside of the Development environment.
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        string informationalVersion = GetGitVersionInformationValue(c_informationalVersionFieldName)!;
        return Ok(informationalVersion);
    }

    [HttpGet]
    [Produces(MediaTypeNames.Text.Plain)]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<string> GetCommitDate()
    {
        if (!m_webHostEnvironment.IsDevelopment())
        {
            // Do not allow to query the application version outside of the Development environment.
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        string commitDate = GetGitVersionInformationValue(c_commitDateFieldName);
        return Ok(commitDate);
    }

    private string GetGitVersionInformationValue(string gitVersionKey)
    {
        string cacheKey = $"{c_gitVersionInformationTypeName}:{gitVersionKey}";

        string gitVersionValue = m_memoryCache.GetOrCreate(cacheKey, GetGitVersionInformationValueCore)!;

        string GetGitVersionInformationValueCore(ICacheEntry cacheEntry)
        {
            cacheEntry.SetSlidingExpiration(TimeSpan.FromDays(5));

            Assembly startupAssembly = m_startupAssemblyProvider.GetStartupAssembly();

            Type gitVersionInformationType = startupAssembly.GetType(c_gitVersionInformationTypeName)
                ?? throw new InvalidOperationException($"Can not find the '{c_gitVersionInformationTypeName}' type in the startup assembly: '{startupAssembly.FullName}'.");

            FieldInfo gitVersionFieldInfo = gitVersionInformationType.GetField(gitVersionKey)
                ?? throw new UnreachableException($"Can not find the '{gitVersionKey}' field of the '{c_gitVersionInformationTypeName}' type.");

            string gitVersionValue = (string?)gitVersionFieldInfo.GetValue(null)
                ?? throw new UnreachableException($"The '{gitVersionKey}' field of the '{c_gitVersionInformationTypeName}' type was not set to a meaningful value.");

            return gitVersionValue;
        }

        return gitVersionValue;
    }
}
