import type { AppBootstrap, DetectionResult, ParkingRecord, SensorSnapshot } from './types'

declare global {
  interface Window {
    HybridWebView?: {
      InvokeDotNet<T>(methodName: string, paramValues?: unknown[]): Promise<T>
      SendRawMessage?(message: string): void
    }
  }
}

const mockBootstrap: AppBootstrap = {
  Capabilities: [
    { Name: 'Wi-Fi scan', IsAvailable: false, Detail: 'Restricted on iOS without private entitlements.' },
    { Name: 'Cell tower info', IsAvailable: false, Detail: 'Not exposed through .NET MAUI on iOS.' },
    { Name: 'Barometer', IsAvailable: true, Detail: 'Available on supported devices.' },
    { Name: 'Bluetooth noise floor', IsAvailable: false, Detail: 'Scanner implementation pending.' },
    { Name: 'Magnetic field', IsAvailable: true, Detail: 'Magnetometer available.' },
    { Name: 'Compass', IsAvailable: true, Detail: 'Compass available.' },
  ],
  CurrentParkingRecord: null,
  Timestamp: new Date().toISOString(),
}

function hasHybridBridge() {
  return typeof window !== 'undefined' && typeof window.HybridWebView?.InvokeDotNet === 'function'
}

async function invoke<T>(methodName: string, paramValues?: unknown[]): Promise<T> {
  if (hasHybridBridge()) {
    return await window.HybridWebView!.InvokeDotNet<T>(methodName, paramValues)
  }

  return mockInvoke<T>(methodName, paramValues)
}

async function mockInvoke<T>(methodName: string, paramValues?: unknown[]): Promise<T> {
  switch (methodName) {
    case 'GetAppBootstrapAsync':
      return mockBootstrap as T
    case 'CaptureSensorSnapshotAsync':
      return {
        CapturedAt: new Date().toISOString(),
        WifiNetworks: [],
        CellTowers: [],
        Pressure: 1008.2,
        BluetoothNoiseFloor: null,
        MagneticField: { X: 12.4, Y: -6.1, Z: 42.9 },
        CompassHeading: 184.2,
      } as T
    case 'DetectLevelAsync':
      return {
        Snapshot: await mockInvoke<SensorSnapshot>('CaptureSensorSnapshotAsync'),
        Signature: {
          Id: 'mock-signature',
          Wifi: [],
          Cell: [],
          Pressure: 1008.2,
          BtNoise: null,
          MagX: 12.4,
          MagY: -6.1,
          MagZ: 42.9,
          CreatedAt: new Date().toISOString(),
        },
        PredictedLevelName: 'P3',
        SimilarityScore: 0.81,
        MatchedSignatureId: 'mock-signature',
        IsKnownLevel: true,
      } as T
    case 'SaveLevelAsync':
      return {
        Id: crypto.randomUUID(),
        LevelName: String(paramValues?.[0] ?? 'P3'),
        SignatureId: 'mock-signature',
        Timestamp: new Date().toISOString(),
      } as T
    case 'GetCurrentParkingRecordAsync':
      return null as T
    case 'SyncAsync':
      return undefined as T
    default:
      throw new Error(`No mock bridge implementation for ${methodName}`)
  }
}

export const bridge = {
  captureSensorSnapshot: () => invoke<SensorSnapshot>('CaptureSensorSnapshotAsync'),
  detectLevel: () => invoke<DetectionResult>('DetectLevelAsync'),
  getAppBootstrap: () => invoke<AppBootstrap>('GetAppBootstrapAsync'),
  getCurrentParkingRecord: () => invoke<ParkingRecord | null>('GetCurrentParkingRecordAsync'),
  saveLevel: (levelName: string) => invoke<ParkingRecord>('SaveLevelAsync', [levelName]),
  sync: () => invoke<void>('SyncAsync'),
}
