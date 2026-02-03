# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
- Planned: AWS X-Ray tracing, SSM Parameter Store integration, simple frontend (React/Razor), k6 load tests.

## [0.1.0] - 2026-02-03
### Added
- .NET 8 minimal API with AWS Lambda hosting via `Amazon.Lambda.AspNetCoreServer.Hosting`.
- Endpoints: `/health`, `/items` (CRUD with DynamoDB).
- AWS SDK for .NET registered via DI; configurable `DYNAMODB_TABLE_NAME`.
- Local development support using `docker-compose` (DynamoDB Local + API).
- Terraform module provisioning:
  - DynamoDB table (PK: `id`).
  - IAM role and least‑privilege policy for Lambda.
  - Lambda function using native .NET 8 runtime.
  - API Gateway (HTTP API v2) proxy integration.
  - CloudWatch log group.
  - Outputs for API URL and table name.
- CI/CD via GitHub Actions: build, test, package Lambda, `terraform fmt` and `terraform validate`, optional deploy with environment approval.
- xUnit test project with Moq and a basic service test.
- Portfolio‑grade README with architecture diagram and usage.

### Notes
- Lambda handler uses `Bootstrap` entry via hosting package.
- DynamoDB context uses `DynamoDBOperationConfig` with override (warnings present); can be refactored to new `*Config` overloads.

