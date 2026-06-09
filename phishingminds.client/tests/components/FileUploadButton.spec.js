import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import FileUploadButton from '@/components/ui/FileUploadButton.vue'

describe('FileUploadButton.vue', () => {
  it('renders correct text', () => {
    const wrapper = mount(FileUploadButton, {
      props: {
        text: 'Carregar Planilha'
      }
    })
    expect(wrapper.text()).toContain('Carregar Planilha')
  })

  it('triggers change event when input file changes', async () => {
    const wrapper = mount(FileUploadButton)
    const input = wrapper.find('input[type="file"]')
    
    // Simulate change event on the file input
    await input.trigger('change')
    
    expect(wrapper.emitted()).toHaveProperty('change')
    expect(wrapper.emitted().change[0][0]).toBeInstanceOf(Event)
  })
})
