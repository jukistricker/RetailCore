import { createBrowserRouter, Navigate } from "react-router-dom";
import { CUSTOMER_BASE, ROUTES, ADMIN_BASE, AUTH_BASE } from "./config/constants/url_routes";
import DashboardPage from "./features/dashboard/components/dashboard";
import { ProtectedRoute } from "./routes/ProtectedRoute";
import AuthLayout from "./components/layouts/AuthLayout"; 
import { LoginForm } from "./features/accounts/components/LoginForm";
import MainLayout from "./components/layouts/MainLayout";
import { RegisterForm } from "./features/accounts/components/RegisterForm";
import { AccountDetails } from "./features/accounts/components/AccountDetails";
import { AccountList } from "./features/accounts/components/AccountList";
import AdminDashboard from "./features/admin/pages/AdminDashboard";
import ProductList from "./features/admin/components/products/ProductList";
import UserList from "./features/admin/components/users/UserList";
import OrderList from "./features/admin/components/orders/OrderList";
import CategoryList from "./features/admin/components/categories/CategoryList";
import ReviewList from "./features/admin/components/reviews/ReviewList";

export const router = createBrowserRouter([
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <MainLayout />,
        children: [
          { path: "", element: <AdminDashboard /> },
          { path: "products", element: <ProductList /> },
          { path: "users", element: <UserList /> },
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
          { path: "create", element: <RegisterForm /> },
        ]
      },
      
      {
        element: <ProtectedRoute />,
        children: [
          {
            element: <MainLayout />,
            children: [
              { index: true, element: <AccountList /> }, 
              { path: ":id", element: <AccountDetails /> },
            ]
          }
        ]
      }
    ]
  },

  { path: "*", element: <Navigate to={ROUTES.AUTH.LOGIN} replace /> },
]);
