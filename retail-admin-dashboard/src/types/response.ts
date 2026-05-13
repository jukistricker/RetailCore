export interface ApiResponse<T> {
  statusCode: number;
  isSuccess: boolean;
  value: T;
  errors: any;
}

export interface PagingResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}