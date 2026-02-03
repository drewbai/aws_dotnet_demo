#!/usr/bin/env bash
set -euo pipefail

# Simple deploy script: build .NET, package zip, apply Terraform

ROOT_DIR=$(cd "$(dirname "$0")/.." && pwd)
API_DIR="$ROOT_DIR/src/aws_dotnet_demo.Api"
TF_DIR="$ROOT_DIR/terraform"
ART_DIR="$TF_DIR/artifacts"
ZIP_PATH="$ART_DIR/aws_dotnet_demo_api.zip"

mkdir -p "$ART_DIR"

echo "Publishing .NET API for linux-x64..."
dotnet publish "$API_DIR/aws_dotnet_demo.Api.csproj" -c Release -r linux-x64 --self-contained false -o "$ART_DIR/publish"

echo "Packaging Lambda zip..."
(cd "$ART_DIR/publish" && zip -r9 "$ZIP_PATH" .)

echo "Running terraform init/validate/apply..."
cd "$TF_DIR"
terraform fmt -recursive
terraform init -input=false
terraform validate
terraform apply -auto-approve

echo "Deployment complete. Outputs:"
terraform output
