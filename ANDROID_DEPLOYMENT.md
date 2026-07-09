# Android Testing & Deployment Guide

## Prerequisites

### On Windows:
1. Android SDK (typically installed via Visual Studio)
2. `adb` command-line tool (Android Debug Bridge)
   - Usually at: `C:\Android\platform-tools\adb.exe`
   - Or install via: `choco install android-sdk`

### On Android Device:
1. Enable **Developer Mode**:
   - Settings → About Phone → tap Build Number 7 times
2. Enable **USB Debugging**:
   - Settings → Developer Options → USB Debugging (toggle ON)
3. Connect via USB cable
4. Accept the "Allow debugging" prompt on phone

## Build Commands

### Quick Local Build (Debug)
```powershell
.\build-android.ps1
```
Output: Unpackaged app for running locally during development

### Create APK (for distribution)
```powershell
.\build-android.ps1 -Configuration Release -Package
```
Output: Standalone APK file (~80-120 MB)

### Build & Install Directly to Device
```powershell
.\build-android.ps1 -Install
```
Builds and immediately installs to connected device

### Clean Build
```powershell
.\build-android.ps1 -Clean -Configuration Release -Package
```
Full rebuild removing cached artifacts

## Deployment Options

### Option 1: Direct from VS Code (Recommended)
```powershell
dotnet build src/ShowMeWhere.App/ShowMeWhere.App.csproj -f net10.0-android -t Install
```

### Option 2: Using ADB
```bash
# List connected devices
adb devices

# Install APK
adb install -r path/to/ShowMeWhere.App.apk

# Uninstall app
adb uninstall com.companyname.showmewhere.app

# View real-time logs
adb logcat -s ShowMeWhere
```

### Option 3: Email/Share APK
1. Build with `-Package` flag
2. Located at: `src/ShowMeWhere.App/bin/Release/net10.0-android/publish/ShowMeWhere.App.apk`
3. Email, cloud upload, or share via Bluetooth to testers
4. Testers tap APK file to install (requires "Unknown Sources" enabled)

## Testing Checklist

- [ ] App launches without crashes
- [ ] Detect Level tab: sensor readings update in real-time
- [ ] Save Level tab: can capture and save parking location
- [ ] Return to Car tab: shows saved levels with similarity score
- [ ] Permissions: Allow location, camera, sensors on first launch
- [ ] Database: Data persists after app restart

## Troubleshooting

### "adb devices" shows nothing
- Disconnect and reconnect USB cable
- Accept "Allow debugging" prompt on phone
- Install correct USB drivers (Samsung, Google, etc.)

### Build fails with "AndroidSDK not found"
- Open Visual Studio → Tools → Android → Android SDK Manager
- Ensure "Android 14.0 (API 34)" or higher is installed

### App crashes on launch
- View logs: `adb logcat -s ShowMeWhere`
- Check AndroidManifest.xml permissions
- Verify Firebase/sensor permissions on device

### Storage permissions denied
- App declares permissions in AndroidManifest
- User must grant at runtime on Android 6.0+
- Grant in Settings → Apps → ShowMeWhere → Permissions

## App Info

**Package Name:** `com.companyname.showmewhere.app`  
**Min SDK:** Android 21 (API 21)  
**Target SDK:** Android 14 (API 34)  
**Architectures:** ARM64, x86_64

## Release Build Signing

For production distribution (Google Play):
```powershell
# Create keystore (one-time)
keytool -genkey -v -keystore ~/showmewhere.keystore -keyalg RSA -keysize 2048 -validity 10000 -alias showmewhere

# Build signed release
dotnet publish src/ShowMeWhere.App/ShowMeWhere.App.csproj -f net10.0-android -c Release \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=true \
  -p:AndroidSigningKeyAlias=showmewhere \
  -p:AndroidSigningKeyPass="YOUR_PASSWORD" \
  -p:AndroidSigningStorePass="YOUR_PASSWORD"
```

## Next Steps

- [ ] Test on physical Android device
- [ ] Gather feedback on sensor accuracy
- [ ] Optimize battery usage (sensor polling intervals)
- [ ] Consider GitHub Actions for automated CI/CD builds
