# Buildify Backend - ASP.NET Core Clean Architecture

## Overview
A complete ASP.NET Core 8.0 Web API project built with Clean Architecture, featuring comprehensive authentication, account management, and OTP-based password reset functionality.

## Architecture

### Four-Layer Clean Architecture:
1. **Buildify.Core** - Domain entities, interfaces, DTOs, specifications
2. **Buildify.Repository** - Data access, EF Core contexts, migrations
3. **Buildify.Service** - Business logic implementation
4. **Buildify.APIs** - Controllers, middleware, configuration

## Features

### Authentication & Account Management
- ✅ User Registration with mandatory email verification
- ✅ Email Verification with OTP (6-digit code)
- ✅ Resend Email Verification OTP
- ✅ User Login with JWT token generation (requires verified email)
- ✅ Get Current User (with token refresh)
- ✅ Logout functionality
- ✅ Forgot Password (OTP-based)
- ✅ Verify OTP (for password reset)
- ✅ Reset Password with Token
- ✅ Resend OTP (with rate limiting)
- ✅ Check Email Exists
- ✅ User Profile Management (Get/Update)
- ✅ Address Management (CRUD operations)

### Products & Categories
- ✅ Product Management (CRUD operations)
- ✅ Category Management (CRUD operations)
- ✅ Get Products by Category
- ✅ Product-Category relationships
- ✅ Admin-only product and category modifications

### Shopping Cart
- ✅ View Cart (with calculated totals)
- ✅ Add to Cart (with stock validation)
- ✅ Update Cart Item Quantity
- ✅ Remove Item from Cart
- ✅ Clear entire Cart
- ✅ User-specific cart management

### Orders
- ✅ Create Order from Cart
- ✅ View User Orders
- ✅ View Single Order Details
- ✅ Automatic Stock Reduction
- ✅ Order Status Tracking (Pending, Processing, Shipped, Delivered, Cancelled)
- ✅ Admin: View All Orders
- ✅ Admin: Update Order Status
- ✅ Shipping Address Management

### Dashboard (Admin)
- ✅ Dashboard Statistics (Total Orders, Revenue, Products, Users)
- ✅ Recent Orders Overview (Last 10 orders)
- ✅ Low Stock Products Alert (Products with stock <= 10)

### Security Features
- 🔐 JWT Bearer Authentication
- 🔐 Password Hashing using ASP.NET Core Identity
- 🔐 OTP Hashing with SHA256
- 🔐 Cryptographically Secure Random OTP Generation
- 🔐 OTP Expiration (10 minutes)
- 🔐 Reset Token Expiration (5 minutes)
- 🔐 Rate Limiting (60-second cooldown for OTP resend)
- 🔐 Failed Attempt Lockout (max 5 attempts)
- 🔐 Role-Based Authorization

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or SQL Server instance)
- Visual Studio 2022 or VS Code
- SQL Server Management Studio (optional)

## Getting Started

### 1. Clone the Repository
```bash
cd d:\Buildify.Backend
```

### 2. Update Connection Strings

Edit `Buildify.APIs/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR-SERVER;Database=BuildifyDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "IdentityConnection": "Server=YOUR-SERVER;Database=BuildifyIdentityDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Configure Email Settings

Update the `EmailSettings` section in `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderName": "Buildify Support",
    "SenderEmail": "your-email@gmail.com",
    "Username": "your-email@gmail.com",
    "Password": "YOUR-GMAIL-APP-PASSWORD"
  }
}
```

**Note:** For Gmail, you need to generate an App Password:
1. Go to Google Account Settings
2. Security → 2-Step Verification
3. App passwords → Generate new app password
4. Use the generated password in the configuration

### 4. Configure JWT Settings

The JWT settings are already configured, but you should change the secret key in production:

```json
{
  "JWT": {
    "Key": "YOUR-VERY-LONG-SECRET-KEY-AT-LEAST-32-CHARACTERS",
    "ValidIssuer": "https://localhost:7101",
    "ValidAudience": "https://localhost:7101",
    "DurationInDays": "2"
  }
}
```

### 5. Create Database Migrations

```powershell
# The migration has already been created and includes the OtpPurpose field
# When you run the application, migrations will be applied automatically
# Or you can apply them manually:

