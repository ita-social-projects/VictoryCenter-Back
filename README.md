<a href="https://softserve.academy/"><img src="https://s.057.ua/section/newsInternalIcon/upload/images/news/icon/000/050/792/vnutr_5ce4f980ef15f.jpg" title="SoftServe IT Academy" alt="SoftServe IT Academy"></a>

# Victory Center

> Subtitle or Short Description Goes Here

> ideally one sentence

> include terms/tags that can be searched

[![Build Status](https://img.shields.io/travis/ita-social-projects/VictoryCenter-Back/master?style=flat-square)](https://travis-ci.org/github/ita-social-projects/VictoryCenter-Back)
[![Coverage Status](https://img.shields.io/gitlab/coverage/ita-social-projects/VictoryCenter-Back/master?style=flat-square)](https://coveralls.io)
[![Github Issues](https://img.shields.io/github/issues/ita-social-projects/VictoryCenter-Back?style=flat-square)](https://github.com/ita-social-projects/VictoryCenter-Back/issues)
[![Pending Pull-Requests](https://img.shields.io/github/issues-pr/ita-social-projects/VictoryCenter-Back?style=flat-square)](https://github.com/ita-social-projects/VictoryCenter-Back/pulls)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ita-social-projects_VictoryCenter-Back&metric=alert_status)](https://sonarcloud.io/project/overview?id=ita-social-projects_VictoryCenter-Back) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ita-social-projects_VictoryCenter-Back&metric=coverage)](https://sonarcloud.io/dashboard?id=ita-social-projects_VictoryCenter-Back) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=ita-social-projects_VictoryCenter-Back&metric=bugs)](https://sonarcloud.io/dashboard?id=ita-social-projects_VictoryCenter-Back) [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=ita-social-projects_VictoryCenter-Back&metric=code_smells)](https://sonarcloud.io/dashboard?id=ita-social-projects_VictoryCenter-Back) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=ita-social-projects_VictoryCenter-Back&metric=security_rating)](https://sonarcloud.io/dashboard?id=ita-social-projects_VictoryCenter-Back)

- For more on these wonderful  badges, refer to <a href="https://shields.io/" target="_blank">shields.io</a>.

---

## Table of Contents (Optional)

> If your `README` has a lot of info, section headers might be nice.

- [Installation](#installation)
  - [Required to install](#Required-to-install)
  - [Environment](#Environment)
  - [Clone](#Clone)
  - [Setup](#Setup)
  - [How to run local](#How-to-run-local)
  - [How to run Docker](#How-to-run-Docker)
- [Usage](#Usage)
  - [How to work with swagger UI](#How-to-work-with-swagger-UI)
  - [How to run tests](#How-to-run-tests)
  - [How to Checkstyle](#How-to-Checkstyle)
- [Documentation](#Documentation)
- [Contributing](#contributing)
  - [git flow](#git-flow)
  - [issue flow](#git-flow)
- [FAQ](#faq)
- [Support](#support)
- [License](#license)

---

## Folder structure 
```
VictoryCenter-Back/
├── .github/
│   ├── ISSUE_TEMPLATE/
│   ├── PULL_REQUEST_TEMPLATE/
│   └── workflows/
│ 
├── VictoryCenter/
│   ├── VictoryCenter.WebAPI/
│   │   ├── Controllers/        # HTTP controllers
│   │   │   ├── Auth/
│   │   │   │   └── AuthController.cs
│   │   │   ├── Categories/
│   │   │   ├── Images/
│   │   │   ├── Payments/
│   │   │   ├── Public/         # Publicly accessible API endpoints
│   │   │   ├── TeamMembers/
│   │   │   └── BaseApiController.cs
│   │   │
│   │   ├── Extensions/         # Extension methods for app configuration
│   │   │   └── ApplicationConfiguration.cs
│   │   │   └── ConfigurationBuilderExtensions.cs
│   │   │   └── .................................
│   │   ├── Factories/
│   │   ├── Middleware/         # Custom middleware components
│   │   ├── Properties/
│   │   ├── Utils/
│   │   ├── Program.cs          # Application entry point
│   │   ├── VictoryCenter.WebAPI.csproj
│   │   ├── appsettings.json    # Application configuration files    
│   │   ├── appsettings.local.json
│   │   └── ......................
│   │ 
│   ├── VictoryCenter.BLL/
│   │   ├── Commands/          # CQRS Commands (Create, Update, Delete)
│   │   │   ├── Auth/
│   │   │   │   ├── Login/
│   │   │   │   │   └── LoginCommand.cs
│   │   │   │   │   └── LoginCommandHandler.cs
│   │   │   │   └── RefreshToken/
│   │   │   │       └── RefreshTokenCommand.cs
│   │   │   │       └── RefreshTokenCommandHandler.cs
│   │   │   │   
│   │   │   ├── Categories/
│   │   │   │   ├── Create/
│   │   │   │   │   └── CreateCategoryCommand.cs
│   │   │   │   │   └── CreateCategoryHandler.cs
│   │   │   │   ├── Delete/
│   │   │   │   └── Update/
│   │   │   │
│   │   │   ├── Images/
│   │   │   │   ├── Create/
│   │   │   │   ├── Delete/
│   │   │   │   └── Update/
│   │   │   │
│   │   │   ├── Payment/
│   │   │   │   ├── Common/
│   │   │   │   └── WayForPay/
│   │   │   │   
│   │   │   └── TeamMembers/
│   │   │       ├── Create/
│   │   │       ├── Delete/
│   │   │       ├── Reorder/
│   │   │       └── Update/
│   │   │
│   │   ├── Constants/        # Static constant values (e.g., error messages)
│   │   │   └── ErrorMessagesConstants.cs
│   │   │
│   │   ├── DTOs/             # Data Transfer Objects
│   │   │   ├── Auth/
│   │   │   ├── Categories/
│   │   │   ├── Images/
│   │   │   ├── TeamMembers/
│   │   │   └── Payment/
│   │   │       ├── Common/
│   │   │       ├── WayForPay/
│   │   │       ├── Currency.cs
│   │   │       └── PaymentSystem.cs
│   │   │
│   │   ├── Exceptions/       # Custom exception classes
│   │   │
│   │   ├── Factories         # Business logic factories
│   │   │   └── Payment/
│   │   │       ├── Implementations/
│   │   │       └── Interfaces/
│   │   │
│   │   ├── Helpers/         # Helper classes and utilities
│   │   │   └── AuthHelper.cs
│   │   │
│   │   ├── Interfaces/      # Interfaces for services and infrastructure
│   │   │   ├── BlobStorage/
│   │   │   ├── PaymentService/
│   │   │   └── TokenService/
│   │   │
│   │   ├── Mapping/        # AutoMapper profiles for object mapping
│   │   │   ├── Categories/
│   │   │   ├── Images/
│   │   │   ├── TeamMembers/
│   │   │
│   │   ├── Options/        # Configuration-bound option classes
│   │   │
│   │   ├── Queries/        # CQRS Queries (Read operations)
│   │   │   ├── Categories/
│   │   │   │   ├── GetAll/
│   │   │   │   │   └── GetAllCategoriesHandler.cs
│   │   │   │   │   └── GetAllCategoriesQuery.cs
│   │   │   ├── Images/
│   │   │   │   ├── GetById/
│   │   │   │   ├── GetByName/
│   │   │   └── TeamMembers/
│   │   │       ├── GetByFilters/
│   │   │       ├── GetById/
│   │   │       └── GetPublished/
│   │   │
│   │   ├── Services/        # Service implementations
│   │   │   ├── BlobStorage/
│   │   │   ├── PaymentService/
│   │   │   └── TokenService/
│   │   │
│   │   └── Validators/      # FluentValidation validators
│   │       ├── Auth/
│   │       │   └── LoginCommandValidator.cs
│   │       ├── Categories/
│   │       ├── Images/
│   │       ├── Payment/
│   │       └── TeamMembers/
│   │
│   ├── VictoryCenter.DAL/
│   │   ├── Data/           # Database context and entity configuration
│   │   │   ├── EntityTypeConfigurations/   # EF Core entity configurations
│   │   │   │   └── CategoryConfig.cs
│   │   │   │   └── ImageConfig.cs
│   │   │   │   └── TeamMemberConfig.cs
│   │   │   └── VictoryCenterDbContext.cs
│   │   │
│   │   ├── Entities/       # Database entity classes
│   │   │   └── Admin.cs
│   │   │   └── Category.cs
│   │   │   └── Image.cs
│   │   │   └── TeamMember.cs
│   │   │
│   │   ├── Enums/          # Enum definitions
│   │   │   └── Status.cs
│   │   │
│   │   ├── Migrations/     # EF Core migrations
│   │   │   └── .editorconfig
│   │   │   └── 20250523215616_InitialMigration.Designer.cs
│   │   │   └── 20250523215616_InitialMigration.cs
│   │   │   └── ........................................
│   │   │   └── VictoryCenterDbContextModelSnapshot.cs
│   │   │
│   │   └── Repositories/  # Repository interfaces and implementations
│   │       ├── Interfaces/
│   │       │   ├── Base/
│   │       │   │   └── IRepositoryBase.cs
│   │       │   │   └── IRepositoryWrapper.cs
│   │       │   ├── Categories/
│   │       │   │   └── ICategoriesRepository.cs
│   │       │   ├── Media/
│   │       │   ├── TeamMembers/
│   │       ├── Options/
│   │       │   └── QueryOptions.cs
│   │       └── Realizations/
│   │           ├── Base/
│   │           │   └── RepositoryBase.cs
│   │           │   └── RepositoryWrapper.cs
│   │           ├── Categories/
│   │           │   └── CategoriesRepository.cs
│   │           ├── Media/
│   │           └── TeamMembers/
│   │   
│   ├── VictoryCenter.IntegrationTests/
│   │   ├── ControllerTests/     # Tests for API controllers
│   │   │   ├── Auth/
│   │   │   │   └── AuthControllerTests.cs
│   │   │   ├── Base/
│   │   │   ├── Categories/
│   │   │   │   ├── Create/
│   │   │   │   │   └── CreateCategoryTests.cs
│   │   │   │   ├── Delete/
│   │   │   │   ├── GetAll/
│   │   │   │   └── Update/
│   │   │   ├── Images/
│   │   │   │   ├── Create/
│   │   │   │   ├── Delete/
│   │   │   │   ├── GetById/
│   │   │   │   ├── GetByName/
│   │   │   │   └── Update/
│   │   │   ├── Payments/
│   │   │   │   └── PaymentsControllerTests.cs
│   │   │   ├── Team/
│   │   │   │   ├── GetPublished/
│   │   │   ├── TeamMembers/
│   │   │   │   ├── Create/
│   │   │   │   ├── Delete/
│   │   │   │   ├── GetById/
│   │   │   │   ├── GetFiltered/
│   │   │   │   ├── Reorder/
│   │   │   │   └── Update/
│   │   ├── MiddlewareTests/     # Tests for middleware behavior
│   │   ├── TestData/           
│   │   └── Utils/               # Test utilities
│   │       ├── Seeder/
│   │       │   ├── CategoriesSeeder/
│   │       │   ├── ImageSeeder/
│   │       │   ├── TeamMembersSeeder/
│   │       │   └── IntegrationTestsDatabaseSeeder.cs
│   │       └── FakeErrorController.cs
│   │       └── InMemoryLoggerProvider.cs
│   │       └── VictoryCenterWebApplicationFactory.cs
│   │
│   ├── VictoryCenter.UnitTests/
│   │   ├── Configuration/
│   │   │   └── DbContextMock.cs
│   │   ├── FactoriesTests/     # Tests for factories
│   │   │   ├── PaymentFactory/
│   │   │   └── CustomProblemDetailsFactoryTests.cs
│   │   ├── MediatRHandlersTests/   # Unit tests for MediatR handlers
│   │   │   ├── Auth/
│   │   │   ├── Categories/
│   │   │   ├── Images/
│   │   │   ├── Payment/
│   │   │   └── TeamMembers/
│   │   ├── MiddlewareTests/    # Unit tests for middleware
│   │   ├── ServiceTests/       # Unit tests for services
│   │   ├── TestData/           # Test data for unit tests
│   │   └── ValidatorsTests/    # Unit tests for FluentValidators
│   │       ├── Auth/
│   │       ├── Categories/
│   │       ├── Payment/
│   │       ├── TeamMembers/
│   │       └── TeamMembersTests/
│   │
├── docs/
│   └── 101-Project-Structure-Initital.md
│   └── .................................
└── .coderabbit.yaml
└── .gitignore
└── docker-compose.yml
```


## Installation

- All the `code` required to get started
- Images of what it should look like

### Required to install
- MS Visual Studio (2022 or later)
  https://visualstudio.microsoft.com/ru/downloads
- .NET SDK (v 6.0.101)
  https://dotnet.microsoft.com/en-us/download/dotnet/6.0
- NodeJS (v 16.13.2) 
  https://nodejs.org

### Environment
environmental variables
```shell
DB_CONNECTION_STRING="<DB_CONNECTION_STRING>"
INTEGRATION_TESTS_DB_CONNECTION_STRING="<INTEGRATION_TESTS_DB_CONNECTION_STRING>"
INITIAL_ADMIN_EMAIL="<INITIAL_ADMIN_EMAIL>"
INITIAL_ADMIN_PASSWORD="<INITIAL_ADMIN_PASSWORD>"
JWTOPTIONS_SECRETKEY="<JWT_ACCESS_SECRET>"
JWTOPTIONS_REFRESH_TOKEN_SECRETKEY="<JWT_REFRESH_SECRET>"
BLOB_LOCAL_STORE_KEY="<BLOB_LOCAL_STORE_KEY>"
WAY4PAY_MERCHANT_LOGIN="<WAY4PAY_MERCHANT_LOGIN>"
WAY4PAY_MERCHANT_SECRET_KEY="<WAY4PAY_MERCHANT_SECRET_KEY>"
WAY4PAY_MERCHANT_DOMAIN_NAME="<WAY4PAY_MERCHANT_DOMAIN_NAME>"
WAY4PAY_API_URL="<WAY4PAY_API_URL>"
```
```properties

### Clone

- Clone this repo to your local machine using `https://github.com/ita-social-projects/SOMEREPO`

### Setup

- If you want more syntax highlighting, format your code like this:

> update and install this package first

```shell
$ brew update
$ brew install SOMEREPOproductions
```

> now install npm and bower packages

```shell
$ npm install
$ bower install
```

- For all the possible languages that support syntax highlithing on GitHub (which is basically all of them), refer <a href="https://github.com/github/linguist/blob/master/lib/linguist/languages.yml" target="_blank">here</a>.

### How to run local
### How to connect to db locally
1. launch SQL Server management Studio
2. In the pop-up window:
    - enter **"localhost"** as the server name;
    - select **"windows authentication"** as authentication mechanism;
3. After the connection has been established, right-click on the server (the first line with the icon), on the left-hand side of the UI
4. In the the appeared window find and click on **"properties"**
5. In the properties section, select **"security"** page
6. Make sure that **"Server authentication"** radio-button is set to **"SQL Server and Windows Authentication mode"**
7. Click "Ok"
8. Then again, on the left-hand side of the UI find folder entitled **"Security"**, and expand it
9. In unrolled list of options find folder "Logins", and expand it
10. At this point, you should have **"sa"** as the last option.
    If for some reason you do not see it, please refer to https://stackoverflow.com/questions/35753254/why-login-without-rights-can-see-sa-login
11. Right-click on the "sa" item, select "properties"
12. Change password to the default system one - **"Admin@1234"**. Don't forget to confirm it afterwards
13. On the left-hand side select **"Status"** page, and set **"Login"** radio-button to **"Enabled"**
14. Click "Ok"
15. Right click on **"localhost"** server on the left-hand side of the UI and click **"Restart"**

Now you can connect to your localhost instance with login (sa) and password (Admin@1234)!


### How to run Docker
### how to connect to db via docker
1. Install and set up Docker if you haven't already
2. Open Docker Desktop
3. Open a terminal (either inside or outside your IDE)
4. Navigate to the project directory: .../VictoryCenter-Back/VictoryCenter
5. Start the Docker containers:
  ```text
  docker compose up 
  ```
6. Wait for Docker to pull and set up the necessary images and containers.
7. then open the appsettings.Development.json file and change Server to:
```text
  Server=localhost,1434;
  ```
The Docker-based database should now be running and ready for use!
### Additioanal information
1.The database container must be running whenever you're working on the backend. To start it:
```text
  docker compose up 
  ```
2.To stop the database:
```text
  docker compose down
  ```
Alternatively, you can use Docker Desktop:
1. Open Docker Desktop.
2. Go to the Containers tab
3. Find victorycenter-back
4. Start or stop it as needed

### Troubleshooting
If you encounter issues or want to reset the database:
```text
  docker compose down -v
  ```
Then recreate it using:
```text
  docker compose up
  ```
---

## Usage
### How to work with swagger UI
### How to run tests

---

## Static Code Analysis

To keep the codebase clean and maintainable, we use:

- `.editorconfig` for unified code formatting
- StyleCop analyzers across all projects
- SonarCloud for static code analysis
- SonarLint for IDE integration

Supported editors: Visual Studio 2022 / VS Code / Rider

### Quick Setup

1. **.editorconfig**  
Already included — most IDEs pick it up automatically.

2. **SonarLint**  
Set up SonarLint and bind it to the `VictoryCenter-Back` project of the `ita-social-projects` org using your personal SonarCloud token.

3. **Fix issues locally**  
Your IDE will highlight style/code issues in real time. Please, fix them before committing anything to avoid CI fails.

**Detailed setup guide:** [docs/104-Static-Code-Analysis.md](docs/104-Static-Code-Analysis.md)

---

## Documentation

---

## Contributing
1. All Pull Requests should start from prefix #xxx-yyy where xxx - task number and and yyy - short description e.g. #020-CreateAdminPanel
2. Pull request should not contain any files that is not required by task.

In case of any violations, pull request will be rejected.

### Git flow
> To get started...
#### Step 1

- **Option 1**
    - 🍴 Fork this repo!

- **Option 2**
    - 👯 Clone this repo to your local machine using `https://github.com/ita-social-projects/SOMEREPO.git`

#### Step 2

- **HACK AWAY!** 🔨🔨🔨

#### Step 3

- 🔃 Create a new pull request using <a href="https://github.com/ita-social-projects/SOMEREPO/compare/" target="_blank">github.com/ita-social-projects/SOMEREPO</a>.

### Issue flow

---

## Our Team

[![@IrynaZavushchak](https://avatars.githubusercontent.com/u/45690640?s=100&v=4)](https://github.com/IrynaZavushchak)
[![@Halyna Melnyk](https://avatars.githubusercontent.com/u/39273210?s=100&v=4)](https://github.com/mehalyna)
[![@LanchevychMaxym](https://avatars.githubusercontent.com/u/47561209?s=100&v=4)](https://github.com/LanchevychMaxym) 
[![@Roman Serhiichuk](https://avatars.githubusercontent.com/u/60231618?s=100&v=4)](https://github.com/Rominos7) 


---

---

## FAQ

- **Сan't  install .NET Core 6.0.0+ in Visual Studio?**
    - Try to install <a href="https://visualstudio.microsoft.com/ru/free-developer-offers/" target="_blank">Visual Studio 2022</a>

---

## Support

Reach out to us at one of the following places!

- Telegram at <a href="https://t.me/ira_zavushchak" target="_blank">`Iryna Zavushchak`</a>

---

## License
- **[MIT license](http://opensource.org/licenses/mit-license.php)**
- Copyright 2025 © <a href="https://softserve.academy/" target="_blank"> SoftServe Academy</a>.
