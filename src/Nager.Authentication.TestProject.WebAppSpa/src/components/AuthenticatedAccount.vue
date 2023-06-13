<script setup lang="ts">
import { useRouter } from 'vue-router'
import { LocalStorage } from 'quasar'

const Router = useRouter()

async function logout () {
  LocalStorage.remove('token')
  await Router.push('/login')
}
</script>

<template>
  <q-btn
    stretch
    flat
    icon="account_circle"
    @click="logout"
  />
</template>
