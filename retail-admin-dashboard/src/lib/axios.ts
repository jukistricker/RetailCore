import axios from 'axios';

const apiBaseURL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5016';

const axiosClient = axios.create({
  baseURL: apiBaseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosClient.interceptors.response.use(
  (response) => response.data, 
  (error) => {
    return Promise.reject(error.response?.data || 'Network Error');
  }
);

export default axiosClient;
