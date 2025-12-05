# Buildify API Endpoints - Examples

Base URL: `https://localhost:7101/api`

---

## 📦 Products Endpoints

### 1. Get All Products
**GET** `/api/products`

**Authorization:** None required

**Response:**
```json
[
  {
    "id": 1,
    "name": "Electric Drill",
    "description": "Powerful 18V cordless drill",
    "price": 129.99,
    "stock": 45,
    "categoryId": 1,
    "categoryName": "Power Tools",
    "imageUrl": "/images/products/drill.jpg",
    "createdDate": "2024-12-01T10:00:00Z"
  },
  {
    "id": 2,
    "name": "Hammer",
    "description": "Steel claw hammer 16oz",
    "price": 19.99,
    "stock": 120,
    "categoryId": 2,
    "categoryName": "Hand Tools",
    "imageUrl": "/images/products/hammer.jpg",
    "createdDate": "2024-12-01T10:15:00Z"
  }
]
```

---

### 2. Get Single Product
**GET** `/api/products/{id}`

**Example:** `/api/products/1`

**Authorization:** None required

**Response:**
```json
{
  "id": 1,
  "name": "Electric Drill",
  "description": "Powerful 18V cordless drill",
  "price": 129.99,
  "stock": 45,
  "categoryId": 1,
  "categoryName": "Power Tools",
  "imageUrl": "/images/products/drill.jpg",
  "createdDate": "2024-12-01T10:00:00Z"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Product not found"
}
```

---

### 3. Get Products by Category
**GET** `/api/products/category/{categoryId}`

**Example:** `/api/products/category/1`

**Authorization:** None required

**Response:**
```json
[
  {
    "id": 1,
    "name": "Electric Drill",
    "description": "Powerful 18V cordless drill",
    "price": 129.99,
    "stock": 45,
    "categoryId": 1,
    "categoryName": "Power Tools",
    "imageUrl": "/images/products/drill.jpg",
    "createdDate": "2024-12-01T10:00:00Z"
  },
  {
    "id": 3,
    "name": "Circular Saw",
    "description": "7-1/4 inch circular saw",
    "price": 89.99,
    "stock": 30,
    "categoryId": 1,
    "categoryName": "Power Tools",
    "imageUrl": "/images/products/saw.jpg",
    "createdDate": "2024-12-01T10:30:00Z"
  }
]
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Category not found"
}
```

---

### 4. Create Product (Admin Only)
**POST** `/api/products`

**Authorization:** Bearer Token (Admin role required)

**Request Body:**
```json
{
  "name": "Cordless Impact Driver",
  "description": "20V MAX XR brushless impact driver",
  "price": 149.99,
  "stock": 25,
  "categoryId": 1,
  "imageUrl": "/images/products/impact-driver.jpg"
}
```

**Response (201 Created):**
```json
{
  "id": 15,
  "name": "Cordless Impact Driver",
  "description": "20V MAX XR brushless impact driver",
  "price": 149.99,
  "stock": 25,
  "categoryId": 1,
  "categoryName": "Power Tools",
  "imageUrl": "/images/products/impact-driver.jpg",
  "createdDate": "2024-12-05T14:30:00Z"
}
```

**Error Response (400):**
```json
{
  "statusCode": 400,
  "message": "Invalid category ID"
}
```

---

### 5. Update Product (Admin Only)
**PUT** `/api/products/{id}`

**Example:** `/api/products/15`

**Authorization:** Bearer Token (Admin role required)

**Request Body:**
```json
{
  "name": "Cordless Impact Driver Pro",
  "description": "20V MAX XR brushless impact driver with battery",
  "price": 179.99,
  "stock": 30,
  "categoryId": 1,
  "imageUrl": "/images/products/impact-driver-pro.jpg"
}
```

