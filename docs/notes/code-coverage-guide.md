# Code Coverage Implementation & Reporting Guide

## What is Code Coverage?

**Code Coverage** is a metric that measures the percentage of your source code that is executed during automated tests. It helps identify untested parts of your codebase and ensures comprehensive test coverage for better software quality.

### Types of Coverage Metrics

1. **Line Coverage**: Percentage of executable lines that are executed
2. **Branch Coverage**: Percentage of decision branches (if/else, switch) that are executed
3. **Method Coverage**: Percentage of methods that are called during tests
4. **Class Coverage**: Percentage of classes that have at least one method executed

## Benefits of Code Coverage

### 1. **Quality Assurance**
- 🎯 **Identify Untested Code**: Find gaps in your test suite
- 🛡️ **Reduce Bugs**: More coverage typically means fewer production bugs
- 📊 **Measurable Quality**: Quantifiable metric for code quality

### 2. **Development Confidence**
- ✅ **Safe Refactoring**: High coverage provides confidence when changing code
- 🚀 **Continuous Integration**: Automated quality gates in CI/CD pipelines
- 📈 **Progress Tracking**: Monitor test coverage improvements over time

### 3. **Team Standards**
- 🎯 **Coverage Goals**: Set and maintain minimum coverage thresholds (e.g., >90%)
- 👥 **Code Review**: Coverage reports help in pull request reviews
- 📋 **Documentation**: Visual reports show which areas need attention

## Implementation in .NET Projects

### 1. **Package Installation**

Add the following packages to your test projects:

```xml
<PackageReference Include="coverlet.collector" Version="6.0.2">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
<PackageReference Include="coverlet.msbuild" Version="6.0.2">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```

### 2. **Basic Coverage Collection**

Run tests with coverage collection:

```bash
# Basic coverage collection
dotnet test --collect:"XPlat Code Coverage"

# Coverage with specific output format
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Coverage for all test projects
dotnet test --collect:"XPlat Code Coverage" --verbosity normal
```

### 3. **Advanced Coverage Configuration**

Create a `coverlet.runsettings` file in your solution root:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <!-- Output formats -->
          <Format>json,cobertura,lcov,opencover</Format>
          
          <!-- Include/Exclude assemblies -->
          <Include>[EduTrack.Application]*,[EduTrack.Domain]*,[EduTrack.Infrastructure]*</Include>
          <Exclude>[*Tests]*,[*Test]*</Exclude>
          
          <!-- Include/Exclude files -->
          <ExcludeByFile>**/Migrations/**,**/Program.cs</ExcludeByFile>
          
          <!-- Include/Exclude attributes -->
          <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
          
          <!-- Threshold settings -->
          <Threshold>90</Threshold>
          <ThresholdType>line,branch,method</ThresholdType>
          <ThresholdStat>minimum</ThresholdStat>
          
          <!-- Report settings -->
          <UseSourceLink>true</UseSourceLink>
          <IncludeTestAssembly>false</IncludeTestAssembly>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

### 4. **Running with Settings**

```bash
# Use the settings file
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Generate specific formats
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=json,cobertura,lcov
```

## Advanced Coverage Scenarios

### 1. **Project-Specific Coverage**

```bash
# Run coverage for specific test project
dotnet test tests/EduTrack.Application.UnitTests --collect:"XPlat Code Coverage"

# Exclude specific assemblies
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude="[*Tests]*"
```

### 2. **Multi-Format Output**

```bash
# Generate multiple report formats
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=json,cobertura,lcov,opencover,teamcity
```

### 3. **Integration with MSBuild**

Add to your test project `.csproj`:

```xml
<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>json,cobertura,lcov,opencover</CoverletOutputFormat>
  <CoverletOutput>./coverage/</CoverletOutput>
  <Exclude>[*Tests]*</Exclude>
  <Include>[EduTrack.*]*</Include>
  <Threshold>90</Threshold>
  <ThresholdType>line,branch,method</ThresholdType>
  <ThresholdStat>minimum</ThresholdStat>
</PropertyGroup>
```

Then run:
```bash
dotnet test /p:CollectCoverage=true
```

## Report Generation & Visualization

### 1. **Install ReportGenerator Tool**

```bash
# Install globally
dotnet tool install -g dotnet-reportgenerator-globaltool

# Install locally
dotnet new tool-manifest
dotnet tool install dotnet-reportgenerator-localtool

# Update existing tool
dotnet tool update -g dotnet-reportgenerator-globaltool
```

### 2. **Generate HTML Reports**

