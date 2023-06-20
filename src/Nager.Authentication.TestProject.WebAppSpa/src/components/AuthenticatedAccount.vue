<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar, LocalStorage } from 'quasar'

import { parseToken } from '../helpers/tokenHelper'

const $q = useQuasar()
const Router = useRouter()

const newPassword = ref<string>()

const token = computed(() => {
  return LocalStorage.getItem<string>('token')
})

const tokenInfo = computed(() => {
  if (token.value === null) {
    return
  }

  return parseToken(token.value)
})

async function logout () {
  LocalStorage.remove('token')
  await Router.push('/login')
}

async function changePassword () {
  const response = await fetch('/api/v1/UserAccount/ChangePassword', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token.value}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      password: newPassword.value
    })
  })

  if (response.status !== 204) {
    console.error('cannot change password')
    $q.notify({
      type: 'negative',
      message: response.statusText,
      caption: 'Cannot change password'
    })

    return
  }

  $q.notify({
    type: 'positive',
    message: 'Password changed'
  })
}

</script>

<template>
  <q-btn-dropdown
    icon="account_circle"
    stretch
    flat
    label="Account"
  >
    <div
      v-if="tokenInfo"
      class="text-subtitle1 q-ma-md"
    >
      <div v-if="tokenInfo.firstname || tokenInfo.lastname">
        {{ tokenInfo.firstname }} {{ tokenInfo.lastname }}
      </div>
      <div class="text-weight-medium">
        {{ tokenInfo.emailAddress }}
      </div>
      <small style="line-height:14px; display:block;">
        Token valid at<br>
        {{ tokenInfo.validAt }}
      </small>
      <q-badge
        v-for="role in tokenInfo.roles"
        :key="role"
        outline
        color="primary"
        class="q-mr-sm"
        :label="role"
      />
    </div>
    <div class="row no-wrap q-pa-md">
      <div class="column">
        <q-form>
          <q-input
            v-model="newPassword"
            label="New Password"
            autocomplete="new-password"
            type="password"
            class="q-mb-sm"
            dense
            outlined
          />
          <q-btn
            outline
            label="Change Password"
            @click="changePassword()"
          />
        </q-form>
      </div>

      <q-separator
        vertical
        inset
        class="q-mx-lg"
      />

      <div class="column items-center">
        <q-btn
          v-close-popup
          color="primary"
          label="Logout"
          outline
          @click="logout"
        />
      </div>
    </div>
  </q-btn-dropdown>
</template>
