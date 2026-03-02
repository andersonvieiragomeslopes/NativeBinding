Generate a .NET MAUI native binding project for a third-party SDK.

Arguments format: `$ARGUMENTS`
Expected: `<sdk-name> <platform: ios|android|both> [output-dir]`

Examples:
- `Amplitude both ./output/amplitude`
- `Firebase ios ./output/firebase`
- `Lottie android ./output/lottie`

## Steps

### 1. Parse arguments

Extract from `$ARGUMENTS`:
- **sdk-name** (required): The SDK to generate bindings for (e.g., "Amplitude", "Firebase", "Lottie")
- **platform** (required): `ios`, `android`, or `both`
- **output-dir** (optional, defaults to `./output/<sdk-name-lowercase>`)

If arguments are missing or malformed, ask the user to provide them in the correct format.

### 2. Research the SDK

Use WebSearch to find the official SDK documentation:
- Search for `<sdk-name> iOS SDK documentation` and/or `<sdk-name> Android SDK documentation`
- Find the official API reference, headers, or Javadoc
- Use WebFetch to read the actual API documentation pages

Gather:
- The SDK namespace / package name
- Public classes, protocols/interfaces, and their methods
- Properties, constructors, static methods
- Enums and constants
- Delegate/callback patterns

### 3. Generate iOS binding (if platform is `ios` or `both`)

Create the output directory at `<output-dir>/ios/` and generate these files:

**`<SdkName>.Binding.iOS.csproj`** — Use this template structure:
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
  <!-- Uncomment and update path to your .xcframework -->
  <!--
  <ItemGroup>
    <NativeReference Include="path/to/Native.xcframework">
      <Kind>Framework</Kind>
      <SmartLink>True</SmartLink>
    </NativeReference>
  </ItemGroup>
  -->
</Project>
```

**`ApiDefinition.cs`** — iOS binding definitions following these conventions:
- Use `using ObjCRuntime;` and `using Foundation;`
- Use `[BaseType(typeof(NSObject))]` for classes
- Use `[Export("selectorName:")]` for methods and properties
- Use `[Static]` for static members
- Use `[NullAllowed]` for nullable parameters/return types
- Use `[Protocol]`, `[Model]` for delegate/protocol patterns
- Use `[Abstract]` for required protocol members
- Map ObjC types: `NSString` → `string`, `NSArray` → `NSObject[]`, etc.

**`StructsAndEnums.cs`** — Enums and structs:
- Use `[Native]` attribute on enums that map to native enums
- Use `long` as the backing type for `nint`-backed enums
- Place in the correct namespace

### 4. Generate Android binding (if platform is `android` or `both`)

Create the output directory at `<output-dir>/android/` and generate these files:

**`<SdkName>.Binding.Android.csproj`** — Use this template structure:
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
  <!-- Uncomment and update path to your .aar file -->
  <!--
  <ItemGroup>
    <AndroidLibrary Include="path/to/library.aar" />
  </ItemGroup>
  -->
</Project>
```

**`Transforms/Metadata.xml`** — Metadata transform rules:
- Map Java package names to C# namespaces
- Fix parameter names and method names to follow C# conventions
- Remove problematic generated types if needed
- Example: `<attr path="/api/package[@name='com.example']" name="managedName">Example.Binding</attr>`

**`Transforms/EnumFields.xml`** — Enum field mappings:
- Map Java `static final int` constants to C# enum members
- Group related constants into enums
- Example: `<mapping jni-class="com/example/Type" clr-enum-type="Example.Binding.TypeEnum">`

**`Additions/Additions.cs`** — Manual additions:
- Partial classes for any types that need manual fixes
- Extension methods or helper wrappers if needed
- Event pattern adapters for Java listener interfaces

### 5. Summary

After generating all files, print a summary:
- List all files created with their paths
- Note any TODO items (e.g., "Add native reference path to .csproj")
- Suggest next steps (e.g., "Run `/fetch-artifact` to download the native library")
