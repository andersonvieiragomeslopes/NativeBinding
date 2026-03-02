Create a complete .NET MAUI native binding end-to-end: discover artifacts, fetch them, research the API, generate bindings, create a unified cross-platform API, generate tests, build, and iterate until everything passes.

Arguments format: `$ARGUMENTS`
Expected: `<sdk-name> <platform: ios|android|both> [output-dir]`

Examples:
- `Amplitude both ./output/amplitude`
- `Firebase ios ./output/firebase`
- `Lottie android ./output/lottie`

---

## Phase 1: Setup

Parse `$ARGUMENTS`:
- **sdk-name** (required): The SDK to generate bindings for (e.g., "Amplitude", "Firebase", "Lottie")
- **platform** (required): `ios`, `android`, or `both`
- **output-dir** (optional, defaults to `./output/<sdk-name-lowercase>`)

If arguments are missing or malformed, ask the user to provide them in the correct format.

Create directory structure:
```
<output-dir>/
  ios/          (if platform is ios or both)
  android/      (if platform is android or both)
  unified/      (if platform is both)
  tests/
```

Use `mkdir -p` to create all needed directories.

---

## Phase 2: Fetch Artifacts

For each target platform, discover and download the native artifact. The goal is **zero user input** when possible — only ask the user if all automatic approaches fail.

### Android

1. **Search Maven Central API** — Use WebFetch on `https://search.maven.org/solrsearch/select?q=<sdk-name>&rows=10&wt=json` to find Maven coordinates (groupId, artifactId, latest version).
2. **Also WebSearch** for `<sdk-name> Android SDK Maven Central coordinates` as a fallback.
3. **Download the AAR** — Construct the Maven Central URL:
   - Convert groupId dots to slashes: `com.amplitude` → `com/amplitude`
   - URL: `https://repo1.maven.org/maven2/{groupId-path}/{artifactId}/{version}/{artifactId}-{version}.aar`
   - If `.aar` 404s, try `.jar`
   - Download with `curl -L -o <output-dir>/android/<filename> "<url>"`
4. **Verify** the download: `file <output-dir>/android/<filename>` — must be a valid archive, not an HTML error page.

### iOS

Try these approaches in order — stop at the first success:

1. **GitHub Releases** — WebSearch for `<sdk-name> iOS SDK site:github.com`. If a repo is found:
   - `gh release list --repo <owner>/<repo> --limit 5`
   - `gh release view <latest-tag> --repo <owner>/<repo> --json assets`
   - Look for `.xcframework.zip` or `.xcframework` assets. Download with `gh release download` or `curl -L`.
   - If the download is a `.zip`, unzip it and locate the `.xcframework` directory.

2. **Build from source** — If no pre-built XCFramework exists:
   - Clone the repo: `git clone --depth 1 <repo-url> /tmp/<sdk-name>-src`
   - Inspect the project: look for `Package.swift` (SPM), `Cartfile` (Carthage), or `.xcodeproj`/`.xcworkspace`.
   - **SPM projects**: `swift build` to verify, then use `xcodebuild` with the scheme.
   - **Carthage projects**: `carthage bootstrap --use-xcframeworks --no-use-binaries --platform iOS` if carthage is installed, otherwise fall back to xcodebuild.
   - **xcodebuild** approach:
     ```bash
     # Find the scheme
     xcodebuild -list -project *.xcodeproj 2>/dev/null || xcodebuild -list -workspace *.xcworkspace

     # Archive for device
     xcodebuild archive \
       -scheme <scheme> \
       -destination "generic/platform=iOS" \
       -archivePath /tmp/<sdk-name>-device.xcarchive \
       SKIP_INSTALL=NO BUILD_LIBRARY_FOR_DISTRIBUTION=YES

     # Archive for simulator
     xcodebuild archive \
       -scheme <scheme> \
       -destination "generic/platform=iOS Simulator" \
       -archivePath /tmp/<sdk-name>-sim.xcarchive \
       SKIP_INSTALL=NO BUILD_LIBRARY_FOR_DISTRIBUTION=YES

     # Create XCFramework
     xcodebuild -create-xcframework \
       -framework /tmp/<sdk-name>-device.xcarchive/Products/Library/Frameworks/<Framework>.framework \
       -framework /tmp/<sdk-name>-sim.xcarchive/Products/Library/Frameworks/<Framework>.framework \
       -output <output-dir>/ios/<Framework>.xcframework
     ```
   - Clean up: `rm -rf /tmp/<sdk-name>-src /tmp/<sdk-name>-device.xcarchive /tmp/<sdk-name>-sim.xcarchive`

