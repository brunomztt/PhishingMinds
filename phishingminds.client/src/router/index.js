import { createRouter, createWebHistory } from 'vue-router'
import SystemHomeView from '../views/SystemHomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Home',
      component: SystemHomeView
    },
    {
      path: '/campanhas',
      name: 'Campanhas',
      component: SystemCampaignsView
    },
    {
      path: '/organizacao',
      name: 'Organização',
      component: SystemOrganizationView
    },
    {
      path: '/contratos',
      name: 'Contratos',
      component: SystemContractsView
    },
    {
      path: '/perfil',
      name: 'Perfil',
      component: SystemProfileView
    }
  ]
})

export default router
