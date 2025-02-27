using Microsoft.EntityFrameworkCore;
using PIMTool.Core.Interfaces.Repositories;
using PIMTool.Core.Interfaces.Services;
using PIMTool.Database;
using PIMTool.Extensions;
using PIMTool.Middlewares;
using PIMTool.Repositories;
using PIMTool.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<PimContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Register();

var app = builder.Build();

EnsureMigrate(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors();

app.MapControllers();

app.Run();



void EnsureMigrate(WebApplication webApp)
{
    using var scope = webApp.Services.CreateScope();
    var pimContext = scope.ServiceProvider.GetRequiredService<PimContext>();
    pimContext.Database.Migrate();
}