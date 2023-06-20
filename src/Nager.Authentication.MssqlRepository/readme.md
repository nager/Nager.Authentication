# Database

## Start docker mssql
`start development mssql server.cmd`


# Entity Framework - Prepare Migration

## Prepare EF Tools Visual Studio
dotnet tool install --global dotnet-ef --ignore-failed-sources

## Update EF Tools Visual Studio
dotnet tool update --global dotnet-ef --ignore-failed-sources


# Entity Framework - Add/Remove Migration

## After change the Database model or Initial
dotnet ef migrations add --project Nager.Authentication.MssqlRepository DESCRIPTION-TEXT-CHANGE-ME

## Revert last commit
dotnet ef migrations remove --project Nager.Authentication.MssqlRepository

