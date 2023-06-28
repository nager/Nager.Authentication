<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { QTableProps, useQuasar, LocalStorage } from 'quasar'

import { User } from 'src/models/User'

import DefaultDialog from './DefaultDialog.vue'
import UserEditForm from './UserEditForm.vue'
import UserRoleManagement from './UserRoleManagement.vue'
import UserAddForm from './UserAddForm.vue'

const $q = useQuasar()

const loading = ref<boolean>()
const users = ref<User[]>()
const editUser = ref<User>()
const showAddDialog = ref(false)
const showEditDialog = ref(false)

const columns : QTableProps['columns'] = [
  {
    name: 'emailAddress',
    required: true,
    label: 'Email Address',
    align: 'left',
    field: row => row.emailAddress,
    format: val => `${val}`
  },
  {
    name: 'roles',
    required: true,
    label: 'Roles',
    align: 'left',
    field: row => row.roles,
    format: val => `${val}`
  },
  {
    name: 'firstname',
    required: true,
    label: 'Firstname',
    align: 'left',
    field: row => row.firstname,
    format: val => `${val}`
  },
  {
    name: 'lastname',
    required: true,
    label: 'Lastname',
    align: 'left',
    field: row => row.lastname,
    format: val => `${val}`
  },
  {
    name: 'actions',
    required: true,
    label: 'Actions',
    align: 'left',
    field: 'actions'
  }
]

const token = computed(() => {
  return LocalStorage.getItem('token')
})

async function getUsers () {
  try {
    loading.value = true

    const response = await fetch('/api/v1/UserManagement', {
      headers: {
        Authorization: `Bearer ${token.value}`,
        'Content-Type': 'application/json'
      }
    })

    if (response.status !== 200) {
      $q.notify({
        type: 'negative',
        message: response.statusText,
        caption: 'Cannot load users'
      })
    }

    users.value = await response.json() as User[]
  } finally {
    loading.value = false
  }
}

async function removeRow (row : User) {
  const response = await fetch(`/api/v1/UserManagement/${row.id}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token.value}`,
      'Content-Type': 'application/json'
    }
  })

  if (response.status !== 204) {
    $q.notify({
      type: 'negative',
      message: 'Request failure',
      caption: response.statusText
    })

    return
  }

  await getUsers()
}

async function editRow (row : User) {
  editUser.value = row
  showEditDialog.value = true
}

async function editDone () {
  await getUsers()
  editUser.value = users.value?.find(o => o.id === editUser.value?.id)
}

async function addDone () {
  showAddDialog.value = false
  await getUsers()
}

onMounted(async () => {
  await getUsers()
})

</script>

<template>
  <q-btn
    class="q-mb-sm"
    outline
    label="Add User"
    @click="showAddDialog = true"
  />

  <q-table
    flat
    bordered
    title="Users"
    :rows="users"
    :columns="columns"
    row-key="name"
    :loading="loading"
  >
    <template #body-cell-actions="props">
      <q-td :props="props">
        <q-btn
          dense
          flat
          color="grey"
          icon="edit"
          @click="editRow(props.row)"
        />

        <q-btn
          dense
          flat
          color="grey"
          icon="delete"
          @click="removeRow(props.row)"
        />
      </q-td>
    </template>
  </q-table>

  <DefaultDialog
    :dialog-visible="showEditDialog"
    title="Edit User"
    @hide="showEditDialog = false"
  >
    <UserEditForm
      v-if="editUser"
      :user="editUser"
      @close="editDone()"
    />

    <UserRoleManagement
      v-if="editUser"
      class="q-mt-xl"
      :user="editUser"
      @role-changed="editDone()"
    />
  </DefaultDialog>

  <DefaultDialog
    :dialog-visible="showAddDialog"
    title="Add User"
    @hide="showAddDialog = false"
  >
    <UserAddForm @close="addDone()" />
  </DefaultDialog>
</template>
