# Running the Frontend Application

## Prerequisites

- [Node.js 18+](https://nodejs.org/) installed
- [Angular CLI 18](https://angular.io/cli) installed globally
- Backend API running on `http://localhost:6100` (see [Running the Backend](running-backend.md))

---

## 1. Install Dependencies

Run once after cloning, or after pulling changes that modify `package.json`:

```bash
cd src/frontend/edutrack-ui
npm install
```

---

## 2. Start the Development Server

```bash
cd src/frontend/edutrack-ui
npm start
# or equivalently:
ng serve
```

The app is available at **`http://localhost:4200`**.

The development server proxies API calls to `http://localhost:6100`. Changes to source files are reflected automatically (hot reload).

---

## 3. Start with a Custom Port

```bash
ng serve --port 4201
```

---

## 4. Build for Production

```bash
npm run build
# or:
ng build --configuration production
```

Output is generated in `dist/edutrack-ui/`.

---

## 5. Run with Server-Side Rendering (SSR)

The project includes Angular SSR support. To run the SSR server after building:

```bash
npm run build
node dist/edutrack-ui/server/server.mjs
```

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| `ng: command not found` | Install Angular CLI globally: `npm install -g @angular/cli` |
| `npm install` fails | Delete `node_modules/` and `package-lock.json`, then re-run `npm install` |
| API calls return 404 or CORS errors | Ensure the backend is running on `http://localhost:6100` |
| Port 4200 already in use | Use `ng serve --port 4201` or stop the conflicting process |
