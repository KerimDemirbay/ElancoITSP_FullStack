<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import {
    ApiError,
    getCities,
    getSpecies,
    submitSighting,
    type CityInfo,
    type ProblemDetails,
    type SightingSubmissionResponse,
    type SpeciesInfo,
} from '@/api'

//same date rules as the API, from 1 January 2000 up to today
const MIN_DATE = '2000-01-01'
const TODAY = new Date().toISOString().slice(0, 10)
const NOTES_LIMIT = 500

type Field = 'date' | 'city' | 'species' | 'notes'

const cities = ref<CityInfo[]>([])
const speciesOptions = ref<SpeciesInfo[]>([])
const loadError = ref('')

const form = ref({ date: '', city: '', species: '', notes: '' })
const errors = ref<Partial<Record<Field, string>>>({})
const generalError = ref('')
const result = ref<SightingSubmissionResponse | null>(null)
const isSubmitting = ref(false)

const savedDate = computed(() => {
    if (!result.value) return ''
    return new Date(result.value.sighting.date).toLocaleDateString('en-GB', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
    })
})

onMounted(async () => {
    try {
        //wait until both have lists arrived before starting
        const [cityList, speciesList] = await Promise.all([getCities(), getSpecies()])
        cities.value = [...cityList].sort((a, b) => a.name.localeCompare(b.name))
        speciesOptions.value = speciesList
    } catch {
        loadError.value = "Couldn't load the lists of cities and species. Check that the API is running, then refresh the page."
    }
})

//check the form in the browser first 
function validate(): boolean {
    const found: Partial<Record<Field, string>> = {}
    const { date, city, species, notes } = form.value

    if (date === '') {
        found.date = 'Enter the date you saw the tick.'
    } else if (date.length !== 10 || date < MIN_DATE || date > TODAY) {
        found.date = 'The date must be between 1 January 2000 and today.'
    }

    if (city === '') found.city = 'Choose the city where you saw the tick.'
    if (species === '') found.species = 'Choose the species.'
    if (notes.length > NOTES_LIMIT) found.notes = `Notes can be up to ${NOTES_LIMIT} characters.`

    errors.value = found
    return Object.keys(found).length === 0
}

// Show each error under its own field and anything else at the top of the form.
function showServerErrors(problem: ProblemDetails | null) {
    const found: Partial<Record<Field, string>> = {}
    const other: string[] = []

    for (const [key, messages] of Object.entries(problem?.errors ?? {})) {
        // Keys can arrive as "City" or "$.city", so tidy them before comparing.
        const field = key.replace('$.', '').toLowerCase()

        if (field === 'date' || field === 'city' || field === 'species' || field === 'notes') {
            found[field] = messages[0]
        } else {
            other.push(...messages)
        }
    }

    errors.value = found

    if (other.length > 0) {
        generalError.value = other.join(' ')
    } else if (Object.keys(found).length === 0) {
        generalError.value = problem?.detail ?? problem?.title ?? "The sighting couldn't be saved."
    }
}

async function submit() {
    result.value = null
    generalError.value = ''

    if (!validate()) return

    isSubmitting.value = true

    try {
        result.value = await submitSighting(form.value)
        form.value = { date: '', city: '', species: '', notes: '' }
    } catch (e) {
        if (e instanceof ApiError && e.status === 400) {
            showServerErrors(e.problem)
        } else {
            generalError.value = "Couldn't reach the server. Check that the API is running, then try again."
        }
    } finally {
        isSubmitting.value = false
    }
}
</script>

