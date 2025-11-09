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
