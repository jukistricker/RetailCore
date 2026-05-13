import React, { useEffect, useMemo } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../../../store/store';
import { fetchAccountById } from '../../../store/thunks/accountThunk';

const DashboardPage: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { user } = useSelector((state: RootState) => state.account || {});

  useEffect(() => {
    if (user?.id) {
      dispatch(
        fetchAccountById(user.id)
      );
    }
  }, [dispatch, user?.id]);

  
};

export default DashboardPage;