<template>
    <div class="page report">
        <h1>Report a tick sighting</h1>
        <p class="intro">Seen a tick in one of the cities we cover? Tell us, and it will appear on the map.</p>

        <p v-if="loadError" class="message message--error">{{ loadError }}</p>

        <div v-if="result" class="message message--success" role="status">
            <p>
                <strong>Thank you.</strong> Your sighting of a {{ result.sighting.species.toLowerCase() }} in
                {{ result.sighting.city }} on {{ savedDate }} has been saved.
            </p>
            <RouterLink :to="`/?city=${encodeURIComponent(result.sighting.city)}`">
                See {{ result.sighting.city }} on the map
            </RouterLink>
        </div>

        <div v-if="result && result.warnings.length > 0" class="message message--warning">
            <p><strong>Please note:</strong></p>
            <ul>
                <li v-for="warning in result.warnings" :key="warning">{{ warning }}</li>
            </ul>
        </div>

        <p v-if="generalError" class="message message--error" role="alert">{{ generalError }}</p>

        <form class="report-form" novalidate @submit.prevent="submit">
            <div class="field">
                <label for="report-date">Date seen</label>
                <input id="report-date"
                       v-model="form.date"
                       type="date"
                       :min="MIN_DATE"
                       :max="TODAY"
                       :aria-invalid="errors.date !== undefined" />
                <p v-if="errors.date" class="field-error">{{ errors.date }}</p>
            </div>

            <div class="field">
                <label for="report-city">City</label>
                <select id="report-city" v-model="form.city" :aria-invalid="errors.city !== undefined">
                    <option value="">Choose a city</option>
                    <option v-for="city in cities" :key="city.name" :value="city.name">{{ city.name }}</option>
                </select>
                <p v-if="errors.city" class="field-error">{{ errors.city }}</p>
            </div>

            <div class="field">
                <label for="report-species">Species</label>
                <select id="report-species" v-model="form.species" :aria-invalid="errors.species !== undefined">
                    <option value="">Choose a species</option>
                    <option v-for="option in speciesOptions" :key="option.name" :value="option.name">
                        {{ option.name }}
                    </option>
                </select>
                <p class="hint">Not sure? <RouterLink to="/guide">See the species guide.</RouterLink></p>
                <p v-if="errors.species" class="field-error">{{ errors.species }}</p>
            </div>

            <div class="field">
                <label for="report-notes">Notes <span class="optional">(optional)</span></label>
                <textarea id="report-notes"
                          v-model="form.notes"
                          rows="4"
                          placeholder="Where exactly you saw it, what it was on, anything else useful"
                          :aria-invalid="errors.notes !== undefined"></textarea>
                <p class="hint" :class="{ 'hint--over': form.notes.length > NOTES_LIMIT }">
                    {{ form.notes.length }} / {{ NOTES_LIMIT }} characters
                </p>
                <p v-if="errors.notes" class="field-error">{{ errors.notes }}</p>
            </div>

            <button class="button" type="submit" :disabled="isSubmitting">
                {{ isSubmitting ? 'Sending…' : 'Submit sighting' }}
            </button>
        </form>
    </div>
</template>

<style scoped>
.report {
    max-width: 40rem;
    margin: 0 auto;
}

.report-form {
    display: flex;
    flex-direction: column;
    gap: var(--space-4);
}

.field {
    display: flex;
    flex-direction: column;
    gap: var(--space-1);
}

.field label {
    font-weight: 700;
}

.optional {
    font-weight: 400;
    color: var(--moss-soft);
}

textarea {
    resize: vertical;
}

.hint {
    margin: 0;
    font-size: var(--text-small);
    color: var(--moss-soft);
}

.hint--over {
    color: var(--bramble);
}

.field-error {
    margin: 0;
    font-size: var(--text-small);
    font-weight: 700;
    color: var(--bramble);
}

.report-form .button {
    align-self: flex-start;
}

.message {
    margin: 0 0 var(--space-4);
    padding: var(--space-3);
    background: var(--paper);
    border-left: 4px solid;
}

.message p {
    margin: 0 0 var(--space-2);
}

.message p:last-child {
    margin-bottom: 0;
}

.message ul {
    margin: 0;
    padding-left: 1.25rem;
}

.message--success {
    border-color: var(--fern);
}

.message--warning {
    border-color: var(--gorse);
}

.message--error {
    border-color: var(--bramble);
    color: var(--bramble);
}
</style>