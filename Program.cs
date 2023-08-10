
using AccesoDatos;
using Hangfire;
using HangFireNet6;
using HangFireNet6.Job;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IRepositorioPersonas, RepositorioPersonas>();
builder.Services.AddTransient<IRepositorioTiposCuentas, RepositorioTiposCuentas>();
builder.Services.AddScoped<IServicioHora, ServicioHora>();

builder.Services.AddHangfire(config =>
{    
    var connectionString = builder.Configuration.GetConnectionString("HangFireConnection");
    config.UseSqlServerStorage(connectionString);
    config.UseColouredConsoleLogProvider();
    
});

GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 1 });


//builder.Services.AddHangfireServer();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();



//app.UseHangfireDashboard();

//RecurringJob.AddOrUpdate(() => Console.Write("Easy!"), Cron.Daily, TimeZoneInfo.Local)
//RecurringJob.AddOrUpdate<IServicioHora>("imprimir-hora", servicio => servicio.ImprimirHora(), "0 15 * * *", TimeZoneInfo.Local);




app.MapControllers();

app.Run();
