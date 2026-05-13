import { useDispatch, useSelector } from 'react-redux';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { ROUTES } from '../config/constants/url_routes';
import { AppDispatch, RootState } from '../store/store';
import { currentDetails, refreshToken } from '../store/thunks/accountThunk';
import { useEffect, useRef } from 'react';

export const ProtectedRoute = () => {
  const dispatch = useDispatch<AppDispatch>();
  const location = useLocation();
  const { isAuthenticated, isInitialized } = useSelector((state: RootState) => state.account);
  
  const refreshIntervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    if (!isInitialized) {
      dispatch(currentDetails());
    }
  }, [isInitialized, dispatch]);

  useEffect(() => {
    if (isAuthenticated && isInitialized) {
      if (refreshIntervalRef.current) clearInterval(refreshIntervalRef.current);

      refreshIntervalRef.current = setInterval(() => {
        console.log("Auto-refreshing session...");
        dispatch(refreshToken());
      }, 10 * 60 * 1000); 
    }

    return () => {
      if (refreshIntervalRef.current) {
        clearInterval(refreshIntervalRef.current);
      }
    };
  }, [isAuthenticated, isInitialized, dispatch]);

  if (!isInitialized) return null; 

  return isAuthenticated 
    ? <Outlet /> 
    : <Navigate to={ROUTES.AUTH.LOGIN} state={{ from: location }} replace />;
};