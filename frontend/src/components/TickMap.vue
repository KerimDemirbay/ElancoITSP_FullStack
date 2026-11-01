<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import markerIcon from 'leaflet/dist/images/marker-icon.png'
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png'
import markerShadow from 'leaflet/dist/images/marker-shadow.png'
import type { CityReport } from '@/api'

const props = defineProps<{
    cities: CityReport[]
    selectedCity: string | null
}>()

const emit = defineEmits<{
    selectCity: [city: string]
}>()

// circle sizes in pixels. every city gets at least the minimum (easy to tap on a phone)
// and the busiest gets the maximum, so circles never grow big enough to spill over each other
const MIN_RADIUS = 14
const MAX_RADIUS = 24

// colour scale: yellow for the fewest sightings, dark red for the most
const YELLOW = { r: 250, g: 204, b: 21 }
const DARK_RED = { r: 153, g: 27, b: 27 }

// Leaflet's default pin images don't load properly
// so we import the image files and build the icon ourselves
// probably because of vite
const pinIcon = L.icon({
    iconUrl: markerIcon,
    iconRetinaUrl: markerIcon2x,
    shadowUrl: markerShadow,
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    tooltipAnchor: [12, -28],
    shadowSize: [41, 41],
})

const container = ref<HTMLDivElement | null>(null)

// Leaflet objects are kept in plain variables, not refs
// wrapping them in Vue's reactivity (being watched) can break Leaflet
let map: L.Map | null = null
let markerLayer: L.LayerGroup | null = null

const visibleCities = computed(() => props.cities.filter((c) => c.count > 0))

// colour and size scale runs from the quietest to the busiest visible city
const counts = computed(() => visibleCities.value.map((c) => c.count))
const minCount = computed(() => (counts.value.length > 0 ? Math.min(...counts.value) : 0))
const maxCount = computed(() => (counts.value.length > 0 ? Math.max(...counts.value) : 0))

// 0 = quietest city and 1 = busiest
function intensityFor(count: number): number {
    if (maxCount.value === minCount.value) return 1
    return (count - minCount.value) / (maxCount.value - minCount.value)
}

function colourFor(count: number): string {

    const t = intensityFor(count)
    const mix = (from: number, to: number) => Math.round(from + (to - from) * t)

    return `rgb(${mix(YELLOW.r, DARK_RED.r)}, ${mix(YELLOW.g, DARK_RED.g)}, ${mix(YELLOW.b, DARK_RED.b)})`
}

function radiusFor(count: number): number {
    return MIN_RADIUS + (MAX_RADIUS - MIN_RADIUS) * intensityFor(count)
}

const legendGradient = `linear-gradient(to right, rgb(${YELLOW.r}, ${YELLOW.g}, ${YELLOW.b}), rgb(${DARK_RED.r}, ${DARK_RED.g}, ${DARK_RED.b}))`

onMounted(() => {
    if (!container.value) return

    map = L.map(container.value).setView([54.4, -3.2], 6)

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        maxZoom: 19,
    }).addTo(map)

    markerLayer = L.layerGroup().addTo(map)

    drawMarkers()
})

onBeforeUnmount(() => {
    map?.remove()
    map = null
})

// redraw whenever the data or the selection changes
watch(() => [props.cities, props.selectedCity], drawMarkers)

// zoom in selected city
watch(
    () => props.selectedCity,
    (name) => {
        const city = props.cities.find((c) => c.city === name)
        if (city && map) {
            map.setView([city.latitude, city.longitude], Math.max(map.getZoom(), 8))
        }
    },
)

function drawMarkers() {
    if (!map || !markerLayer) return

    markerLayer.clearLayers()

    for (const city of visibleCities.value) {
        const isSelected = city.city === props.selectedCity
        const colour = colourFor(city.count)
        const select = () => emit('selectCity', city.city)

        L.circleMarker([city.latitude, city.longitude], {
            radius: radiusFor(city.count),
            fillColor: colour,
            fillOpacity: 0.7,
            color: isSelected ? '#1b2e24' : colour,
            weight: isSelected ? 3 : 1.5,
        })
            .on('click', select)
            .addTo(markerLayer)

        L.marker([city.latitude, city.longitude], { icon: pinIcon, alt: city.city })
            .bindTooltip(`${city.city}: ${city.count.toLocaleString('en-GB')} sightings`)
            .on('click', select)
            .addTo(markerLayer)
    }
}
</script>

<template>
    <div class="map-wrap">
        <div ref="container" class="map"></div>

        <div v-if="visibleCities.length > 0" class="legend">
            <p class="legend__title">Tick sightings</p>
            <div class="legend__gradient" :style="{ background: legendGradient }"></div>
            <div class="legend__labels">
                <span>{{ minCount }}</span>
                <span>{{ maxCount }}</span>
            </div>
        </div>
    </div>
</template>

<style scoped>
    .map-wrap {
        position: absolute;
        inset: 0;
    }

    .map {
        height: 100%;
        width: 100%;
        background: var(--lichen);
    }
    
    .map :deep(.leaflet-tile-pane) {
        filter: grayscale(1);
    }

    .legend {
        position: absolute;
        left: var(--space-3);
        bottom: var(--space-4);
        z-index: 1000;
        width: 12rem;
        padding: var(--space-2) var(--space-3);
        background: var(--paper);
        border: 1px solid var(--lichen);
        border-radius: 0.5rem;
        font-size: var(--text-small);
    }

    .legend__title {
        margin: 0 0 var(--space-1);
        font-weight: 700;
    }

    .legend__gradient {
        height: 0.75rem;
        border-radius: 0.25rem;
    }

    .legend__labels {
        display: flex;
        justify-content: space-between;
        color: var(--moss-soft);
    }

</style>