<template>
  <div>
    <AppLayout>
      <div class="card">
        <div class="card-body">
          <div class="my-2 d-flex">
            <div class="d-flex w-100">
              <h4 class="ms-0 me-auto fw-bold my-auto">Modules</h4>
              <div class="ms-auto me-0">
                <button v-for="(option, optionIndex) in clientsOptions"
                        :key="`client-${option.clientId}-${optionIndex}`"
                        type="button"
                        class="btn btn-primary btn-get-trigger ms-auto me-1"
                        @click="generateModulesByClientId(option.clientId)">
                  <span class="d-flex">
                    <span class="my-auto">Generate {{ option.scaffoldTypeName }} ({{ option.count }})</span>
                  </span>
                </button>
                <button v-for="(option, optionIndex) in instancesOptions"
                        :key="`instance-${option.type}-${optionIndex}`"
                        type="button" class="btn btn-primary btn-get-trigger ms-auto me-1"
                        @click="generateModulesByInstanceType(option.type)">
                  <span class="d-flex">
                    <span class="my-auto">Generate {{ option.instanceName }} ({{ option.count }})</span>
                  </span>
                </button>
                <button type="button" class="btn btn-primary mx-0" @click="generateModule(null)">
                  Generate All ({{ modules.length }})
                </button>
              </div>
            </div>
        </div>
      </div>
      </div>
      <div class="accordion accordion-flush" id="modules">
        <div class="accordion-item module-item mb-3" v-for="module in modules" :key="module.id">
          <h2 class="accordion-header" :id="`heading-${module.id}`">
            <button class="accordion-button bg-secondary fw-bold collapsed no-transition"
                    type="button"
                    data-bs-toggle="collapse"
                    :data-bs-target="`#collapse-${module.id}`"
                    aria-expanded="true"
                    :aria-controls="`collapse-${module.id}`">
              {{ module.name }}
              <span class="badge bg-primary ms-2">{{ module.scaffoldTypeName }}</span>
              <span class="badge bg-info ms-2">{{ instanceMap[module.type] }}</span>
              <span class="ms-2">
              <span v-if="module.generated" class="badge bg-success">Generated</span>
              <span v-else class="badge bg-dark">Not Generated</span>
            </span>
            </button>
          </h2>
          <div :id="`collapse-${module.id}`"
               class="accordion-collapse no-transition collapse"
               :aria-labelledby="`heading-${module.id}`"
               data-bs-parent="#modules">
            <div class="accordion-body pt-2">
              <div class="my-2 d-flex mb-3">
                <div class="d-flex w-100">
                  <h6 class="ms-0 me-auto fw-semibold my-auto text-primary">Module's Items</h6>
                  <div class="ms-auto me-0">
                    <button type="button" class="btn btn-primary mx-0" @click="generateModule(module.id)">
                      Generate
                    </button>
                  </div>
                </div>
              </div>
              <AppFileSystemItems :folders="module.folders" :files="module.files" :key="module.id"/>
            </div>
          </div>
        </div>
      </div>
    </AppLayout>
    <AppToastsContainer :toasts="notifications" @toast:hidden="removeNotification" />
  </div>
</template>

<script setup>

import {computed, onMounted, provide, ref} from "vue";
import {generate, generateByClientId, generateByInstance, getAllModules} from "./api";
import AppLayout from "./AppLayout.vue";
import AppFileSystemItems from "./AppFileSystemItems.vue";
import AppToastsContainer from "./AppToastsContainer.vue";
import {getUniqueId, NotificationStates} from "./shared";

const modules = ref([]);
const notifications = ref([]);

const instanceMap = [
    "Undefined", // 0
    "Web", // 1
    "Mobile", // 2
    "Desktop" // 3
];

const showNotification = (notification) => {
  notification.id = getUniqueId(16);
  notifications.value.push(notification);
}

const removeNotification = (notificationId) => {
  const targetNotification = notifications.value.find(x => x.id === notificationId);
  notifications.value.splice(notifications.value.indexOf(targetNotification), 1);
}

provide('notifier', showNotification);

const handleGenerationRequest = (request) => {
  request
      .then((res) => {
        let message = "Selected modules have been generated.";
        if (res.data.errors && res.data.errors.length) {
          message = res.data.errors.join('; ');
        }

        showNotification({
          state: res.data.generationStatus,
          message
        })

        if (res.data.generationStatus === NotificationStates.Success) {
          reloadModules();
        }
      })
      .catch((error) => {
        console.log(error);
        showNotification({
          state: NotificationStates.Error,
          message: error.response.data
        })
      })
}

const generateModule = (moduleId) => {
  handleGenerationRequest(generate(moduleId));
}

const generateModulesByClientId = (clientId) => {
  handleGenerationRequest(generateByClientId(clientId));
}

const generateModulesByInstanceType = (instanceType) => {
  handleGenerationRequest(generateByInstance(instanceType));
}

const clientsOptions = computed(() => {
  if (!modules.value || !modules.value.length) {
    return [];
  }
  
  const uniqueClientIds = Array.from(new Set(modules.value.map(x => x.clientId)));
  return uniqueClientIds.map(x => {
    return {
      clientId: x,
      scaffoldTypeName: modules.value.find(m => m.clientId === x)?.scaffoldTypeName,
      count: modules.value.filter(m => m.clientId === x).length
    }
  })
})

const instancesOptions = computed(() => {
  if (!modules.value || !modules.value.length) {
    return [];
  }

  const uniqueInstances = Array.from(new Set(modules.value.map(x => x.type)));
  return uniqueInstances.map(x => {
    return {
      type: x,
      instanceName: instanceMap[x],
      count: modules.value.filter(m => m.type === x).length
    }
  })
})

function reloadModules() {
  getAllModules().then(res => {
    modules.value = res.data;
  })
}

onMounted(() => {
  reloadModules();
})

</script>