# Apply Identity database migration
dotnet ef database update --project Buildify.Repository --context AppIdentityDbContext --startup-project Buildify.APIs

# Apply Store database migration (if needed)
dotnet ef database update --project Buildify.Repository --context StoreContext --startup-project Buildify.APIs
```

### 6. Run the Application

```powershell
cd Buildify.APIs
dotnet run
```

The application will:
- Automatically apply migrations
- Seed default roles (Admin, User)
- Seed admin user (admin@example.com / Admin@123) with verified email
- Start listening on https://localhost:7101 and http://localhost:7100

**Note:** New users must verify their email address before they can login. An OTP will be sent to their email upon registration.

### 7. Access Swagger UI

Navigate to: https://localhost:7101/swagger

## API Endpoints

### Products

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/products` | Get all products with categories | No |
| GET | `/api/products/{id}` | Get single product by ID | No |
| GET | `/api/products/category/{categoryId}` | Get products by category | No |
| POST | `/api/products` | Create new product | Yes (Admin) |
| PUT | `/api/products/{id}` | Update product | Yes (Admin) |
| DELETE | `/api/products/{id}` | Delete product | Yes (Admin) |

### Categories

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/categories` | Get all categories | No |
| GET | `/api/categories/{id}` | Get single category by ID | No |
| POST | `/api/categories` | Create new category | Yes (Admin) |
| PUT | `/api/categories/{id}` | Update category | Yes (Admin) |
| DELETE | `/api/categories/{id}` | Delete category | Yes (Admin) |

### Cart

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/cart` | Get current user's cart | Yes |
| POST | `/api/cart` | Add product to cart | Yes |
| PUT | `/api/cart/{itemId}` | Update cart item quantity | Yes |
| DELETE | `/api/cart/{itemId}` | Remove item from cart | Yes |
| DELETE | `/api/cart` | Clear entire cart | Yes |

### Orders

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/orders` | Get current user's orders | Yes |
| GET | `/api/orders/{id}` | Get single order details | Yes |
| POST | `/api/orders` | Create order from cart | Yes |
| GET | `/api/orders/admin` | Get all orders | Yes (Admin) |
| PUT | `/api/orders/{id}/status` | Update order status | Yes (Admin) |

### Dashboard (Admin Only)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/dashboard/stats` | Get dashboard statistics | Yes (Admin) |

### Account Management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/account/register` | Register new user (sends verification OTP) | No |
| POST | `/api/account/verify-email` | Verify email with OTP | No |
| POST | `/api/account/resend-verification-otp` | Resend email verification OTP | No |
| POST | `/api/account/login` | Login user (requires verified email) | No |
| GET | `/api/account/GetCurrentUser` | Get current user info | Yes |
| POST | `/api/account/logout` | Logout user | Yes |
| POST | `/api/account/ForgotPassword` | Request password reset OTP | No |
| POST | `/api/account/VerifyOtp` | Verify OTP code for password reset | No |
| POST | `/api/account/ResetPasswordWithToken` | Reset password with token | No |
| POST | `/api/account/ResendOtp` | Resend password reset OTP | No |
| GET | `/api/account/emailexists` | Check if email exists | No |
| GET | `/api/account/profile` | Get user profile | Yes |
| PUT | `/api/account/profile` | Update user profile | Yes |
| GET | `/api/account/addresses` | Get user addresses | Yes |
| POST | `/api/account/addresses` | Create new address | Yes |
| PUT | `/api/account/addresses/{id}` | Update address | Yes |
| DELETE | `/api/account/addresses/{id}` | Delete address | Yes |
| GET | `/api/account/token` | Get JWT token | Yes |

## Testing the API

### Example: Products API

1. **Get All Products:**
```json
GET /api/products

Response:
[
  {
    "id": 1,
    "name": "Cement Bag",
    "description": "50kg Premium Portland Cement",
    "price": 45.99,
    "stock": 150,
    "categoryId": 1,
    "categoryName": "Building Materials",
    "imageUrl": "https://example.com/cement.jpg",
    "createdDate": "2025-11-12T10:30:00Z"
  }
]
```

