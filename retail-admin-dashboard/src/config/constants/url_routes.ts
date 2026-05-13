export const CUSTOMER_BASE = '/customers';
export const AUTH_BASE= '/auth';
export const ADMIN_BASE = '/admin';

export const ROUTES = {
  HOME: '/',
  AUTH:{
    LOGIN: `${AUTH_BASE}/login`,
    REGISTER: `${AUTH_BASE}/register`,
  },
  CUSTOMER: {
    LIST: `${CUSTOMER_BASE}`,
    DETAILS: `${CUSTOMER_BASE}/:id`,
  },
  
  ADMIN: {
    DASHBOARD: `${ADMIN_BASE}`,
    PRODUCTS: `${ADMIN_BASE}/products`,
    USERS: `${ADMIN_BASE}/users`,
    ORDERS: `${ADMIN_BASE}/orders`,
    CATEGORIES: `${ADMIN_BASE}/categories`,
    REVIEWS: `${ADMIN_BASE}/reviews`,
  }
} as const;
