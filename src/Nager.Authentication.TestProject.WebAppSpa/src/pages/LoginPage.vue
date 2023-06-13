<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar, LocalStorage } from 'quasar'

const $q = useQuasar()
const Router = useRouter()

const loading = ref(false)
const emailAddress = ref('')
const password = ref('')

async function login () {
  loading.value = true
  try {
    const response = await fetch('/api/v1/Authentication', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        emailAddress: emailAddress.value,
        password: password.value
      })
    })

    if (response.status === 200) {
      const responseData = await response.json()

      LocalStorage.set('token', responseData.token)
      await Router.push('/')
      return
    }

    password.value = ''

    $q.notify({
      type: 'negative',
      message: 'Request failure',
      caption: 'Login not possible'
    })
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <q-layout>
    <q-page-container>
      <q-page class="row bg-primary justify-center items-center">
        <div class="column">
          <div class="text-center text-white q-mb-md">
            <span class="text-h4 text-uppercase">
              NAGER
            </span>
            <div class="text-weight-thin text-uppercase">
              Authentication
            </div>
          </div>
          <div>
            <q-spinner-cube
              v-if="loading"
              color="white"
              size="12em"
            />
            <q-card
              v-else
              square
              bordered
              class="q-pa-lg shadow-1"
            >
              <q-form
                @submit.prevent="login"
              >
                <q-card-section class="q-gutter-md">
                  <q-input
                    v-model="emailAddress"
                    square
                    filled
                    type="email"
                    autocomplete="email"
                    label="Email"
                  />
                  <q-input
                    v-model="password"
                    square
                    filled
                    type="password"
                    autocomplete="current-password"
                    label="Password"
                  />
                </q-card-section>
                <q-card-actions class="q-px-md">
                  <q-btn
                    type="submit"
                    unelevated
                    color="primary"
                    size="lg"
                    class="full-width"
                    label="Login"
                  />
                </q-card-actions>
              </q-form>
            </q-card>
          </div>
          <div class="q-mt-sm text-white text-right">
            <a
              class="text-white"
              href="https://github.com/nager/Nager.Authentication"
            >Nager.Authentication</a>
          </div>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<style scoped>
.q-card {
  width: 360px;
}
</style>