2. **Get Single Product:**
```json
GET /api/products/1

Response:
{
  "id": 1,
  "name": "Cement Bag",
  "description": "50kg Premium Portland Cement",
  "price": 45.99,
  "stock": 150,
  "categoryId": 1,
  "categoryName": "Building Materials",
  "imageUrl": "https://example.com/cement.jpg",
  "createdDate": "2025-11-12T10:30:00Z"
}
```

3. **Get Products by Category:**
```json
GET /api/products/category/1

Response:
[
  {
    "id": 1,
    "name": "Cement Bag",
    "description": "50kg Premium Portland Cement",
    "price": 45.99,
    "stock": 150,
    "categoryId": 1,
    "categoryName": "Building Materials",
    "imageUrl": "https://example.com/cement.jpg",
    "createdDate": "2025-11-12T10:30:00Z"
  }
]
```

4. **Create Product (Admin Only):**
```json
POST /api/products
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "name": "Cement Bag",
  "description": "50kg Premium Portland Cement",
  "price": 45.99,
  "stock": 150,
  "categoryId": 1,
  "imageUrl": "https://example.com/cement.jpg"
}

Response:
{
  "id": 1,
  "name": "Cement Bag",
  "description": "50kg Premium Portland Cement",
  "price": 45.99,
  "stock": 150,
  "categoryId": 1,
  "categoryName": "Building Materials",
  "imageUrl": "https://example.com/cement.jpg",
  "createdDate": "2025-11-12T10:30:00Z"
}
```

5. **Update Product (Admin Only):**
```json
PUT /api/products/1
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "name": "Cement Bag - Updated",
  "description": "50kg Premium Portland Cement - New Formula",
  "price": 49.99,
  "stock": 200,
  "categoryId": 1,
  "imageUrl": "https://example.com/cement-new.jpg"
}

Response:
{
  "id": 1,
  "name": "Cement Bag - Updated",
  "description": "50kg Premium Portland Cement - New Formula",
  "price": 49.99,
  "stock": 200,
  "categoryId": 1,
  "categoryName": "Building Materials",
  "imageUrl": "https://example.com/cement-new.jpg",
  "createdDate": "2025-11-12T10:30:00Z"
}
```

6. **Delete Product (Admin Only):**
```json
DELETE /api/products/1
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN

Response:
{
  "statusCode": 200,
  "message": "Product deleted successfully"
}
```

### Example: Categories API

1. **Get All Categories:**
```json
GET /api/categories

Response:
[
  {
    "id": 1,
    "name": "Building Materials",
    "description": "Cement, bricks, concrete, etc."
  },
  {
    "id": 2,
    "name": "Tools",
    "description": "Hand tools and power tools"
  }
]
```

2. **Get Single Category:**
```json
GET /api/categories/1

Response:
{
  "id": 1,
  "name": "Building Materials",
  "description": "Cement, bricks, concrete, etc."
}
```

3. **Create Category (Admin Only):**
```json
POST /api/categories
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "name": "Building Materials",
  "description": "Cement, bricks, concrete, etc."
}

Response:
{
  "id": 1,
  "name": "Building Materials",
  "description": "Cement, bricks, concrete, etc."
}
```

4. **Update Category (Admin Only):**
```json
PUT /api/categories/1
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "name": "Construction Materials",
  "description": "All types of construction materials"
}

Response:
{
  "id": 1,
  "name": "Construction Materials",
  "description": "All types of construction materials"
}
```

5. **Delete Category (Admin Only):**
```json
DELETE /api/categories/1
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN

Response:
{
  "statusCode": 200,
  "message": "Category deleted successfully"
}

Error (if category has products):
{
  "statusCode": 400,
  "message": "Cannot delete category with existing products"
}
```

### Example: Cart API

