# StoreCenter Pull Request Guidelines

## 📥 How to Create a Pull Request

We welcome contributions! Follow these steps to submit your pull request (PR) to the **StoreCenter** project:

---

### ✅ Prerequisites

* Git installed on your local machine.
* A GitHub account.
* Basic understanding of Git and GitHub workflows.

---

### 🧭 Step-by-Step Instructions

#### 1. **Fork the Repository**

Go to the [StoreCenter GitHub repository](https://github.com/your-username/store-center) and click the **Fork** button in the top-right corner. This creates a copy of the repository under your GitHub account.

#### 2. **Clone the Forked Repository**

Open your terminal and run:

```bash
git clone https://github.com/your-username/store-center.git
cd store-center
```

> Replace `your-username` with your GitHub username.

#### 3. **Create a New Branch**

Create a new branch for your feature or fix:

```bash
git checkout -b feature/your-feature-name
```

> Use descriptive names like `fix/student-email-validation` or `feature/add-student-grade-api`.

#### 4. **Make Your Changes**

* Make your code changes in your branch.
* Follow the project’s coding conventions.
* Add or update tests if needed.
* Run the project locally and verify everything works.

#### 5. **Commit Your Changes**

```bash
git add .
git commit -m "feat: add [your feature/fix description]"
```

#### 6. **Push Your Branch to GitHub**

```bash
git push origin feature/your-feature-name
```

#### 7. **Create a Pull Request**

* Go to your repository on GitHub.
* Click on **Compare & pull request**.
* Provide a clear title and description of your changes.
* Link any related issues using `Fixes #issue_number` or `Closes #issue_number`.
* Submit the pull request.

#### 8. **Respond to Feedback**

The maintainers may review your PR and suggest changes. Please:

* Be responsive.
* Make the suggested changes in the same branch.
* Push your updates using:

```bash
git add .
git commit -m "fix: addressed review comments"
git push
```

---

### 📂 Example Pull Request Template

You can also create a `.github/PULL_REQUEST_TEMPLATE.md` file to guide contributors. Here's an example:

```markdown
## Description

Please include a summary of the change and which issue is fixed.

Fixes # (issue)

## Type of change

- [ ] Bug fix
- [ ] New feature
- [ ] Documentation
- [ ] Refactor

## Checklist

- [ ] I have performed a self-review of my code
- [ ] I have commented my code where necessary
- [ ] I have added tests that prove my fix is effective or my feature works
- [ ] I have added necessary documentation (if appropriate)
```

---

### 🙌 Need Help?

If you're stuck, open an [issue](https://github.com/your-username/store-center/issues) or ask in the Discussions tab.

---