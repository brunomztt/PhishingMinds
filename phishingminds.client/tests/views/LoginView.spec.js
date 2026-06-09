import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import LoginView from '@/views/LoginView.vue'

const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush
  }),
  useRoute: () => ({
    query: {}
  })
}))

describe('LoginView.vue', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.stubGlobal('fetch', vi.fn())
    mockPush.mockClear()
  })

  it('performs successful login', async () => {
    const mockResponse = {
      ok: true,
      json: vi.fn().mockResolvedValue({
        token: 'token123',
        user: { idEmpresa: 2, nome: 'Helen' }
      })
    }
    vi.mocked(fetch).mockResolvedValue(mockResponse)

    const wrapper = mount(LoginView)

    const emailInput = wrapper.find('input[type="email"]')
    const passwordInput = wrapper.find('input[type="password"]')
    await emailInput.setValue('test@email.com')
    await passwordInput.setValue('123456')

    const form = wrapper.find('form')
    await form.trigger('submit.prevent')

    expect(fetch).toHaveBeenCalledWith('/api/auth/login', expect.objectContaining({
      method: 'POST',
      body: JSON.stringify({ email: 'test@email.com', password: '123456' })
    }))

    await vi.waitFor(() => {
      expect(localStorage.getItem('token')).toBe('token123')
      expect(JSON.parse(localStorage.getItem('user'))).toEqual({ idEmpresa: 2, nome: 'Helen' })
      expect(mockPush).toHaveBeenCalledWith('/painel')
    })
  })

  it('handles credentials error', async () => {
    const mockResponse = { ok: false }
    vi.mocked(fetch).mockResolvedValue(mockResponse)

    const wrapper = mount(LoginView)

    const form = wrapper.find('form')
    await form.trigger('submit.prevent')

    await vi.waitFor(() => {
      expect(wrapper.text()).toContain('Credenciais inválidas')
      expect(mockPush).not.toHaveBeenCalled()
    })
  })
})
