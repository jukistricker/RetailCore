import { createBrowserRouter, Navigate } from "react-router-dom";
import { CUSTOMER_BASE, ROUTES, ADMIN_BASE, AUTH_BASE } from "./config/constants/url_routes";
import { ProtectedRoute } from "./routes/ProtectedRoute";
import AuthLayout from "./components/layouts/AuthLayout"; 
import { LoginForm } from "./features/accounts/components/LoginForm";
import MainLayout from "./components/layouts/MainLayout";
import { AccountDetails } from "./features/accounts/components/AccountDetails";
import AdminDashboard from "./features/admin/pages/AdminDashboard";
import {ProductList} from "./features/admin/components/products/ProductList";
import {CustomerList} from "./features/admin/components/customers/CustomerList";
import OrderList from "./features/admin/components/orders/OrderList";
import {CategoryList} from "./features/admin/components/categories/CategoryList";
import ReviewList from "./features/admin/components/reviews/ReviewList";

export const router = createBrowserRouter([
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <MainLayout />,
        children: [
          { path: "", element: <AdminDashboard /> },
          { path: "profile", element: <AccountDetails /> },
          { path: "products", element: <ProductList /> },
          { path: "customers", element: <CustomerList /> },
          { path: "orders", element: <OrderList /> },
          { path: "categories", element: <CategoryList /> },
          { path: "reviews", element: <ReviewList /> },
        ]
      }
    ]
  },
  {
    path: AUTH_BASE, 
    children: [
      {
        element: <AuthLayout />,
        children: [
          { path: "login", element: <LoginForm /> },
        ]
      },
      
    ]
  },

  { path: "*", element: <Navigate to={ROUTES.AUTH.LOGIN} replace /> },
]);