1. **Get Cart:**
```json
GET /api/cart
Authorization: Bearer YOUR_JWT_TOKEN

Response:
{
  "id": 1,
  "userId": "user123",
  "createdDate": "2025-11-12T10:00:00Z",
  "updatedDate": "2025-11-12T10:30:00Z",
  "items": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Cement Bag",
      "productImageUrl": "https://example.com/cement.jpg",
      "quantity": 2,
      "price": 45.99,
      "subtotal": 91.98
    },
    {
      "id": 2,
      "productId": 5,
      "productName": "Steel Rods",
      "productImageUrl": "https://example.com/steel.jpg",
      "quantity": 10,
      "price": 12.50,
      "subtotal": 125.00
    }
  ],
  "totalPrice": 216.98,
  "totalItems": 12
}

Response (empty cart):
{
  "userId": "user123",
  "createdDate": "2025-11-12T10:00:00Z",
  "items": [],
  "totalPrice": 0,
  "totalItems": 0
}
```

2. **Add to Cart:**
```json
POST /api/cart
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}

Response:
{
  "id": 1,
  "userId": "user123",
  "createdDate": "2025-11-12T10:00:00Z",
  "updatedDate": "2025-11-12T10:30:00Z",
  "items": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Cement Bag",
      "productImageUrl": "https://example.com/cement.jpg",
      "quantity": 2,
      "price": 45.99,
      "subtotal": 91.98
    }
  ],
  "totalPrice": 91.98,
  "totalItems": 2
}

Error (insufficient stock):
{
  "statusCode": 400,
  "message": "Only 5 items available in stock"
}
```

3. **Update Cart Item Quantity:**
```json
PUT /api/cart/1
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "quantity": 5
}

Response:
{
  "id": 1,
  "userId": "user123",
  "createdDate": "2025-11-12T10:00:00Z",
  "updatedDate": "2025-11-12T10:45:00Z",
  "items": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Cement Bag",
      "productImageUrl": "https://example.com/cement.jpg",
      "quantity": 5,
      "price": 45.99,
      "subtotal": 229.95
    }
  ],
  "totalPrice": 229.95,
  "totalItems": 5
}
```

4. **Delete Cart Item:**
```json
DELETE /api/cart/1
Authorization: Bearer YOUR_JWT_TOKEN

Response:
{
  "id": 1,
  "userId": "user123",
  "createdDate": "2025-11-12T10:00:00Z",
  "updatedDate": "2025-11-12T10:50:00Z",
  "items": [],
  "totalPrice": 0,
  "totalItems": 0
}
```

5. **Clear Cart:**
```json
DELETE /api/cart
Authorization: Bearer YOUR_JWT_TOKEN

Response:
{
  "statusCode": 200,
  "message": "Cart cleared successfully"
}
```

### Example: Orders API

1. **Get User Orders:**
```json
GET /api/orders
Authorization: Bearer YOUR_JWT_TOKEN

Response:
[
  {
    "id": 1,
    "userId": "user123",
    "orderDate": "2025-11-12T11:00:00Z",
    "totalPrice": 229.95,
    "status": "Processing",
    "shippingAddress": {
      "firstName": "John",
      "lastName": "Doe",
      "street": "123 Main St",
      "city": "New York",
      "state": "NY",
      "zipCode": "10001",
      "country": "USA"
    },
    "orderItems": [
      {
        "id": 1,
        "productId": 1,
        "productName": "Cement Bag",
        "productImageUrl": "https://example.com/cement.jpg",
        "price": 45.99,
        "quantity": 5,
        "subtotal": 229.95
      }
    ],
    "updatedDate": "2025-11-12T12:00:00Z"
  }
]
```

2. **Get Single Order:**
```json
GET /api/orders/1
Authorization: Bearer YOUR_JWT_TOKEN

Response:
{
  "id": 1,
  "userId": "user123",
  "orderDate": "2025-11-12T11:00:00Z",
  "totalPrice": 229.95,
  "status": "Processing",
  "shippingAddress": {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  },
  "orderItems": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Cement Bag",
      "productImageUrl": "https://example.com/cement.jpg",
      "price": 45.99,
      "quantity": 5,
      "subtotal": 229.95
    }
  ],
  "updatedDate": "2025-11-12T12:00:00Z"
}
```

