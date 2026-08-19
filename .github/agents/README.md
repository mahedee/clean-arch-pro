# EduTrack Custom Chat Modes (Agents)

This guide explains how to use the custom chat modes created for EduTrack.

## Available Agents

### 1. 🏗️ EduTrack Feature Builder
**File**: `.github/agents/edutrack-feature-builder.agent.md`

**Purpose**: Scaffold complete CQRS features following Clean Architecture - creates all necessary files across all layers.

**When to use**:
- Creating new entity features (Teacher, Assignment, Grade, etc.)
- Adding CRUD operations
- Implementing business operations
- Ensuring architectural consistency

**How to use**:
```
@edutrack-feature-builder Create a Teacher entity with CRUD operations
@edutrack-feature-builder Add an Assignment feature with Create, Update, and GetList
@edutrack-feature-builder Implement Grade entity with Create and GetByStudentId
```

**What it creates**:
- ✅ Domain entities and repository interfaces
- ✅ Application commands/queries, handlers, validators
- ✅ DTOs and AutoMapper profiles
- ✅ Infrastructure repository implementations
- ✅ API controller actions
- ✅ Unit tests with proper mocking
- ✅ All following Clean Architecture and CQRS patterns

---

### 2. 🔍 EduTrack Code Reviewer
**File**: `.github/agents/edutrack-code-reviewer.agent.md`

**Purpose**: Review code for architecture compliance, CQRS patterns, security vulnerabilities, and best practices.

**When to use**:
- Before merging pull requests
- Auditing existing code
- Checking new implementations
- Identifying security issues
- Validating test coverage

**How to use**:
```
@edutrack-code-reviewer Review this CreateTeacherCommand implementation
@edutrack-code-reviewer Check this PR for architecture violations
@edutrack-code-reviewer Audit the GradeRepository for security issues
@edutrack-code-reviewer Review my handler - is it following best practices?
```

**What it checks**:
- ✅ Clean Architecture compliance
- ✅ CQRS pattern implementation
- ✅ Repository pattern usage
- ✅ FluentValidation completeness
- ✅ OWASP security vulnerabilities
- ✅ Naming conventions
- ✅ Logging standards
- ✅ Test quality and coverage
- ✅ API controller standards

---

## How to Activate Agents in VS Code

### Method 1: Using @ Mention in Chat
1. Open Copilot Chat (`Ctrl+Alt+I`)
2. Type `@` to see available agents
3. Select the agent you want:
   - `@edutrack-feature-builder`
   - `@edutrack-code-reviewer`
4. Ask your question or give your request

### Method 2: Direct Command
Just type the agent name followed by your request:
```
@edutrack-feature-builder Create a Course entity with all CRUD operations
```

---

## Example Workflows

### Workflow 1: Creating a New Feature
```
1. @edutrack-feature-builder Create a Department entity with Create, Update, Delete, GetById, and GetAll operations

2. [Wait for files to be generated]

3. @edutrack-code-reviewer Review the generated Department feature files

4. [Apply any recommended fixes]

5. Run tests to verify everything works
```

### Workflow 2: Code Review Process
```
1. Create your feature manually or with modifications

2. @edutrack-code-reviewer Review my DepartmentRepository implementation

3. @edutrack-code-reviewer Check if my validator has all required rules

4. [Fix any violations found]

5. @edutrack-code-reviewer Final review before merging
```

### Workflow 3: Adding to Existing Entity
```
1. @edutrack-feature-builder Add GetStudentsByEnrollmentDate query to Student feature

2. @edutrack-code-reviewer Verify the new query follows patterns from existing queries

3. Commit changes
```

---

## Agent Features Comparison

| Feature | Feature Builder | Code Reviewer |
|---------|----------------|---------------|
| **Generates Code** | ✅ Yes | ❌ No (reviews only) |
| **Creates Tests** | ✅ Yes | ⚠️ Evaluates existing |
| **Architecture Check** | ✅ Built-in | ✅ Comprehensive |
| **Security Analysis** | ⚠️ Basic | ✅ OWASP Top 10 |
| **Best Practices** | ✅ Enforced | ✅ Validated |
| **Use Case** | Creating features | Reviewing code |

---

## Tips for Best Results

### When Using Feature Builder:
1. **Be specific** about entity properties and operations
2. **Mention business rules** upfront (e.g., "email must be unique")
3. **Specify relationships** if the entity relates to others
4. **Review generated code** before committing
5. **Run tests** to ensure everything works

### When Using Code Reviewer:
1. **Provide full file context** or clear code snippets
2. **Mention specific concerns** if you have them
3. **Ask for priority order** for fixes
4. **Request explanations** if recommendations are unclear
5. **Review one layer at a time** for complex features

---

## Quick Reference Commands

### Feature Builder
```bash
# Basic CRUD
@edutrack-feature-builder Create a Teacher entity with full CRUD

# Specific operations
@edutrack-feature-builder Add GetCoursesByDepartment query

# With relationships
@edutrack-feature-builder Create Assignment entity related to Course and Student

# Business operations
@edutrack-feature-builder Implement EnrollStudent command with validation
```

### Code Reviewer
```bash
# Full feature review
@edutrack-code-reviewer Review the entire Teacher feature

# Specific file
@edutrack-code-reviewer Check CreateTeacherCommandHandler

# Security focus
@edutrack-code-reviewer Audit for OWASP vulnerabilities

# Architecture focus
@edutrack-code-reviewer Verify Clean Architecture compliance

# Before merge
@edutrack-code-reviewer Final PR review for Department feature
```

---

## Common Issues & Solutions

### Issue: Agent not appearing in @ mentions
**Solution**: Make sure the `.agent.md` files are in `.github/agents/` directory and restart VS Code

### Issue: Agent doesn't follow EduTrack patterns
**Solution**: The agents automatically reference `.github/copilot-instructions.md` - ensure it's up to date

### Issue: Generated code has errors
**Solution**: Use Code Reviewer to identify issues, then manually fix or regenerate specific files

### Issue: Agent creates duplicate files
**Solution**: Check existing structure first, specify exact location, or delete duplicates

---

## Extending the Agents

Want to customize the agents? Edit the `.agent.md` files:

- **Add new rules**: Update the checklist sections
- **Change patterns**: Modify the code examples
- **Add languages**: Include additional validation rules
- **Customize output**: Adjust the format sections

---

## Related Files

- **Project Guidelines**: `.github/copilot-instructions.md`
- **Security Rules**: `.github/instructions/security.instructions.md`
- **Prompt Template**: `.github/prompts/new-feature.prompt.md`

---

## Support & Feedback

- **Issues**: Create a GitHub issue with `agent` label
- **Improvements**: Submit PR with agent enhancements
- **Questions**: Ask in team chat or code review

---

**Happy Coding!** 🚀

Use these agents to maintain consistency, speed up development, and ensure quality across the EduTrack codebase.
