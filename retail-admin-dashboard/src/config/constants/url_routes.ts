import { Link } from "react-router-dom";

export const CUSTOMER_BASE = '/customers';
export const AUTH_BASE= '/auth';
export const ADMIN_BASE = '/admin';
export const PRODUCT_BASE = '/products';
export const CATEGORY_BASE = '/categories';

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
  PRODUCT: {
    LIST: `${PRODUCT_BASE}`,
  },
  CATEGORY : {
    LIST: `${CATEGORY_BASE}`,
  },

  
  ADMIN: {
    DASHBOARD: `${ADMIN_BASE}`,
    PRODUCTS: `${ADMIN_BASE}/products`,
    ORDERS: `${ADMIN_BASE}/orders`,
    CATEGORIES: `${ADMIN_BASE}/categories`,
    REVIEWS: `${ADMIN_BASE}/reviews`,
  }
} as const;