```bash
# Basic HTML report
reportgenerator -reports:"tests/**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html

# Advanced HTML report with multiple formats
reportgenerator \
  -reports:"tests/**/coverage.cobertura.xml" \
  -targetdir:"CoverageReport" \
  -reporttypes:"Html;HtmlSummary;Badges;Cobertura" \
  -sourcedirs:"src" \
  -historydir:"CoverageHistory"
```

### 3. **Report Types Available**

```bash
# Various output formats
reportgenerator \
  -reports:"coverage.cobertura.xml" \
  -targetdir:"Reports" \
  -reporttypes:"Html;HtmlSummary;HtmlChart;HtmlInline;HtmlInline_AzurePipelines;HtmlInline_AzurePipelines_Dark;Badges;CsvSummary;Json;JsonSummary;Latex;LatexSummary;lcov;MarkdownSummary;MarkdownAssemblies;MarkdownSummaryGithub;MHtml;PngChart;SonarQube;TeamCitySummary;TextSummary;Xml;XmlSummary"
```

### 4. **Automated Report Generation Script**

Create `generate-coverage-report.ps1`:

```powershell
#!/usr/bin/env pwsh

# Clean previous reports
if (Test-Path "CoverageReport") {
    Remove-Item -Recurse -Force "CoverageReport"
}

# Run tests with coverage
Write-Host "Running tests with coverage collection..." -ForegroundColor Green
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults --verbosity normal

# Find coverage files
$coverageFiles = Get-ChildItem -Recurse -Include "coverage.cobertura.xml" -Path "./TestResults"

if ($coverageFiles.Count -eq 0) {
    Write-Error "No coverage files found!"
    exit 1
}

# Generate reports
Write-Host "Generating coverage reports..." -ForegroundColor Green
reportgenerator `
  -reports:"$($coverageFiles -join ';')" `
  -targetdir:"CoverageReport" `
  -reporttypes:"Html;HtmlSummary;Badges;JsonSummary" `
  -sourcedirs:"src" `
  -historydir:"CoverageHistory" `
  -title:"EduTrack Code Coverage" `
  -tag:"$(git rev-parse --short HEAD)"

# Open report
Write-Host "Coverage report generated successfully!" -ForegroundColor Green
Write-Host "Opening report at: ./CoverageReport/index.html" -ForegroundColor Yellow

if ($IsWindows) {
    Start-Process "./CoverageReport/index.html"
} elseif ($IsMacOS) {
    open "./CoverageReport/index.html"
} else {
    xdg-open "./CoverageReport/index.html"
}
```

## Graphical Coverage Reports

### 1. **HTML Dashboard Report**

The HTML report provides:
- 📊 **Interactive Dashboard**: Overview of coverage metrics
- 📈 **Trend Analysis**: Historical coverage changes
- 🎯 **Drill-down Capability**: From assembly → class → method level
- 🌈 **Color-coded Visualization**: Red (uncovered) to green (covered)

### 2. **Coverage Badges**

Generate SVG badges for README files:

```bash
reportgenerator \
  -reports:"coverage.cobertura.xml" \
  -targetdir:"badges" \
  -reporttypes:"Badges"
```

Results in badges like:
- ![Line Coverage](https://img.shields.io/badge/Line%20Coverage-85%25-green)
- ![Branch Coverage](https://img.shields.io/badge/Branch%20Coverage-78%25-yellow)

### 3. **VS Code Integration**

Install the **Coverage Gutters** extension:

1. Install `Coverage Gutters` extension
2. Open VS Code settings
3. Add coverage file path:
   ```json
   {
     "coverage-gutters.coverageFileNames": [
       "coverage.lcov",
       "coverage.cobertura.xml"
     ],
     "coverage-gutters.showLineCoverage": true,
     "coverage-gutters.showBranchCoverage": true
   }
   ```
4. Run tests with LCOV format:
   ```bash
   dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=lcov
   ```
5. Use `Ctrl+Shift+P` → "Coverage Gutters: Display Coverage"

### 4. **GitHub Actions Integration**

Create `.github/workflows/coverage.yml`:

```yaml
name: Code Coverage

on:
  push:
    branches: [ main, dev ]
  pull_request:
    branches: [ main ]

jobs:
  coverage:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Run tests with coverage
      run: dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
    
    - name: Install ReportGenerator
      run: dotnet tool install -g dotnet-reportgenerator-globaltool
    
    - name: Generate coverage report
      run: reportgenerator -reports:"coverage/**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html
    
    - name: Upload coverage reports to Codecov
      uses: codecov/codecov-action@v3
      with:
        files: ./coverage/**/coverage.cobertura.xml
        flags: unittests
        name: codecov-umbrella
        fail_ci_if_error: false
    
    - name: Comment PR with coverage
      if: github.event_name == 'pull_request'
      uses: 5monkeys/cobertura-action@master
      with:
        path: coverage/**/coverage.cobertura.xml
        repo_token: ${{ secrets.GITHUB_TOKEN }}
        minimum_coverage: 75
