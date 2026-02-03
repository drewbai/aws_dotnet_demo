using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.AspNetCoreServer.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using aws_dotnet_demo.Api.Models;
using aws_dotnet_demo.Api.Services;
using aws_dotnet_demo.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Enable AWS Lambda hosting for HTTP API (API Gateway v2)
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

// Configure AWS/DynamoDB client
var dynamoEndpoint = Environment.GetEnvironmentVariable("DYNAMODB_ENDPOINT_URL");
if (!string.IsNullOrWhiteSpace(dynamoEndpoint))
{
    builder.Services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(new AmazonDynamoDBConfig
    {
        ServiceURL = dynamoEndpoint,
        UseHttp = true
    }));
}
else
{
    builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
    builder.Services.AddAWSService<IAmazonDynamoDB>();
}

builder.Services.AddSingleton<IDynamoDBContext>(sp => new DynamoDBContext(sp.GetRequiredService<IAmazonDynamoDB>()));

// App config for DynamoDB table name
builder.Services.Configure<DynamoSettings>(builder.Configuration.GetSection("DynamoDb"));

// Repository & Service DI
builder.Services.AddSingleton<IItemRepository, DynamoDbItemRepository>();
builder.Services.AddSingleton<ItemService>();

var app = builder.Build();

string tableName = builder.Configuration["DynamoDb:TableName"]
    ?? Environment.GetEnvironmentVariable("DYNAMODB_TABLE_NAME")
    ?? "aws_dotnet_demo_items";

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/items", async (ItemService svc) => Results.Ok(await svc.ListAsync(tableName)));

app.MapGet("/items/{id}", async (string id, ItemService svc) =>
{
    var item = await svc.GetAsync(id, tableName);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPost("/items", async (Item item, ItemService svc) =>
{
    item.Id ??= Guid.NewGuid().ToString();
    item.CreatedAt = DateTimeOffset.UtcNow;
    await svc.CreateAsync(item, tableName);
    return Results.Created($"/items/{item.Id}", item);
});

app.MapPut("/items/{id}", async (string id, Item item, ItemService svc) =>
{
    item.Id = id;
    item.UpdatedAt = DateTimeOffset.UtcNow;
    await svc.UpdateAsync(item, tableName);
    return Results.Ok(item);
});

app.MapDelete("/items/{id}", async (string id, ItemService svc) =>
{
    var deleted = await svc.DeleteAsync(id, tableName);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
