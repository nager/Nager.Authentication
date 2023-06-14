<script setup lang="ts">
import { computed, ref } from 'vue'
import { useQuasar, LocalStorage } from 'quasar'

const $q = useQuasar()

const responseData = ref('')

const token = computed(() => {
  return LocalStorage.getItem('token')
})

async function getTime () {
  const response = await fetch('/api/v1/Time', {
    headers: {
      Authorization: `Bearer ${token.value}`,
      'Content-Type': 'application/json'
    }
  })

  if (response.status !== 200) {
    $q.notify({
      type: 'negative',
      message: response.statusText,
      caption: 'Cannot get time'
    })
  }

  responseData.value = await response.text()
}
</script>

<template>
  <q-btn
    outline
    label="Get Time"
    @click="getTime"
  />

  <div
    v-if="responseData"
    class="bg-grey-2 q-pa-sm q-mt-sm"
  >
    <pre>{{ responseData }}</pre>
  </div>
</template>
