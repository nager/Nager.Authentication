<script setup lang="ts">
import { ref } from 'vue'

const firstname = ref('')
const lastname = ref('')

</script>

<template>
  <q-form class="q-gutter-sm">
    <q-input
      v-model="firstname"
      outlined
      label="Firstname"
    />
    <q-input
      v-model="lastname"
      outlined
      label="Lastname"
    />
    <q-btn
      outline
      label="Save"
    />
  </q-form>
</template>