3. **Create Order from Cart:**
```json
POST /api/orders
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "shippingAddress": {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  }
}

Response:
{
  "id": 1,
  "userId": "user123",
  "orderDate": "2025-11-12T11:00:00Z",
  "totalPrice": 229.95,
  "status": "Pending",
  "shippingAddress": {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  },
  "orderItems": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Cement Bag",
      "productImageUrl": "https://example.com/cement.jpg",
      "price": 45.99,
      "quantity": 5,
      "subtotal": 229.95
    }
  ],
  "updatedDate": null
}

Note: 
- Cart will be automatically cleared after successful order
- Product stock will be automatically reduced
- Order starts with "Pending" status

Error (empty cart):
{
  "statusCode": 400,
  "message": "Cart is empty"
}

Error (insufficient stock):
{
  "statusCode": 400,
  "message": "Insufficient stock for Cement Bag. Available: 3"
}
```

4. **Get All Orders (Admin Only):**
```json
GET /api/orders/admin
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN

Response:
[
  {
    "id": 1,
    "userId": "user123",
    "orderDate": "2025-11-12T11:00:00Z",
    "totalPrice": 229.95,
    "status": "Processing",
    "shippingAddress": {
      "firstName": "John",
      "lastName": "Doe",
      "street": "123 Main St",
      "city": "New York",
      "state": "NY",
      "zipCode": "10001",
      "country": "USA"
    },
    "orderItems": [
      {
        "id": 1,
        "productId": 1,
        "productName": "Cement Bag",
        "productImageUrl": "https://example.com/cement.jpg",
        "price": 45.99,
        "quantity": 5,
        "subtotal": 229.95
      }
    ],
    "updatedDate": "2025-11-12T12:00:00Z"
  },
  {
    "id": 2,
    "userId": "user456",
    "orderDate": "2025-11-12T10:00:00Z",
    "totalPrice": 125.00,
    "status": "Delivered",
    "shippingAddress": {
      "firstName": "Jane",
      "lastName": "Smith",
      "street": "456 Oak Ave",
      "city": "Los Angeles",
      "state": "CA",
      "zipCode": "90001",
      "country": "USA"
    },
    "orderItems": [
      {
        "id": 2,
        "productId": 5,
        "productName": "Steel Rods",
        "productImageUrl": "https://example.com/steel.jpg",
        "price": 12.50,
        "quantity": 10,
        "subtotal": 125.00
      }
    ],
    "updatedDate": "2025-11-12T11:30:00Z"
  }
]
```

5. **Update Order Status (Admin Only):**
```json
PUT /api/orders/1/status
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN
Content-Type: application/json

{
  "status": 2
}

Response:
{
  "id": 1,
  "userId": "user123",
  "orderDate": "2025-11-12T11:00:00Z",
  "totalPrice": 229.95,
  "status": "Processing",
  "shippingAddress": {
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  },
  "orderItems": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Cement Bag",
      "productImageUrl": "https://example.com/cement.jpg",
      "price": 45.99,
      "quantity": 5,
      "subtotal": 229.95
    }
  ],
  "updatedDate": "2025-11-12T13:00:00Z"
}

Order Status Values:
- 1 = Pending
- 2 = Processing
- 3 = Shipped
- 4 = Delivered
- 5 = Cancelled
```

### Example: Dashboard Statistics (Admin Only)