**Response (200 OK):**
```json
{
  "id": 15,
  "name": "Cordless Impact Driver Pro",
  "description": "20V MAX XR brushless impact driver with battery",
  "price": 179.99,
  "stock": 30,
  "categoryId": 1,
  "categoryName": "Power Tools",
  "imageUrl": "/images/products/impact-driver-pro.jpg",
  "createdDate": "2024-12-05T14:30:00Z"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Product not found"
}
```

---

### 6. Delete Product (Admin Only)
**DELETE** `/api/products/{id}`

**Example:** `/api/products/15`

**Authorization:** Bearer Token (Admin role required)

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Product deleted successfully"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Product not found"
}
```

---

## 📂 Categories Endpoints

### 1. Get All Categories
**GET** `/api/categories`

**Authorization:** None required

**Response:**
```json
[
  {
    "id": 1,
    "name": "Power Tools",
    "description": "Electric and battery-powered tools"
  },
  {
    "id": 2,
    "name": "Hand Tools",
    "description": "Manual tools for construction"
  },
  {
    "id": 3,
    "name": "Building Materials",
    "description": "Lumber, cement, and construction materials"
  }
]
```

---

### 2. Get Single Category
**GET** `/api/categories/{id}`

**Example:** `/api/categories/1`

**Authorization:** None required

**Response:**
```json
{
  "id": 1,
  "name": "Power Tools",
  "description": "Electric and battery-powered tools"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Category not found"
}
```

---

### 3. Create Category (Admin Only)
**POST** `/api/categories`

**Authorization:** Bearer Token (Admin role required)

**Request Body:**
```json
{
  "name": "Safety Equipment",
  "description": "Protective gear and safety equipment"
}
```

**Response (201 Created):**
```json
{
  "id": 8,
  "name": "Safety Equipment",
  "description": "Protective gear and safety equipment"
}
```

**Error Response (400):**
```json
{
  "statusCode": 400,
  "message": "Category with this name already exists"
}
```

---

### 4. Update Category (Admin Only)
**PUT** `/api/categories/{id}`

**Example:** `/api/categories/8`

**Authorization:** Bearer Token (Admin role required)

**Request Body:**
```json
{
  "name": "Safety & Protection",
  "description": "Complete protective gear and safety equipment"
}
```

**Response (200 OK):**
```json
{
  "id": 8,
  "name": "Safety & Protection",
  "description": "Complete protective gear and safety equipment"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Category not found"
}
```

---

### 5. Delete Category (Admin Only)
**DELETE** `/api/categories/{id}`

**Example:** `/api/categories/8`

**Authorization:** Bearer Token (Admin role required)

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Category deleted successfully"
}
```

**Error Response (400):**
```json
{
  "statusCode": 400,
  "message": "Cannot delete category with existing products"
}
```

---

## 🛒 Cart Endpoints

**Note:** All cart endpoints require authentication (Bearer Token)

### 1. Get Current User's Cart
**GET** `/api/cart`

**Authorization:** Bearer Token (Required)

**Response:**
```json
{
  "id": 5,
  "userId": "abc123-def456-ghi789",
  "createdDate": "2024-12-03T09:00:00Z",
  "updatedDate": "2024-12-05T11:30:00Z",
  "items": [
    {
      "id": 12,
      "productId": 1,
      "productName": "Electric Drill",
      "productImageUrl": "/images/products/drill.jpg",
      "quantity": 2,
      "price": 129.99,
      "subtotal": 259.98
    },
    {
      "id": 13,
      "productId": 2,
      "productName": "Hammer",
      "productImageUrl": "/images/products/hammer.jpg",
      "quantity": 1,
      "price": 19.99,
      "subtotal": 19.99
    }
  ],
  "totalPrice": 279.97,
  "totalItems": 3
}
```

**Response (Empty Cart):**
```json
{
  "userId": "abc123-def456-ghi789",
  "createdDate": "2024-12-05T14:45:00Z",
  "items": [],
  "totalPrice": 0,
  "totalItems": 0
}
```

---

