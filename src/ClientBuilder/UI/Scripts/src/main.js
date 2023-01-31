import {createApp} from "vue";
import App from "./App.vue";
import boostrap from '../../node_modules/bootstrap/dist/js/bootstrap.bundle';

const app = createApp(App);
app.provide('bootstrap', boostrap);
app.mount('#cb');
