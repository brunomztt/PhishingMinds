import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, RouterLinkStub } from '@vue/test-utils'
import Sidebar from '@/components/Layout/Sidebar.vue'

const mockPush = vi.fn()
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush
  }),
  useRoute: () => ({
    path: '/painel',
    startsWith: (path) => '/painel'.startsWith(path)
  })
}))

describe('Sidebar.vue', () => {
  beforeEach(() => {
    localStorage.clear()
    localStorage.setItem('user', JSON.stringify({ idEmpresa: 2, isPessoa: false }))
    localStorage.setItem('token', 'mock-token')
    mockPush.mockClear()

    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
      ok: true,
      json: vi.fn().mockResolvedValue([])
    }))
  })

  it('renders navigation links', () => {
    const wrapper = mount(Sidebar, {
      global: {
        stubs: {
          RouterLink: RouterLinkStub
        },
        mocks: {
          $route: {
            path: '/painel'
          }
        }
      }
    })
    expect(wrapper.text()).toContain('Overview')
    expect(wrapper.text()).toContain('Campanhas')
    expect(wrapper.text()).toContain('Organização')
    expect(wrapper.text()).toContain('Contratos')
  })

  it('performs logout successfully', async () => {
    const wrapper = mount(Sidebar, {
      global: {
        stubs: {
          RouterLink: RouterLinkStub
        },
        mocks: {
          $route: {
            path: '/painel'
          }
        }
      }
    })

    const logoutBtn = wrapper.find('button.text-red-500')
    expect(logoutBtn.text()).toContain('Sair')

    await logoutBtn.trigger('click')
    expect(localStorage.getItem('token')).toBeNull()
    expect(localStorage.getItem('user')).toBeNull()
    expect(mockPush).toHaveBeenCalledWith('/login')
  })
})
