import axios from "axios";

const API_BASE = "/_cb/api/scaffold";

export const getAllModules = () => {
    return axios.get(`${API_BASE}/modules`);
}

export const generate = (moduleId) => {
    return axios.post(`${API_BASE}/generate`, {
        moduleId
    });
}

export const generateByInstance = (instanceType) => {
    return axios.post(`${API_BASE}/generate/by-instance`, {
        instanceType
    });
}

export const generateByClientId = (clientId) => {
    return axios.post(`${API_BASE}/generate/by-client`, {
        clientId
    });
}