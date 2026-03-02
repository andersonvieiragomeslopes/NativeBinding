using Xunit;

namespace Amplitude.Binding.Tests;

public class CompilationTests
{
    private static readonly string SolutionRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    [Fact]
    public void iOSApiDefinition_Exists_And_NonEmpty()
    {
        var path = Path.Combine(SolutionRoot, "ios", "ApiDefinition.cs");
        Assert.True(File.Exists(path), $"ApiDefinition.cs not found at {path}");
        var content = File.ReadAllText(path);
        Assert.True(content.Length > 100, "ApiDefinition.cs appears to be empty or too small");
    }

    [Fact]
    public void iOSStructsAndEnums_Exists_And_NonEmpty()
    {
        var path = Path.Combine(SolutionRoot, "ios", "StructsAndEnums.cs");
        Assert.True(File.Exists(path), $"StructsAndEnums.cs not found at {path}");
        var content = File.ReadAllText(path);
        Assert.True(content.Length > 50, "StructsAndEnums.cs appears to be empty or too small");
    }

    [Fact]
    public void AndroidMetadataXml_Exists_And_NonEmpty()
    {
        var path = Path.Combine(SolutionRoot, "android", "Transforms", "Metadata.xml");
        Assert.True(File.Exists(path), $"Metadata.xml not found at {path}");
        var content = File.ReadAllText(path);
        Assert.True(content.Length > 50, "Metadata.xml appears to be empty or too small");
    }

    [Fact]
    public void UnifiedInterface_Exists_And_NonEmpty()
    {
        var path = Path.Combine(SolutionRoot, "unified", "IAmplitudeService.cs");
        Assert.True(File.Exists(path), $"IAmplitudeService.cs not found at {path}");
        var content = File.ReadAllText(path);
        Assert.True(content.Length > 100, "IAmplitudeService.cs appears to be empty or too small");
    }

    [Fact]
    public void iOSCsproj_Exists()
    {
        var path = Path.Combine(SolutionRoot, "ios", "Amplitude.Binding.iOS.csproj");
        Assert.True(File.Exists(path), $"iOS csproj not found at {path}");
    }

    [Fact]
    public void AndroidCsproj_Exists()
    {
        var path = Path.Combine(SolutionRoot, "android", "Amplitude.Binding.Android.csproj");
        Assert.True(File.Exists(path), $"Android csproj not found at {path}");
    }

    [Fact]
    public void UnifiedCsproj_Exists()
    {
        var path = Path.Combine(SolutionRoot, "unified", "Amplitude.Maui.csproj");
        Assert.True(File.Exists(path), $"Unified csproj not found at {path}");
    }
}