3. **Ask user** — Only if both approaches above fail. Tell the user what was tried and ask for a direct URL to an XCFramework or source repo.

### After fetching

Confirm the artifact exists and print its location and file size.

---

## Phase 3: Research API

Gather comprehensive API information to generate accurate bindings.

### WebSearch + WebFetch
- Search for `<sdk-name> iOS SDK API reference` and/or `<sdk-name> Android SDK API reference`
- Search for `<sdk-name> iOS SDK documentation` and/or `<sdk-name> Android SDK Javadoc`
- Use WebFetch to read the actual API documentation pages
- Gather: namespace/package name, public classes, protocols/interfaces, methods, properties, constructors, static methods, enums, constants, delegate/callback patterns

### Read ObjC headers from XCFramework (iOS)
If an XCFramework was fetched, inspect its headers:
```bash
find <output-dir>/ios/*.xcframework -name "*.h" -path "*/Headers/*"
```
Read the header files to get the actual Objective-C API surface — this is the most accurate source for iOS bindings.

### Inspect AAR contents (Android)
If an AAR was fetched, inspect it:
```bash
# List AAR contents
unzip -l <output-dir>/android/*.aar

# Extract and inspect the classes
mkdir -p /tmp/<sdk-name>-aar-inspect
unzip -o <output-dir>/android/*.aar -d /tmp/<sdk-name>-aar-inspect
# Check for public API
jar tf /tmp/<sdk-name>-aar-inspect/classes.jar | head -50

# Clean up
rm -rf /tmp/<sdk-name>-aar-inspect
```

### Combine all sources
Cross-reference the documentation with the actual artifact contents to build a complete API model. The artifact headers/classes are the source of truth; documentation fills in context about usage patterns.

---

## Phase 4: Generate Bindings

### iOS binding (if platform is `ios` or `both`)

Generate in `<output-dir>/ios/`:

**`<SdkName>.Binding.iOS.csproj`**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-ios</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsBindingProject>true</IsBindingProject>
  </PropertyGroup>
  <ItemGroup>
    <ObjcBindingApiDefinition Include="ApiDefinition.cs" />
    <ObjcBindingCoreSource Include="StructsAndEnums.cs" />
  </ItemGroup>
  <ItemGroup>
    <NativeReference Include="<actual-xcframework-filename>.xcframework">
      <Kind>Framework</Kind>
      <SmartLink>True</SmartLink>
    </NativeReference>
  </ItemGroup>
</Project>
```

**`ApiDefinition.cs`** — iOS binding definitions:
- Use `using ObjCRuntime;` and `using Foundation;`
- Use `[BaseType(typeof(NSObject))]` for classes
- Use `[Export("selectorName:")]` for methods and properties
- Use `[Static]` for static members
- Use `[NullAllowed]` for nullable parameters/return types
- Use `[Protocol]`, `[Model]` for delegate/protocol patterns
- Use `[Abstract]` for required protocol members
- Map ObjC types: `NSString` → `string`, `NSArray` → `NSObject[]`, etc.
- **Use actual API surface from headers** — do not guess

**`StructsAndEnums.cs`** — Enums and structs:
- Use `[Native]` attribute on enums that map to native enums
- Use `long` as the backing type for `nint`-backed enums
- Place in the correct namespace

### Android binding (if platform is `android` or `both`)

Generate in `<output-dir>/android/`:

**`<SdkName>.Binding.Android.csproj`**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-android</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <TransformFile Include="Transforms\Metadata.xml" />
    <TransformFile Include="Transforms\EnumFields.xml" />
  </ItemGroup>
  <ItemGroup>
    <AndroidLibrary Include="<actual-aar-filename>.aar" />
  </ItemGroup>
</Project>
```

**`Transforms/Metadata.xml`** — Map Java packages to C# namespaces, fix naming conventions.

**`Transforms/EnumFields.xml`** — Map Java `static final int` constants to C# enums.

**`Additions/Additions.cs`** — Partial classes, event adapters, manual fixes.

---

## Phase 5: Unified MAUI API (only when platform is `both`)

Generate in `<output-dir>/unified/`:

**`<SdkName>.Maui.csproj`**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net10.0-ios;net10.0-android</TargetFrameworks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
  </PropertyGroup>
  <ItemGroup Condition="'$(TargetFramework)' == 'net10.0-ios'">
    <ProjectReference Include="..\ios\<SdkName>.Binding.iOS.csproj" />
  </ItemGroup>
  <ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
    <ProjectReference Include="..\android\<SdkName>.Binding.Android.csproj" />
  </ItemGroup>
