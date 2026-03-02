Fetch a native artifact (AAR, JAR, or XCFramework) and place it in a binding project.

Arguments format: `$ARGUMENTS`
Expected: `<sdk-name> <platform: ios|android|both> <output-dir>`

Examples:
- `Amplitude both ./output/amplitude`
- `Lottie android ./output/lottie`
- `com.amplitude:analytics-android:1.22.4 android ./output/amplitude/android`
- `https://github.com/nicklockwood/iCarousel/releases/download/1.8.3/iCarousel.xcframework.zip ios ./output/icarousel/ios`

## Steps

### 1. Parse arguments

Extract from `$ARGUMENTS`:
- **sdk-name or source** (required): Either an SDK name (e.g., "Amplitude"), Maven coordinates (`groupId:artifactId:version`), or a direct URL
- **platform** (required): `ios`, `android`, or `both`
- **output-dir** (required): Where to place the downloaded artifact(s)

If arguments are missing or malformed, ask the user to provide them in the correct format.

### 2. Auto-discover artifact sources

If the user provided just an SDK name (not Maven coordinates or a URL), automatically search for the artifact:

**For Android:**
1. Use WebSearch to search for `<sdk-name> Android SDK Maven Central` or `<sdk-name> Android artifact Maven coordinates`
2. Also try searching on `search.maven.org` for the artifact: use WebFetch on `https://search.maven.org/solrsearch/select?q=<sdk-name>&rows=5&wt=json` to find Maven coordinates
3. Look for the groupId, artifactId, and latest version
4. Construct Maven coordinates from the results

**For iOS:**
1. Use WebSearch to search for `<sdk-name> iOS SDK XCFramework download` or `<sdk-name> iOS SDK GitHub releases`
2. Use WebSearch to find the GitHub repository: `<sdk-name> iOS SDK site:github.com`
3. If a GitHub repo is found, check releases for `.xcframework.zip` or `.xcframework` assets using: `gh release list --repo <owner>/<repo> --limit 5` and then `gh release view <tag> --repo <owner>/<repo> --json assets`
4. Look for direct download URLs for the XCFramework

If auto-discovery succeeds, proceed to download. If it fails, tell the user what was tried and ask them to provide the exact source (Maven coordinates for Android, or a direct URL for iOS).

### 3. Download the artifact

**For Android (Maven coordinates like `com.amplitude:analytics-android:1.22.4`):**

Construct the Maven Central URL:
- Convert groupId dots to slashes: `com.amplitude` → `com/amplitude`
- URL pattern: `https://repo1.maven.org/maven2/{groupId-as-path}/{artifactId}/{version}/{artifactId}-{version}.aar`
- If `.aar` fails, try `.jar`

Download using curl:
```bash
curl -L -o <output-dir>/<artifactId>-<version>.aar "<maven-url>"
```

Verify the download succeeded (check file size > 0 and not an HTML error page):
```bash
file <output-dir>/<artifactId>-<version>.aar
```

**For iOS / direct URLs:**

Download the file directly:
```bash
curl -L -o <output-dir>/<filename> "<url>"
```

If the downloaded file is a `.zip`, extract it:
```bash
unzip <output-dir>/<filename> -d <output-dir>/
```

After extraction, find the `.xcframework` directory:
```bash
find <output-dir> -name "*.xcframework" -type d
```

**If download fails:** Inform the user that the artifact could not be fetched automatically and ask them to provide a direct URL or Maven coordinates.

### 4. Update the .csproj (if it exists)

Look for a `.csproj` file in `<output-dir>`. If found:

**For Android:** Uncomment or add the `<AndroidLibrary>` item:
```xml
<ItemGroup>
  <AndroidLibrary Include="<artifactId>-<version>.aar" />
</ItemGroup>
```

**For iOS:** Uncomment or add the `<NativeReference>` item:
```xml
<ItemGroup>
  <NativeReference Include="<framework-name>.xcframework">
    <Kind>Framework</Kind>
    <SmartLink>True</SmartLink>
  </NativeReference>
</ItemGroup>
```

### 5. Summary

Print:
- The artifact that was downloaded and its location
- Whether the .csproj was updated
- File size of the downloaded artifact
- Any next steps needed
