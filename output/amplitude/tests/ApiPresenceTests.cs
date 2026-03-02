using Xunit;

namespace Amplitude.Binding.Tests;

public class ApiPresenceTests
{
    private static readonly string SolutionRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private string ReadiOSApiDefinition() =>
        File.ReadAllText(Path.Combine(SolutionRoot, "ios", "ApiDefinition.cs"));

    private string ReadUnifiedInterface() =>
        File.ReadAllText(Path.Combine(SolutionRoot, "unified", "IAmplitudeService.cs"));

    // --- iOS API Presence ---

    [Theory]
    [InlineData("interface Amplitude")]
    [InlineData("interface AMPIdentify")]
    [InlineData("interface AMPRevenue")]
    [InlineData("interface AMPTrackingOptions")]
    [InlineData("interface AMPDefaultTrackingOptions")]
    [InlineData("interface AMPPlan")]
    [InlineData("interface AMPIngestionMetadata")]
    [InlineData("interface AMPMiddlewarePayload")]
    public void iOSApiDefinition_Contains_Type(string typeDeclaration)
    {
        var content = ReadiOSApiDefinition();
        Assert.Contains(typeDeclaration, content);
    }

    [Theory]
    [InlineData("InitializeApiKey")]
    [InlineData("LogEvent")]
    [InlineData("LogRevenueV2")]
    [InlineData("Identify")]
    [InlineData("SetUserId")]
    [InlineData("SetDeviceId")]
    [InlineData("SetGroup")]
    [InlineData("UploadEvents")]
    [InlineData("SetServerZone")]
    [InlineData("SetOptOut")]
    [InlineData("ClearUserProperties")]
    [InlineData("SetTrackingOptions")]
    [InlineData("EnableCoppaControl")]
    [InlineData("RegenerateDeviceId")]
    [InlineData("GetSessionId")]
    [InlineData("GetDeviceId")]
    public void iOSApiDefinition_Contains_Method(string methodName)
    {
        var content = ReadiOSApiDefinition();
        Assert.Contains(methodName, content);
    }

    // --- iOS Enums ---

    [Theory]
    [InlineData("AMPServerZone")]
    [InlineData("US = 0")]
    [InlineData("EU = 1")]
    public void iOSStructsAndEnums_Contains(string expected)
    {
        var content = File.ReadAllText(Path.Combine(SolutionRoot, "ios", "StructsAndEnums.cs"));
        Assert.Contains(expected, content);
    }

    // --- Unified API Presence ---

    [Theory]
    [InlineData("void Initialize(string apiKey")]
    [InlineData("void LogEvent(string eventType")]
    [InlineData("void LogRevenue(")]
    [InlineData("void SetUserProperty(")]
    [InlineData("void IncrementUserProperty(")]
    [InlineData("void ClearUserProperties()")]
    [InlineData("void SetUserId(")]
    [InlineData("void SetDeviceId(")]
    [InlineData("string? GetDeviceId()")]
    [InlineData("long GetSessionId()")]
    [InlineData("void SetGroup(")]
    [InlineData("void SetOptOut(")]
    [InlineData("void UploadEvents()")]
    [InlineData("void Reset()")]
    [InlineData("void SetServerZone(")]
    [InlineData("void SetServerUrl(")]
    public void UnifiedInterface_Contains_Method(string methodSignature)
    {
        var content = ReadUnifiedInterface();
        Assert.Contains(methodSignature, content);
    }

    [Fact]
    public void UnifiedInterface_Contains_ServerZoneEnum()
    {
        var content = ReadUnifiedInterface();
        Assert.Contains("enum AmplitudeServerZone", content);
        Assert.Contains("US = 0", content);
        Assert.Contains("EU = 1", content);
    }
}
