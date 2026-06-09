import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import CadastroView from '@/views/CadastroView.vue'

const mockPush = vi.fn()
let mockQuery = {}

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush
  }),
  useRoute: () => ({
    get query() {
      return mockQuery
    }
  })
}))

describe('CadastroView.vue', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn())
    mockPush.mockClear()
    mockQuery = {}
  })

  it('navigates through steps and registers successfully', async () => {
    mockQuery = { plano: '2' }
    const mockResponse = { ok: true }
    vi.mocked(fetch).mockResolvedValue(mockResponse)

    const wrapper = mount(CadastroView)

    // Verify IdPlano initialized from query param
    expect(wrapper.vm.form.IdPlano).toBe(2)

    // Step 1: E-mail
    const emailInput = wrapper.find('input[type="email"]')
    await emailInput.setValue('test@company.com')
    const form = wrapper.find('form')
    await form.trigger('submit')

    expect(wrapper.text()).toContain('Passo 2 de 3')

    // Step 2: CNPJ
    const cnpjInput = wrapper.find('input[type="text"]')
    await cnpjInput.setValue('12345678000100') // 14 digits
    await form.trigger('submit')

    expect(wrapper.text()).toContain('Passo 3 de 3')

    // Step 3: Fields and submit
    const nmEmpresaInput = wrapper.find('input[placeholder="Sua Empresa LTDA"]')
    const nmDonoInput = wrapper.find('input[placeholder="João Silva"]')
    const passwordInput = wrapper.find('input[type="password"]')

    await nmEmpresaInput.setValue('Tech Solutions')
    await nmDonoInput.setValue('Maria Silva')
    await passwordInput.setValue('secret123')

    await form.trigger('submit')

    expect(fetch).toHaveBeenCalledWith('/api/auth/register', expect.objectContaining({
      method: 'POST',
      body: JSON.stringify({
        Mail: 'test@company.com',
        CNPJ: '12.345.678/0001-00', // autoformatted CNPJ
        Nm_Empresa: 'Tech Solutions',
        Nm_Dono: 'Maria Silva',
        Senha: 'secret123',
        IdPlano: 2
      })
    }))

    await vi.waitFor(() => {
      expect(mockPush).toHaveBeenCalledWith('/login')
    })
  })

  it('fails Step 1 for invalid email', async () => {
    const wrapper = mount(CadastroView)

    const emailInput = wrapper.find('input[type="email"]')
    await emailInput.setValue('invalid-email')
    const form = wrapper.find('form')
    await form.trigger('submit')

    expect(wrapper.text()).toContain('Por favor, insira um e-mail válido')
    expect(wrapper.text()).toContain('Passo 1 de 3')
  })

  it('fails Step 2 for invalid CNPJ length', async () => {
    const wrapper = mount(CadastroView)

    // Advance to Step 2 with valid email
    await wrapper.find('input[type="email"]').setValue('test@company.com')
    await wrapper.find('form').trigger('submit')

    // Set short CNPJ
    const cnpjInput = wrapper.find('input[type="text"]')
    await cnpjInput.setValue('123') // Less than 14 digits
    await wrapper.find('form').trigger('submit')

    expect(wrapper.text()).toContain('Por favor, insira um CNPJ válido')
    expect(wrapper.text()).toContain('Passo 2 de 3')
  })
})
