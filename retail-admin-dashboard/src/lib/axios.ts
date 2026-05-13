import axios from 'axios';

export const apiBaseURL = import.meta.env.VITE_API_BASE_URL;

const axiosClient = axios.create({
  baseURL: apiBaseURL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

axiosClient.interceptors.response.use(
  (response) => {
    if (response.data && response.data.hasOwnProperty('value')) {
      return response.data.value; 
    }
    return response;
  },
  (error) => {
    let errorMessage = "Đã có lỗi xảy ra";

    if (error.response && error.response.data) {
      const data = error.response.data;
      
      if (Array.isArray(data.errors) && data.errors.length > 0) {
        errorMessage = data.errors[0].message;
      } 
      else if (typeof data.errors === 'string') {
        errorMessage = data.errors;
      }
    }

    return Promise.reject(new Error(errorMessage));
  }
);

export default axiosClient;
