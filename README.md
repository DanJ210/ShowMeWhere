# ShowMeWhere - Parking Level Detection System

A cross-platform MAUI + Vue hybrid application for detecting and remembering parking garage levels using sensor fingerprinting.

## 🎯 Overview

ShowMeWhere uses advanced sensor data (accelerometer, compass, barometer, magnetic field, Wi-Fi/cellular signals) to create unique fingerprints for each parking **LEVEL/FLOOR**. The app learns locations and can identify which floor you parked on with high confidence using cosine similarity matching.

**Key Differentiator:** Identifies parking **LEVEL** (P1, P2, P3, B1, B2), not individual spot numbers. This is environmental fingerprinting, not GPS-based.

**Key Features:**
- 📍 Detect parking level via sensor fingerprinting
- 💾 Save detected levels with one tap
- 🔄 Return to car: Shows which floor your car is on
- 🌐 Works without internet (offline-first)
- 📦 Cross-platform: Windows, macOS, iOS, Android

## 🏗️ Architecture

**3-Layer Design:**
- **Core**: Domain logic, sensor models, fingerprinting algorithm (platform-agnostic)
- **Infrastructure**: SQLite persistence, database migrations
- **App**: MAUI UI, Vue frontend (HybridWebView), platform-specific sensor implementations

**Technology Stack:**
- .NET 10 MAUI (cross-platform framework)
- Vue 3 + TypeScript (embedded frontend)
- Entity Framework Core (ORM)
- SQLite (local database)
- Sensor APIs: Accelerometer, Compass, Barometer, Magnetometer

## 🚀 Quick Start

### Prerequisites
- .NET 10 SDK
- Node.js 18+ (for Vue frontend)
- Android SDK (for Android builds)

### Build & Run

**Windows Desktop:**
```bash
dotnet build src/ShowMeWhere.App/ShowMeWhere.App.csproj -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

**Android:**
```bash
# Quick start with build script
.\build-android.ps1

