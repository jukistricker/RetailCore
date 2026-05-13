export interface OrderItem {
  productId: string | number;
  quantity: number;
  price: number;
}

export interface Order {
  id: string | number;
  userId: string | number;
  items: OrderItem[];
  totalAmount: number;
  status?: 'pending' | 'processing' | 'shipped' | 'delivered' | 'cancelled';
  shippingAddress?: string;
  notes?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateOrderRequest {
  userId: string | number;
  items: OrderItem[];
  shippingAddress?: string;
  notes?: string;
}

export interface UpdateOrderRequest extends CreateOrderRequest {
  id: string | number;
  status?: 'pending' | 'processing' | 'shipped' | 'delivered' | 'cancelled';
}
