<script setup lang="ts">
import { computed, ref } from 'vue'
import { LocalStorage, useQuasar } from 'quasar'

import { UserAdd } from 'src/models/UserAdd'

const $q = useQuasar()

const emit = defineEmits
<{(e: 'close'): void
}>()

const form = ref<UserAdd>({})

const token = computed(() => {
  return LocalStorage.getItem('token')
})

async function create () {
  const response = await fetch('/api/v1/UserManagement/', {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token.value}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(form.value)
  })

  if (response.status === 201) {
    emit('close')
    return
  }

  const responseText = await response.text()
  $q.notify({
    type: 'negative',
    message: response.statusText,
    caption: responseText

  })
}

</script>

<template>
  <q-form class="q-gutter-sm">
    <q-input
      v-model="form.emailAddress"
      type="email"
      autocomplete="email"
      label="Email Address"
      outlined
      autofocus
    />
    <q-input
      v-model="form.password"
      type="password"
      label="Password"
      outlined
    />
    <q-input
      v-model="form.firstname"
      label="Firstname"
      outlined
    />
    <q-input
      v-model="form.lastname"
      label="Lastname"
      outlined
    />
    <q-btn
      label="Save"
      outline
      @click="create"
    />
  </q-form>
</template>