```

## Coverage Analysis & Interpretation

### 1. **Understanding Coverage Metrics**

From our current EduTrack coverage report:
```xml
<coverage line-rate="0.1799" branch-rate="0.0737" 
          lines-covered="349" lines-valid="1939" 
          branches-covered="36" branches-valid="488">
```

- **Line Coverage**: 17.99% (349/1939 lines covered)
- **Branch Coverage**: 7.37% (36/488 branches covered)
- **Total Lines**: 1,939 executable lines
- **Total Branches**: 488 decision points

### 2. **Coverage Goals**

| Coverage Level | Quality | Recommendation |
|----------------|---------|----------------|
| 90%+ | Excellent | Maintain this level |
| 80-89% | Good | Aim for improvement |
| 70-79% | Acceptable | Add more tests |
| 60-69% | Poor | Significant improvement needed |
| <60% | Critical | Immediate attention required |

### 3. **Actionable Insights**

Based on coverage reports, focus on:

1. **Uncovered Classes**: Identify classes with 0% coverage
2. **Complex Methods**: High cyclomatic complexity with low coverage
3. **Critical Paths**: Business-critical code that must be tested
4. **Edge Cases**: Ensure all branches are covered

## Best Practices

### 1. **Coverage Targets**
- 🎯 **Minimum**: 80% line coverage for production code
- 🎯 **Target**: 90%+ line coverage for critical business logic
- 🎯 **Branches**: 75%+ branch coverage for decision logic

### 2. **What to Exclude**
```xml
<ExcludeByFile>
  **/Migrations/**,
  **/Program.cs,
  **/Startup.cs,
  **/*Designer.cs,
  **/AssemblyInfo.cs
</ExcludeByFile>
<ExcludeByAttribute>
  Obsolete,
  GeneratedCodeAttribute,
  CompilerGeneratedAttribute,
  ExcludeFromCodeCoverageAttribute
</ExcludeByAttribute>
```

### 3. **Quality over Quantity**
- ✅ **Focus on Meaningful Tests**: Don't write tests just for coverage
- ✅ **Test Business Logic**: Prioritize critical application logic
- ✅ **Edge Cases**: Ensure error conditions are tested
- ❌ **Avoid Gaming**: Don't write trivial tests to boost numbers

### 4. **Continuous Monitoring**
- 📊 **CI/CD Integration**: Fail builds below threshold
- 📈 **Trend Tracking**: Monitor coverage over time
- 🎯 **Team Goals**: Make coverage a team responsibility

## Tools & Integrations

### 1. **Reporting Tools**
- **ReportGenerator**: .NET HTML reports
- **Codecov**: Cloud-based coverage tracking
- **SonarQube**: Comprehensive code quality analysis
- **Coveralls**: GitHub integration for coverage tracking

### 2. **IDE Extensions**
- **VS Code**: Coverage Gutters extension
- **Visual Studio**: Built-in code coverage tools
- **JetBrains Rider**: Integrated coverage analysis

### 3. **CI/CD Platforms**
- **GitHub Actions**: Automated coverage in workflows
- **Azure DevOps**: Built-in coverage reporting
- **Jenkins**: Coverage plugins and reporting
- **GitLab CI**: Integrated coverage visualization

## Example: Complete Coverage Workflow

### 1. **Project Setup**
```bash
# Add coverage packages to test projects
dotnet add tests/EduTrack.Application.UnitTests package coverlet.collector
dotnet add tests/EduTrack.Application.UnitTests package coverlet.msbuild

# Install ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### 2. **Run Coverage Analysis**
```bash
# Generate coverage data
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Generate HTML report
reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"CoverageReport" \
  -reporttypes:"Html;Badges;JsonSummary"
```

### 3. **View Results**
- Open `CoverageReport/index.html` in browser
- Review uncovered lines highlighted in red
- Identify areas needing additional tests
- Add tests and repeat process

## Conclusion

Code coverage is a powerful tool for ensuring comprehensive testing, but it should be used wisely:

- ✅ **Use as a Guide**: Coverage identifies gaps, not quality
- ✅ **Set Realistic Goals**: 100% coverage isn't always necessary
- ✅ **Focus on Critical Code**: Prioritize business logic and complex scenarios
- ✅ **Automate Monitoring**: Integrate into CI/CD for continuous quality
- ✅ **Visualize Progress**: Use reports to track improvements over time

**Remember**: High coverage doesn't guarantee bug-free code, but low coverage almost certainly means insufficient testing!
