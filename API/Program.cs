using API.Errors;
using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

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
//Connection to Redis DB, using IConnectionMultiplexer API from stackexchange.Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var options = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
    return ConnectionMultiplexer.Connect(options);
});
//Addscoped has lifetime until HTTP requst ends
//Adding repository services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
//Gneric repository service
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
//Adding Redis Service
builder.Services.AddScoped<IBasketRepository,BasketRepository>();
//adding automapper service to replace DTO codes
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//Configuring API Bhaviour to capturing validation error from the API/ like bad value in URL
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = ActionContext =>
    {
        var errors = ActionContext.ModelState
        .Where(e => e.Value.Errors.Count > 0)
        .SelectMany(x => x.Value.Errors)
        .Select(x => x.ErrorMessage).ToArray();

        var errorResponse = new ApiValidationErrorResponse
        {
            Errors = errors.Append("Error in the Client passed value from API URL")
        };
        return new BadRequestObjectResult(errorResponse);
    };
});


//CORS
//CORS should be enabled and CORS header should be returned,
//in order to let the browser to make use of API returning data
builder.Services.AddCors(op =>
{
    op.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

//To catch the developer exception using custom Exception middleware
//System.NullReferenceException: Object reference not set to an instance of an object.
app.UseMiddleware<ExceptionMiddleware>();

//Redirecting to errors controller with status code using usestatuscode middleware
//below will give error information when the URL itself was wrong from client
app.UseStatusCodePagesWithReExecute("/errors/{0}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Let the API serve static files like picture
app.UseStaticFiles();

//Below code is not rquired
//app.UseHttpsRedirection();

//CORS middleware
app.UseCors("CorsPolicy");

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
