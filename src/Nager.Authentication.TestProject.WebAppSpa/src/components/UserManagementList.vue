<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { QTableProps, useQuasar, LocalStorage } from 'quasar'

import DefaultDialog from './DefaultDialog.vue'
import EditUserForm from './EditUserForm.vue'

const $q = useQuasar()

const users = ref()
const showEditDialog = ref(false)
const editUser = ref()

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
  const response = await fetch('/api/v1/UserManagement', {
    headers: {
      Authorization: `Bearer ${token.value}`,
      'Content-Type': 'application/json'
    }
  })

  if (response.status !== 200) {
    $q.notify({
      type: 'negative',
      message: 'Request failure',
      caption: response.statusText
    })
  }

  users.value = await response.json()
}

async function removeRow (row) {
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
  }

  await getUsers()
}

async function editRow (row) {
  editUser.value = row
  showEditDialog.value = true
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
  />

  <q-table
    flat
    bordered
    title="Users"
    :rows="users"
    :columns="columns"
    row-key="name"
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
    <EditUserForm />
  </DefaultDialog>
</template>
