# Running the Backend Application

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed
- PostgreSQL 14+ running locally (or another supported database)
- `appsettings.Development.json` configured (see below)

---

## 1. Configure the Database Connection

Create `appsettings.Development.json` inside `src/backend/EduTrack/src/EduTrack.Api/` (this file is git-ignored):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EduTrackDb;Username=postgres;Password=yourpassword;"
  }
}
```

Copy `appsettings.Production.json` as a template and update the connection string.

---

## 2. Restore Packages

```bash
cd src/backend/EduTrack
dotnet restore
```

---

## 3. Apply Database Migrations

```bash
cd src/backend/EduTrack/src/EduTrack.Api
dotnet ef database update
```

---

## 4. Run the Application

```bash
cd src/backend/EduTrack/src/EduTrack.Api
dotnet run
```

The application starts using the `Development` profile by default.

| URL | Description |
|-----|-------------|
| `http://localhost:6100` | API base URL |
| `http://localhost:6100/swagger` | Swagger / OpenAPI UI |

### Run with a specific launch profile

```bash
dotnet run --launch-profile http        # HTTP only
dotnet run --launch-profile https       # HTTPS + HTTP
dotnet run --launch-profile Staging     # Staging environment
dotnet run --launch-profile Production  # Production environment
```

---

## 5. Run with Visual Studio / VS Code

**Visual Studio:** Open `src/backend/EduTrack/EduTrack.sln`, select the `EduTrack.Api` project, and press **F5**.

**VS Code:** Open the `src/backend/EduTrack` folder, then use the **Run and Debug** panel. A launch configuration is included in `.vscode/`.

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `connection refused` on DB | Ensure PostgreSQL is running and credentials are correct in `appsettings.Development.json` |
| Port 6100 already in use | Change `applicationUrl` in `Properties/launchSettings.json` or stop the conflicting process |
| Pending migrations error | Run `dotnet ef database update` from the `EduTrack.Api` folder |
| `dotnet ef` not found | Install EF Core tools: `dotnet tool install --global dotnet-ef` |
