# VictoryCenter Backend Application: Static Code Analysis

This guide explains how to configure code formatting and static analysis tools that are used in development. Following these steps will help ensure your code matches team conventions, avoids common issues and most importantly passes CI validation.

## Why it even matters

Enforcing code style and formatting helps us:

- Write cleaner and more maintainable code
- Catch issues **before** they reach CI
- Ensure all team members follow the same standards

## 1. Prerequisites

Before you begin, make sure you have:

- **Personal SonarCloud Token:** Go to `SonarCloud` → `My Account` → `Security` → `Generate Token` 

- **Local repo clone of this project**

- **IDE of your choice installed:**
  - Visual Studio 2022  
  - VS Code  
  - JetBrains Rider

## 2. Code Style with `.editorconfig`

At the root of the repo there is a `.editorconfig` file that defines formatting rules for all C# files.

- **Visual Studio:** No setup needed — works out of the box
- **VS Code:** Install the C# extension. Also enable `Editor: Format On Save` and `Editor: Detect Indentation` in settings
- **Rider:** Automatically picked up

## 3. Code Quality with SonarLint

Use SonarLint to connect your IDE with SonarCloud and get real-time feedback on code quality.

### 3.1 Visual Studio

1. `Extensions` → `Manage Extensions` → Search **SonarQube (2022)** → Download → Restart IDE
2. `Extensions` → `SonarQube` → `Connected Mode` → `Manage Binding`
3. `Manage Connections` → `New Connection` → `SonarQube Cloud`
4. Enter your personal token → Select organization `ita-social-projects`
5. Bind to project: `VictoryCenter-Back`

### 3.2 Visual Studio Code

1. `Ctrl+P` → `ext install SonarSource.sonarlint-vscode`
2. `Ctrl+Shift+P` → `SonarQube: Connect to SonarQube Cloud`
3. Provide:
   - Your personal token
   - Organization key: `ita-social-projects`
4. Bind to project `VictoryCenter-Back`
5. Restart IDE

### 3.3 JetBrains Rider

1. `File` → `Settings` → `Plugins` → Search **SonarQube for IDE** → Install → Restart IDE
2. `View` → `Tool Windows` → `SonarQube for IDE` → `+ Bind`
3. Configure new connection → `SonarQube Cloud` → Auth type: Token
4. Enter:
   - Token
   - Organization key: `ita-social-projects`
   - Project key: `ita-social-projects_VictoryCenter-Back`

## 4. Usage

Once setup is complete:

- IDE will **highlight code issues in real-time**
- Code will **auto-format on save**
- Fix issues locally **before committing** to avoid failed pipelines
