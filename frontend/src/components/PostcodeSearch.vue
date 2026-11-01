<script setup lang="ts">
import { ref } from 'vue'

const props = defineProps<{
    cityNames: string[]
}>()

const emit = defineEmits<{
    found: [city: string, postcode: string]
}>()

// used the parts of a postcodes.io result
interface PostcodeResult {
    postcode: string
    admin_district: string | null
    region: string | null
}

// postcodes.io gives the official council name
// four of our cities are named differently
const OFFICIAL_NAMES: Record<string, string> = {
    'Bristol, City of': 'Bristol',
    'City of Edinburgh': 'Edinburgh',
    'Glasgow City': 'Glasgow',
    'Newcastle upon Tyne': 'Newcastle',
}

const query = ref('')
const isLoading = ref(false)
const message = ref('')

function cityFor(result: PostcodeResult): string | null {
    // every London borough (Westminster, Croydon, …) has the region "London".
    if (result.region === 'London') return 'London'
    if (result.admin_district === null) return null
    return OFFICIAL_NAMES[result.admin_district] ?? result.admin_district
}

async function search() {
    const postcode = query.value.trim()

    if (postcode === '') {
        message.value = 'Enter a UK postcode, for example LS1 4AP.'
        return
    }

    isLoading.value = true
    message.value = ''

    try {
        const response = await fetch(`https://api.postcodes.io/postcodes/${encodeURIComponent(postcode)}`)

        if (!response.ok) {
            message.value = `We couldn't find the postcode "${postcode}". Check it and try again.`
            return
        }

        const data = await response.json()
        const result: PostcodeResult = data.result
        const city = cityFor(result)

        if (city !== null && props.cityNames.includes(city)) {
            emit('found', city, result.postcode)
        } else {
            const area = result.admin_district ?? 'that area'
            message.value = `${result.postcode} is in ${area}. We don't have sighting data there. We cover: ${props.cityNames.join(', ')}.`
        }
    } catch {
        message.value = "The postcode service didn't respond. Try again in a moment."
    } finally {
        isLoading.value = false
    }
}
</script>

<template>
    <div class="postcode">
        <form class="pill" @submit.prevent="search">
            <label for="postcode" class="visually-hidden">Postcode</label>
            <input id="postcode"
                   v-model="query"
                   type="text"
                   placeholder="Enter a postcode, e.g. S1 1AA"
                   autocomplete="postal-code"
                   @input="message = ''" />
            <button class="button" type="submit" :disabled="isLoading">
                {{ isLoading ? 'Searching…' : 'Search' }}
            </button>
        </form>
        <p v-if="message" class="postcode__message">{{ message }}</p>
    </div>
</template>

<style scoped>
    .postcode {
        position: absolute;
        top: var(--space-3);
        left: 50%;
        transform: translateX(-50%);
        z-index: 1000;
        width: min(26rem, calc(100% - 2 * var(--space-3)));
    }

    .pill {
        display: flex;
        align-items: center;
        gap: var(--space-2);
        padding: 0.3rem 0.3rem 0.3rem 1.1rem;
        background: var(--paper);
        border: 1px solid var(--lichen);
        border-radius: 999px;
        box-shadow: 0 4px 14px rgb(27 46 36 / 0.18);
    }

        .pill input {
            flex: 1;
            min-width: 0;
            border: none;
            background: transparent;
            font: inherit;
            color: var(--moss);
        }

            .pill input:focus-visible {
                outline: none;
            }

        .pill .button {
            border-radius: 999px;
        }

    .postcode__message {
        margin: var(--space-2) auto 0;
        padding: var(--space-2) var(--space-3);
        background: var(--paper);
        border-radius: 0.5rem;
        font-size: var(--text-small);
        text-align: center;
    }

    /* phones sit to the right of the zoom buttons instead of on top of them */
    @media (max-width: 700px) {
        .postcode {
            left: 3.25rem;
            right: var(--space-3);
            width: auto;
            transform: none;
        }
    }
</style>