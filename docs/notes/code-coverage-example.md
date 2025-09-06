# Quick Start: Code Coverage Example

This guide demonstrates how to generate and view code coverage reports using our EduTrack project as an example.

## Current Coverage Status

Based on our latest coverage analysis:

- **📊 Overall Line Coverage: 56%** (917/1637 lines)
- **🌿 Branch Coverage: 49.7%** (243/488 branches)  
- **🎯 Method Coverage: 56.5%** (302/534 methods)

### Assembly Breakdown:
- **EduTrack.Application**: 40% coverage (Course features well-tested)
- **EduTrack.Domain**: 74.9% coverage (Strong entity coverage)
- **EduTrack.Infrastructure**: 0% coverage (Needs integration tests)

## Step-by-Step Example

### 1. Run Tests with Coverage Collection

```bash
# Basic coverage collection
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# With configuration file
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### 2. Generate HTML Report

```bash
# Install ReportGenerator (one-time setup)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate comprehensive report
reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"CoverageReport" \
  -reporttypes:"Html;HtmlSummary;Badges;JsonSummary" \
  -sourcedirs:"src" \
  -title:"EduTrack Code Coverage Report"
```

### 3. Use Our Automated Script

```powershell
# Run our comprehensive coverage script
.\generate-coverage.ps1

# With custom settings
.\generate-coverage.ps1 -Threshold 90 -OutputDir "Reports"
```

## What You'll See

### 1. **Interactive HTML Dashboard**

The main `index.html` provides:

- 📊 **Coverage Overview**: Summary metrics with color-coded indicators
- 📈 **Assembly Breakdown**: Coverage by project/assembly
- 🔍 **Drill-down Navigation**: Click to explore classes and methods
- 📋 **Risk Assessment**: Identifies uncovered critical code

### 2. **Coverage Badges**

Auto-generated SVG badges:
- `badge_linecoverage.svg`: ![56%](https://img.shields.io/badge/Line%20Coverage-56%25-orange)
- `badge_branchcoverage.svg`: ![49.7%](https://img.shields.io/badge/Branch%20Coverage-49.7%25-orange)
- `badge_methodcoverage.svg`: ![56.5%](https://img.shields.io/badge/Method%20Coverage-56.5%25-orange)

### 3. **Detailed Analysis**

Example of detailed class coverage from our report:

```json
{
  "name": "EduTrack.Application.Features.Courses.Commands.CreateCourse.CreateCourseCommandHandler",
  "coverage": 100,
  "coveredlines": 28,
  "coverablelines": 28,
  "branchcoverage": 100,
  "methodcoverage": 100
}
```

✅ **Well-tested**: CreateCourseCommandHandler has 100% coverage  
❌ **Needs attention**: ActivateCourseCommandHandler has 0% coverage

## Visual Coverage Analysis

### Color Coding in HTML Report:

- 🟢 **Green (80-100%)**: Well-tested code
- 🟡 **Yellow (60-79%)**: Moderately tested
- 🟠 **Orange (40-59%)**: Needs improvement  
- 🔴 **Red (0-39%)**: Critical attention needed

### Line-by-Line View:

The HTML report shows exact lines:
```csharp
✅ Line 15: var course = await _repository.GetByIdAsync(command.Id);
❌ Line 16: if (course == null) return null;  // ← Not covered!
✅ Line 17: return _mapper.Map<CourseDto>(course);
```

## Real Example Output

When you run our script, you'll see:

```
🧪 EduTrack Code Coverage Report Generator
=========================================

🧪 Running tests with coverage collection...
Threshold: 80%

📈 Coverage Summary:
  📏 Line Coverage:   56% ❌ (Below 80% threshold)
  🌿 Branch Coverage: 49.7%
  🎯 Method Coverage: 56.5%

📁 Generated Reports:
  📄 index.html
  📄 Summary.json
  📄 badge_linecoverage.svg
  📄 badge_branchcoverage.svg

🎯 Next Steps:
  1. Review the HTML report for detailed coverage analysis
  2. Identify uncovered lines (highlighted in red)
  3. Add tests for critical uncovered code paths
  4. Re-run this script to track improvement
```

## Integration Examples

### GitHub Actions Badge

Add to your README.md:
```markdown
![Coverage](./CoverageReport/badge_linecoverage.svg)
```

### VS Code Integration

1. Install "Coverage Gutters" extension
2. Run: `dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=lcov`
3. Press `Ctrl+Shift+P` → "Coverage Gutters: Display Coverage"
4. See coverage directly in your editor with green/red line highlighting

### SonarQube Integration

```bash
# Generate OpenCover format for SonarQube
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
```

## Tips for Improving Coverage

### Focus Areas from Our Analysis:

1. **Infrastructure Layer (0%)**: Add integration tests
2. **Course Commands**: ActivateCourse, ScheduleCourse, UpdateCourse handlers
3. **Student Features**: All student-related functionality needs tests
4. **Validation Behaviors**: ValidationBehavior has 0% coverage

### Quick Wins:

1. **Test Simple Commands**: Start with basic CRUD operations
2. **Cover Happy Paths**: Test successful scenarios first  
3. **Add Edge Cases**: Test null inputs, validation failures
4. **Integration Tests**: Test Infrastructure layer with test database

## Conclusion

Code coverage visualization helps you:

- ✅ **Identify gaps**: See exactly what's not tested
- ✅ **Track progress**: Monitor improvement over time  
- ✅ **Focus efforts**: Prioritize critical uncovered code
- ✅ **Build confidence**: Ensure comprehensive testing

Our EduTrack project shows **56% coverage** with strong Domain layer testing but Infrastructure needing attention. The visual reports make it easy to see where to focus testing efforts next!
