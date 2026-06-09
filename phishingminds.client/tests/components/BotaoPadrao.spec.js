import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import BotaoPadrao from '@/components/ui/BotaoPadrao.vue'

describe('BotaoPadrao.vue', () => {
  it('renders correctly with default secondary styles', () => {
    const wrapper = mount(BotaoPadrao, {
      slots: {
        default: 'Clique Aqui'
      }
    })
    expect(wrapper.text()).toBe('Clique Aqui')
    expect(wrapper.classes()).toContain('bg-white')
    expect(wrapper.classes()).toContain('text-gray-800')
  })

  it('renders primary styles when tipo is primario', () => {
    const wrapper = mount(BotaoPadrao, {
      props: {
        tipo: 'primario'
      },
      slots: {
        default: 'Enviar'
      }
    })
    expect(wrapper.text()).toBe('Enviar')
    expect(wrapper.classes()).toContain('bg-green-600')
    expect(wrapper.classes()).toContain('text-white')
  })
})
