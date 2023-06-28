<script setup lang="ts">
import { ref, computed } from 'vue'
import type { PropType } from 'vue'
import { LocalStorage, useQuasar } from 'quasar'

import { User } from 'src/models/User'

const $q = useQuasar()

const props = defineProps({
  user: {
    type: Object as PropType<User>,
    required: true
  }
})

const emit = defineEmits
<{(e: 'roleChanged'): void
}>()

const newRoleName = ref<string>()

const token = computed(() => {
  return LocalStorage.getItem('token')
})

async function addRoleToUser () {
  const response = await fetch(`/api/v1/UserManagement/${props.user.id}/Role`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token.value}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      roleName: newRoleName.value
    })
  })

  if (response.status === 204) {
    emit('roleChanged')
    return
  }

  const responseText = await response.text()
  $q.notify({
    type: 'negative',
    message: response.statusText,
    caption: responseText

  })
}

async function removeRoleFromUser (roleName : string) {
  const response = await fetch(`/api/v1/UserManagement/${props.user.id}/Role`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token.value}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      roleName
    })
  })

  if (response.status === 204) {
    emit('roleChanged')
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
  <div class="text-subtitle1 q-mt-xl">
    Roles
  </div>
  <q-card
    flat
    bordered
  >
    <q-card-section class="q-pa-none">
      <q-list
        v-if="user.roles && user.roles.length > 0"
        separator
      >
        <q-item
          v-for="role in user.roles"
          :key="role"
        >
          <q-item-section>
            <q-item-label>{{ role }}</q-item-label>
          </q-item-section>

          <q-item-section side>
            <q-btn
              flat
              outline
              icon="delete"
              @click="removeRoleFromUser(role)"
            />
          </q-item-section>
        </q-item>
      </q-list>
    </q-card-section>
  </q-card>

  <div class="text-subtitle2 q-mt-md">
    Add Role
  </div>
  <q-input
    v-model="newRoleName"
    label="Role"
    outlined
  />
  <q-btn
    class="q-mt-sm"
    label="Add"
    outline
    @click="addRoleToUser()"
  />
</template>
