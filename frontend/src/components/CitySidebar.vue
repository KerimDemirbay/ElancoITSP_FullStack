<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import type { CityReport } from '@/api'

const props = defineProps<{
    city: CityReport | null
    notice: string
}>()

const emit = defineEmits<{
    close: []
}>()

const shareMessage = ref('')

//Google Maps opens directions from wherever the user is to the cities centre
const directionsUrl = computed(() => {
    if (!props.city) return ''
    return `https://www.google.com/maps/dir/?api=1&destination=${props.city.latitude},${props.city.longitude}`
})

//clear "Link copied." when a different city is opened
watch(
    () => props.city?.city,
    () => {
        shareMessage.value = ''
    },
)

async function share() {
    if (!props.city) return

    //link back to this site that opens the same city (MapView reads ?city= when it loads)
    const url = `${window.location.origin}/?city=${encodeURIComponent(props.city.city)}`

    // Phones, and some desktop browsers, have a built-in share menu.
    if (navigator.share) {
        try {
            await navigator.share({ title: `Tick sightings in ${props.city.city}`, url })
        } catch {
            //when user closes the menu
        }
        return
    }

    // Everywhere else, copy the link instead.
    try {
        await navigator.clipboard.writeText(url)
        shareMessage.value = 'Link copied.'
    } catch {
        shareMessage.value = `Couldn't copy the link. Here it is: ${url}`
    }
}

const latestSighting = computed(() => {
    if (!props.city?.latestSighting) return 'None for these filters'
    return new Date(props.city.latestSighting).toLocaleDateString('en-GB', {
        day: 'numeric',
        month: 'short',
        year: 'numeric',
    })
})
</script>

<template>
    <aside class="sidebar" :class="{ open: city }">
        <template v-if="city">
            <button class="sidebar__close" type="button" aria-label="Close" @click="emit('close')">✕</button>

            <p v-if="notice" class="sidebar__notice">{{ notice }}</p>

            <h2>{{ city.city }}</h2>
            <p class="sidebar__region">{{ city.region }}</p>

            <dl class="facts">
                <div>
                    <dt>Sightings</dt>
                    <dd>{{ city.count.toLocaleString('en-GB') }}</dd>
                </div>
                <div>
                    <dt>Latest sighting</dt>
                    <dd>{{ latestSighting }}</dd>
                </div>
            </dl>

            <div class="actions">
                <a class="button" :href="directionsUrl" target="_blank" rel="noopener">Get directions</a>
                <button type="button" class="button button--quiet" @click="share">Share</button>
            </div>
            <p v-if="shareMessage" class="share-message">{{ shareMessage }}</p>

            <h3>Species seen here</h3>
            <p v-if="city.species.length === 0">No sightings match the current filters.</p>
            <ul v-else class="species">
                <li v-for="item in city.species" :key="item.species">
                    <span>{{ item.species }}</span>
                    <span class="species__count">{{ item.count }}</span>
                </li>
            </ul>
        </template>
    </aside>
</template>

<style scoped>
    .sidebar {
        position: absolute;
        top: 0;
        right: 0;
        bottom: 0;
        z-index: 1100;
        width: min(24rem, 100%);
        padding: var(--space-4);
        overflow-y: auto;
        background: var(--paper);
        border-left: 1px solid var(--lichen);
        box-shadow: -4px 0 12px rgb(27 46 36 / 0.15);
        transform: translateX(100%);
        transition: transform 0.45s ease;
    }

        .sidebar.open {
            transform: translateX(0);
        }

    .sidebar__close {
        float: right;
        padding: var(--space-1) var(--space-2);
        border: none;
        background: none;
        color: var(--moss);
        font-size: 1.25rem;
        cursor: pointer;
    }

        .sidebar__close:hover {
            color: var(--fern);
        }

    .sidebar__notice {
        clear: both;
        padding: var(--space-2) var(--space-3);
        background: var(--heath);
        border-left: 3px solid var(--fern);
        font-size: var(--text-small);
    }

    .sidebar h2 {
        margin-bottom: 0;
    }

    .sidebar__region {
        margin: 0 0 var(--space-3);
        color: var(--moss-soft);
    }

    .facts {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: var(--space-3);
        margin: 0 0 var(--space-4);
    }

        .facts dt {
            font-size: var(--text-small);
            color: var(--moss-soft);
        }

        .facts dd {
            margin: 0;
            font-size: 1.35rem;
            font-weight: 700;
        }

    .actions {
        display: flex;
        gap: var(--space-2);
        margin: 0 0 var(--space-4);
    }

        .actions a {
            text-decoration: none;
        }

    .share-message {
        margin: calc(-1 * var(--space-3)) 0 var(--space-4);
        font-size: var(--text-small);
        color: var(--moss-soft);
    }

    .species {
        list-style: none;
        margin: 0;
        padding: 0;
    }

        .species li {
            display: flex;
            justify-content: space-between;
            padding: var(--space-2) 0;
            border-bottom: 1px solid var(--lichen);
        }

    .species__count {
        font-weight: 700;
    }

    /* phones slide up from the bottom instead */
    @media (max-width: 700px) {
        .sidebar {
            top: auto;
            left: 0;
            width: 100%;
            max-height: 70%;
            border-left: none;
            border-top: 1px solid var(--lichen);
            border-radius: 1rem 1rem 0 0;
            box-shadow: 0 -4px 12px rgb(27 46 36 / 0.15);
            transform: translateY(100%);
        }

            .sidebar.open {
                transform: translateY(0);
            }
    }
</style>