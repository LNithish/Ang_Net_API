using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

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

app.Run();
