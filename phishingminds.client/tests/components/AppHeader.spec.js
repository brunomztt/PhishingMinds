import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import AppHeader from '@/components/Layout/AppHeader.vue'

describe('AppHeader.vue', () => {
  it('renders logo and navigation items', () => {
    const $router = { push: vi.fn() }
    const wrapper = mount(AppHeader, {
      global: {
        mocks: {
          $router
        }
      }
    })
    expect(wrapper.text()).toContain('PhishingMinds')
    expect(wrapper.text()).toContain('COMO FUNCIONA')
    expect(wrapper.text()).toContain('PREÇOS')
  })

  it('triggers router push when clicking ENTRAR and COMEÇAR AGORA', async () => {
    const $router = { push: vi.fn() }
    const wrapper = mount(AppHeader, {
      global: {
        mocks: {
          $router
        }
      }
    })

    const buttons = wrapper.findAll('button')
    const entrarBtn = buttons.find(b => b.text() === 'ENTRAR')
    const cadastrarBtn = buttons.find(b => b.text() === 'COMEÇAR AGORA')

    expect(entrarBtn).toBeDefined()
    expect(cadastrarBtn).toBeDefined()

    await entrarBtn.trigger('click')
    expect($router.push).toHaveBeenCalledWith('/login')

    await cadastrarBtn.trigger('click')
    expect($router.push).toHaveBeenCalledWith('/cadastro')
  })

  it('toggles mobile menu when clicking menu button', async () => {
    const $router = { push: vi.fn() }
    const wrapper = mount(AppHeader, {
      global: {
        mocks: {
          $router
        }
      }
    })

    // Mobile navigation container should not be visible or present by default
    expect(wrapper.find('div.md\\:hidden.w-full').exists()).toBe(false)

    // Toggle menu
    const toggleBtn = wrapper.find('button.md\\:hidden')
    expect(toggleBtn.exists()).toBe(true)

    await toggleBtn.trigger('click')
    
    // Mobile navigation container should be present now
    expect(wrapper.find('div.md\\:hidden.w-full').exists()).toBe(true)
    expect(wrapper.text()).toContain('COMO FUNCIONA')
  })
})
