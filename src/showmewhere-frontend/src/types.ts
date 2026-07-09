export interface SensorModuleAvailability {
  Name: string
  IsAvailable: boolean
  Detail: string
}

export interface WifiNetworkReading {
  Ssid: string
  Rssi: number
}

export interface CellTowerReading {
  TowerId: string
  Rssi: number
}

export interface MagneticVector {
  X: number
  Y: number
  Z: number
}

export interface AccelerometerVector {
  X: number
  Y: number
  Z: number
}

export interface SensorSnapshot {
  CapturedAt: string
  WifiNetworks: WifiNetworkReading[]
  CellTowers: CellTowerReading[]
  Pressure: number | null
  BluetoothNoiseFloor: number | null
  MagneticField: MagneticVector | null
  CompassHeading: number | null
  AccelerometerGravity: AccelerometerVector | null
}

export interface LevelSignature {
  Id: string
  Wifi: WifiNetworkReading[]
  Cell: CellTowerReading[]
  Pressure: number | null
  BtNoise: number | null
  MagX: number | null
  MagY: number | null
  MagZ: number | null
  Compass: number | null
  AccX: number | null
  AccY: number | null
  AccZ: number | null
  CreatedAt: string
}

export interface DetectionResult {
  Snapshot: SensorSnapshot
  Signature: LevelSignature
  PredictedLevelName: string | null
  SimilarityScore: number
  MatchedSignatureId: string | null
  IsKnownLevel: boolean
}

export interface ParkingRecord {
  Id: string
  LevelName: string
  SignatureId: string
  Timestamp: string
}

export interface AppBootstrap {
  Capabilities: SensorModuleAvailability[]
  CurrentParkingRecord: ParkingRecord | null
  Timestamp: string
}
