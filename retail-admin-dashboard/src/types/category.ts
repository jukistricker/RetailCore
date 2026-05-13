export interface Category {
  id: string; // Guid từ BE
  name: string;
  description?: string;
  sortOrder: number;
  createdDate?: string;
  updatedDate?: string;
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
  sortOrder: number;
}

export interface UpdateCategoryRequest {
  id: string;
  name: string;
  description?: string;
  sortOrder: number;
}