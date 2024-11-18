using Contracts;
using Entities.Exceptions;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using TaskTreck.Extensions;

var builder = WebApplication.CreateBuilder(args);


LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureSwagger();

builder.Services.AddProblemDetails(opt =>
{
    opt.ExceptionDetailsPropertyName = "ExceptionDetails";
    opt.IncludeExceptionDetails = (ctx, ex) => builder.Environment.IsDevelopment() || builder.Environment.IsStaging();
    opt.Map<NotFoundException>(exception => new ProblemDetails
    {
        Title = exception.Title,
        Detail = exception.Detail,
        Status = StatusCodes.Status500InternalServerError,
        Type = exception.Type,
        Instance = exception.Instance
    });
    opt.Map<BadRequestException>(exception => new ProblemDetails
    {
        Title = exception.Title,
        Detail = exception.Detail,
        Status = StatusCodes.Status500InternalServerError,
        Type = exception.Type,
        Instance = exception.Instance
    });
});

builder.Services.AddControllers()
    .AddApplicationPart(typeof(TaskTreck.Presentation.AssemblyReference).Assembly);

var app = builder.Build();


if (app.Environment.IsProduction())
    app.UseHsts();

app.UseProblemDetails();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskTrek API v1");
});

app.MapControllers();

app.Run();

