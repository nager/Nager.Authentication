# Nager.Authentication
ASP.NET Web API headless-authentication, for edge applications that want to do without external authentication.
This project provides the required REST endpoints in a simple way. 
Only the `IUserRepository` has to be mapped to the existing database context. The passwords are encrypted with a salt and `HMACSHA1` in the database.

## Available NuGet Packages
https://www.nuget.org/packages?q=Nager.Authentication

- **Nager.Authentication**<br>
  This package contains Helpers and Services
- **Nager.Authentication.Abstraction**<br>
  This package contains Interfaces, Models and UserEntity
- **Nager.Authentication.AspNet**<br>
  This package contains ASP.NET Controllers, Dtos, Exceptions

## Screenshots
![Demo 1](/doc/AuthenticationDemo1.png)
![Demo 2](/doc/AuthenticationDemo2.png)


## Integration

Install NuGet Package into the WebApi Project
```
Install-Package Nager.Authentication.AspNet
```

Install NuGet Package into the EF Context Project
```
Install-Package Nager.Authentication.Abstraction
```

Add the UserEntity to the Context
```
public DbSet<UserEntity> Users { get; set; }
```

Update EF Migration logic

`appsettings.json`<br>
Add following config part
```
  "Authentication": {
    "Tokens": {
      "Issuer": "Issuer.PLEASE.CHANGE.ME",
      "Audience": "Audience.PLEASE.CHANGE.ME",
      "SigningKey": "SigningKey.PLEASE.CHANGE.ME"
    }
  },
```

`Program.cs`
Add following code parts

**Initial User**
```
var users = new UserInfoWithPassword[]
{
    new UserInfoWithPassword
    {
        EmailAddress = "admin@domain.com",
        Password = "password",
        Roles = new [] { "administrator" }
    }
};
```

**Activate Brute-Force Protection (uses MemoryCache)**
```
builder.Services.AddMemoryCache();
```

**Register Repository**
```
// The MssqlUserRepository must be created by yourself
builder.Services.AddScoped<IUserRepository, MssqlUserRepository>();
```

**Register Services**
```
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
```

**Activate Authentication**
```
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
```

**After builder.Build()**

**Create Initial User**
```
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var userManagementService = services.GetRequiredService<IUserManagementService>();
    await InitialUserHelper.CreateUsersAsync(users, userManagementService);
}
```

**Use Configurartion**
```
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
```
