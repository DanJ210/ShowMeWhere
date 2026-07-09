# 📱 Android Build Quick Reference

## One-Liner Commands

### Debug Build (Local Testing)
```powershell
dotnet build src/ShowMeWhere.App/ShowMeWhere.App.csproj -f net10.0-android
```

### Release APK (Distribution)
```powershell
dotnet publish src/ShowMeWhere.App/ShowMeWhere.App.csproj -f net10.0-android -c Release -p:AndroidPackageFormat=apk
```

### Install Directly to Phone
```powershell
dotnet build src/ShowMeWhere.App/ShowMeWhere.App.csproj -f net10.0-android -t Install
```

### Using Build Script
```powershell
# Interactive build script
.\build-android.ps1

# Build & package for distribution
.\build-android.ps1 -Configuration Release -Package

# Build, package, AND install to device
.\build-android.ps1 -Install

# Full clean rebuild
.\build-android.ps1 -Clean -Configuration Release -Package
```

## Device Setup (First Time Only)

### Enable Developer Mode
1. Go to **Settings** → **About Phone**
2. Tap **Build Number** 7 times rapidly
3. Navigate to **Settings** → **Developer Options**
4. Toggle **USB Debugging** ON
5. Tap **File Transfer Mode** (or MTP)

### Verify Connection
```bash
adb devices
# Should show your device as: ABC123XYZ456  device
```

## Output Locations

| Build Type | Location |
|-----------|----------|
| Debug Build | `src/ShowMeWhere.App/bin/Debug/net10.0-android/` |
| Release APK | `src/ShowMeWhere.App/bin/Release/net10.0-android/publish/` |
| App Name | `ShowMeWhere.App.apk` (~80-120 MB) |
| Package ID | `com.companyname.showmewhere.app` |

## Testing Workflow

```powershell
# 1. Build debug version
.\build-android.ps1

# 2. Install to device
.\build-android.ps1 -Install

# 3. Launch app on phone manually (tap icon)

# 4. View logs in real-time
adb logcat -s ShowMeWhere

# 5. Repeat: make changes, rebuild, reinstall
```

## Common Issues & Fixes

| Issue | Solution |
|-------|----------|
| "adb not found" | Install Android SDK or add to PATH |
| "Device not found" | Check USB cable, accept debug prompt on phone |
| "Build timeout" | Increase timeout: `-p:DebuggerDisplayNameSymbols=false` |
| "Permission denied" | Grant in Settings → Apps → ShowMeWhere → Permissions |
| "App crashes" | Check logcat: `adb logcat -s ShowMeWhere` |

## Distribution Checklist

- [ ] Test on physical Android device (min API 21)
- [ ] Verify all permissions granted
- [ ] Test offline functionality (database works)
- [ ] Profile battery usage
- [ ] Build Release APK: `.\build-android.ps1 -Configuration Release -Package`
- [ ] Sign release APK for Play Store (see ANDROID_DEPLOYMENT.md)
- [ ] Share unsigned APK with testers or upload to Play Store

## Next: GitHub Actions CI/CD

See `CICD_SETUP.md` for automated cloud builds on every commit.
