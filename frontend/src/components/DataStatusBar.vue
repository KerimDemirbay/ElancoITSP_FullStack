<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { getDataStatus, type DataStatus } from '@/api'

const status = ref<DataStatus | null>(null)
const unreachable = ref(false)

onMounted(async () => {
  try {
    status.value = await getDataStatus()
  } catch {
    unreachable.value = true
  }
})

const message = computed(() => {
  if (unreachable.value) return "Can't reach the sightings service. Check that the API is running."
  if (!status.value) return 'Checking sighting data…'
  if (!status.value.isReady) return 'Sighting data is still loading. Try again in a minute.'

  const count = status.value.sightingCount.toLocaleString('en-GB')
  const synced = status.value.lastRefreshedAt
    ? `, last synced at ${formatTime(status.value.lastRefreshedAt)}`
    : ''

  return `${count} sightings${synced}.`
})

function formatTime(iso: string): string {
  return new Date(iso).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })
}
</script>

<template>
    <div role="status">
        <p class="status">{{ message }}</p>
        <p v-if="status?.lastFailure" class="status status--warning">
            The latest sync with Elanco failed, so you're seeing the last saved data.
        </p>
    </div>
</template>

<style scoped>
    .status {
        margin: 0;
        font-size: var(--text-small);
        color: var(--moss-soft);
    }

    .status--warning {
        color: var(--gorse);
    }
</style>