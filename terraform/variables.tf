# General variables
variable "aws_region" {
  description = "AWS region to deploy to"
  type        = string
  default     = "us-east-1"
}

variable "table_name" {
  description = "DynamoDB table name"
  type        = string
  default     = "aws_dotnet_demo_items"
}

variable "lambda_function_name" {
  description = "Lambda function name"
  type        = string
  default     = "aws-dotnet-demo-api"
}

variable "lambda_package" {
  description = "Path to the packaged Lambda zip"
  type        = string
  default     = "./terraform/artifacts/aws_dotnet_demo_api.zip"
}

variable "api_stage" {
  description = "API Gateway stage name"
  type        = string
  default     = "prod"
}

variable "log_retention_days" {
  description = "CloudWatch Log group retention in days"
  type        = number
  default     = 14
}