**Get Dashboard Stats:**
```json
GET /api/dashboard/stats
Authorization: Bearer YOUR_ADMIN_JWT_TOKEN

Response:
{
  "totalOrders": 150,
  "totalRevenue": 45000.00,
  "totalProducts": 45,
  "totalUsers": 320,
  "recentOrders": [
    {
      "id": 25,
      "userId": "user123",
      "orderDate": "2025-11-12T14:30:00Z",
      "totalPrice": 299.99,
      "status": "Pending"
    },
    {
      "id": 24,
      "userId": "user456",
      "orderDate": "2025-11-12T13:15:00Z",
      "totalPrice": 125.50,
      "status": "Processing"
    },
    {
      "id": 23,
      "userId": "user789",
      "orderDate": "2025-11-12T11:00:00Z",
      "totalPrice": 450.00,
      "status": "Shipped"
    }
  ],
  "lowStockProducts": [
    {
      "id": 12,
      "name": "Premium Paint - White",
      "stock": 3,
      "categoryName": "Paint & Finishing"
    },
    {
      "id": 8,
      "name": "Steel Rods - 12mm",
      "stock": 5,
      "categoryName": "Building Materials"
    },
    {
      "id": 15,
      "name": "Wood Planks - Oak",
      "stock": 7,
      "categoryName": "Timber"
    }
  ]
}

Features:
- Total Orders: Count of all orders in the system
- Total Revenue: Sum of all order totals
- Total Products: Count of all products in catalog
- Total Users: Count of all registered users
- Recent Orders: Last 10 orders ordered by date (newest first)
- Low Stock Products: Products with stock <= 10, ordered by stock level (lowest first)
```

### Example: Registration & Email Verification Flow

1. **Register a New User:**
```json
POST /api/account/register
Content-Type: application/json

{
  "email": "user@example.com",
  "displayName": "John Doe",
  "phoneNumber": "1234567890",
  "password": "Password123"
}

Response:
{
  "statusCode": 200,
  "message": "Registration successful. Please check your email to verify your account."
}
```

2. **Check Email for OTP** (6-digit code sent to user@example.com)

3. **Verify Email:**
```json
POST /api/account/verify-email
Content-Type: application/json

{
  "email": "user@example.com",
  "otpCode": "123456"
}

Response:
{
  "displayName": "John Doe",
  "email": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

4. **Resend Verification OTP (if needed):**
```json
POST /api/account/resend-verification-otp
Content-Type: application/json

{
  "email": "user@example.com"
}
```

### Example: Login

```json
POST /api/account/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123"
}

Response (Success):
{
  "displayName": "John Doe",
  "email": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}

Response (Email Not Verified):
{
  "statusCode": 401,
  "message": "Please verify your email address before logging in"
}
```

### Example: Password Reset Flow

1. Request OTP:
```json
POST /api/account/ForgotPassword
{
  "email": "user@example.com"
}
```

2. Verify OTP (check your email):
```json
POST /api/account/VerifyOtp
{
  "email": "user@example.com",
  "otpCode": "123456"
}
```

3. Reset Password with the returned token:
```json
POST /api/account/ResetPasswordWithToken
{
  "email": "user@example.com",
  "resetToken": "TOKEN_FROM_VERIFY_OTP_RESPONSE",
  "newPassword": "NewPassword123"
}
```

### Example: Get Current User

```json
GET /api/account/GetCurrentUser
Authorization: Bearer YOUR_JWT_TOKEN

