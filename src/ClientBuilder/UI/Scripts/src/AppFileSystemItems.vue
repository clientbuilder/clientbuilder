<template>
  <div class="table-responsive">
    <table class="table">
      <thead>
      <tr>
        <th></th>
        <th>Name</th>
        <th>Path</th>
      </tr>
      </thead>
      <tbody>
      <tr v-if="props.folders.length">
        <th colspan="3" class="bg-secondary fw-bold text-primary">
          Folders ({{ props.folders.length }})
        </th>
      </tr>
      <tr v-for="(item, itemIndex) in props.folders" :key="`${key}-folder-${itemIndex}`">
        <td class="fit">
          <span class="text-warning"><FolderIcon/></span>
        </td>
        <td>
          {{ item.name }}
        </td>
        <td>{{ item.path }}</td>
      </tr>
      <tr v-if="props.files.length">
        <th colspan="3" class="bg-secondary fw-bold text-primary">
          Files ({{ props.files.length }})
        </th>
      </tr>
      <tr v-for="(item, itemIndex) in props.files" :key="`${key}-file-${itemIndex}`">
        <td class="fit">
          <span class="text-info"><FileIcon/></span>
        </td>
        <td>
          {{ item.name }}
        </td>
        <td>{{ item.path }}</td>
      </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import FolderIcon from "./svg/FolderIcon.vue";
import FileIcon from "./svg/FileIcon.vue";

const props = defineProps({
  folders: Array,
  files: Array,
  key: String
})
</script>

<style scoped>

</style>