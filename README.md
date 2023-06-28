# Nager.Authentication
ASP.NET Web API headless-authentication, for edge applications that want to do without external authentication.
This project provides the required REST endpoints in a simple way. 
Only the `IUserRepository` has to be mapped to the existing database context. The passwords are encrypted with a salt and `HMACSHA1` in the database.

In addition, 2 implementations are available for a user administration interface.
The first can be implemented in any existing web application via vue. The second is implemented with vue as 'single-file components'.

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