</Project>
```

**`I<SdkName>Service.cs`** — Clean C# interface abstracting the native API:
- Use idiomatic C# naming and patterns (PascalCase, events, async/await)
- Cover the core public API surface (initialization, main operations, configuration)

**`Platforms/iOS/<SdkName>Service.cs`** — iOS implementation:
- Implement the interface by calling into the iOS binding project types
- Handle platform-specific initialization

**`Platforms/Android/<SdkName>Service.cs`** — Android implementation:
- Implement the interface by calling into the Android binding project types
- Handle platform-specific initialization (Android Context, etc.)

**`<SdkName>ServiceExtensions.cs`** — DI registration:
```csharp
public static class <SdkName>ServiceExtensions
{
    public static MauiAppBuilder Use<SdkName>(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<I<SdkName>Service, <SdkName>Service>();
        return builder;
    }
}
```

---

## Phase 6: Generate Tests

Generate in `<output-dir>/tests/`:

**`<SdkName>.Binding.Tests.csproj`**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
  </ItemGroup>
</Project>
```

**Test classes:**

- **`CompilationTests.cs`** — Verify that the generated binding source files are valid C# by checking syntax (use Roslyn `CSharpSyntaxTree.ParseText` if possible, or simpler heuristics). At minimum verify files exist and are non-empty.
- **`ApiPresenceTests.cs`** — Use reflection or file-content checks to verify that key types, methods, and properties exist in the generated binding files.
- **`SmokeTests.cs`** — Basic smoke tests that verify the generated code structure (e.g., namespace declarations, class declarations, required attributes present).

**IMPORTANT**: Since test project targets `net10.0` (not ios/android), tests should validate the generated **source files** rather than trying to load platform-specific assemblies. Tests should read and analyze the `.cs` files as text or parse them with Roslyn.

Add project references or file includes as appropriate to access the generated source files.

---

## Phase 7: Build & Verify

### Install workloads if needed
```bash
# Check if required workloads are installed
dotnet workload list
# Install if missing
dotnet workload install maui-ios    # if targeting iOS
dotnet workload install maui-android # if targeting Android
```

### Build each project
Build in dependency order:
1. iOS binding project (if applicable): `dotnet build <output-dir>/ios/<SdkName>.Binding.iOS.csproj`
2. Android binding project (if applicable): `dotnet build <output-dir>/android/<SdkName>.Binding.Android.csproj`
3. Unified project (if applicable): `dotnet build <output-dir>/unified/<SdkName>.Maui.csproj`
4. Test project: `dotnet build <output-dir>/tests/<SdkName>.Binding.Tests.csproj`

### Run tests
```bash
dotnet test <output-dir>/tests/<SdkName>.Binding.Tests.csproj --verbosity normal
```

### Iterate on failures (max 5 attempts)
If build or test fails:
1. Read the error output carefully
2. Diagnose the root cause (missing attribute, wrong type mapping, incorrect selector, missing namespace, etc.)
3. Fix the generated source file(s)
4. Rebuild and retest
5. Repeat up to 5 times

Common fixes:
- **CS0246 type not found**: Add missing `using` statement or fix type name
- **BI1001 missing selector**: Fix `[Export]` attribute selector string
- **MT5210 native reference not found**: Fix path in `.csproj`
- **Java.Interop errors**: Fix Metadata.xml transforms
- **Test failures**: Fix test assertions to match actual generated code

---

## Phase 8: Report

### On success (all tests pass)
Print a clear summary:
- **Status**: SUCCESS
- **Files created**: List every file with its relative path
- **Build results**: Confirm each project built successfully
- **Test results**: Show test count (passed/failed/skipped)
- **Next steps**: How to use the generated binding in a MAUI app (add ProjectReference, register service in MauiProgram.cs, etc.)

### On max retries exhausted
Print:
- **Status**: PARTIAL — some issues remain
- **Files created**: List every file with its relative path
- **Build results**: Which projects succeeded/failed
- **Test results**: Which tests passed/failed
- **Remaining issues**: List each error with a suggested manual fix
- **What was tried**: Brief summary of fix attempts

---

## Important Notes

- **Artifact paths in .csproj files must be real** — always reference the actual downloaded artifact filename, never use placeholder paths.
- **Headers are the source of truth for iOS** — always prefer ObjC header content over documentation when they conflict.
- **AAR contents are the source of truth for Android** — always prefer inspecting the actual JAR classes over documentation.
- **Keep bindings focused** — bind the public API surface, not internal/private types.
- **Use the project's conventions** — follow patterns from CLAUDE.md (net10.0 TFM, attribute patterns, etc.).
