import { createRouter, createWebHistory } from 'vue-router'
import MapView from '@/views/MapView.vue'
import GuideView from '@/views/GuideView.vue'
import ReportView from '@/views/ReportView.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        { path: '/', name: 'map', component: MapView },
        { path: '/guide', name: 'guide', component: GuideView },
        { path: '/report', name: 'report', component: ReportView },
    ],
})

export default router