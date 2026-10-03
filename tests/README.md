# Portable source regressions

## Scope
The bitwise inverse previously cleared the alpha byte of every opaque pixel. Only RGB is now inverted and the source alpha is preserved. Tests cover opaque, partial and zero alpha, inverse luminance, multiple rows and untouched padding.

## Run
From repository root, with .NET SDK 10.0.x:

```sh
dotnet run --project tests/Portable.Tests/Portable.Tests.csproj --configuration Release
```

No NuGet packages are restored: tests/NuGet.Config clears package sources. The SDK targets only this harness; original application project files, package references and framework targets are unchanged. C# 5 syntax is enforced. CI uses commit-pinned checkout, a read-only token, no persisted credentials and a ten-minute timeout. It uses only a preinstalled .NET 10 SDK selected by global.json; it never installs a toolchain. If that SDK is absent, the workflow fails instead of downloading one.

## What is (and is not) verified
The project links actual production source files directly; it does not copy their logic. Minimal Nokia PixelRegion/CustomEffectBase adapters exercise the actual production OnProcess method on synthetic integer arrays. No camera, image SDK binary, XNA integration, native imaging pipeline, UI or device is tested. Source and target fixtures use matching indexing, as expected by the unchanged implementation.

The unchanged original source at `70df4e599932cb7390c86ad524aa7fd6b32482ed` failed the new synthetic regression harness before the fix; the final source passes. Additional reversal tests independently fail for each corrected notification/null path and for the asynchronous duplicate guard where applicable.

## Dependency upgrade boundary
Phone 8, NokiaImagingSDK 1.0.272.0 native imports and XNA-dependent WriteableBitmapEx require original SDK and device validation; MVVM/JSON upgrades were not guessed. This is not a camera-stack migration.

Microsoft documents [historical Windows/Phone toolchain requirements](https://learn.microsoft.com/en-us/visualstudio/releases/2022/port-migrate-and-upgrade-visual-studio-projects). [Newtonsoft.Json 13.0.4 package assets](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) must be evaluated against each original target; computed compatibility is not a native Phone assembly or an app validation. No full application build, native runtime/device test or exhaustive dependency vulnerability audit is claimed. The next decision is whether to recover the matching historical toolchain for compatible dependency work, or separately scope a supported-platform migration.
