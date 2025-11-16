using ManewryMorskie.Server;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes
        .Concat(new[] { "application/octet-stream" });
});
builder.Services.AddSignalR()
    .AddJsonProtocol(options => {
        options.PayloadSerializerOptions.Converters.Add(new CellLib.CellLocationConverter());
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<Rooms>();
builder.Services.AddScoped<Client>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MAUI_AND_WEB_APP", policy =>
        policy.WithOrigins(
            "https://0.0.0.1"
            ,"https://manewrymorskie.protoalea.dpdns.org"
            ,"https://www.manewrymorskie.protoalea.dpdns.org"
            ,"https://manewry-morskie.protoalea.dpdns.org"
            ,"https://www.manewry-morskie.protoalea.dpdns.org"
#if DEBUG
            ,"https://localhost:7176"
            ,"http://localhost:5244"
#endif
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});

var app = builder.Build();

app.UseCors("MAUI_AND_WEB_APP");

app.UseHttpsRedirection();

app.MapHub<ManewryMorskieHub>("/ManewryMorskie");
app.MapGet("/ping", () => Results.Ok());

app.Run();