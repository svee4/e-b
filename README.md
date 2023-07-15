# e-b

hii this is the source code for a certain unnamed website

## Running

1. Install .NET newest version
2. Install Entity Framework & tools
3. Install Postgres
4. Create an app on Discord for authentication
5. Create an `appsettings.secret.json` based on `appsettings.secret.json.example`
6. Run `dotnet watch` or `dotnet run`

## Database changes

1. Make changes in Domain.Models.Database
2. Run `dotnet ef migrations add <currentdate>-<iteration>`
3. Run `dotnet ef database update`
