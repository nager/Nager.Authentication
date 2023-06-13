<script setup lang="ts">
import { computed } from 'vue'
import { useQuasar } from 'quasar'

const $q = useQuasar()

const props = defineProps({
  dialogVisible: {
    type: Boolean,
    required: true
  },
  title: {
    type: String,
    required: true
  },
  padding: {
    type: Boolean,
    default: true
  },
  wideDialog: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits<
{(e: 'hide', value: boolean): void}>()

const position = computed(() => {
  if ($q.platform.is.mobile) {
    return 'standard'
  }

  return 'right'
})

const dialogClass = computed(() => {
  if ($q.platform.is.mobile) {
    return 'mobileDialog'
  }

  if (props.wideDialog) {
    return 'desktopDialogWide'
  }

  return 'desktopDialog'
})

const showDialog = computed({
  get: () => props.dialogVisible,
  set: value => emit('hide', value)
})

</script>

<template>
  <q-dialog
    v-model="showDialog"
    :position="position"
    maximized
  >
    <q-layout
      view="Lhh lpR fff"
      container
      :class="dialogClass"
    >
      <q-header class="bg-primary">
        <q-toolbar>
          <q-toolbar-title>{{ title }}</q-toolbar-title>
          <q-btn
            v-close-popup
            flat
            dense
            icon="close"
          />
        </q-toolbar>
      </q-header>

      <q-page-container>
        <q-page :padding="padding">
          <slot />
        </q-page>
      </q-page-container>
    </q-layout>
  </q-dialog>
</template>

<style scoped>

.desktopDialog {
  width: 40vw;
  max-width: 80vw;
  background-color: #fff;
}

.desktopDialogWide {
  width: 85vw;
  max-width: 85vw;
  background-color: #fff;
}

.mobileDialog {
  width: 100vw;
  background-color: #fff;
}

</style>
