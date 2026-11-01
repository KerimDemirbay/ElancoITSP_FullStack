<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { ApiError, getCityReports, getSpecies, type CityReport, type SpeciesInfo } from '@/api'
import TickMap from '@/components/TickMap.vue'
import MapFilters from '@/components/MapFilters.vue'
import PostcodeSearch from '@/components/PostcodeSearch.vue'
import CitySidebar from '@/components/CitySidebar.vue'

//the earliest date the filters accept (the same rule the API uses for new reports)
//first 10 chars of toISOString() gives UTC date, ex: "2026-10-31T19:55:00.000Z"
const MIN_DATE = '2000-01-01'
const TODAY = new Date().toISOString().slice(0, 10)

const from = ref('')
const to = ref('')
const species = ref('')

const cities = ref<CityReport[]>([])
const speciesOptions = ref<SpeciesInfo[]>([])
const selectedCityName = ref<string | null>(null)

const isLoading = ref(false)
const error = ref('')
const notice = ref('')

const selectedCity = computed(() => cities.value.find((c) => c.city === selectedCityName.value) ?? null)
const cityNames = computed(() => cities.value.map((c) => c.city).sort())
const noSightings = computed(() => cities.value.length > 0 && cities.value.every((c) => c.count === 0))

function isOutOfRange(date: string): boolean {
    if (date === '') return false
    return date.length !== 10 || date < MIN_DATE || date > TODAY
}

//returns '' when there is no date error
const dateError = computed(() => {
    if (isOutOfRange(from.value) || isOutOfRange(to.value)) {
        return 'Dates must be between 1 January 2000 and today.'
    }
    if (from.value !== '' && to.value !== '' && from.value > to.value) {
        return 'The "from" date must be on or before the "to" date.'
    }
    return ''
})

//only the newest request is allowed to update the page
let latestRequest = 0

async function loadCities() {
    if (dateError.value !== '') return

    const requestId = ++latestRequest
    isLoading.value = true
    error.value = ''

    try {
        const result = await getCityReports({ from: from.value, to: to.value, species: species.value })
        if (requestId === latestRequest) cities.value = result
    } catch (e) {
        if (requestId !== latestRequest) return

        if (e instanceof ApiError && e.status === 503) {
            error.value = 'Sighting data is still loading. Try again in a minute.'
        } else if (e instanceof ApiError && e.status === 400) {
            error.value = "The server didn't accept those filters. Check them and try again."
        } else {
            error.value = "Couldn't load sightings. Check that the API is running, then try again."
        }
    } finally {
        if (requestId === latestRequest) isLoading.value = false
    }
}

async function loadSpecies() {
    try {
        speciesOptions.value = await getSpecies()
    } catch {
        // The map still works without the species list; the dropdown just shows "All species".
    }
}

onMounted(async () => {
    loadSpecies()
    await loadCities()

    // A shared link like /?city=Leeds opens that city straight away.
    const sharedCity = route.query.city
    if (typeof sharedCity === 'string') selectCity(sharedCity)
})

watch([from, to, species], loadCities)

function selectCity(name: string | null, message = '') {
    selectedCityName.value = name
    notice.value = message
}

function onPostcodeFound(city: string, postcode: string) {
    selectCity(city, `${postcode} is in ${city}.`)
}
</script>

<template>
    <div class="map-page">
        <h1 class="visually-hidden">Tick sightings across the UK</h1>

        <MapFilters v-model:from="from"
                    v-model:to="to"
                    v-model:species="species"
                    :species-options="speciesOptions"
                    :min-date="MIN_DATE"
                    :max-date="TODAY"
                    :date-error="dateError" />

        <div class="map-area">
            <TickMap :cities="cities" :selected-city="selectedCityName" @select-city="selectCity" />

            <PostcodeSearch :city-names="cityNames" @found="onPostcodeFound" />

            <p v-if="isLoading" class="map-overlay">Loading sightings…</p>

            <div v-else-if="error" class="map-overlay map-overlay--error">
                <p>{{ error }}</p>
                <button type="button" class="button" @click="loadCities">Try again</button>
            </div>

            <p v-else-if="noSightings" class="map-overlay">No sightings match these filters.</p>

            <CitySidebar :city="selectedCity" :notice="notice" @close="selectCity(null)" />
        </div>
    </div>
</template>

<style scoped>
    .map-page {
        flex: 1;
        display: flex;
        flex-direction: column;
    }

    .map-area {
        position: relative;
        flex: 1;
        min-height: 30rem;
        overflow: hidden;
    }

    .map-overlay {
        position: absolute;
        left: 50%;
        bottom: var(--space-4);
        transform: translateX(-50%);
        z-index: 1000;
        max-width: calc(100% - 2 * var(--space-3));
        margin: 0;
        padding: var(--space-2) var(--space-3);
        background: var(--paper);
        border: 1px solid var(--lichen);
        border-radius: 0.5rem;
        text-align: center;
    }

    .map-overlay--error {
        border-color: var(--bramble);
        color: var(--bramble);
    }

        .map-overlay--error p {
            margin: 0 0 var(--space-2);
        }
</style>