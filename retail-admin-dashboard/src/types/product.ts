export interface Product {
  id: string;
  name: string;
  slug: string;
  description?: string;
  price: number;
  stock: number;
  categoryName?: string; 
  categoryId?: string;
  thumbnailUrl?: string;
  images?: { id: string; url: string; altText?: string }[];
  isFeatured?: boolean;
  isActive?: boolean;
}

export interface ProductCreateRequest {
  name: string;
  description?: string;
  slug: string;
  price: number;
  stock: number;
  categoryId: string;
  thumbnailUrl?: string;
  isFeatured: boolean;
  images?: File[]; 
  attributes?: string;
}

export interface ProductUpdateRequest extends ProductCreateRequest {
  id: string;
  isActive: boolean;
}