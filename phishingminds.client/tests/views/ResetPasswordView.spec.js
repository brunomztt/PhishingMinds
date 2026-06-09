import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import ResetPasswordView from '@/views/ResetPasswordView.vue'

const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush
  }),
  useRoute: () => ({
    query: {}
  })
}))

describe('ResetPasswordView.vue', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn())
    vi.useFakeTimers()
    mockPush.mockClear()
  })

  it('performs successful reset when passwords match', async () => {
    const mockResponse = { ok: true }
    vi.mocked(fetch).mockResolvedValue(mockResponse)

    const wrapper = mount(ResetPasswordView)

    const emailInput = wrapper.find('input[type="email"]')
    const passInputs = wrapper.findAll('input[type="password"]')
    
    await emailInput.setValue('test@email.com')
    await passInputs[0].setValue('newpwd123')
    await passInputs[1].setValue('newpwd123')

    const form = wrapper.find('form')
    await form.trigger('submit.prevent')

    expect(fetch).toHaveBeenCalledWith('/api/auth/reset-password', expect.objectContaining({
      method: 'POST',
      body: JSON.stringify({ email: 'test@email.com', newPassword: 'newpwd123' })
    }))

    await vi.waitFor(() => {
      expect(wrapper.text()).toContain('Senha atualizada com sucesso')
    })

    vi.advanceTimersByTime(2000)
    expect(mockPush).toHaveBeenCalledWith('/login')
  })

  it('shows error message if passwords do not match', async () => {
    const wrapper = mount(ResetPasswordView)

    const emailInput = wrapper.find('input[type="email"]')
    const passInputs = wrapper.findAll('input[type="password"]')

    await emailInput.setValue('test@email.com')
    await passInputs[0].setValue('pwd1')
    await passInputs[1].setValue('pwd2')

    const form = wrapper.find('form')
    await form.trigger('submit.prevent')

    expect(fetch).not.toHaveBeenCalled()
    expect(wrapper.text()).toContain('As senhas não coincidem')
  })
})
