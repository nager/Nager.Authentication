<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { LocalStorage } from 'quasar'

const Router = useRouter()

const newPassword = ref<string>()

const token = computed(() => {
  return LocalStorage.getItem('token')
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
  }
}

</script>

<template>
  <q-btn-dropdown
    icon="account_circle"
    stretch
    flat
    label="Account"
  >
    <div class="text-subtitle1 q-ma-md">
      John Doe
    </div>
    <div class="row no-wrap q-pa-md">
      <div class="column">
        <q-input
          v-model="newPassword"
          label="New Password"
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
