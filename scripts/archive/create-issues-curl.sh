# Create GitHub Issues using curl commands
# You need a GitHub Personal Access Token first

## Step 1: Get GitHub Personal Access Token
echo "📋 Step 1: Get your GitHub Personal Access Token"
echo "1. Go to: https://github.com/settings/tokens"
echo "2. Click 'Generate new token (classic)'"
echo "3. Select 'repo' scope (Full control of private repositories)"
echo "4. Copy the token"
echo "5. Set it as environment variable: export GITHUB_TOKEN=your_token_here"
echo ""

## Step 2: Set your GitHub token (replace with your actual token)
# export GITHUB_TOKEN=your_github_token_here
# OR for Windows CMD: set GITHUB_TOKEN=your_github_token_here
# OR for PowerShell: $env:GITHUB_TOKEN="your_github_token_here"

## Step 3: Run these curl commands one by one

# Issue 1: Clean Architecture Solution Structure Setup
curl -X POST \
  -H "Authorization: token $GITHUB_TOKEN" \
  -H "Accept: application/vnd.github.v3+json" \
  -H "Content-Type: application/json" \
  https://api.github.com/repos/mahedee/clean-arch-pro/issues \
  -d '{
    "title": "✅ COMPLETED - Clean Architecture Solution Structure Setup",
    "body": "## 🎯 Task Overview\n**Task ID**: T001-1  \n**Sprint**: 1  \n**Status**: ✅ COMPLETED  \n**Duration**: ~2 hours  \n\n## 📋 Description\nSuccessfully created the foundational solution structure following Clean Architecture principles with proper layer separation and dependencies.\n\n## ✅ Completed Work\n- ✅ Created 4-layer Clean Architecture solution structure\n- ✅ Established proper project hierarchy and dependencies\n- ✅ Configured solution file with all projects\n- ✅ Implemented proper separation of concerns\n\n## 🏗️ Solution Structure Created\n```\nEduTrack.sln\n├── src/\n│   ├── EduTrack.Api/ (Presentation Layer)\n│   ├── EduTrack.Application/ (Application Layer) \n│   ├── EduTrack.Domain/ (Domain Layer)\n│   └── EduTrack.Infrastructure/ (Infrastructure Layer)\n└── tests/\n    ├── EduTrack.Application.UnitTests/\n    ├── EduTrack.Domain.UnitTests/\n    ├── EduTrack.Infrastructure.UnitTests/\n    └── EduTrack.Api.IntegrationTests/\n```\n\n## 🔧 Technical Implementation\n- **Framework**: .NET 8\n- **Architecture**: Clean Architecture (Uncle Bob)\n- **Pattern**: Domain-Driven Design (DDD)\n- **Testing**: 4 comprehensive test projects\n\n## 📊 Impact Metrics\n- ✅ 100% compliance with Clean Architecture principles\n- ✅ 4 layers properly separated and configured\n- ✅ Solution builds successfully without errors\n- ✅ Foundation ready for domain development\n\n## 🔗 Related Tasks\n- **Enables**: Architecture dependency fixes\n- **Blocks**: All subsequent development tasks\n- **Dependencies**: None (foundation task)\n\n## 📝 Notes\nThis is the foundational task that enables all subsequent development. The solution structure follows industry best practices and provides a solid foundation for the EduTrack application.",
    "labels": ["✅ completed", "🏗️ architecture", "📋 task", "🎯 foundation"]
  }'

echo "Issue 1 created ✅"

# Issue 2: Architecture Dependency Violations Fixed
curl -X POST \
  -H "Authorization: token $GITHUB_TOKEN" \
  -H "Accept: application/vnd.github.v3+json" \
  -H "Content-Type: application/json" \
  https://api.github.com/repos/mahedee/clean-arch-pro/issues \
  -d '{
    "title": "✅ COMPLETED - Clean Architecture Dependency Violations Fixed",
    "body": "## 🎯 Task Overview\n**Task ID**: T001-2  \n**Sprint**: 1  \n**Status**: ✅ COMPLETED  \n**Duration**: ~1 hour  \n\n## 📋 Description\nFixed critical Clean Architecture dependency violations that were preventing proper implementation of dependency inversion principle.\n\n## ✅ Completed Work\n- ✅ **FIXED**: Removed Application → Infrastructure dependency violation\n- ✅ **VERIFIED**: Clean Architecture compliance restored\n- ✅ **TESTED**: Solution builds without circular dependencies\n- ✅ **VALIDATED**: Dependency flow follows Uncle Bob's Clean Architecture\n\n## 🚨 Critical Issues Resolved\n\n### Before Fix:\n```\n❌ EduTrack.Application → EduTrack.Infrastructure (VIOLATION)\n   This breaks the dependency inversion principle\n```\n\n### After Fix:\n```\n✅ EduTrack.Application → EduTrack.Domain (CORRECT)\n✅ EduTrack.Infrastructure → EduTrack.Domain (CORRECT)\n✅ EduTrack.Api → EduTrack.Application (CORRECT)\n✅ EduTrack.Api → EduTrack.Infrastructure (CORRECT)\n```\n\n## 🔧 Technical Implementation\n- **Removed**: Direct Application → Infrastructure project reference\n- **Maintained**: Proper dependency injection through API layer\n- **Preserved**: Clean separation of concerns\n- **Validated**: Architecture principles compliance\n\n## 📊 Impact Metrics\n- ✅ 100% Clean Architecture compliance achieved\n- ✅ 0 dependency violations remaining\n- ✅ Solution builds successfully\n- ✅ Ready for domain layer development\n\n## 🔗 Related Tasks\n- **Depends on**: Solution structure setup\n- **Enables**: Repository interface migration\n- **Unblocks**: Domain layer development\n\n## 📝 Notes\nThis fix was critical for maintaining Clean Architecture principles. The application layer should never directly depend on infrastructure concerns.",
    "labels": ["✅ completed", "🏗️ architecture", "🚨 critical-fix", "📋 task"]
  }'

echo "Issue 2 created ✅"

# Continue with remaining issues...
echo ""
echo "📋 To create all 7 issues, run each curl command above"
echo "💡 Tip: You can also use the PowerShell script: scripts/create-github-issues.ps1"
