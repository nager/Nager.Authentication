<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import type { PropType } from 'vue'
import { LocalStorage, useQuasar } from 'quasar'

import { User } from 'src/models/User'
import { UserEdit } from 'src/models/UserEdit'

const $q = useQuasar()

const props = defineProps({
  user: {
    type: Object as PropType<User>,
    required: true
  }
})

const emit = defineEmits
<{(e: 'close'): void
}>()

const form = ref<UserEdit>({})

onMounted(() => {
  if (!form.value) {
    return
  }

  form.value.firstname = props.user.firstname
  form.value.lastname = props.user.lastname
})

const token = computed(() => {
  return LocalStorage.getItem('token')
})

async function updateUser () {
  const response = await fetch(`/api/v1/UserManagement/${props.user.id}`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token.value}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(form.value)
  })

  if (response.status === 204) {
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
  <q-form
    v-if="form"
    class="q-gutter-sm"
  >
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
      @click="updateUser()"
    />
  </q-form>
</template>
