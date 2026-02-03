# aws_dotnet_demo

A production-style, portfolio-ready AWS + .NET 8 demo showcasing serverless APIs, Infrastructure as Code (Terraform), and clean DevOps patterns.

## Architecture

```mermaid
flowchart LR
    client((Client))
    apigw[API Gateway (HTTP API)]
    lambda[Lambda (.NET 8 Minimal API)]
    dynamo[(DynamoDB Table)]
    logs[(CloudWatch Logs)]

    client -->|HTTPS| apigw
    apigw --> lambda
    lambda --> dynamo
    lambda --> logs
```

- .NET 8 Minimal API exposing `/health` and `/items` (CRUD)
- Serverless hosting in AWS Lambda with API Gateway (HTTP API v2)
- DynamoDB for persistence (PK: `id`)
- AWS SDK for .NET via DI
- Local development via `docker-compose` (DynamoDB Local + API)

## Getting Started (Local)

Prerequisites:
- Docker Desktop
- .NET 8 SDK (optional for local container build)

Run locally with Docker Compose:

```bash
docker compose up --build
```

- API: http://localhost:8080
- DynamoDB Local: http://localhost:8000 (in-memory)

## API Endpoints

- Health:
  ```bash
  curl http://localhost:8080/health
  ```
- Create Item:
  ```bash
  curl -X POST http://localhost:8080/items \
    -H "Content-Type: application/json" \
    -d '{"name":"Sample","description":"Demo item"}'
  ```
- List Items:
  ```bash
  curl http://localhost:8080/items
  ```
- Get Item:
  ```bash
  curl http://localhost:8080/items/{id}
  ```
- Update Item:
  ```bash
  curl -X PUT http://localhost:8080/items/{id} \
    -H "Content-Type: application/json" \
    -d '{"name":"Updated","description":"New description"}'
  ```
- Delete Item:
  ```bash
  curl -X DELETE http://localhost:8080/items/{id}
  ```

## Deploy with Terraform

Prerequisites:
- Terraform >= 1.5
- AWS access via credentials or OIDC (GitHub Actions)
- .NET 8 SDK

Manual deploy (bash):
```bash
scripts/deploy.sh
```

Terraform provisions:
- DynamoDB table
- Lambda (.NET 8 managed runtime) with least-privilege IAM role/policy
- API Gateway HTTP API with Lambda proxy integration
- CloudWatch log groups
- Outputs: API base URL, table name

## CI/CD (GitHub Actions)

Workflow includes:
- Build and test (.NET 8, xUnit)
- Package Lambda artifact (linux-x64)
- `terraform fmt` + `terraform validate`
- Optional deploy to AWS on push to `main` behind environment approval (configure `AWS_OIDC_ROLE_ARN` secret and environment protection in GitHub)

## Code Quality

- Clean layering: `Models`, `Repositories`, `Services`, minimal API endpoints
- XML summaries on public classes/methods
- Logging & basic error paths
- Small xUnit test project with Moq

## Skills Demonstrated

- AWS serverless (Lambda, API Gateway, DynamoDB, CloudWatch)
- .NET 8 minimal APIs, dependency injection
- AWS SDK for .NET, environment-based configuration
- Terraform IaC and validation routines
- DevOps via GitHub Actions (OIDC, environment approvals)

## Optional Enhancements

- AWS X-Ray tracing (add AWS X-Ray SDK and middleware)
- Parameter Store for configuration (SSM parameters -> environment)
- Simple frontend (React or Razor Pages)
- Load testing script (k6)
