<script setup lang="ts">
import { computed, ref } from 'vue'
import type { SpeciesInfo } from '@/api'

defineProps<{
    speciesOptions: SpeciesInfo[]
    minDate: string
    maxDate: string
    dateError: string
}>()

// defineModel = a prop the parent can bind with v-model so changes flow both ways
const from = defineModel<string>('from', { required: true })
const to = defineModel<string>('to', { required: true })
const species = defineModel<string>('species', { required: true })

//on phones the filters fold away behind a button so the map gets the screen.
const isOpen = ref(false)
const activeCount = computed(() => [from.value, to.value, species.value].filter((v) => v !== '').length)

const toggleLabel = computed(() => {
    if (isOpen.value) return 'Hide filters'
    if (activeCount.value > 0) return `Filters (${activeCount.value} on)`
    return 'Filters'
})

function reset() {
    from.value = ''
    to.value = ''
    species.value = ''
}
</script>

<template>
    <div class="filters-wrap">
        <button type="button" class="filters__toggle button button--quiet" @click="isOpen = !isOpen">
            {{ toggleLabel }}
        </button>

        <form class="filters" :class="{ open: isOpen }" @submit.prevent>
            <div class="field">
                <label for="filter-from">From</label>
                <input id="filter-from"
                       v-model="from"
                       type="date"
                       :min="minDate"
                       :max="maxDate"
                       :aria-invalid="dateError !== ''" />
            </div>

            <label for="filter-to">To</label>
            <input id="filter-to"
                   v-model="to"
                   type="date"
                   :min="minDate"
                   :max="maxDate"
                   :aria-invalid="dateError !== ''" />

            <div class="field">
                <label for="filter-species">Species</label>
                <select id="filter-species" v-model="species">
                    <option value="">All species</option>
                    <option v-for="option in speciesOptions" :key="option.name" :value="option.name">
                        {{ option.name }}
                    </option>
                </select>
            </div>

            <button type="button" class="button button--quiet" @click="reset">Clear filters</button>

            <p v-if="dateError" class="filters__error">{{ dateError }}</p>
        </form>
    </div>
</template>

<style scoped>
    .filters-wrap {
        border-bottom: 1px solid var(--lichen);
    }

    .filters {
        display: flex;
        flex-wrap: wrap;
        align-items: flex-end;
        gap: var(--space-2) var(--space-3);
        padding: var(--space-3) var(--gutter);
    }

    .filters__toggle {
        display: none;
    }

    .field {
        display: flex;
        flex-direction: column;
        gap: var(--space-1);
    }

        .field label {
            font-size: var(--text-small);
            font-weight: 700;
        }

    .filters__error {
        flex-basis: 100%;
        margin: 0;
        color: var(--bramble);
        font-size: var(--text-small);
    }

    /* phones hide the filters behind the toggle button */
    @media (max-width: 700px) {
        .filters__toggle {
            display: block;
            margin: var(--space-2) var(--gutter);
        }

        .filters {
            display: none;
            padding-top: 0;
        }

            .filters.open {
                display: flex;
            }
    }
</style>