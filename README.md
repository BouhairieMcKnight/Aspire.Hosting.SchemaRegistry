# Aspire Hosting Confluent Schema Registry

A .NET Aspire hosting integration for running **Confluent Schema Registry** alongside a Kafka resource.

This package provides the following Aspire extension method:

```csharp
builder.AddSchemaRegistry(string name, int? port = null)
```
It runs Confluent Schema Registry using the Docker image:
```dockerfile
docker.io/confluentinc/cp-schema-registry:7.7.8
```

## Basic Usage
Schema Registry requires a Kafka resource reference.
```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka");

var registry = builder
    .AddSchemaRegistry("schema")
    .WithReference(kafka);

builder.Build().Run();
```

This creates:
````
Kafka resource:             kafka
Schema Registry resource:   schema
````
Usage With a Custom Port
You can expose Schema Registry on a specific host port by passing the optional port parameter:
````csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka");

var registry = builder
    .AddSchemaRegistry("schema", port: 8081)
    .WithReference(kafka);

builder.Build().Run();
````
When a fixed port is provided, Schema Registry will be available locally at:
```
http://localhost:8081
```
When no port is provided, Aspire will allocate a port automatically.
Required Kafka Reference
Schema Registry must be connected to Kafka.
Always create the Kafka resource first:
```csharp
var kafka = builder.AddKafka("kafka");
```
Then add Schema Registry and reference Kafka:
```csharp
var registry = builder
    .AddSchemaRegistry("schema")
    .WithReference(kafka);
````

Referencing Schema Registry From Another Resource
You can reference the Schema Registry resource from another Aspire project, service, worker, or API.
```csharp
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka");

var schemaRegistry = builder
    .AddSchemaRegistry("schema")
    .WithReference(kafka);

var api = builder
    .AddProject<Projects.MyApi>("api")
    .WithReference(kafka)
    .WithReference(schemaRegistry);

builder.Build().Run();
```
