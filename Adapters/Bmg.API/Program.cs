using Bmg.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureDataProtection();
builder.Services.ConfigureControllers();
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureMediatr();
builder.Services.ConfigureFluentValidations();
builder.Services.ConfigureHttp();
builder.Services.ConfigureDependecyInjection();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors("Bmg");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Services.RunMigrations();

app.Run();
