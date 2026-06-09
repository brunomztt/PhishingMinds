import { describe, it, expect, beforeEach, afterEach } from 'vitest'
import router from '@/router/index.js'

describe('Router', () => {
  beforeEach(() => {
    localStorage.clear()
  })

  afterEach(() => {
    localStorage.clear()
  })

  it('contains correctly defined routes', () => {
    const routes = router.getRoutes()
    const paths = routes.map(r => r.path)
    expect(paths).toContain('/login')
    expect(paths).toContain('/organizacao')
    expect(paths).toContain('/painel')
  })

  it('redirects to /login if token is missing and route requires auth', async () => {
    await router.push('/painel')
    expect(router.currentRoute.value.path).toBe('/login')
  })

  it('allows access to protected routes if token is present', async () => {
    localStorage.setItem('token', 'valid-token')
    await router.push('/painel')
    expect(router.currentRoute.value.path).toBe('/painel')
  })

  it('allows access to public routes without a token', async () => {
    await router.push('/cadastro')
    expect(router.currentRoute.value.path).toBe('/cadastro')
  })
})
