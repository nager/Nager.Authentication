<script setup lang="ts">
import { computed, ref } from 'vue'
import { useQuasar, LocalStorage } from 'quasar'
import { TimeInfo } from 'src/models/TimeInfo'

const $q = useQuasar()

const responseData = ref<TimeInfo>()

const token = computed(() => {
  return LocalStorage.getItem('token')
})

async function getUtcTime () {
  const response = await fetch('/api/v1/UtcTime', {
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

  responseData.value = await response.json()
}
</script>

<template>
  <q-btn
    outline
    label="Get Utc Time"
    @click="getUtcTime"
  />

  <div
    v-if="responseData"
    class="bg-grey-2 q-pa-sm q-mt-sm"
  >
    Hour: {{ responseData.hour }}<br>
    Minute: {{ responseData.minute }}<br>
    Second: {{ responseData.second }}
  </div>
</template>
