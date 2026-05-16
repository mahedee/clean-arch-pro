# Running Frontend Unit Tests

## Prerequisites

- [Node.js 18+](https://nodejs.org/) installed
- Dependencies installed (`npm install` inside `frontend/edutrack-ui`)
- [Google Chrome](https://www.google.com/chrome/) installed (Karma uses Chrome by default)

---

## Run All Tests

```bash
cd frontend/edutrack-ui
npm test
# or equivalently:
ng test
```

Karma launches Chrome, runs all specs, and prints results to the terminal. The browser window stays open and re-runs tests on file changes.

---

## Run Tests Once (CI / No Watch)

```bash
ng test --watch=false
```

---

## Run Tests in Headless Chrome (CI)

```bash
ng test --watch=false --browsers=ChromeHeadless
```

---

## Run Tests for a Specific File

```bash
ng test --include="src/app/features/students/**/*.spec.ts"
```

---

## Test Output

Results are printed to the terminal. A summary shows the total number of executed, passed, and failed specs. Failure details include the spec name and the assertion error.

---

## Tech Stack

- **Test framework:** Jasmine
- **Test runner:** Karma
- **Browser launcher:** karma-chrome-launcher
- **HTML reporter:** karma-jasmine-html-reporter (results also viewable in the browser window at `http://localhost:9876`)
