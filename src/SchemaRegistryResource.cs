using Aspire.Hosting.ApplicationModel;

namespace Mack.Aspire.Hosting.SchemaRegistry;

public sealed class SchemaRegistryResource([ResourceName] string name)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string PrimaryEndpointName = "tcp";
    internal const string InternalEndpointName = "internal";

    public EndpointReference InternalEndpoint =>
        field ??= new(this, InternalEndpointName);
    
    public EndpointReference PrimaryEndpoint =>
        field ??= new(this, PrimaryEndpointName);

    public EndpointReferenceExpression Port => PrimaryEndpoint.Property(EndpointProperty.Port);
    
    public EndpointReferenceExpression Host => PrimaryEndpoint.Property(EndpointProperty.Host);
    
    public EndpointReferenceExpression TargetPort => PrimaryEndpoint.Property(EndpointProperty.TargetPort);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{PrimaryEndpoint.Property(EndpointProperty.HostAndPort)}");

    IEnumerable<KeyValuePair<string, ReferenceExpression>> IResourceWithConnectionString.GetConnectionProperties()
    {
        yield return new("Host", ReferenceExpression.Create($"{Host}"));
        yield return new("Port", ReferenceExpression.Create($"{Port}"));
        yield return new("TargetPort", ReferenceExpression.Create($"{TargetPort}"));
    }
}