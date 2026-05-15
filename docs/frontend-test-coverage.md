# Frontend Test Coverage Report

## Prerequisites

- [Node.js 18+](https://nodejs.org/) installed
- Dependencies installed: `npm install` inside `src/frontend/edutrack-ui`
- [Google Chrome](https://www.google.com/chrome/) installed

---

## Run Tests with Coverage

```bash
cd src/frontend/edutrack-ui
ng test --watch=false --code-coverage
```

Karma runs all specs once, collects coverage via Istanbul, and writes the report to `coverage/edutrack-ui/`.

---

## Run in Headless Chrome (CI)

```bash
ng test --watch=false --code-coverage --browsers=ChromeHeadless
```

---

## View the HTML Report

After the test run, open the generated report in a browser:

```bash
# Windows
start coverage/edutrack-ui/index.html

# macOS
open coverage/edutrack-ui/index.html

# Linux
xdg-open coverage/edutrack-ui/index.html
```

The report shows:
- **Statements**, **Branches**, **Functions**, and **Lines** coverage per file
- Color-coded source view: green = covered, red = not covered, yellow = partially covered

---

## Report Location

```
src/frontend/edutrack-ui/
└── coverage/
    └── edutrack-ui/
        ├── index.html        ← open this in a browser
        ├── lcov.info         ← for CI/CD integration (SonarQube, Coveralls, etc.)
        └── ...
```

---

## Enforce a Coverage Threshold

Add thresholds to `karma.conf.js` to fail the build when coverage drops below a target:

```js
coverageReporter: {
  dir: require('path').join(__dirname, './coverage/edutrack-ui'),
  reporters: [
    { type: 'html', subdir: '.' },
    { type: 'lcovonly', subdir: '.', file: 'lcov.info' },
    { type: 'text-summary' }
  ],
  check: {
    global: {
      statements: 80,
      branches: 75,
      functions: 80,
      lines: 80
    }
  }
}
```

---

## Tech Stack

| Tool | Role |
|------|------|
| Karma | Test runner |
| karma-coverage | Coverage instrumentation (Istanbul) |
| Jasmine | Test framework |
| lcov.info | Machine-readable output for CI tools |
