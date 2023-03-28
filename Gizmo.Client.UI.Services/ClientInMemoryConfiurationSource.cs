namespace Gizmo.Client.UI
{
    /// <summary>
    /// UI composition configuration source.
    /// </summary>
    public sealed class UICompositionInMemoryConfiurationSource : Microsoft.Extensions.Configuration.Json.InMemoryJsonStreamConfigurationSource
    {
    }

    /// <summary>
    /// UI options configuration source.
    /// </summary>
    public sealed class UIOptionsInMemoryConfigurationSource : Microsoft.Extensions.Configuration.Json.InMemoryJsonStreamConfigurationSource
    {

    }
}
