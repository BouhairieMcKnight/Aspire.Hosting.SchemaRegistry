using Confluent.SchemaRegistry;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mack.Aspire.Hosting.SchemaRegistry;

/// <summary>
/// Implements the <see cref="IHealthCheck"/> and <see cref="IDisposable"/> interfaces for providing health checks on schema registry containers
/// </summary>
/// <param name="options">
/// <see cref="SchemaRegistryHealthCheckOptions"/> that provide the <see cref="SchemaRegistryConfig"/> for initializing a schema registry client
/// </param>
public class SchemaRegistryHealthCheck(SchemaRegistryHealthCheckOptions options) 
    : IHealthCheck, IDisposable
{
    private ISchemaRegistryClient? _schemaRegistryClient;

    /// <summary>
    /// Lazily creates a Cached Schema Registry Client from options to async register a test schema and get result
    /// of registration
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// returns health check result of a failure status with description or a result with a healthy status
    /// </returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            _schemaRegistryClient ??= new CachedSchemaRegistryClient(options.Configuration);
            var schemaString = "{\n \"namespace\": \"io.confluent.examples.clients.basicavro\",\n \"type\": \"record\",\n \"name\": \"Payment\",\n \"fields\": [\n     {\"name\": \"id\", \"type\": \"string\"},\n     {\"name\": \"amount\", \"type\": \"double\"}\n ]\n}";
            var subject = "my-topic-value";

            await _schemaRegistryClient.RegisterSchemaAsync(subject, new Schema(schemaString, SchemaType.Avro)).ConfigureAwait(false);
            
            var subjects = await _schemaRegistryClient.GetAllSubjectsAsync().ConfigureAwait(false);
            
            if (!subjects.Any() || !subjects.Contains(subject))
            {
                return new HealthCheckResult(context.Registration.FailureStatus, "Could not get subjects");
            }

        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "Exception on health check was thrown", ex);
        }
        
        return HealthCheckResult.Healthy();
    }

    public void Dispose()
    {
        _schemaRegistryClient?.Dispose();
    }
}