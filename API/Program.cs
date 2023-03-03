using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//DB connection
builder.Services.AddDbContext<StoreContext>(
    options => {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

//Adding repository services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
//Addscopd has lifetime until HTTP requst ends


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Below code is not rquired
//app.UseHttpsRedirection();

app.UseAuthorization();

//Maps requests with associated endpoints/action methods
app.MapControllers();

//Asynchronously applies any pending migration for the context to the database.
//Will create the data if not already exists
using var scope=app.Services.CreateScope();
var services = scope.ServiceProvider;
var context=services.GetRequiredService<StoreContext>();
var logger=services.GetRequiredService<ILogger<Program>>();
try
{
    await context.Database.MigrateAsync();

    //Seeding data during start of application
    await StoreContextSeed.SeedAsync(context);

}
catch (Exception ex)
{
    logger.LogError(ex.Message, "An error occured during app startup migration");
}

app.Run();
