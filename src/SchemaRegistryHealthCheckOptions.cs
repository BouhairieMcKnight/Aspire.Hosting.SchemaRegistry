using Confluent.SchemaRegistry;

namespace Aspire.Hosting;

public sealed class SchemaRegistryHealthCheckOptions
{
    public SchemaRegistryConfig Configuration { get; set; } = null!; 
}