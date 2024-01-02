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

## Integration

You can find an example of a possible implementation here https://github.com/nager/Nager.AuthenticationService