### 2. Add Product to Cart
**POST** `/api/cart`

**Authorization:** Bearer Token (Required)

**Request Body:**
```json
{
  "productId": 1,
  "quantity": 2
}
```

**Response (200 OK):**
```json
{
  "id": 5,
  "userId": "abc123-def456-ghi789",
  "createdDate": "2024-12-03T09:00:00Z",
  "updatedDate": "2024-12-05T14:50:00Z",
  "items": [
    {
      "id": 12,
      "productId": 1,
      "productName": "Electric Drill",
      "productImageUrl": "/images/products/drill.jpg",
      "quantity": 2,
      "price": 129.99,
      "subtotal": 259.98
    }
  ],
  "totalPrice": 259.98,
  "totalItems": 2
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Product not found"
}
```

**Error Response (400 - Insufficient Stock):**
```json
{
  "statusCode": 400,
  "message": "Only 5 items available in stock"
}
```

---

### 3. Update Cart Item Quantity
**PUT** `/api/cart/{itemId}`

**Example:** `/api/cart/12`

**Authorization:** Bearer Token (Required)

**Request Body:**
```json
{
  "quantity": 3
}
```

**Response (200 OK):**
```json
{
  "id": 5,
  "userId": "abc123-def456-ghi789",
  "createdDate": "2024-12-03T09:00:00Z",
  "updatedDate": "2024-12-05T15:00:00Z",
  "items": [
    {
      "id": 12,
      "productId": 1,
      "productName": "Electric Drill",
      "productImageUrl": "/images/products/drill.jpg",
      "quantity": 3,
      "price": 129.99,
      "subtotal": 389.97
    }
  ],
  "totalPrice": 389.97,
  "totalItems": 3
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Cart item not found"
}
```

**Error Response (401):**
```json
{
  "statusCode": 401,
  "message": "You are not authorized to update this cart item"
}
```

---

### 4. Delete Item from Cart
**DELETE** `/api/cart/{itemId}`

**Example:** `/api/cart/12`

**Authorization:** Bearer Token (Required)

**Response (200 OK):**
```json
{
  "id": 5,
  "userId": "abc123-def456-ghi789",
  "createdDate": "2024-12-03T09:00:00Z",
  "updatedDate": "2024-12-05T15:10:00Z",
  "items": [
    {
      "id": 13,
      "productName": "Hammer",
      "productImageUrl": "/images/products/hammer.jpg",
      "quantity": 1,
      "price": 19.99,
      "subtotal": 19.99
    }
  ],
  "totalPrice": 19.99,
  "totalItems": 1
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Cart item not found"
}
```

---

### 5. Clear All Items from Cart
**DELETE** `/api/cart`

