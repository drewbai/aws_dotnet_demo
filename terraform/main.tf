terraform {
  required_version = ">= 1.5.0"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  region = var.aws_region
}

# ------------------------------------------------------------------------------------
# DynamoDB table for items (partition key: id)
# ------------------------------------------------------------------------------------
resource "aws_dynamodb_table" "items" {
  name         = var.table_name
  billing_mode = "PAY_PER_REQUEST"

  hash_key = "id"

  attribute {
    name = "id"
    type = "S"
  }

  tags = {
    Project = "aws_dotnet_demo"
  }
}

# ------------------------------------------------------------------------------------
# IAM role and policy for the Lambda API with least privileges
# ------------------------------------------------------------------------------------
resource "aws_iam_role" "api_role" {
  name = "${var.lambda_function_name}-role"
  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect = "Allow"
      Principal = { Service = "lambda.amazonaws.com" }
      Action   = "sts:AssumeRole"
    }]
  })
}

data "aws_iam_policy_document" "api_policy_doc" {
  statement {
    sid     = "DynamoCrud"
    actions = [
      "dynamodb:GetItem",
      "dynamodb:PutItem",
      "dynamodb:UpdateItem",
      "dynamodb:DeleteItem",
      "dynamodb:Scan",
      "dynamodb:Query"
    ]
    resources = [aws_dynamodb_table.items.arn]
  }

  statement {
    sid     = "CloudWatchLogs"
    actions = [
      "logs:CreateLogGroup",
      "logs:CreateLogStream",
      "logs:PutLogEvents"
    ]
    resources = ["*"]
  }
}

resource "aws_iam_policy" "api_policy" {
  name   = "${var.lambda_function_name}-policy"
  policy = data.aws_iam_policy_document.api_policy_doc.json
}

resource "aws_iam_role_policy_attachment" "api_policy_attach" {
  role       = aws_iam_role.api_role.name
  policy_arn = aws_iam_policy.api_policy.arn
}

# ------------------------------------------------------------------------------------
# CloudWatch Log group for the Lambda function
# ------------------------------------------------------------------------------------
resource "aws_cloudwatch_log_group" "lambda_logs" {
  name              = "/aws/lambda/${var.lambda_function_name}"
  retention_in_days = var.log_retention_days
}

# ------------------------------------------------------------------------------------
# Lambda function using native .NET 8 managed runtime
# ------------------------------------------------------------------------------------
resource "aws_lambda_function" "api" {
  function_name = var.lambda_function_name
  filename      = var.lambda_package
  source_code_hash = filebase64sha256(var.lambda_package)
  role          = aws_iam_role.api_role.arn
  handler       = "Bootstrap" # Using Amazon.Lambda.AspNetCoreServer.Hosting
  runtime       = "dotnet8"
  timeout       = 30
  memory_size   = 512

  environment {
    variables = {
      DYNAMODB_TABLE_NAME   = aws_dynamodb_table.items.name
      ASPNETCORE_ENVIRONMENT = "Production"
    }
  }

  depends_on = [aws_iam_role_policy_attachment.api_policy_attach]
}

# ------------------------------------------------------------------------------------
# API Gateway HTTP API integrating the Lambda (proxy-style routing)
# ------------------------------------------------------------------------------------
resource "aws_apigatewayv2_api" "http_api" {
  name          = "${var.lambda_function_name}-http-api"
  protocol_type = "HTTP"
}

resource "aws_apigatewayv2_integration" "lambda_integration" {
  api_id                 = aws_apigatewayv2_api.http_api.id
  integration_type       = "AWS_PROXY"
  integration_method     = "POST"
  payload_format_version = "2.0"
  integration_uri        = "arn:aws:apigateway:${var.aws_region}:lambda:path/2015-03-31/functions/${aws_lambda_function.api.arn}/invocations"
}

# Catch-all routes for proxy to ASP.NET Core minimal API
resource "aws_apigatewayv2_route" "root_any" {
  api_id    = aws_apigatewayv2_api.http_api.id
  route_key = "ANY /"
  target    = "integrations/${aws_apigatewayv2_integration.lambda_integration.id}"
}

resource "aws_apigatewayv2_route" "proxy_any" {
  api_id    = aws_apigatewayv2_api.http_api.id
  route_key = "ANY /{proxy+}"
  target    = "integrations/${aws_apigatewayv2_integration.lambda_integration.id}"
}

resource "aws_apigatewayv2_stage" "prod" {
  api_id      = aws_apigatewayv2_api.http_api.id
  name        = var.api_stage
  auto_deploy = true
}

data "aws_caller_identity" "current" {}

resource "aws_lambda_permission" "allow_apigw" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.api.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.aws_region}:${data.aws_caller_identity.current.account_id}:${aws_apigatewayv2_api.http_api.id}/*/*/*"
}

# ------------------------------------------------------------------------------------
# Outputs
# ------------------------------------------------------------------------------------
output "api_base_url" {
  description = "Base URL for the HTTP API"
  value       = "https://${aws_apigatewayv2_api.http_api.id}.execute-api.${var.aws_region}.amazonaws.com/${aws_apigatewayv2_stage.prod.name}"
}

output "dynamodb_table_name" {
  description = "Provisioned DynamoDB table name"
  value       = aws_dynamodb_table.items.name
}
