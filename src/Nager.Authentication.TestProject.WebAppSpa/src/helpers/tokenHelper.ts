import { date } from 'quasar'
import { TokenInfo } from 'src/models/TokenInfo'

export function parseToken (token: string) : TokenInfo | undefined {
  const parts = token.split('.')

  if (parts.length !== 3) {
    return
  }

  const tokenObject = JSON.parse(atob(parts[1]))

  const validAt = date.formatDate(new Date(tokenObject.exp * 1000), 'YYYY-MM-DDTHH:mm')

  return {
    emailAddress: tokenObject.unique_name,
    firstname: tokenObject.given_name,
    lastname: tokenObject.family_name,
    roles: '',
    validAt
  }
}
