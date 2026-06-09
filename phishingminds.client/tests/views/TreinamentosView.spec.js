import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import TreinamentosView from '@/views/TreinamentosView.vue'

describe('TreinamentosView.vue', () => {
  beforeEach(() => {
    localStorage.clear()
    localStorage.setItem('user', JSON.stringify({ idUser: 5, nome: 'Helen' }))
    localStorage.setItem('token', 'mock-token')
    
    vi.stubGlobal('fetch', vi.fn())
    vi.stubGlobal('alert', vi.fn())
  })

  it('renders "Tudo em dia!" when training is not required', async () => {
    const mockResponse = {
      ok: true,
      json: vi.fn().mockResolvedValue({
        necessitaTreinamento: false
      })
    }
    vi.mocked(fetch).mockResolvedValue(mockResponse)

    const wrapper = mount(TreinamentosView, {
      global: {
        stubs: {
          MainLayout: {
            template: '<div><slot /></div>'
          }
        }
      }
    })

    await vi.waitFor(() => {
      expect(wrapper.text()).toContain('Tudo em dia!')
      expect(wrapper.text()).not.toContain('Treinamento Obrigatório')
    })
  })

  it('renders quiz and allows successful completion', async () => {
    const mockResponse = {
      ok: true,
      json: vi.fn().mockResolvedValue({
        necessitaTreinamento: true
      })
    }
    vi.mocked(fetch).mockResolvedValue(mockResponse)

    const wrapper = mount(TreinamentosView, {
      global: {
        stubs: {
          MainLayout: {
            template: '<div><slot /></div>'
          }
        }
      }
    })

    await vi.waitFor(() => {
      expect(wrapper.text()).toContain('Treinamento Obrigatório')
    })

    // Set respuestas directly
    wrapper.vm.respostas = [false, true, false, true, true]

    // Mock fetch for /api/Treinamento/concluir
    const mockConcluirRes = { ok: true }
    vi.mocked(fetch).mockImplementation(async (url) => {
      if (url === '/api/Treinamento/concluir') return mockConcluirRes
      return mockResponse
    })

    const finalizeBtn = wrapper.find('button')
    await finalizeBtn.trigger('click')

    expect(fetch).toHaveBeenCalledWith('/api/Treinamento/concluir', expect.objectContaining({
      method: 'POST',
      body: JSON.stringify({ idUser: 5 })
    }))

    expect(wrapper.text()).toContain('Treinamento concluído')
  })

  it('renders failure when quiz answers are incorrect', async () => {
    const mockResponse = {
      ok: true,
      json: vi.fn().mockResolvedValue({
        necessitaTreinamento: true
      })
    }
    vi.mocked(fetch).mockResolvedValue(mockResponse)

    const wrapper = mount(TreinamentosView, {
      global: {
        stubs: {
          MainLayout: {
            template: '<div><slot /></div>'
          }
        }
      }
    })

    await vi.waitFor(() => {
      expect(wrapper.text()).toContain('Treinamento Obrigatório')
    })

    wrapper.vm.respostas = [true, true, true, true, true]

    const finalizeBtn = wrapper.find('button')
    await finalizeBtn.trigger('click')

    expect(fetch).not.toHaveBeenCalledWith('/api/Treinamento/concluir', expect.any(Object))
    expect(wrapper.text()).toContain('Treinamento não aprovado')
  })
})
