using Xunit;

namespace Amplitude.Binding.Tests;

public class SmokeTests
{
    private static readonly string SolutionRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    [Fact]
    public void iOSApiDefinition_Has_Correct_Namespace()
    {
        var content = File.ReadAllText(Path.Combine(SolutionRoot, "ios", "ApiDefinition.cs"));
        Assert.Contains("namespace AmplitudeBinding.iOS", content);
    }

    [Fact]
    public void iOSApiDefinition_Uses_Required_Attributes()
    {
        var content = File.ReadAllText(Path.Combine(SolutionRoot, "ios", "ApiDefinition.cs"));
        Assert.Contains("[BaseType(typeof(NSObject))]", content);
        Assert.Contains("[Export(", content);
        Assert.Contains("[Static]", content);
        Assert.Contains("[NullAllowed", content);
        Assert.Contains("[Protocol", content);
    }

    [Fact]
    public void iOSApiDefinition_Uses_Required_Usings()
    {
        var content = File.ReadAllText(Path.Combine(SolutionRoot, "ios", "ApiDefinition.cs"));
        Assert.Contains("using Foundation;", content);
        Assert.Contains("using ObjCRuntime;", content);
    }

    [Fact]
    public void iOSCsproj_IsBindingProject()
    {
        var content = File.ReadAllText(Path.Combine(SolutionRoot, "ios", "Amplitude.Binding.iOS.csproj"));
        Assert.Contains("<IsBindingProject>true</IsBindingProject>", content);
        Assert.Contains("net10.0-ios", content);
        Assert.Contains("ObjcBindingApiDefinition", content);
        Assert.Contains("ObjcBindingCoreSource", content);
        Assert.Contains("NativeReference", content);
        Assert.Contains("Amplitude.xcframework", content);
    }

    [Fact]
    public void AndroidCsproj_HasCorrectStructure()
    {
        var content = File.ReadAllText(Path.Combine(SolutionRoot, "android", "Amplitude.Binding.Android.csproj"));
        Assert.Contains("net10.0-android", content);
        Assert.Contains("TransformFile", content);
        Assert.Contains("Metadata.xml", content);
        Assert.Contains("AndroidLibrary", content);
        Assert.Contains("analytics-android-1.21.3.aar", content);
    }

    [Fact]
    public void AndroidMetadata_HasPackageMappings()
    {
        var content = File.ReadAllText(Path.Combine(SolutionRoot, "android", "Transforms", "Metadata.xml"));
        Assert.Contains("com.amplitude.android", content);
        Assert.Contains("managedName", content);
        Assert.Contains("Amplitude.Android", content);
    }

    [Fact]
    public void UnifiedCsproj_HasCorrectStructure()
    {
        var content = File.ReadAllText(Path.Combine(SolutionRoot, "unified", "Amplitude.Maui.csproj"));
        Assert.Contains("net10.0-ios", content);
        Assert.Contains("net10.0-android", content);
        Assert.Contains("<UseMaui>true</UseMaui>", content);
        Assert.Contains("ProjectReference", content);
        Assert.Contains("Amplitude.Binding.iOS.csproj", content);
        Assert.Contains("Amplitude.Binding.Android.csproj", content);
    }

    [Fact]
    public void UnifiedDIExtensions_Exists_And_Registers_Service()
    {
        var path = Path.Combine(SolutionRoot, "unified", "AmplitudeServiceExtensions.cs");
        Assert.True(File.Exists(path));
        var content = File.ReadAllText(path);
        Assert.Contains("UseAmplitude", content);
        Assert.Contains("AddSingleton<IAmplitudeService, AmplitudeService>", content);
        Assert.Contains("MauiAppBuilder", content);
    }

    [Fact]
    public void XcFramework_Exists_InOutputDirectory()
    {
        var path = Path.Combine(SolutionRoot, "ios", "Amplitude.xcframework");
        Assert.True(Directory.Exists(path), $"XCFramework not found at {path}");
    }

    [Fact]
    public void AndroidAar_Exists_InOutputDirectory()
    {
        var path = Path.Combine(SolutionRoot, "android", "analytics-android-1.21.3.aar");
        Assert.True(File.Exists(path), $"AAR not found at {path}");
    }
}
