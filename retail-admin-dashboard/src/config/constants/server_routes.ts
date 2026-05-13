export const SERVER_CUSTOMERS = '/api/customers';
export const SERVER_AUTH = '/api/auth';
export const SERVER_CATEGORIES = '/api/categories';
export const SERVER_PRODUCTS = '/api/products';
export const SERVER_ORDERS = '/api/orders';
export const SERVER_REVIEWS = '/api/reviews';

export const SERVER_ROUTES ={
    AUTH: {
        LOGIN: `${SERVER_AUTH}/login`,
        REGISTER: `${SERVER_AUTH}/register`,
    },
    CUSTOMER: {
        LIST: `${SERVER_CUSTOMERS}`,
        DETAILS: `${SERVER_CUSTOMERS}/:id`,
    },
    PRODUCT: {

    }

}