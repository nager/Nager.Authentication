import { boot } from 'quasar/wrappers'
import { LocalStorage } from 'quasar'

function isLoggedIn () : boolean {
  const token = LocalStorage.getItem<string>('token')
  if (token === null) {
    return false
  }

  return true
}

export default boot(({ router }) => {
  router.beforeEach(async (to, from, next) => {
    if (!isLoggedIn()) {
      if (to.path !== '/login') {
        router.push('/login')
      }
      next()
      return
    }

    if (isLoggedIn() && to.path === '/login') {
      router.push('/')
      next()
      return
    }

    next()
  })
})
