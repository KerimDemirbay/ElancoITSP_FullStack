<script setup lang="ts">
import DataStatusBar from '@/components/DataStatusBar.vue'
</script>

<template>
    <a class="skip-link" href="#main">Skip to content</a>

    <header class="site-header">
        <RouterLink to="/" class="brand">UK Tick Sightings</RouterLink>
        <nav aria-label="Main">
            <RouterLink to="/">Map</RouterLink>
            <RouterLink to="/guide">Species and prevention</RouterLink>
            <RouterLink to="/report" class="nav-report">Report a sighting</RouterLink>
        </nav>
    </header>

    <main id="main">
        <RouterView />
    </main>

    <footer class="site-footer">
        <DataStatusBar />
    </footer>
</template>

<style scoped>
    .site-header {
        display: flex;
        flex-wrap: wrap;
        align-items: baseline;
        justify-content: space-between;
        gap: var(--space-2) var(--space-4);
        padding: var(--space-3) var(--gutter);
        border-bottom: 1px solid var(--lichen);
        background: var(--heath);
    }

    .brand {
        font-family: var(--font-display);
        font-size: 1.6rem;
        font-weight: 700;
        color: var(--moss);
        text-decoration: none;
    }

    nav {
        display: flex;
        flex-wrap: wrap;
        gap: var(--space-1) var(--space-4);
    }

        nav a {
            color: var(--moss);
            text-decoration: none;
            padding-block: 0.25rem;
            border-bottom: 2px solid transparent;
        }

            nav a:hover {
                border-bottom-color: var(--lichen);
            }

            nav a.router-link-exact-active {
                border-bottom-color: var(--fern);
                font-weight: 700;
            }

    .nav-report {
        color: var(--fern);
    }

    main {
        flex: 1;
    }

    .site-footer {
        padding: var(--space-2) var(--gutter);
        border-top: 1px solid var(--lichen);
    }
</style>