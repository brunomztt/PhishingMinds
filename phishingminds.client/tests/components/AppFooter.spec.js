import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import AppFooter from '@/components/Layout/AppFooter.vue'

describe('AppFooter.vue', () => {
  it('renders footer sections correctly', () => {
    const wrapper = mount(AppFooter)
    expect(wrapper.text()).toContain('Produto')
    expect(wrapper.text()).toContain('Empresa')
    expect(wrapper.text()).toContain('Fique Atualizado')
    expect(wrapper.text()).toContain('contato@phishingminds.com')
  })
})
