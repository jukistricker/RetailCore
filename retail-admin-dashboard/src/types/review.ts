export interface Review {
  id: string | number;
  productId: string | number;
  userId: string | number;
  rating: number;
  comment?: string;
  status?: 'pending' | 'approved' | 'rejected';
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateReviewRequest {
  productId: string | number;
  userId: string | number;
  rating: number;
  comment?: string;
}

export interface UpdateReviewRequest extends CreateReviewRequest {
  id: string | number;
  status?: 'pending' | 'approved' | 'rejected';
}
