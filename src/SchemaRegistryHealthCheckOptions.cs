using Confluent.SchemaRegistry;

namespace Mack.Aspire.Hosting.SchemaRegistry;

public sealed class SchemaRegistryHealthCheckOptions
{
    public SchemaRegistryConfig Configuration { get; set; } = null!; 
}