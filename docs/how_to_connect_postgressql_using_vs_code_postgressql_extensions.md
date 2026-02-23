# How to Connect PostgreSQL using VS Code Extensions

## Step 1: Install PostgreSQL Extension

1. Open VS Code
2. Click on the **Extensions** icon in the sidebar (or press `Ctrl+Shift+X`)
3. Search for "PostgreSQL" in the extensions marketplace
4. Install the **PostgreSQL** extension by Chris Kolkman
5. Alternatively, install **SQLTools** with **SQLTools PostgreSQL/Cockroach Driver**

## Step 2: Install SQLTools (Recommended Approach)

1. Search for **SQLTools** in the extensions marketplace
2. Install **SQLTools** by Matheus Teixeira
3. Install **SQLTools PostgreSQL/Cockroach Driver** by Matheus Teixeira

## Step 3: Create a New Connection

### Using SQLTools:

1. Press `Ctrl+Shift+P` to open the command palette
2. Type "SQLTools: Add New Connection" and select it
3. Choose **PostgreSQL** as the database driver
4. Fill in the connection details:
   - **Connection Name**: Give your connection a name (e.g., "My PostgreSQL DB")
   - **Server**: `localhost` (or your server IP)
   - **Port**: `5432` (default PostgreSQL port)
   - **Database**: Your database name
   - **Username**: Your PostgreSQL username
   - **Password**: Your PostgreSQL password

## Step 4: Configure Connection Settings

````json
// Example connection settings
{
  "name": "My PostgreSQL DB",
  "server": "localhost",
  "port": 5432,
  "database": "mydb",
  "username": "postgres",
  "password": "your_password",
  "driver": "PostgreSQL"
}
````

## Step 5: Test the Connection

1. Click **"Test Connection"** button
2. If successful, click **"Save Connection"**
3. The connection will appear in the SQLTools sidebar

## Step 6: Connect to Database

1. Open the **SQLTools** panel in the sidebar
2. Find your connection in the connections list
3. Click the **connect** icon next to your connection
4. Enter your password if prompted

## Step 7: Execute SQL Queries

1. Create a new SQL file (`.sql` extension)
2. Write your SQL queries
3. Select the query you want to execute
4. Press `Ctrl+E Ctrl+E` or use the command palette: "SQLTools: Run Selected Query"
5. Results will appear in the SQLTools Results panel

## Step 8: Browse Database Objects

1. In the SQLTools sidebar, expand your connected database
2. Browse tables, views, functions, and other database objects
3. Right-click on objects for context menu options like:
   - Show table data
   - Describe table structure
   - Generate SELECT statements

## Troubleshooting Tips

- **Connection fails**: Check if PostgreSQL service is running
- **Authentication error**: Verify username/password and pg_hba.conf settings
- **Port issues**: Ensure port 5432 is not blocked by firewall
- **Database not found**: Create the database first using `createdb` command or pgAdmin

## Additional Features

- **Auto-completion**: SQLTools provides intelligent SQL completion
- **Query history**: Access previous queries from the history panel
- **Export results**: Export query results to CSV, JSON, or other formats
- **Multiple connections**: Manage connections to different PostgreSQL instances

Your PostgreSQL database is now connected and ready to use within VS Code!
