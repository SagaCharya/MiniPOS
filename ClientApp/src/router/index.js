import { createRouter, createWebHistory } from 'vue-router'
import AdminView from '../views/AdminView.vue'
import WaiterView from '../views/WaiterView.vue'
import KdsView from '../views/KdsView.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/admin',
            name: 'admin',
            component: AdminView
        },
        {
            path: '/waiter',
            name: 'waiter',
            component: WaiterView
        },
        {
            path: '/kds',
            name: 'kds',
            component: KdsView
        },
        {
            path: '/',
            redirect: '/waiter'
        }
    ]
})

export default router
