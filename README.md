# Simple Blog

[![Build status](https://ci.appveyor.com/api/projects/status/5d3455r9huyd6k67?svg=true)](https://ci.appveyor.com/project/BrandonHill/simple-blog)

A simple blog app built on ASP.NET Core.

## Get Started

### Prerequisites:

-   .NET Core 2.2 SDK
-   Visual Studio 2017 version 15.9 or later with the ASP.NET and web development workload (Optional)

### Configurations:

You can set configurations in file `appsettings.json` and `appsettings.Development.json`.

```javascript
  // appsettings.json
  "Config": {
    "Database": {
      // Available values: Sqlite, SqlServer
      "Provider": "Sqlite",
      "ConnectionStrings": "DataSource=app.db"
    },
    "SiteName": "Simple Blog"
  },
  // appsettings.Development.json
  "Development": {
    // Enable or disable data seeding. Admin user's username/password: "admin@example.com"/"Passw0rd."
    "IsDataSeedingEnabled": true
  }
```
