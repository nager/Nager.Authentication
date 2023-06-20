import { date } from 'quasar'
import { TokenInfo } from 'src/models/TokenInfo'

export function parseToken (token: string) : TokenInfo | undefined {
  const parts = token.split('.')

  if (parts.length !== 3) {
    return
  }

  const tokenObject = JSON.parse(atob(parts[1]))

  const validAt = date.formatDate(new Date(tokenObject.exp * 1000), 'YYYY-MM-DDTHH:mm')

  const roles = []
  const roleKey = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
  if (Object.prototype.hasOwnProperty.call(tokenObject, roleKey)) {
    if (Array.isArray(tokenObject[roleKey])) {
      roles.push(...tokenObject[roleKey])
    } else {
      roles.push(tokenObject[roleKey])
    }

    console.log(tokenObject[roleKey])
  }

  return {
    emailAddress: tokenObject.unique_name,
    firstname: tokenObject.given_name,
    lastname: tokenObject.family_name,
    roles,
    validAt
  }
}
