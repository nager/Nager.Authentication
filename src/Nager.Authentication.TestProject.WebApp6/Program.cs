using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nager.Authentication.Abstraction.Services;
using Nager.Authentication.Abstraction.Validators;
using Nager.Authentication.InMemoryRepository;
using Nager.Authentication.TestProject.WebApp6.Dtos;
using System.Text;

var users = new UserInfoWithPassword[]
{
    new UserInfoWithPassword
    {
        EmailAddress = "user1@domain.com",
        Password = "password",
        Roles = new [] { "invoice-view" }
    },
    new UserInfoWithPassword 
    {
        EmailAddress = "user2@domain.com",
        Password = "password",
        Roles = new [] { "invoice-view", "invoice-create" }
    },
    new UserInfoWithPassword
    {
        EmailAddress = "admin@domain.com",
        Password = "password",
        Roles = new [] { "administrator" }
    }
};

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
//builder.Services.AddSingleton(users);
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(configuration =>
{
    

    var issuer = builder.Configuration["Authentication:Tokens:Issuer"];
    var audience = builder.Configuration["Authentication:Tokens:Audience"];
    var signingKey = builder.Configuration["Authentication:Tokens:SigningKey"];

    //configuration.RequireHttpsMetadata = false;
    configuration.SaveToken = true;
    configuration.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ValidateLifetime = true,
        ValidateAudience = true,
        ValidateIssuer = true
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(configuration =>
{
    #region Provide the extended endpoint description from the xml comments

    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //configuration.IncludeXmlComments(xmlPath);

    foreach (var filePath in Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly))
    {
        configuration.IncludeXmlComments(filePath);
    }

    #endregion

    configuration.SwaggerDoc("general", new OpenApiInfo
    {
        Title = "General Documentation",
        Description = "",
        Contact = null,
        Version = "v1"
    });

    configuration.SwaggerDoc("authentication", new OpenApiInfo
    {
        Title = "Authentication Documentation",
        Description = "Authentication",
        Contact = null,
        Version = "v1"
    });

    configuration.SwaggerDoc("usermanagement", new OpenApiInfo
    {
        Title = "UserManagement Documentation",
        Description = "UserManagement",
        Contact = null,
        Version = "v1"
    });

    configuration.SwaggerDoc("useraccount", new OpenApiInfo
    {
        Title = "UserAccount Documentation",
        Description = "UserAccount",
        Contact = null,
        Version = "v1"
    });
});

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var userManagementService = services.GetRequiredService<IUserManagementService>();
    UserTestHelper.CreateAsync(users, userManagementService).GetAwaiter().GetResult();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(configuration =>
    {
        configuration.EnableTryItOutByDefault();
        configuration.DisplayRequestDuration();
        configuration.SwaggerEndpoint($"/swagger/general/swagger.json", "General");
        configuration.SwaggerEndpoint($"/swagger/authentication/swagger.json", "Authentication");
        configuration.SwaggerEndpoint($"/swagger/usermanagement/swagger.json", "UserManagement");
        configuration.SwaggerEndpoint($"/swagger/useraccount/swagger.json", "UserAccount");
    });
}



app.UseRouting();
app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions() {  ServeUnknownFileTypes = true});

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("api/v1/UtcTime", () =>
{
    var now = DateTime.UtcNow;

    return new TimeInfoDto
    {
        Hour = now.Hour,
        Minute = now.Minute,
        Second = now.Second
    };
})
.RequireAuthorization()
.WithGroupName("general")
.WithName("GetTime");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