**Authorization:** Bearer Token (Required)

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "message": "Cart cleared successfully"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Cart not found or already empty"
}
```

---

## 📋 Orders Endpoints

**Note:** All order endpoints require authentication (Bearer Token)

### 1. Get Current User's Orders
**GET** `/api/orders`

**Authorization:** Bearer Token (Required)

**Response:**
```json
[
  {
    "id": 45,
    "userId": "abc123-def456-ghi789",
    "orderDate": "2024-12-01T14:30:00Z",
    "totalPrice": 259.98,
    "status": "Delivered",
    "shippingAddress": {
      "firstName": "John",
      "lastName": "Doe",
      "street": "123 Main Street",
      "city": "Springfield",
      "state": "IL",
      "zipCode": "62701",
      "country": "USA"
    },
    "orderItems": [
      {
        "id": 101,
        "productId": 1,
        "productName": "Electric Drill",
        "productImageUrl": "/images/products/drill.jpg",
        "price": 129.99,
        "quantity": 2,
        "subtotal": 259.98
      }
    ],
    "updatedDate": "2024-12-03T10:00:00Z"
  },
  {
    "id": 47,
    "userId": "abc123-def456-ghi789",
    "orderDate": "2024-12-04T16:45:00Z",
    "totalPrice": 89.97,
    "status": "Processing",
    "shippingAddress": {
      "firstName": "John",
      "lastName": "Doe",
      "street": "123 Main Street",
      "city": "Springfield",
      "state": "IL",
      "zipCode": "62701",
      "country": "USA"
    },
    "orderItems": [
      {
        "id": 105,
        "productId": 2,
        "productName": "Hammer",
        "productImageUrl": "/images/products/hammer.jpg",
        "price": 19.99,
        "quantity": 3,
        "subtotal": 59.97
      },
      {
        "id": 106,
        "productId": 5,
        "productName": "Screwdriver Set",
        "productImageUrl": "/images/products/screwdriver.jpg",
        "price": 29.99,
        "quantity": 1,
        "subtotal": 29.99
      }
    ],
    "updatedDate": "2024-12-04T18:00:00Z"
  }
]
```

---

### 2. Get Single Order
**GET** `/api/orders/{id}`

**Example:** `/api/orders/45`

**Authorization:** Bearer Token (Required)

**Response:**
```json
{
  "id": 45,
  "userId": "abc123-def456-ghi789",
  "orderDate": "2024-12-01T14:30:00Z",
  "totalPrice": 259.98,
  "status": "Delivered",
  "shippingAddress": {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main Street",
    "city": "Springfield",
    "state": "IL",
    "zipCode": "62701",
    "country": "USA"
  },
  "orderItems": [
    {
      "id": 101,
      "productId": 1,
      "productName": "Electric Drill",
      "productImageUrl": "/images/products/drill.jpg",
      "price": 129.99,
      "quantity": 2,
      "subtotal": 259.98
    }
  ],
  "updatedDate": "2024-12-03T10:00:00Z"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Order not found"
}
```

**Error Response (401 - Not user's order):**
```json
{
  "statusCode": 401,
  "message": "You are not authorized to view this order"
}
```

---

### 3. Create New Order (From Cart)
**POST** `/api/orders`

**Authorization:** Bearer Token (Required)

**Request Body:**
```json
{
  "shippingAddress": {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main Street",
    "city": "Springfield",
    "state": "IL",
    "zipCode": "62701",
    "country": "USA"
  }
}
```

**Response (201 Created):**
```json
{
  "id": 48,
  "userId": "abc123-def456-ghi789",
  "orderDate": "2024-12-05T15:30:00Z",
  "totalPrice": 389.97,
  "status": "Pending",
  "shippingAddress": {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main Street",
    "city": "Springfield",
    "state": "IL",
    "zipCode": "62701",
    "country": "USA"
  },
  "orderItems": [
    {
      "id": 107,
      "productId": 1,
      "productName": "Electric Drill",
      "productImageUrl": "/images/products/drill.jpg",
      "price": 129.99,
      "quantity": 3,
      "subtotal": 389.97
    }
  ],
  "updatedDate": null
}
```

**Error Response (400 - Empty Cart):**
```json
{
  "statusCode": 400,
  "message": "Cart is empty"
}
```

**Error Response (400 - Insufficient Stock):**
```json
{
  "statusCode": 400,
  "message": "Insufficient stock for Electric Drill. Available: 2"
}
```

**Note:** After successful order creation:
- Product stock is automatically reduced
- Cart is cleared
- Order status is set to "Pending"

---

### 4. Get All Orders (Admin Only)
**GET** `/api/orders/admin`

**Authorization:** Bearer Token (Admin role required)

**Response:**
```json
[
  {
    "id": 45,
    "userId": "abc123-def456-ghi789",
    "orderDate": "2024-12-01T14:30:00Z",
    "totalPrice": 259.98,
    "status": "Delivered",
    "shippingAddress": {
      "firstName": "John",
      "lastName": "Doe",
      "street": "123 Main Street",
      "city": "Springfield",
      "state": "IL",
      "zipCode": "62701",
      "country": "USA"
    },
    "orderItems": [
      {
        "id": 101,
        "productId": 1,
        "productName": "Electric Drill",
        "productImageUrl": "/images/products/drill.jpg",
        "price": 129.99,
        "quantity": 2,
        "subtotal": 259.98
      }
    ],
    "updatedDate": "2024-12-03T10:00:00Z"
  },
  {
    "id": 46,
    "userId": "xyz789-uvw456-rst123",
    "orderDate": "2024-12-02T09:15:00Z",
    "totalPrice": 149.99,
    "status": "Shipped",
    "shippingAddress": {
      "firstName": "Jane",
      "lastName": "Smith",
      "street": "456 Oak Avenue",
      "city": "Chicago",
      "state": "IL",
      "zipCode": "60601",
      "country": "USA"
    },
    "orderItems": [
      {
        "id": 102,
        "productId": 3,
        "productName": "Circular Saw",
        "productImageUrl": "/images/products/saw.jpg",
        "price": 89.99,
        "quantity": 1,
        "subtotal": 89.99
      },
      {
        "id": 103,
        "productId": 2,
        "productName": "Hammer",
        "productImageUrl": "/images/products/hammer.jpg",
        "price": 19.99,
        "quantity": 3,
        "subtotal": 59.97
      }
    ],
    "updatedDate": "2024-12-04T14:20:00Z"
  }
]
```

---

### 5. Update Order Status (Admin Only)
**PUT** `/api/orders/{id}/status`

**Example:** `/api/orders/48/status`

**Authorization:** Bearer Token (Admin role required)

**Request Body:**
```json
{
  "status": 2
}
```

**Valid Status Values:**
- `1` = Pending
- `2` = Processing
- `3` = Shipped
- `4` = Delivered
- `5` = Cancelled

**Response (200 OK):**
```json
{
  "id": 48,
  "userId": "abc123-def456-ghi789",
  "orderDate": "2024-12-05T15:30:00Z",
  "totalPrice": 389.97,
  "status": "Processing",
  "shippingAddress": {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main Street",
    "city": "Springfield",
    "state": "IL",
    "zipCode": "62701",
    "country": "USA"
  },
  "orderItems": [
    {
      "id": 107,
      "productId": 1,
      "productName": "Electric Drill",
      "productImageUrl": "/images/products/drill.jpg",
      "price": 129.99,
      "quantity": 3,
      "subtotal": 389.97
    }
  ],
  "updatedDate": "2024-12-05T16:00:00Z"
}
```

**Error Response (404):**
```json
{
  "statusCode": 404,
  "message": "Order not found"
}
```

---

## 📊 Dashboard Endpoints (Admin Only)

### Get Dashboard Statistics
**GET** `/api/dashboard/stats`

**Authorization:** Bearer Token (Admin role required)

**Response:**
```json
{
  "totalOrders": 156,
  "totalRevenue": 45678.50,
  "totalProducts": 89,
  "totalUsers": 234,
  "recentOrders": [
    {
      "id": 156,
      "userId": "abc123-def456-ghi789",
      "orderDate": "2024-12-05T15:30:00Z",
      "totalPrice": 389.97,
      "status": "Pending"
    },
    {
      "id": 155,
      "userId": "xyz789-uvw456-rst123",
      "orderDate": "2024-12-05T14:15:00Z",
      "totalPrice": 259.98,
      "status": "Processing"
    },
    {
      "id": 154,
      "userId": "mno456-pqr789-stu012",
      "orderDate": "2024-12-05T11:45:00Z",
      "totalPrice": 89.99,
      "status": "Shipped"
    },
    {
      "id": 153,
      "userId": "def123-ghi456-jkl789",
      "orderDate": "2024-12-05T09:30:00Z",
      "totalPrice": 549.95,
      "status": "Processing"
    },
    {
      "id": 152,
      "userId": "uvw012-xyz345-abc678",
      "orderDate": "2024-12-04T18:20:00Z",
      "totalPrice": 129.99,
      "status": "Delivered"
    }
  ],
  "lowStockProducts": [
    {
      "id": 15,
      "name": "Cordless Impact Driver",
      "stock": 3,
      "categoryName": "Power Tools"
    },
    {
      "id": 23,
      "name": "Concrete Mix 50lb",
      "stock": 5,
      "categoryName": "Building Materials"
    },
    {
      "id": 31,
      "name": "Safety Goggles",
      "stock": 7,
      "categoryName": "Safety Equipment"
    },
    {
      "id": 42,
      "name": "Tape Measure 25ft",
      "stock": 8,
      "categoryName": "Hand Tools"
    }
  ]
}
```

**Error Response (401):**
```json
{
  "statusCode": 401,
  "message": "Unauthorized"
}
```

---

## 🔐 Authentication Notes

### Authorization Header Format
All authenticated endpoints require a JWT Bearer token in the Authorization header:

```
Authorization: Bearer <your-jwt-token-here>
```

### Admin-Only Endpoints
The following endpoints require Admin role:
- **Products:** POST, PUT, DELETE
- **Categories:** POST, PUT, DELETE
- **Orders:** GET `/api/orders/admin`, PUT `/api/orders/{id}/status`
- **Dashboard:** All endpoints

### User Endpoints
Regular authenticated users can access:
- **Cart:** All endpoints (own cart only)
- **Orders:** GET (own orders only), POST

---

## 📝 General Error Responses

### 400 Bad Request
```json
{
  "statusCode": 400,
  "message": "Validation error or business logic violation"
}
```

### 401 Unauthorized
```json
{
  "statusCode": 401,
  "message": "User not authenticated"
}
```

### 404 Not Found
```json
{
  "statusCode": 404,
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "statusCode": 500,
  "message": "An error occurred while processing your request"
}
```

---

## 💡 Important Business Logic Notes

### Products
- Product stock is automatically reduced when an order is created
- Products cannot be deleted if they are in existing orders
- Product prices are stored in order items to maintain historical accuracy

### Categories
- Category names must be unique (case-insensitive)
- Categories with associated products cannot be deleted
- Empty categories can be deleted freely

### Cart
- Cart items automatically check product stock availability
- Adding the same product again increases the quantity
- Cart is automatically cleared after successful order creation
- Each user has only one cart

### Orders
- Orders are created from the user's current cart
- Order status flow: Pending → Processing → Shipped → Delivered
- Orders can also be marked as Cancelled
- Shipping address is stored as denormalized data in the order
- Order items store product name, image, and price at the time of order

### Dashboard
- Recent orders are typically the last 5-10 orders
- Low stock products are filtered by a threshold (typically < 10 items)
- Statistics include all-time totals

---

## 🚀 Quick Testing Scenarios

### Scenario 1: Customer Shopping Flow
1. Browse products: `GET /api/products`
2. View product details: `GET /api/products/1`
3. Add to cart: `POST /api/cart` with `{"productId": 1, "quantity": 2}`
4. View cart: `GET /api/cart`
5. Update quantity: `PUT /api/cart/12` with `{"quantity": 3}`
6. Create order: `POST /api/orders` with shipping address
7. View order: `GET /api/orders/48`

### Scenario 2: Admin Product Management
1. Create category: `POST /api/categories` with category data
2. Create product: `POST /api/products` with product data
3. View all products: `GET /api/products`
4. Update product: `PUT /api/products/15` with updated data
5. View dashboard: `GET /api/dashboard/stats`

### Scenario 3: Admin Order Management
1. View all orders: `GET /api/orders/admin`
2. View specific order: `GET /api/orders/48`
3. Update order status: `PUT /api/orders/48/status` with `{"status": 2}`
4. Check dashboard: `GET /api/dashboard/stats`

---

**Last Updated:** December 5, 2024
