import { createRouter, createWebHistory } from 'vue-router'
import SystemHomeView from '../views/SystemHomeView.vue'
import SystemCampaignsView from '../views/SystemCampaignsView.vue'
import SystemOrganizationView from '../views/SystemOrganizationView.vue'
import SystemContractsView from '../views/SystemContractsView.vue'
import SystemProfileView from '../views/SystemProfileView.vue'
import LoginView from '../views/LoginView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: LoginView
    },
    {
      path: '/',
      name: 'Home',
      component: SystemHomeView,
      meta: { requiresAuth: true }
    },
    {
      path: '/campanhas',
      name: 'Campanhas',
      component: SystemCampaignsView,
      meta: { requiresAuth: true }
    },
    {
      path: '/organizacao',
      name: 'Organização',
      component: SystemOrganizationView,
      meta: { requiresAuth: true }
    },
    {
      path: '/contratos',
      name: 'Contratos',
      component: SystemContractsView,
      meta: { requiresAuth: true }
    },
    {
      path: '/perfil',
      name: 'Perfil',
      component: SystemProfileView,
      meta: { requiresAuth: true }
    }
  ]
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  if (to.meta.requiresAuth && !token) {
    next('/login');
  } else {
    next();
  }
});

export default router
