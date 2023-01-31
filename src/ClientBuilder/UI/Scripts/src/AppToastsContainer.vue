<template>
  <div class="toast-container position-fixed bottom-0 end-0 p-3" id="toastPlacement">
    <div class="toast" v-for="toast in props.toasts" :ref="(el) => toast.el = el" :key="`toast-${toast.id}`">
      <div :class="`toast-header ${toastStateClassesMap[toast.state]}`">
        <strong class="me-auto">Client Builder</strong>
        <small>{{ toastStateTypeMap[toast.state] }}</small>
      </div>
      <div class="toast-body">
        {{ toast.message }}
      </div>
    </div>
  </div>
</template>

<script setup>
import {inject, onUpdated, watch} from "vue";

const props = defineProps({
  toasts: Array
})

const emits = defineEmits(['toast:hidden'])

const toastStateClassesMap = [
  "bg-danger text-white",
  "bg-success text-white",
  "bg-warning text-white"
];

const toastStateTypeMap = [
  "error",
  "success",
  "warning"  
];

const bootstrap = inject('bootstrap');

onUpdated(() => {
  const hiddenToasts = props.toasts.filter(x => !x.visible);
  hiddenToasts.forEach(function (toast) {
    try {
      const toastInstance = bootstrap.Toast.getOrCreateInstance(toast.el);
      toastInstance.show();
      toast.visible = true;
      toast.el.addEventListener("hidden.bs.toast", () => {
        emits('toast:hidden', toast.id);
      });
    }
    catch (e) {
      console.log(e);
    }
  });
});

</script>

<style scoped>

</style>