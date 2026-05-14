# How to Configure PostgreSQL with pgAdmin 4

A short, step-by-step guide to install PostgreSQL, set up pgAdmin 4, and create the `EduTrackDb` database used by this project.

---

## 1. Install PostgreSQL (includes pgAdmin 4)

Download the official installer from [postgresql.org/download](https://www.postgresql.org/download/) and run it.

During installation:

- **Components**: keep `PostgreSQL Server`, `pgAdmin 4`, `Command Line Tools` checked.
- **Installation directory**: accept default (e.g. `C:\Program Files\PostgreSQL\17`).
- **Data directory**: accept default.
- **Superuser password**: set a strong password for the `postgres` user — **remember it**.
- **Port**: `5432` (default).
- **Locale**: `Default locale`.

Finish the wizard. Skip Stack Builder unless you need extra extensions.

---

## 2. Launch pgAdmin 4

- Open **pgAdmin 4** from the Start menu.
- On first launch, set a **master password** (used to unlock saved server credentials in pgAdmin).

---

## 3. Register the PostgreSQL Server

In the left tree:

1. Right-click **Servers → Register → Server…**
2. **General** tab:
   - **Name**: `Localhost` (any friendly label)
3. **Connection** tab:
   - **Host name/address**: `localhost`
   - **Port**: `5432`
   - **Maintenance database**: `postgres`
   - **Username**: `postgres`
   - **Password**: the password set during installation
   - Check **Save password**
4. Click **Save**.

The server should appear under **Servers** with a green icon.

---

## 4. Create the EduTrack Database

1. Expand **Servers → Localhost → Databases**.
2. Right-click **Databases → Create → Database…**
3. **General** tab:
   - **Database**: `EduTrackDb`
   - **Owner**: `postgres`
4. Click **Save**.

---

## 5. (Optional) Create a Dedicated Application User

Using a non-superuser for the app is recommended.

1. Right-click **Login/Group Roles → Create → Login/Group Role…**
2. **General** tab → **Name**: `edutrack_user`
3. **Definition** tab → **Password**: set a strong password
4. **Privileges** tab → enable **Can login?**
5. Click **Save**.

Grant access to the database:

```sql
GRANT ALL PRIVILEGES ON DATABASE "EduTrackDb" TO edutrack_user;
```

Run via **Tools → Query Tool** while connected to `EduTrackDb`.

---

## 6. Update the Application Connection String

Edit `src/backend/EduTrack/src/EduTrack.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EduTrackDb;Username=postgres;Password=YOUR_PASSWORD;"
  }
}
```

> ⚠️ Never commit real passwords. Keep `appsettings.Development.json` in `.gitignore` and use environment variables or User Secrets in production.

---

## 7. Apply EF Core Migrations

From the repo root:

```bash
cd src/backend/EduTrack/src/EduTrack.Api
dotnet ef database update
```

Refresh **EduTrackDb → Schemas → public → Tables** in pgAdmin — the EduTrack tables should now appear.

---

## 8. Verify the Connection

Run a quick test in pgAdmin **Query Tool**:

```sql
SELECT version();
SELECT current_database();
```

You should see the PostgreSQL version and `EduTrackDb` as the current database.

---

## Common Issues

| Problem | Fix |
|---------|-----|
| `password authentication failed` | Reset the `postgres` password via `psql` or reinstall pgAdmin saved credential. |
| `could not connect to server` | Ensure the **postgresql-x64-17** Windows service is running (`services.msc`). |
| Port 5432 already in use | Another Postgres instance is running — stop it or change the port. |
| pgAdmin master password forgotten | Delete `%APPDATA%\pgAdmin\pgadmin4.db` (you'll lose saved server entries). |

---

## References

- [PostgreSQL Downloads](https://www.postgresql.org/download/)
- [pgAdmin Documentation](https://www.pgadmin.org/docs/)
- VS Code alternative: [how_to_connect_postgressql_using_vs_code_postgressql_extensions.md](how_to_connect_postgressql_using_vs_code_postgressql_extensions.md)
