# UserService

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 10.8.0.

## Install template

```bash
dotnet new install Clean.Architecture.Solution.Template
dotnet new ca-sln -n UserService
```

## Run

```bash
dotnet run --project .\src\AppHost
```

The Aspire dashboard will open automatically, showing the application URLs and logs.

## Database Migrations

The project uses EF Core with SQLite.

To apply existing migrations to the database:

```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

To add a new migration after modifying entities:

```bash
dotnet ef migrations add <MigrationName> --project src/Infrastructure --startup-project src/Web --output-dir Data/Migrations
```

To remove the last migration (if not yet applied):

```bash
dotnet ef migrations remove --project src/Infrastructure --startup-project src/Web
```

To generate a SQL script for the migrations:

```bash
dotnet ef migrations script --project src/Infrastructure --startup-project src/Web
```

> **Note:** If the `dotnet ef` tool is not installed, install it with:
> ```bash
> dotnet tool install --global dotnet-ef
> ```
