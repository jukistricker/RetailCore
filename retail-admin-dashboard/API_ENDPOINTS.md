# Admin Dashboard - API Endpoints Specification

**Base URL**: `https://localhost:5016`

All endpoints should support pagination with query parameters: `page` (default: 1) and `pageSize` (default: 10)

## Products API

### GET /products
Fetch all products with pagination

**Query Parameters:**
- `page` (optional): Page number (default: 1)
- `pageSize` (optional): Items per page (default: 10)

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "Product Name",
      "description": "Product description",
      "price": 99999,
      "stock": 100,
      "categoryId": 1,
      "sku": "SKU123",
      "status": "active",
      "createdAt": "2024-05-12T10:00:00Z",
      "updatedAt": "2024-05-12T10:00:00Z"
    }
  ],
  "total": 50
}
```

### POST /products
Create a new product

**Body:**
```json
{
  "name": "New Product",
  "description": "Description",
  "price": 99999,
  "stock": 100,
  "categoryId": 1,
  "sku": "SKU123",
  "image": "https://..."
}
```

**Response:** Return the created product object

### PUT /products/:id
Update an existing product

**Body:** Same as POST body

**Response:** Return the updated product object

### DELETE /products/:id
Delete a product

**Response:**
```json
{
  "success": true,
  "message": "Product deleted"
}
```

---

## Users API

### GET /users
Fetch all users with pagination

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "username": "john_doe",
      "email": "john@example.com",
      "fullName": "John Doe",
      "phone": "0123456789",
      "address": "123 Street, City",
      "role": "customer",
      "status": "active",
      "createdAt": "2024-05-12T10:00:00Z",
      "updatedAt": "2024-05-12T10:00:00Z"
    }
  ],
  "total": 100
}
```

### POST /users
Create a new user

**Body:**
```json
{
  "username": "john_doe",
  "email": "john@example.com",
  "fullName": "John Doe",
  "phone": "0123456789",
  "address": "123 Street, City",
  "role": "customer"
}
```

**Response:** Return the created user object

### PUT /users/:id
Update an existing user

**Body:** Same as POST body with optional status field

**Response:** Return the updated user object

### DELETE /users/:id
Delete a user

**Response:**
```json
{
  "success": true,
  "message": "User deleted"
}
```

---

## Orders API

### GET /orders
Fetch all orders with pagination

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "userId": 1,
      "items": [
        {
          "productId": 1,
          "quantity": 2,
          "price": 99999
        }
      ],
      "totalAmount": 199998,
      "status": "pending",
      "shippingAddress": "123 Street, City",
      "notes": "Special delivery instructions",
      "createdAt": "2024-05-12T10:00:00Z",
      "updatedAt": "2024-05-12T10:00:00Z"
    }
  ],
  "total": 150
}
```

### POST /orders
Create a new order

**Body:**
```json
{
  "userId": 1,
  "items": [
    {
      "productId": 1,
      "quantity": 2,
      "price": 99999
    }
  ],
  "shippingAddress": "123 Street, City",
  "notes": "Special instructions"
}
```

**Response:** Return the created order object

### PUT /orders/:id
Update an existing order

**Body:**
```json
{
  "userId": 1,
  "items": [...],
  "totalAmount": 199998,
  "status": "shipped",
  "shippingAddress": "123 Street, City",
  "notes": "Updated notes"
}
```

**Response:** Return the updated order object

### DELETE /orders/:id
Delete an order

**Response:**
```json
{
  "success": true,
  "message": "Order deleted"
}
```

---

## Categories API

### GET /categories
Fetch all categories with pagination

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "Electronics",
      "description": "Electronic products",
      "image": "https://...",
      "status": "active",
      "createdAt": "2024-05-12T10:00:00Z",
      "updatedAt": "2024-05-12T10:00:00Z"
    }
  ],
  "total": 20
}
```

### POST /categories
Create a new category

**Body:**
```json
{
  "name": "Electronics",
  "description": "Electronic products",
  "image": "https://..."
}
```

**Response:** Return the created category object

### PUT /categories/:id
Update an existing category

**Body:**
```json
{
  "name": "Electronics",
  "description": "Updated description",
  "image": "https://...",
  "status": "active"
}
```

**Response:** Return the updated category object

### DELETE /categories/:id
Delete a category

**Response:**
```json
{
  "success": true,
  "message": "Category deleted"
}
```

---

## Reviews API

### GET /reviews
Fetch all reviews with pagination

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "productId": 1,
      "userId": 1,
      "rating": 5,
      "comment": "Great product!",
      "status": "approved",
      "createdAt": "2024-05-12T10:00:00Z",
      "updatedAt": "2024-05-12T10:00:00Z"
    }
  ],
  "total": 250
}
```

### POST /reviews
Create a new review

**Body:**
```json
{
  "productId": 1,
  "userId": 1,
  "rating": 5,
  "comment": "Great product!"
}
```

**Response:** Return the created review object

### PUT /reviews/:id
Update an existing review

**Body:**
```json
{
  "productId": 1,
  "userId": 1,
  "rating": 4,
  "comment": "Updated comment",
  "status": "approved"
}
```

**Response:** Return the updated review object

### DELETE /reviews/:id
Delete a review

**Response:**
```json
{
  "success": true,
  "message": "Review deleted"
}
```

---

## Error Response Format

All endpoints should return errors in this format:

```json
{
  "error": "Error message",
  "status": 400
}
```

**Common HTTP Status Codes:**
- 200: Success
- 400: Bad Request (validation error)
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Server Error

---

## Pagination Response Format

All GET list endpoints must return:
```json
{
  "items": [...array of items],
  "total": 100
}
```

---

## Notes

1. **Timestamps**: All dates should be in ISO 8601 format (e.g., "2024-05-12T10:00:00Z")

2. **Numeric IDs**: Can be string or number (both are handled)

3. **Status Fields**:
   - Product: "active" | "inactive"
   - User: "active" | "inactive" | "blocked"
   - Order: "pending" | "processing" | "shipped" | "delivered" | "cancelled"
   - Category: "active" | "inactive"
   - Review: "pending" | "approved" | "rejected"

4. **Currency**: Prices are in the smallest unit (cents/units) - no decimal conversion needed on backend

5. **Sorting**: Frontend doesn't require sorting, implement on backend if needed

6. **Filtering**: Basic filtering can be added via query parameters if needed

7. **Authentication**: Ensure endpoints check user is admin before allowing access

---

## Testing the API

Use tools like Postman or cURL to test:

```bash
# Get products
curl -X GET "https://localhost:5016/products?page=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Create product
curl -X POST "https://localhost:5016/products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "name": "New Product",
    "price": 99999,
    "stock": 100,
    "categoryId": 1
  }'

# Update product
curl -X PUT "https://localhost:5016/products/1" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "name": "Updated Name",
    "price": 89999,
    "stock": 50,
    "categoryId": 1
  }'

# Delete product
curl -X DELETE "https://localhost:5016/products/1" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## Frontend Configuration

The frontend axios client automatically uses `https://localhost:5016` as the base URL (from `.env.local`). All requests will be made relative to this base URL.

Example: `axiosClient.get('/products')` → `https://localhost:5016/products`
