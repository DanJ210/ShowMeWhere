# 🚗 Return to Car - Quick Reference Card\n\n**Primary Goal:** Identify which **parking LEVEL/FLOOR** you're on (not exact spot)\n\n```\nExample outcomes:\n✅ \"You're on Level P3\"\n❌ \"You're on spot P3-47\" (not this)\n```

## The Three Tabs

```
┌─────────────────────────────────────────────────────────┐
│ SCREEN 1: DETECT LEVEL (Find your car)                 │
├─────────────────────────────────────────────────────────┤
│ Button: "Detect Level"                                  │
│ Time: ~2 seconds to run                                 │
│ Output:                                                 │
│   ✓ Prediction: Level name (e.g., "P3")               │
│   ✓ Similarity: Confidence 0-100%                      │
│   ✓ Known Match: Yes/No (>72% threshold)               │
│                                                         │
│ USE CASE: You're in the garage, need to find your car  │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ SCREEN 2: SAVE LEVEL (Teach the app)                   │
├─────────────────────────────────────────────────────────┤
│ Input: Text field for level name ("P3", "B2", etc.)    │
│ Button: "Save Level"                                    │
│ Time: ~2 seconds to capture fingerprint                │
│ Output: App stores sensor data + label                  │
│                                                         │
│ USE CASE: Just parked, want to teach app this location │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ SCREEN 3: RETURN TO CAR (Reference)                    │
├─────────────────────────────────────────────────────────┤
│ Shows: Your saved destination (e.g., "Head back to P3")│
│ Shows: Which sensors are available                      │
│        ✓ Accelerometer  ✓ Compass                       │
│        ✗ Wi-Fi limited  ✗ Cellular limited (iOS)      │
│                                                         │
│ USE CASE: Quick reminder of where your car is          │
└─────────────────────────────────────────────────────────┘
```

## Workflow

```
WHEN YOU PARK:
  1. Go to Screen 2
  2. Enter level name: "P3"
  3. Tap "Save Level"
  ✅ Done! Fingerprint stored

WHEN YOU WANT YOUR CAR:
  1. Go to Screen 1
  2. Tap "Detect Level"
  3. Wait 2 seconds
  4. See prediction: "P3" at 89.3% confidence
  ✅ Go to that level!
```

## Similarity Score Reference

| Score | Meaning |
|-------|---------|
| 95%+ | Very confident ✅ Trust it |
| 80-95% | Confident ✅ Trust it |
| 72-80% | Decent (min threshold) ✅ Trust it |
| 60-72% | Weak ⚠️ Don't trust |
| <60% | No match ❌ Completely unknown |

**Key:** App requires 72% minimum to say "Known Match: Yes"

## Sensor Data Captured (per level)

When you save a level, the app stores:
- **Accelerometer**: X, Y, Z gravity (how tilted the floor is)
- **Compass**: Direction (0-360°)
- **Pressure**: Air density (different floors have different pressures)
- **Magnetometer**: X, Y, Z magnetic field (metal structures create unique patterns)
- **Bluetooth**: Ambient BLE signal strength
- **Wi-Fi**: Nearby networks + signal strength
- **Cellular**: Tower ID + signal strength

**All data stored locally on phone (offline)**

## Accuracy Expectations

### With Full Sensors (Android, Mac):
- Same level: **95%+** accuracy
- Different levels: **85-90%** accuracy

### Limited Sensors (iOS, accel/compass/barometer only):
- Same level: **80-85%** accuracy
- Different levels: **75-80%** accuracy

## Tips for Better Accuracy

✅ DO:
- Stand still during capture (2 sec)
- Save in representative location
- Save multiple times same level
- Use clear, consistent names ("P3", not "garage")
- Try detecting 2-3 times (sensors vary slightly)

❌ DON'T:
- Save right next to metal pillar (skews magnetic field)
- Save only once (build database with multiple visits)
- Rely 100% on borderline scores (70-75%)
- Expect it to work deep underground (no signals)
- Use it outside (indoor localization only)

## When It Fails

| Problem | Why | Fix |
|---------|-----|-----|
| Low scores (50-60%) | No saved data yet | Save the level first! |
| Same score for 2 levels | Levels are too similar | Save in different spot on level |
| Decreasing accuracy over time | Building changed | Delete old fingerprint, re-save |
| All sensors say "limited" | iOS device | Accept limited accuracy, use compass + accel |
| No detection result | Sensors blocked | Move to open area, try again |

## Database

**Stored on phone:**
- Level names & fingerprints (one per saved location)
- Timestamps (when you saved each)
- Sensor readings (compressed for efficiency)
- Parking records (links levels to save times)

**Not synced to cloud** (local only for privacy)

## Commands (Debug)

```powershell
# See logs while using app (Android)
adb logcat -s ShowMeWhere

# Uninstall app (clears all saved data)
adb uninstall com.companyname.showmewhere.app
```

## One-Liner Scenarios

| Scenario | Steps |
|----------|-------|
| First time: Save my spot | Screen 2 → type "P3" → tap Save Level |
| Find my car | Screen 1 → tap Detect Level → read prediction |
| What sensors work? | Screen 3 → look at capability list |
| Check my car location | Screen 3 → shows your saved level |
| Capture without saving | "Capture Snapshot" button → see sensor data |

---

**TL;DR:**
- **Save Level** = Teach app your parking spot
- **Detect Level** = Ask app "where am I?" 
- **Return to Car** = Shows where your car was parked

That's it! 🚗
