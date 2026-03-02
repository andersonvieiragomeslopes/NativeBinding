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
4. **Check for companion/core JARs** — Kotlin-based SDKs often split their code across multiple artifacts. The main AAR may only contain the platform-specific layer, while core types (Configuration, Events, etc.) live in a separate `-core` JAR.
   - Inspect the AAR's POM file for dependencies: `https://repo1.maven.org/maven2/{groupId-path}/{artifactId}/{version}/{artifactId}-{version}.pom`
   - Look for a `*-core-*.jar` or similar artifact in the same groupId
   - Download companion JARs and add them as `<AndroidLibrary>` in the csproj
   - **This is critical**: without the core JAR, many classes (Amplitude, Configuration, Identify, BaseEvent, etc.) will be missing from the binding
5. **Verify** the download: `file <output-dir>/android/<filename>` — must be a valid archive, not an HTML error page.

### iOS

Try these approaches in order — stop at the first success:

1. **GitHub Releases** — WebSearch for `<sdk-name> iOS SDK site:github.com`. If a repo is found:
   - `gh release list --repo <owner>/<repo> --limit 5`
   - `gh release view <latest-tag> --repo <owner>/<repo> --json assets`
   - Look for `.xcframework.zip` or `.xcframework` assets. Download with `gh release download` or `curl -L`.
   - If the download is a `.zip`, unzip it and locate the `.xcframework` directory.

2. **Build from source via `swift build` + `ar` + `xcodebuild -create-xcframework`** — This is the most reliable approach when no pre-built XCFramework exists. **Do NOT use `xcodebuild archive`** as it often produces Mac Catalyst binaries instead of iOS, even with `-destination "generic/platform=iOS"`.

   ```bash
   # Clone the repo
   git clone --depth 1 --branch <tag> <repo-url> /tmp/<sdk-name>-src
   cd /tmp/<sdk-name>-src

   # Detect Xcode path — check for both Xcode.app and Xcode-beta.app
   XCODE_PATH=""
   if [ -d "/Applications/Xcode.app" ]; then
       XCODE_PATH="/Applications/Xcode.app/Contents/Developer"
   elif [ -d "/Applications/Xcode-beta.app" ]; then
       XCODE_PATH="/Applications/Xcode-beta.app/Contents/Developer"
   fi
   export DEVELOPER_DIR="$XCODE_PATH"

   # Get SDK paths
   IPHONEOS_SDK=$(xcrun --sdk iphoneos --show-sdk-path)
   IPHONESIMULATOR_SDK=$(xcrun --sdk iphonesimulator --show-sdk-path)

   # Check for SPM dependencies and resolve them
   if [ -f "Package.swift" ]; then
       swift package resolve
   fi

   # Build for device (arm64-apple-ios)
   swift build --triple arm64-apple-ios \
       --sdk "$IPHONEOS_SDK" \
       -Xswiftc "-sdk" -Xswiftc "$IPHONEOS_SDK"

   # Build for simulator (arm64-apple-ios-simulator)
   swift build --triple arm64-apple-ios-simulator \
       --sdk "$IPHONESIMULATOR_SDK" \
       -Xswiftc "-sdk" -Xswiftc "$IPHONESIMULATOR_SDK"

   # Collect all .o files and create static libraries with ar
   DEVICE_OBJECTS=$(find .build/arm64-apple-ios -name "*.o" -path "*/<SdkName>.build/*")
   ar rcs /tmp/<sdk-name>-device.a $DEVICE_OBJECTS

   SIM_OBJECTS=$(find .build/arm64-apple-ios-simulator -name "*.o" -path "*/<SdkName>.build/*")
   ar rcs /tmp/<sdk-name>-sim.a $SIM_OBJECTS

   # Collect headers — find all public .h files in the source or build
   # For ObjC projects: find headers in the source's include/Headers directory
   mkdir -p /tmp/<sdk-name>-headers
   find . -name "*.h" -path "*/include/*" -exec cp {} /tmp/<sdk-name>-headers/ \;
   # Also check build output for generated headers
   find .build -name "*.h" -path "*/Headers/*" -exec cp {} /tmp/<sdk-name>-headers/ \;

   # Create a module.modulemap
   cat > /tmp/<sdk-name>-headers/module.modulemap << 'MAPEOF'
   framework module <SdkName> {
       umbrella header "<SdkName>.h"
       export *
       module * { export * }
   }
   MAPEOF

   # Create XCFramework with -library flag (not -framework)
   xcodebuild -create-xcframework \
       -library /tmp/<sdk-name>-device.a -headers /tmp/<sdk-name>-headers \
       -library /tmp/<sdk-name>-sim.a -headers /tmp/<sdk-name>-headers \
       -output <output-dir>/ios/<SdkName>.xcframework

   # Clean up
   rm -rf /tmp/<sdk-name>-src /tmp/<sdk-name>-device.a /tmp/<sdk-name>-sim.a /tmp/<sdk-name>-headers
   ```

   **Key gotchas for iOS builds:**
   - `xcodebuild archive` often builds Mac Catalyst (platform 6) instead of iOS — avoid it
   - Always set `DEVELOPER_DIR` — `xcode-select -p` may point to CommandLineTools, not Xcode.app
   - Check for **both** `/Applications/Xcode.app` and `/Applications/Xcode-beta.app`
   - SPM dependencies must be resolved before `swift build`
   - Use `-library` flag (not `-framework`) with `xcodebuild -create-xcframework` for static libraries
   - Headers must be copied alongside the library for the binding project to find them

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

