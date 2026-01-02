## Requirements
- .NET 8 SDK and above
- Docker (with Docker Compose) - Optional if not using the Quickstart environment

## Unit + Integration Tests

This project features unit and integration tests that can be run from any IDE or command line.

### Setting Up Integration Tests

Inside the `cyberark-conjur-pam.IntegrationTests` directory there is a [.env.test.example](./cyberark-conjur-pam.IntegrationTests/.env.test.example) file with the example environment variables. Copy the file to `.env.test` within the same directory. Fill out the environment variables. Make sure to configure `.env.test` to **always copy** to the output directory (this should already be set within the integration test's .csproj).

You are welcome to fill out the `.env.test` file to set these values to point to an existing CyberArk Conjur environment. Otherwise, these integration tests can also run against the Quickstart environment.

#### Running the Quickstart Environment

> This section can be skipped if you do not plan to run the integration tests against a local Docker environment.

The environment variables for the Quickstart environment are already defined in the `.env.ci` file under the `cyberark-conjur-pam.IntegrationTests` directory. Copy the `.env.ci` file to `.env.test`.

The Quickstart environment hosts a Docker Compose project that builds out a local CyberArk Conjur instance pre-populated with an admin account, a test user account, an example secret variable, and an example policy for the test user account to access the variable. The definition for the Quickstart environment is defined in [the quickstart directory](./quickstart/), which also includes the example policy defined under [the conf/policy](./quickstart/conf/policy) directory.

To run the Quickstart environment, run the following command from the project root:

```bash
# Use docker-compose if you're on an older Docker version
docker compose -f ./quickstart/docker-compose.yml up --build
```

The output of the `conjur-init` container should provide information on how to access the local Conjur instance, including the credentials to use to access the instance.

```bash
docker logs conjur-init

===========================================
ConjurUrl: http://localhost:8080 (or your mapped port if different)
AccountId: testConjurAccount

Admin Login: admin
Admin API Key: [randomly generated admin api key]

Test User Login: TestUser@local
Test User Password: Mypassw0rD2!
Test User API Key: [randomly generated user api key]

Test Variable: local/MyTestVar
Test Variable Value: MyTestSecretValue123
===========================================
```

The secret value for variable `local/MyTestVar` can be accessed by either the admin or the test user, with either their respective API keys or passwords.

The Quickstart environment can be cleaned up with the following command:

```bash
docker compose -f ./quickstart/docker-compose.yml down -v
```

### Running the Tests

Here are some command line scripts to run the test suites.

Restore project dependencies (optional):
```bash
dotnet restore
```

Run integration and unit tests:
```bash
dotnet test
```

Run just the unit tests:
```bash
dotnet test --filter "Category!=Integration"
```

Run just the integration tests:
```bash
dotnet test --filter "Category=Integration"
```