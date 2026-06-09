import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import Topbar from '@/components/Layout/Topbar.vue'

describe('Topbar.vue', () => {
  beforeEach(() => {
    localStorage.clear()
    localStorage.setItem('user', JSON.stringify({ nome: 'Helen Lauren', isPessoa: true }))
  })

  it('renders user details from localStorage', async () => {
    const wrapper = mount(Topbar, {
      props: {
        isDarkMode: false,
        canToggleTheme: true
      }
    })
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Helen Lauren')
    expect(wrapper.text()).toContain('Funcionário')
  })

  it('emits toggle-theme event when button is clicked', async () => {
    const wrapper = mount(Topbar, {
      props: {
        isDarkMode: false,
        canToggleTheme: true
      }
    })

    const toggleBtn = wrapper.find('button[title="Alternar Tema"]')
    expect(toggleBtn.exists()).toBe(true)

    await toggleBtn.trigger('click')
    expect(wrapper.emitted()).toHaveProperty('toggle-theme')
  })
})