# Use javap to inspect actual method signatures
cd /tmp/<sdk-name>-aar-inspect
jar xf classes.jar
javap -public com/example/SdkName/*.class

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

**IMPORTANT: iOS build requires `DEVELOPER_DIR`** — Always build the iOS binding project with the Xcode environment variable:
```bash
DEVELOPER_DIR=/Applications/Xcode.app/Contents/Developer dotnet build <csproj>
# or Xcode-beta.app if that's what's installed
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
- **CRITICAL: Avoid namespace/class name clashes** — If the SDK's main class name matches a namespace segment (e.g., class `Amplitude` in namespace `Amplitude.iOS`), the compiler will resolve `Amplitude` as the namespace, not the class. Use a binding namespace like `<SdkName>Binding.iOS` instead of `<SdkName>.iOS`.
- **Protocol interface types may not resolve** — If a protocol like `IAMPMiddleware` causes CS0246 errors, use `NSObject` as the parameter type instead. The caller can still pass protocol-conforming objects.

**`StructsAndEnums.cs`** — Enums and structs:
- Use `[Native]` attribute on enums that map to native enums
- Use `long` as the backing type for `nint`-backed enums
- Place in the correct namespace (same binding namespace as ApiDefinition.cs)

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
    <!-- Include companion/core JARs if they exist -->
    <AndroidLibrary Include="<core-jar-filename>.jar" />
  </ItemGroup>
</Project>
```

**`Transforms/Metadata.xml`** — Map Java packages to C# namespaces and fix common issues:
```xml
<metadata>
  <!-- Map Java packages to C# namespaces -->
  <attr path="/api/package[@name='com.example.sdk']" name="managedName">Example.Sdk</attr>

  <!-- Remove internal packages not part of public API -->
  <remove-node path="/api/package[@name='com.example.sdk.internal']" />

  <!-- Remove plugin classes that clash with object.GetType() -->
  <!-- Java classes with a getType() method conflict with System.Object.GetType() -->
  <remove-node path="/api/package[@name='com.example.sdk.plugins']" />
  <remove-node path="/api/package[@name='com.example.sdk.platform']" />
  <remove-node path="/api/package[@name='com.example.sdk.platform.plugins']" />

  <!-- Remove logger classes with unimplemented interface members -->
  <!-- Classes implementing logging interfaces often have abstract members that -->
  <!-- can't be bound properly -->
  <remove-node path="/api/package[@name='com.example.common']/class[@name='ConsoleLogger']" />

  <!-- Fix field name clashes (member name same as enclosing type name — CS0542) -->
  <!-- When a class has a static field with the same name as the class, rename it -->
  <attr path="/api/package[@name='com.example.sdk.events']/class[@name='Revenue']/field[@name='REVENUE']" name="managedName">RevenueKey</attr>

  <!-- Remove network/storage/utility internals -->
  <remove-node path="/api/package[@name='com.example.sdk.network']" />
  <remove-node path="/api/package[@name='com.example.sdk.storage']" />
  <remove-node path="/api/package[@name='com.example.sdk.utilities']" />
</metadata>
```

**Common Metadata.xml patterns for fixing build errors:**
- **CS0535/CS0738 (unimplemented interface members)**: `<remove-node>` the offending class
- **CS0542 (member name same as type)**: Rename the field with `managedName`
- **CS0234 (type not found in namespace)**: Remove the package referencing missing types
- **`getType()` conflict**: Any Java class with a `getType()` method will clash with `System.Object.GetType()`. Remove the class or its package.
- **Kotlin internal classes**: Always remove packages named `internal`, `internal.*`
- **Build artifacts vs public API**: Proactively remove `BuildConfig`, `R`, `R$*` classes

**`Transforms/EnumFields.xml`** — Map Java `static final int` constants to C# enums.

**`Additions/Additions.cs`** — Partial classes, event adapters, manual fixes.

**Android build may require SDK installation:**
```bash
# If build fails with XA5207 (Android API not installed), run:
dotnet build -t:InstallAndroidDependencies \
    -p:AcceptAndroidSDKLicenses=true \
    <csproj-path>
```

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
- Use `global::` prefix for binding namespace types to avoid ambiguity
- Handle platform-specific initialization

**`Platforms/Android/<SdkName>Service.cs`** — Android implementation:
- Implement the interface by calling into the Android binding project types
- Handle platform-specific initialization (Android Context, etc.)
- **Type conversion gotchas:**
  - Use `new Java.Lang.Double(value)` for nullable Java Double properties
  - Use `new Java.Lang.String(value)` for dictionary values (Java Object type)
  - Use `new Dictionary<string, Java.Lang.Object>()` for Java Map parameters
  - Android `Configuration` often needs `global::Android.App.Application.Context`

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

### Check .NET SDK version
```bash
dotnet --version
```
If the installed .NET SDK is older than 10.0, install .NET 10:
```bash
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0 --quality preview
# Add to PATH: export PATH="$HOME/.dotnet:$PATH"
```

### Install workloads if needed
```bash
# Check if required workloads are installed
dotnet workload list
# Install if missing
dotnet workload install maui-ios    # if targeting iOS
dotnet workload install maui-android # if targeting Android
```

### Detect Xcode path (required for iOS builds)
```bash
# xcode-select -p often points to CommandLineTools, not Xcode.app
# Always check for the actual Xcode.app location
if [ -d "/Applications/Xcode.app" ]; then
    export DEVELOPER_DIR="/Applications/Xcode.app/Contents/Developer"
elif [ -d "/Applications/Xcode-beta.app" ]; then
    export DEVELOPER_DIR="/Applications/Xcode-beta.app/Contents/Developer"
fi
```

### Build each project
Build in dependency order. **Prefix iOS builds with DEVELOPER_DIR:**
1. iOS binding project (if applicable): `DEVELOPER_DIR=... dotnet build <output-dir>/ios/<SdkName>.Binding.iOS.csproj`
2. Android binding project (if applicable): `dotnet build <output-dir>/android/<SdkName>.Binding.Android.csproj`
3. Unified project (if applicable): `DEVELOPER_DIR=... dotnet build <output-dir>/unified/<SdkName>.Maui.csproj`
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
- **CS0246 type not found**: Add missing `using` statement or fix type name. For protocol interfaces (e.g., `IAMPMiddleware`), use `NSObject` instead.
- **CS0426 type used like namespace**: Namespace segment matches a class name. Rename the binding namespace (e.g., `AmplitudeBinding.iOS` instead of `Amplitude.iOS`).
- **CS0542 member name same as type**: Rename the field in Metadata.xml with `managedName`.
- **CS0535/CS0738 unimplemented interface**: Remove the class in Metadata.xml with `<remove-node>`.
- **BI1001 missing selector**: Fix `[Export]` attribute selector string.
- **MT5210 native reference not found**: Fix path in `.csproj`.
- **XA5207 Android API not installed**: Run `dotnet build -t:InstallAndroidDependencies -p:AcceptAndroidSDKLicenses=true`.
- **Java.Interop errors**: Fix Metadata.xml transforms.
- **NETSDK1045 SDK version too old**: Install .NET 10 SDK.
- **Xcode not found (MSB...)**:  Set `DEVELOPER_DIR` environment variable.
- **Test failures**: Fix test assertions to match actual generated code.

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
- **Kotlin SDKs need companion JARs** — A Kotlin multiplatform or Android SDK often splits code into `-android` (AAR) and `-core` (JAR). Both must be included.
- **Avoid namespace/class name collisions** — Use `<SdkName>Binding.iOS` instead of `<SdkName>.iOS` if the SDK has a class with the same name as the SDK.
- **Always set DEVELOPER_DIR for iOS builds** — `xcode-select -p` may point to CommandLineTools.
- **Android Metadata.xml is iterative** — Start with namespace mappings and internal removals, then fix errors one by one. The most common issues are `getType()` conflicts, unimplemented interfaces, and name clashes.
