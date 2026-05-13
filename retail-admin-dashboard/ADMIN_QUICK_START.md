# Admin Dashboard - Quick Start Guide

## Getting Started

### 1. Environment Setup
The API base URL is already configured in `.env.local`:
```
VITE_API_BASE_URL=https://localhost:5016
```

If you need to change it, edit this file and restart the dev server.

### 2. Access the Admin Dashboard

After logging in as an admin user, you'll see a new "Admin Dashboard" section in the sidebar with these options:
- **Admin Home**: Overview dashboard with statistics
- **Products**: Manage your product catalog
- **Users**: Manage system users and their roles
- **Orders**: Track and manage customer orders
- **Categories**: Organize products into categories
- **Reviews**: Moderate customer reviews and ratings

### 3. Using the Management Modules

Each management module follows the same pattern:

#### List View
- View all items in a table format
- See pagination controls at the bottom
- Search/filter options (if implemented on backend)

#### Add New Item (Green "+" Button)
1. Click the "Thêm [Item Type]" button
2. Fill in the form fields
3. Click "Lưu" to submit
4. Toast notification confirms success/error

#### Edit Item (Pencil Icon)
1. Click the pencil icon on any row
2. Update the information
3. Click "Lưu" to save changes
4. Item updates in the list

#### View Details (Eye Icon)
1. Click the eye icon to see full information
2. Read-only view of all item details
3. Click "Đóng" to close the dialog

#### Delete Item (Trash Icon)
1. Click the trash icon
2. Confirm deletion in the popup dialog
3. Item is removed from the list

### 4. Field Requirements

#### Products
- **Name**: Required
- **Price**: Required, displayed in VND format
- **Stock**: Required, quantity available
- **Category**: Required, ID or name
- **Description**: Optional
- **SKU**: Optional, unique product code

#### Users
- **Username**: Required
- **Email**: Required
- **Full Name**: Required
- **Role**: Required (customer/staff/admin)
- **Phone**: Optional
- **Address**: Optional

#### Orders
- **User ID**: Required, which user placed the order
- **Items**: Product list for the order
- **Total Amount**: Total order value
- **Status**: pending → processing → shipped → delivered → cancelled
- **Shipping Address**: Optional
- **Notes**: Optional special instructions

#### Categories
- **Name**: Required
- **Description**: Optional
- **Image URL**: Optional, link to category image
- **Status**: active/inactive

#### Reviews
- **Product ID**: Required
- **User ID**: Required
- **Rating**: 1-5 stars
- **Comment**: Optional review text
- **Status**: pending → approved → rejected (moderation)

### 5. API Response Format

Your backend should return data in this format:

```json
// List Endpoint Response
{
  "items": [
    {
      "id": 1,
      "name": "Product Name",
      ...other fields
    }
  ],
  "total": 50
}

// Single Item Response
{
  "id": 1,
  "name": "Product Name",
  ...other fields
}
```

### 6. Pagination

- Default page size: 10 items per page
- Automatically fetches page data when you navigate
- Shows "No items found" when list is empty

### 7. Error Handling

- **API Errors**: Displayed in red alert box at top of list
- **Validation Errors**: Toast notification in top-right
- **Confirmation**: Required before deletion
- **Loading State**: Spinner shows while fetching data

### 8. Theme Support

- The admin dashboard fully supports light/dark mode
- Click the sun/moon icon in the top-right navbar to toggle
- Theme preference is saved to localStorage
- All dialogs respect the current theme

### 9. Keyboard & Accessibility

- All form buttons are keyboard accessible
- Dialogs can be closed with Escape key
- Tab navigation supported throughout
- ARIA labels included for screen readers

### 10. Troubleshooting

**Admin menu not showing?**
- Ensure user is logged in with admin role (`role: 'Admin'`)
- Check user object in Redux store (DevTools)

**API calls failing?**
- Verify backend is running at `https://localhost:5016`
- Check .env.local file for correct URL
- Check browser console for error details
- Ensure CORS is enabled on your backend

**Dialogs not appearing?**
- Clear browser cache and reload
- Check for JavaScript errors in console
- Verify BaseDialog component is properly imported

**Toast notifications not showing?**
- Check if react-hot-toast is properly initialized in main App
- Look for toast container in DOM

## Backend Implementation Checklist

- [ ] Create REST endpoints: `/products`, `/users`, `/orders`, `/categories`, `/reviews`
- [ ] Support CRUD operations (GET, POST, PUT, DELETE)
- [ ] Implement pagination with `page` and `pageSize` parameters
- [ ] Add authentication/authorization checks
- [ ] Validate input data on backend
- [ ] Return data in expected format
- [ ] Handle errors gracefully
- [ ] Enable CORS for frontend domain
- [ ] Test all endpoints before going live

## Common Issues & Solutions

### Issue: "Failed to fetch" errors
**Solution**: 
1. Check if API server is running
2. Verify API URL in .env.local
3. Check CORS settings on backend
4. Look at Network tab in DevTools

### Issue: Form not submitting
**Solution**:
1. Check if all required fields are filled
2. Look for validation error messages
3. Check console for JavaScript errors
4. Verify field names match backend expectations

### Issue: Items not updating after edit
**Solution**:
1. Clear Redux store and refresh page
2. Check if PUT request is working in backend
3. Verify response format matches expectations
4. Check console for any errors

### Issue: Dark mode not working
**Solution**:
1. Clear localStorage
2. Toggle theme switcher again
3. Check if data-bs-theme attribute is set on html element

## Database Schema Example

```sql
-- Products
CREATE TABLE products (
  id INT PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  description TEXT,
  price DECIMAL(10,2) NOT NULL,
  stock INT NOT NULL,
  category_id INT NOT NULL,
  sku VARCHAR(100),
  status VARCHAR(50) DEFAULT 'active',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Similar structure for users, orders, categories, reviews
```

## Support & Resources

- Check ADMIN_IMPLEMENTATION.md for detailed architecture
- Review example API calls in Redux thunks
- Look at component files for UI implementation details
- Use browser DevTools to debug state and network calls

Happy managing! 🚀
