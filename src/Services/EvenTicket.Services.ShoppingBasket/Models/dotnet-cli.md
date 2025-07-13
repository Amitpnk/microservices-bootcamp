# Entity Framework Core Commands

This document provides commands for both .NET CLI and Package Manager Console for managing Entity Framework Core migrations.

## Installation

### .NET CLI

#### Install Entity Framework Core CLI Tool Globally
```sh
dotnet tool install --global dotnet-ef
```

Install Specific Version
```sh
dotnet tool install --global dotnet-ef --version="9.0"
```

## Managing Migrations

### Add a New Migration
To add a new migration, use the following command. Replace `Init` with the desired migration name:

```sh
dotnet ef migrations add Init -context ShoppingBasketDbContext -o Migrations/ShoppingBasketDb
```

### Generate a Migration Script
To generate a SQL script for the migrations, use:
```sh
dotnet ef migrations script --context ShoppingBasketDbContext
```

### Updating the Database

### Apply Migrations to the Database
To apply the migrations to the database, use:

```sh
dotnet ef database update --context ShoppingBasketDbContext
```

## Package Manager Console

### Add a New Migration
To add a new migration using the Package Manager Console, use the following command. Replace `Initial-commit-Event` with the desired migration name:


```sh
add-migration Initial-commit-Event -Context ShoppingBasketDbContext -o Migrations/ShoppingBasketDb
```


### Generate a Migration Script
To generate a SQL script for the migrations, use:

```sh
Script-Migration -Context ShoppingBasketDbContext
```

### Apply Migrations to the Database
To apply the migrations to the database, use:

```sh
Update-Database -Context ShoppingBasketDbContext
```