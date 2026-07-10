# CI/CD Setup - iOS TestFlight Without a Physical Mac

This repository includes a GitHub Actions workflow at [.github/workflows/ios-testflight.yml](.github/workflows/ios-testflight.yml) that builds a signed iOS IPA on GitHub-hosted macOS and uploads it to TestFlight.

## What You Need

- Apple Developer account
- App Store Connect access to your app
- Distribution signing certificate (.p12)
- iOS provisioning profile (.mobileprovision)
- App Store Connect API key (.p8)

## Required GitHub Secrets

Create these in GitHub: Settings -> Secrets and variables -> Actions.

- IOS_CERTIFICATE_P12_BASE64
- IOS_CERTIFICATE_P12_PASSWORD
- IOS_PROVISIONING_PROFILE_BASE64
- IOS_CODESIGN_KEY
- IOS_PROVISIONING_PROFILE_NAME
- APPSTORE_CONNECT_KEY_ID
- APPSTORE_CONNECT_ISSUER_ID
- APPSTORE_CONNECT_API_KEY_BASE64

## How to Prepare Secret Values

### 1) Base64 encode certificate and provisioning profile

On macOS/Linux:

```bash
base64 -i ios_distribution.p12 | pbcopy
base64 -i profile.mobileprovision | pbcopy
base64 -i AuthKey_ABC123XYZ.p8 | pbcopy
```

On PowerShell:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("ios_distribution.p12"))
[Convert]::ToBase64String([IO.File]::ReadAllBytes("profile.mobileprovision"))
[Convert]::ToBase64String([IO.File]::ReadAllBytes("AuthKey_ABC123XYZ.p8"))
```

### 2) Determine IOS_CODESIGN_KEY

Use the exact certificate common name, for example:

- Apple Distribution: Your Company, Inc. (TEAMID1234)

### 3) Determine IOS_PROVISIONING_PROFILE_NAME

Use the profile name shown in Apple Developer portal, for example:

- ShowMeWhere AppStore Profile

## How to Run

### Manual build (optional upload)

- Open Actions -> iOS TestFlight -> Run workflow
- Set Upload build to TestFlight = true or false

If false, the workflow still produces IPA artifact named ios-ipa.

### Tag-driven release upload

Push a tag like:

```bash
git tag v1.0.0
git push origin v1.0.0
```

The workflow runs automatically and uploads to TestFlight.

## Notes

- The workflow builds from macOS in GitHub Actions, so no local Mac is required.
- The app project already performs frontend build via npm during .NET publish.
- If code signing fails, validate IOS_CODESIGN_KEY and IOS_PROVISIONING_PROFILE_NAME first.
