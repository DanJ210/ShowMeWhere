<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { bridge } from './bridge'
import type { AppBootstrap, DetectionResult, ParkingRecord, SensorSnapshot } from './types'

const bootstrap = ref<AppBootstrap | null>(null)
const currentRecord = ref<ParkingRecord | null>(null)
const lastDetection = ref<DetectionResult | null>(null)
const lastSnapshot = ref<SensorSnapshot | null>(null)
const levelName = ref('P3')
const isBusy = ref(false)
const status = ref('Hybrid shell ready. Capture a live signature to predict the current level.')

const unsupportedCapabilities = computed(() =>
  (bootstrap.value?.Capabilities ?? []).filter((item) => !item.IsAvailable),
)

const supportedCapabilities = computed(() =>
  (bootstrap.value?.Capabilities ?? []).filter((item) => item.IsAvailable),
)

async function refreshBootstrap() {
  bootstrap.value = await bridge.getAppBootstrap()
  currentRecord.value = bootstrap.value.CurrentParkingRecord
}

async function captureSnapshot() {
  isBusy.value = true
  status.value = 'Sampling sensors for 2 seconds...'
  try {
    lastSnapshot.value = await bridge.captureSensorSnapshot()
    status.value = 'Snapshot captured. You can inspect sensor values below.'
  } finally {
    isBusy.value = false
  }
}

async function detectLevel() {
  isBusy.value = true
  status.value = 'Comparing live signature to saved garage levels...'
  try {
    lastDetection.value = await bridge.detectLevel()
    lastSnapshot.value = lastDetection.value.Snapshot
    status.value = lastDetection.value.IsKnownLevel
      ? `Predicted ${lastDetection.value.PredictedLevelName} at ${(lastDetection.value.SimilarityScore * 100).toFixed(1)}% confidence.`
      : 'No confident match yet. Save this level to teach the model.'
  } finally {
    isBusy.value = false
  }
}

async function saveLevel() {
  if (!levelName.value.trim()) {
    status.value = 'Enter a level label before saving.'
    return
  }

  isBusy.value = true
  status.value = `Saving ${levelName.value}...`
  try {
    currentRecord.value = await bridge.saveLevel(levelName.value)
    await refreshBootstrap()
    status.value = `Saved ${currentRecord.value.LevelName} for return guidance.`
  } finally {
    isBusy.value = false
  }
}

async function syncStub() {
  isBusy.value = true
  status.value = 'Running cloud sync stub...'
  try {
    await bridge.sync()
    status.value = 'Cloud sync stub completed locally.'
  } finally {
    isBusy.value = false
  }
}

onMounted(async () => {
  await refreshBootstrap()
})
</script>

<template>
  <main class="app-shell">
    <section class="hero-card">
      <p class="eyebrow">Parking Garage Level Marker</p>
      <h1>Ambient fingerprinting for the floor where you parked.</h1>
      <p class="hero-copy">
        Vue runs inside a .NET MAUI HybridWebView while native sensor services and SQLite stay local on device.
      </p>
      <div class="hero-actions">
        <button class="primary" :disabled="isBusy" @click="detectLevel">Detect Level</button>
        <button class="secondary" :disabled="isBusy" @click="captureSnapshot">Capture Snapshot</button>
        <button class="secondary" :disabled="isBusy" @click="syncStub">Cloud Sync Stub</button>
      </div>
      <p class="status">{{ status }}</p>
    </section>

    <section class="grid">
      <article class="panel">
        <div class="panel-header">
          <p class="eyebrow">Screen 1</p>
          <h2>Detect Level</h2>
        </div>
        <div v-if="lastDetection" class="metric-stack">
          <div class="metric-card accent-blue">
            <span>Prediction</span>
            <strong>{{ lastDetection.PredictedLevelName ?? 'Unknown' }}</strong>
          </div>
          <div class="metric-card accent-sand">
            <span>Similarity</span>
            <strong>{{ (lastDetection.SimilarityScore * 100).toFixed(1) }}%</strong>
          </div>
          <div class="metric-card accent-green">
            <span>Known Match</span>
            <strong>{{ lastDetection.IsKnownLevel ? 'Yes' : 'No' }}</strong>
          </div>
        </div>
        <p v-else class="placeholder">No detection run yet.</p>
      </article>

      <article class="panel">
        <div class="panel-header">
          <p class="eyebrow">Screen 2</p>
          <h2>Saved Level Card</h2>
        </div>
        <label class="field">
          <span>Level label</span>
          <input v-model="levelName" maxlength="24" placeholder="P3 / B2 / East Ramp" />
        </label>
        <button class="primary wide" :disabled="isBusy" @click="saveLevel">Save Level</button>
        <div v-if="currentRecord" class="saved-card">
          <span>Current saved destination</span>
          <strong>{{ currentRecord.LevelName }}</strong>
          <small>{{ new Date(currentRecord.Timestamp).toLocaleString() }}</small>
        </div>
      </article>

      <article class="panel">
        <div class="panel-header">
          <p class="eyebrow">Screen 3</p>
          <h2>Return to Car</h2>
        </div>
        <div class="return-stack">
          <p>
            {{ currentRecord ? `Head back to ${currentRecord.LevelName}.` : 'No saved spot yet.' }}
          </p>
          <div class="tag-row">
            <span v-for="capability in supportedCapabilities" :key="capability.Name" class="tag tag-ok">
              {{ capability.Name }}
            </span>
            <span v-for="capability in unsupportedCapabilities" :key="capability.Name" class="tag tag-warn">
              {{ capability.Name }} limited
            </span>
          </div>
        </div>
      </article>
    </section>

    <section class="grid lower-grid">
      <article class="panel">
        <div class="panel-header">
          <p class="eyebrow">Snapshot</p>
          <h2>Sensor Readings</h2>
        </div>
        <div v-if="lastSnapshot" class="sensor-grid">
          <div>
            <span>Pressure</span>
            <strong>{{ lastSnapshot.Pressure ?? 'n/a' }}</strong>
          </div>
          <div>
            <span>Bluetooth noise</span>
            <strong>{{ lastSnapshot.BluetoothNoiseFloor ?? 'n/a' }}</strong>
          </div>
          <div>
            <span>Compass</span>
            <strong>{{ lastSnapshot.CompassHeading ?? 'n/a' }}</strong>
          </div>
          <div>
            <span>Wi-Fi fingerprints</span>
            <strong>{{ lastSnapshot.WifiNetworks.length }}</strong>
          </div>
          <div>
            <span>Cell fingerprints</span>
            <strong>{{ lastSnapshot.CellTowers.length }}</strong>
          </div>
          <div>
            <span>Magnetometer</span>
            <strong>
              {{ lastSnapshot.MagneticField ? `${lastSnapshot.MagneticField.X.toFixed(1)}, ${lastSnapshot.MagneticField.Y.toFixed(1)}, ${lastSnapshot.MagneticField.Z.toFixed(1)}` : 'n/a' }}
            </strong>
          </div>
        </div>
        <p v-else class="placeholder">Capture a snapshot to inspect raw sensor inputs.</p>
      </article>

      <article class="panel">
        <div class="panel-header">
          <p class="eyebrow">Platform Notes</p>
          <h2>iOS Constraints</h2>
        </div>
        <ul class="notes-list">
          <li v-for="capability in unsupportedCapabilities" :key="capability.Name">
            <strong>{{ capability.Name }}:</strong> {{ capability.Detail }}
          </li>
        </ul>
      </article>
    </section>
  </main>
</template>
