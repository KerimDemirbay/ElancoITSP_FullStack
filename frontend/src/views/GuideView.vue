<script setup lang="ts">
interface SpeciesGuide {
    name: string
    latinName: string
    habitat: string
    feedsOn: string
    image: string | null
    note?: string
}

//facts from the UK Health Security Agency's tick species profiles
const species: SpeciesGuide[] = [
    {
        name: 'Marsh tick',
        latinName: 'Ixodes apronophorus',
        habitat: 'Wetlands. Only recorded in a few places, such as Kent, the Norfolk Broads and Wicken Fen.',
        feedsOn: 'Water voles.',
        image: null,
    },
    {
        name: 'Southern rodent tick',
        latinName: 'Ixodes acuminatus',
        habitat: 'Burrows of small rodents, mainly in southern England, including Cornwall, Devon and the Isles of Scilly.',
        feedsOn: 'Small rodents.',
        image: '/tick-images/Southern-Rodent-Tick.jpg',
    },
    {
        name: 'Passerine tick',
        latinName: 'Ixodes frontalis',
        habitat: 'The ground under places where birds roost. Recorded in England and, more recently, Wales.',
        feedsOn: 'Birds, especially perching birds. Sometimes found feeding on people.',
        image: '/tick-images/Passerine-Tick.jpg',
        note: 'The Elanco data lists this species as "Dermacentor frontalis".',
    },
    {
        name: 'Fox/badger tick',
        latinName: 'Ixodes canisuga',
        habitat: 'Widespread across the UK.',
        feedsOn: 'Badgers, foxes and pet dogs.',
        image: '/tick-images/Fox-Badger-Tick.jpg',
    },
    {
        name: 'Tree-hole tick',
        latinName: 'Ixodes arboricola',
        habitat: 'Tree holes used by nesting birds. Mostly recorded in England, with a few records in Wales and Scotland.',
        feedsOn: 'Birds that nest in tree holes, such as great tits and blue tits.',
        image: '/tick-images/Tree-Hole-Tick.jpg',
    },
]

//from NHS guidance on Lyme disease
const preventionTips = [
    'Cover your skin when walking outdoors, and tuck your trousers into your socks.',
    'Use insect repellent on your clothes and skin. Products containing DEET work best.',
    'Stay on clear paths and avoid long grass where you can.',
    'Wear light-coloured clothing so ticks are easier to spot and brush off.',
    'Check yourself, your children and your pets for ticks after being outdoors.',
    'Ask your vet about tick treatments for your dog or cat.',
]

const removalSteps = [
    'Use fine-tipped tweezers or a tick-removal tool.',
    'Grasp the tick as close to the skin as possible.',
    'Pull upwards slowly, without squeezing or crushing the tick.',
    'Clean the bite with antiseptic or soap and water.',
]
</script>

<template>
    <div class="page guide">
        <h1>Tick species and prevention</h1>
        <p class="intro">
            Five tick species appear in our sighting data. Most of them rarely bite people, but they can affect
            pets and wildlife, and any tick bite is worth knowing how to handle.
        </p>

        <section>
            <h2>Species in our data</h2>
            <ul class="species-grid">
                <li v-for="item in species" :key="item.name" class="species-card">
                    <img v-if="item.image" :src="item.image" :alt="`${item.name} (${item.latinName})`" loading="lazy" />
                    <div v-else class="no-photo">No photo available</div>

                    <h3>{{ item.name }}</h3>
                    <p class="latin">{{ item.latinName }}</p>

                    <dl>
                        <dt>Where it lives</dt>
                        <dd>{{ item.habitat }}</dd>
                        <dt>What it feeds on</dt>
                        <dd>{{ item.feedsOn }}</dd>
                    </dl>

                    <p v-if="item.note" class="note">{{ item.note }}</p>
                </li>
            </ul>
            <p class="credit">Photos courtesy of ESCCAP UK &amp; Ireland.</p>
        </section>

        <section class="advice">
            <div>
                <h2>Avoiding tick bites</h2>
                <ul>
                    <li v-for="tip in preventionTips" :key="tip">{{ tip }}</li>
                </ul>
            </div>

            <div>
                <h2>If you find a tick</h2>
                <ol>
                    <li v-for="step in removalSteps" :key="step">{{ step }}</li>
                </ol>
                <p class="warning">
                    See a GP if, within 3 months of a tick bite, you get a round or oval rash or flu-like symptoms.
                    These can be signs of Lyme disease.
                </p>
            </div>
        </section>

        <p class="sources">
            Sources:
            <a href="https://www.gov.uk/guidance/tick-species-profiles" target="_blank" rel="noopener">UKHSA tick species profiles</a>
            and
            <a href="https://www.nhs.uk/conditions/lyme-disease/" target="_blank" rel="noopener">NHS guidance on Lyme disease</a>.
        </p>
    </div>
</template>

<style scoped>
    .guide {
        max-width: 72rem;
        margin: 0 auto;
    }

    .intro {
        margin-bottom: var(--space-5);
    }

    section {
        margin-bottom: var(--space-5);
    }

    .species-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(15rem, 1fr));
        gap: var(--space-4);
        list-style: none;
        margin: 0;
        padding: 0;
    }

    .species-card {
        padding: var(--space-3);
        background: var(--paper);
        border: 1px solid var(--lichen);
        border-radius: 0.5rem;
    }

        .species-card img,
        .no-photo {
            display: block;
            width: 100%;
            aspect-ratio: 4 / 3;
            margin-bottom: var(--space-3);
            border-radius: 0.375rem;
            background: #fff;
            object-fit: contain;
        }

    .no-photo {
        display: flex;
        align-items: center;
        justify-content: center;
        background: var(--heath);
        color: var(--moss-soft);
        font-size: var(--text-small);
    }

    .species-card h3 {
        margin-bottom: 0;
    }

    .latin {
        margin: 0 0 var(--space-3);
        font-style: italic;
        color: var(--moss-soft);
    }

    dl {
        margin: 0;
    }

    dt {
        font-size: var(--text-small);
        font-weight: 700;
    }

    dd {
        margin: 0 0 var(--space-2);
    }

    .note,
    .credit,
    .sources {
        font-size: var(--text-small);
        color: var(--moss-soft);
    }

    .advice {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(18rem, 1fr));
        gap: var(--space-5);
    }

        .advice li {
            margin-bottom: var(--space-2);
        }

    .warning {
        padding: var(--space-2) var(--space-3);
        border-left: 3px solid var(--gorse);
        background: var(--paper);
    }
</style>