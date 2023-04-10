using API.Errors;
using API.Extensions;
using API.Middleware;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//Adding swagger documntation servic from swagger extensions
builder.Services.AddSwaggerDocumentation();



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
//adding identity DB Connction
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
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
//adding Itoken service
builder.Services.AddScoped<ITokenService, TokenService>();
//adding order service
builder.Services.AddScoped<IOrderService, OrderService>();
//adding unitOfWork Service
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//adding Identity service
builder.Services.AddIdentityCore<AppUser>(options =>
{
    //We can add different identity options here
})
    //Below function will create necessary tables to store created users
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddSignInManager<SignInManager<AppUser>>();
//first AddAuthentication comes befor AddAuthorization
//Setting up Identity to use tokens(Adding JwtBearerDefaults is type of authentication)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        //options are instructions to validate the token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //any JWT token will be accepted if not validated
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidateIssuer = true,
            //validate audience will validate client application domain where it is hosted on,
            //it will notify that token only from validated audience will be accepted
            //As per microsoft default ValidateAudience will be true
            ValidateAudience=false

        };
    });

builder.Services.AddAuthorization();


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
    //Adding swagger middleware from swagger extensions
    app.UseSwaggerDocumentation();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

//Let the API serve static files like picture
app.UseStaticFiles();

//Below code is not rquired
//app.UseHttpsRedirection();

//CORS middleware
app.UseCors("CorsPolicy");

//authentication is added above UseAuthorization as part of identity
//User should have access to perform action until they are authenticated
app.UseAuthentication();
app.UseAuthorization();

//Maps requests with associated endpoints/action methods
app.MapControllers();

//Asynchronously applies any pending migration for the context to the database.
//Will create the data if not already exists
using var scope=app.Services.CreateScope();
var services = scope.ServiceProvider;
var context=services.GetRequiredService<StoreContext>();
//adding Identity,userManager context for seeding data 
var identitycontext = services.GetRequiredService<AppIdentityDbContext>();
var userManager = services.GetRequiredService<UserManager<AppUser>>();
var logger =services.GetRequiredService<ILogger<Program>>();
try
{
    //Migrating database
    await context.Database.MigrateAsync();
    await identitycontext.Database.MigrateAsync();
    //Seeding data during start of application
    await StoreContextSeed.SeedAsync(context);
    await AppIdentityDbContextSeed.SeedUserAsync(userManager);

}
catch (Exception ex)
{
    logger.LogError(ex.Message, "An error occured during app startup migration");
}

app.Run();
