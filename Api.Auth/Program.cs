using Api.Auth.Services;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddDebug();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder =>
    {
        resourceBuilder.AddService("be-observer")
        .AddAttributes(new Dictionary<string, object>
        {
            { "service.name", "be-observer-attribute"},
            {"service.version", "1.0.0"},
            { "service.instance.id" , "be-observer-instance-id"},
            { "service.application.name", "Api.Auth" }
        });
    })
    .WithMetrics(metric =>
    {
        metric.AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation();
    }
    ).WithTracing(trace =>
    {
        trace.AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation();
    })
    .UseOtlpExporter();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI  
builder.Services.AddOpenApi();
builder.Services.AddScoped<ILoginService, LoginService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
