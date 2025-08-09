using AndreyTalanin0x00.AspNetCore.SpaServices.Extensions;
using AndreyTalanin0x00.Extensions.Hosting;
using AndreyTalanin0x00.Extensions.Hosting.Extensions;
using AndreyTalanin0x00.Extensions.Hosting.Services.Abstractions;

using JapaneseLanguageTools.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JapaneseLanguageTools;

public class WebStartup : WebStartupBase
{
    public WebStartup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        : base(configuration, webHostEnvironment)
    {
    }

    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(options =>
        {
            options.AddApplicationAutoMapperProfiles();
        });

        services.AddDbContextServices(Configuration);

        services.AddApplicationRepositories();

        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddConfiguredSwaggerGen();

        services.AddStartupAssemblyProvider<WebStartup>(out IStartupAssemblyProvider startupAssemblyProvider);

        services.AddViteSpaStaticFiles(Configuration, startupAssemblyProvider);

        services.AddViteSpaDevelopmentServer(Configuration, startupAssemblyProvider);
    }

    /// <inheritdoc />
    public override void Configure(IApplicationBuilder applicationBuilder)
    {
        if (WebHostEnvironment.IsDevelopment())
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseConfiguredSwaggerUI();

            applicationBuilder.UseDeveloperExceptionPage();
        }
        else
        {
            applicationBuilder.UseHsts();
        }

        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseStaticFiles();
        applicationBuilder.UseSpaStaticFiles();

        applicationBuilder.UseRouting();

        applicationBuilder.UseAuthorization();

        applicationBuilder.UseEndpoints(endpointRouteBuilder =>
        {
            endpointRouteBuilder.MapControllers();
        });

        applicationBuilder.UseViteSpa();
    }
}