# Or manual build
dotnet build src/ShowMeWhere.App/ShowMeWhere.App.csproj -f net10.0-android -t Install
```

**iOS (requires Mac):**
```bash
dotnet build src/ShowMeWhere.App/ShowMeWhere.App.csproj -f net10.0-ios
```

## 📱 Platform-Specific Guides

- **[Android Build & Deployment](./ANDROID_DEPLOYMENT.md)** - Build, test, and distribute APK
- **[Android Quick Start](./ANDROID_QUICK_START.md)** - One-liners and common commands
- **[CI/CD Setup](./CICD_SETUP.md)** - GitHub Actions for automated builds

## 🧪 Testing

**Run Unit Tests:**
```bash
dotnet test tests/ShowMeWhere.Core.Tests/ShowMeWhere.Core.Tests.csproj
```

**Expected Output:**
```
Test summary: total: 4, failed: 0, succeeded: 4
- LevelSimilarityService_Compare_ReturnsHighScore_WhenSignaturesMatch
- LevelSimilarityService_Compare_ReturnsLowerScore_WhenSignaturesDiffer
- LevelSimilarityService_Compare_ReturnsHighScore_WhenAccelerometerAndCompassMatch
- LevelSimilarityService_Compare_ReturnsLowerScore_WhenCompassDiffers
```

## 📊 Sensor Fingerprinting Algorithm

Each parking level creates a unique fingerprint based on:

| Sensor | Data | Resolution |
|--------|------|-----------|
| Accelerometer | X, Y, Z gravity (g's) | ±1g, normalized to [-1, 1] |
| Compass | Heading (0-360°) | Decomposed into sin/cos for circular correctness |
| Barometer | Atmospheric pressure (hPa) | ±3 decimals |
| Magnetometer | X, Y, Z field (µT) | ±1 decimal |
| Bluetooth | Noise floor (dBm) | Ambient BLE signal strength |
| Wi-Fi | SSID + RSSI (dBm) | Signal strength patterns |
| Cellular | Tower ID + RSSI (dBm) | Signal strength from towers |

**Matching:** Cosine similarity on normalized feature vector (~8-10 dimensions)

## 📂 Project Structure

```
ShowMeWhere/
├── src/
│   ├── ShowMeWhere.Core/              # Domain logic (platform-agnostic)
│   │   ├── Models/                    # SensorSnapshot, LevelSignature, etc.
│   │   ├── Services/                  # LevelDetectionService, LevelSimilarityService
│   │   └── Abstractions/              # IAccelerometerReader, IBarometerReader, etc.
│   ├── ShowMeWhere.Infrastructure/    # Data persistence
│   │   ├── Storage/                   # SQLite database, migrations
│   │   └── Repositories/              # LevelSignatureRepository, ParkingRecordRepository
│   ├── ShowMeWhere.App/               # MAUI app + Vue frontend
│   │   ├── MauiProgram.cs             # DI container, bootstrapping
│   │   ├── Platforms/                 # Platform-specific code
│   │   ├── Resources/
│   │   │   ├── AppIcon/               # App icons
│   │   │   ├── Splash/                # Splash screen
│   │   │   └── Raw/wwwroot/           # Vue frontend (embedded)
│   │   └── *Reader.cs                 # MauiAccelerometerReader, MauiCompassReader, etc.
│   └── showmewhere-frontend/          # Vue 3 app (TypeScript)
│       ├── src/
│       │   ├── App.vue                # Main UI (3 tabs)
│       │   ├── types.ts               # TypeScript interfaces (mirrors C# DTOs)
│       │   └── components/
│       └── package.json
├── tests/
│   └── ShowMeWhere.Core.Tests/        # Unit tests (xUnit)
├── ANDROID_DEPLOYMENT.md              # Android build & deployment guide
├── ANDROID_QUICK_START.md             # Quick commands reference
├── build-android.ps1                  # Build script for Android
└── README.md                          # This file
```

## 🔧 Development Workflow

### Adding a New Sensor

1. **Define Core Model** (Core layer):
   ```csharp
   // ShowMeWhere.Core/Models/YourSensorReading.cs
   public sealed record YourSensorReading(double Value);
   ```

2. **Create Abstraction** (Core layer):
   ```csharp
   // ShowMeWhere.Core/Abstractions/IYourSensorReader.cs
   public interface IYourSensorReader
   {
       Task<YourSensorReading?> GetReadingAsync(CancellationToken cancellationToken);
   }
   ```

3. **Implement MAUI Version** (App layer):
   ```csharp
   // ShowMeWhere.App/MauiYourSensorReader.cs
   public class MauiYourSensorReader : IYourSensorReader { }
   ```

4. **Integrate into Snapshot** (Core layer):
   - Update `SensorSnapshot` record
   - Update `SensorSnapshotService` to capture reading
   - Add to `LevelSignature` and `LevelSignatureFactory`

5. **Update Similarity** (Core layer):
   - Add normalization function to `LevelSimilarityService`
   - Include in comparison vector

6. **Update Frontend** (Vue layer):
   - Add to `types.ts` interfaces
   - Display in `App.vue` sensor grid

## 🐛 Known Issues & Limitations

| Issue | Status | Explanation |
|-------|--------|-------------|
| iOS Wi-Fi/SSID access | ❌ Apple-restricted | Privacy policy: Apple blocks Wi-Fi SSID scanning to prevent location tracking. App uses accel+compass+barometer instead (~80-85% accuracy) |
| iOS Cellular tower access | ❌ Apple-restricted | Privacy policy: Apple blocks tower triangulation to prevent covert location tracking. Fallback to accelerometer+compass |
| iOS Bluetooth scanning | ⚠️ Limited | Apple restricts BLE RSSI scanning. Would require special entitlements (rarely approved) |
| Android SQLite (2.1.x) | ⚠️ Warning | Non-blocking vulnerability on Android variant; Windows/iOS use 3.50.3 |
| Database migration | ✅ Fixed | Uses `CreateTableAsync(CreateFlags.None)` for non-destructive evolution |

## 📈 Performance

| Metric | Value |
|--------|-------|
| Debug Build Time | ~6s (Windows) |
| Release APK Build | ~5-10m (first build); ~1-2m (incremental) |
| Startup Time | ~2-3s |
| Sensor Capture Window | 500ms (configurable) |
| Similarity Calculation | <1ms |

## 🚢 Deployment

### Android (Immediate)
```bash
.\build-android.ps1 -Configuration Release -Package
# Output: APK file (~30 MB) ready for distribution
```

### iOS (No Physical Mac Required)
- Use the GitHub Actions workflow for macOS cloud builds and TestFlight upload
- See [CI/CD Setup](./CICD_SETUP.md) for secrets and trigger instructions

### Windows Desktop
- Build executable for distribution
- Or deploy via Microsoft Store (WinAppSDK wrapper)

## 🤝 Contributing

1. Clone: `git clone https://github.com/yourusername/ShowMeWhere.git`
2. Create branch: `git checkout -b feature/my-feature`
3. Make changes in isolated layers (Core → Infrastructure → App)
4. Run tests: `dotnet test`
5. Commit: `git commit -am "feat: add my feature"`
6. Push: `git push origin feature/my-feature`

## 📝 License

MIT

## 🙌 Acknowledgments

- MAUI team for cross-platform framework
- Vue team for reactive UI framework
- Entity Framework team for ORM
- Community contributions

---

**Questions?** Open an issue or check the platform-specific guides linked above.