Response:
{
  "displayName": "John Doe",
  "email": "user@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Example: Logout

```json
POST /api/account/logout
Authorization: Bearer YOUR_JWT_TOKEN

Response:
{
  "statusCode": 200,
  "message": "Logged out successfully"
}
```

### Example: Check Email Exists

```json
GET /api/account/emailexists?email=user@example.com

Response:
true
```

### Example: Get User Profile

```json
GET /api/account/profile
Authorization: Bearer YOUR_JWT_TOKEN

Response:
{
  "displayName": "John Doe",
  "email": "user@example.com",
  "phoneNumber": "1234567890"
}
```

### Example: Update User Profile

```json
PUT /api/account/profile
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "displayName": "John Smith",
  "phoneNumber": "0987654321"
}

Response:
{
  "displayName": "John Smith",
  "email": "user@example.com",
  "phoneNumber": "0987654321"
}
```

### Example: Get User Addresses

```json
GET /api/account/addresses
Authorization: Bearer YOUR_JWT_TOKEN

Response:
[
  {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  }
]
```

### Example: Create New Address

```json
POST /api/account/addresses
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "street": "123 Main St",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "country": "USA"
}

Response:
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "street": "123 Main St",
  "city": "New York",
  "state": "NY",
  "zipCode": "10001",
  "country": "USA"
}
```

### Example: Update Address

```json
PUT /api/account/addresses/1
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Smith",
  "street": "456 Oak Ave",
  "city": "Los Angeles",
  "state": "CA",
  "zipCode": "90001",
  "country": "USA"
}

Response:
{
  "id": 1,
  "firstName": "John",
  "lastName": "Smith",
  "street": "456 Oak Ave",
  "city": "Los Angeles",
  "state": "CA",
  "zipCode": "90001",
  "country": "USA"
}
```

### Example: Delete Address

```json
DELETE /api/account/addresses/1
Authorization: Bearer YOUR_JWT_TOKEN

Response:
{
  "statusCode": 200,
  "message": "Address deleted successfully"
}
```

### Example: Get Token

```json
GET /api/account/token
Authorization: Bearer YOUR_JWT_TOKEN

Response:
"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## Default Admin Account

- **Email:** admin@example.com
- **Password:** Admin@123
- **Role:** Admin
- **Access:** Full CRUD on Products, Categories, Orders, and Dashboard Statistics

## Project Structure

```
Buildify.Backend/
├── Buildify.Core/
│   ├── Entities/Identity/
│   ├── DTOs/
│   ├── Repositories/
│   ├── Services/
│   └── Specifications/
├── Buildify.Repository/
│   ├── Data/
│   ├── Identity/
│   └── Implementations/
├── Buildify.Service/
│   ├── TokenService.cs
│   ├── OtpService.cs
│   └── EmailService.cs
└── Buildify.APIs/
    ├── Controllers/
    ├── Middlewares/
    ├── Extensions/
    ├── Helpers/
    └── Errors/
```

## Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core 8.0.11
- ASP.NET Core Identity
- JWT Bearer Authentication
- AutoMapper 12.0.1
- MailKit 4.9.0
- Swashbuckle (Swagger)
- SQL Server

## Security Best Practices Implemented

✅ Password hashing using Identity  
✅ OTP hashing with SHA256  
✅ Cryptographically secure random generation  
✅ Token expiration  
✅ Rate limiting  
✅ Failed attempt lockout  
✅ HTTPS enforcement  
✅ CORS configuration  
✅ Input validation  
✅ Global exception handling  
✅ Email enumeration prevention  

## CORS Configuration

The API allows requests from:
- **Frontend (Angular)**: http://localhost:4200 and https://localhost:4200
- **API itself**: http://localhost:7100 and https://localhost:7101

To add more origins, edit `Program.cs`:

```csharp
policy.WithOrigins(
    "http://localhost:4200",
    "https://localhost:4200",
    "YOUR-FRONTEND-URL"
);
```

## Troubleshooting

### Migration Issues
If you encounter migration issues, delete the `Migrations` folders and recreate:
```powershell
Remove-Item -Recurse -Force Buildify.Repository\Identity\Migrations
Remove-Item -Recurse -Force Buildify.Repository\Data\Migrations
```

Then recreate migrations as shown in step 5.

### Email Not Sending
1. Verify Gmail App Password is correct
2. Check if 2-Step Verification is enabled on your Google account
3. Ensure SMTP settings are correct
4. Check firewall/antivirus blocking port 587

### Database Connection Issues
1. Verify SQL Server is running
2. Check connection strings in appsettings.json
3. Ensure you have permissions to create databases
4. Try using `Server=.;` for LocalDB or `Server=localhost;` for SQL Server Express

## Future Enhancements

- [ ] Two-Factor Authentication (2FA)
- [ ] Refresh Tokens
- [ ] Email Verification on Registration
- [ ] Rate Limiting Middleware
- [ ] Redis Caching
- [ ] SignalR for Real-time Features
- [ ] Logging with Serilog
- [ ] API Versioning
- [ ] Health Checks

## License

This project is created for educational purposes.

## Support

For issues or questions, please check:
1. Configuration files (appsettings.json)
2. Database connection strings
3. Email settings
4. Migration status
5. Package versions compatibility

---

**Created:** October 31, 2025  
**Framework:** ASP.NET Core 8.0  
**Architecture:** Clean Architecture  
**Pattern:** Repository Pattern + Unit of Work
