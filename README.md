# 🛍️ PixelMart.API

A modern **ASP.NET Core 8.0 Web API** for e-commerce operations featuring JWT authentication, role-based authorization, global exception handling, and comprehensive API documentation.

## 🌟 Features

### 🔐 Authentication & Authorization
- **JWT Bearer Token** authentication
- **Role-based access control** (Admin, User, Client)
- Automatic role seeding on application startup
- Secure token validation with issuer, audience, and signature verification

### 🛒 E-commerce Operations
- **Product management** with full CRUD operations
- **Category-based organization** of products
- **Advanced filtering, sorting, and pagination**
- **JSON Patch support** for partial updates
- **Dynamic LINQ queries** for flexible data retrieval

### 🏗️ Architecture & Design
- **Clean architecture** with Repository/Service pattern
- **Entity Framework Core** with SQL Server
- **AutoMapper** for seamless DTO mapping
- **Global exception handling** for consistent error responses
- **Structured logging** with Console and Debug providers

### 📚 Documentation & Testing
- **Swagger UI** for interactive API documentation
- **Postman collection** included for easy testing
- **Comprehensive API documentation** with examples

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code (optional)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/PixelMart.API.git
   cd PixelMart.API
   ```

2. **Configure application settings**
   
   Update `appsettings.json` with your configuration:
   ```json
   {
     "ConnectionStrings": {
       "PixelMartDbContextConnection": "Server=(localdb)\\mssqllocaldb;Database=PixelMartDb;Trusted_Connection=true"
     },
     "JWT": {
       "Secret": "your-super-secret-jwt-key-here",
       "Issuer": "PixelMart.API",
       "Audience": "PixelMart.Client"
     }
   }
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

### 🌐 Access Points
- **HTTPS**: `https://localhost:8082`
- **HTTP**: `http://localhost:8081`
- **Swagger UI**: `https://localhost:8082/swagger`

## 🛠️ Technology Stack

### Core Framework
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 9.0** - ORM with SQL Server provider
- **ASP.NET Core Identity** - User management and authentication

### Key Dependencies
- **AutoMapper 13.0.1** - Object-to-object mapping
- **JWT Bearer Authentication** - Token-based security
- **Swashbuckle.AspNetCore 6.6.2** - OpenAPI/Swagger documentation
- **System.Linq.Dynamic.Core** - Dynamic LINQ queries
- **Newtonsoft.Json** - JSON serialization with PATCH support

## 📁 Project Structure

```
PixelMart.API/
├── Properties/              # Project launch settings
├── Controllers/             # API controllers
├── Data/                    # Database context and Db initilizers
├── Entities/                # Entity models
├── Helpers/                 # Utility classes and extensions
├── Migrations/              # Entity Framework migrations
├── Models/                  # DTOs
├── Profiles/                # AutoMapper profiles
├── Repositories/            # Data access layer
├── ResourceParameters/      # Query parameters for filtering/pagination
└── Services/                # Property mapper services
```
## 🔧 Development

### Launch Profiles
The application includes multiple launch profiles in `launchSettings.json`:

```bash
# HTTPS profile (recommended)
dotnet run --launch-profile "https"

# HTTP profile
dotnet run --launch-profile "http"
```

### Database Management
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 📋 API Testing

### Postman Collection
Import the included `PixelMart-Restful-API.postman_collection.json` for comprehensive API testing with pre-configured requests and authentication.

### Sample API Endpoints
- `POST /api/auth/login` - User authentication
- `GET /api/products` - List products with pagination
- `POST /api/products` - Create new product (Admin only)
- `PATCH /api/products/{id}` - Partial product update
- `GET /api/categories` - List all categories

## 🛡️ Security Features

### JWT Configuration
- **Token expiration** validation
- **Issuer and audience** verification
- **Signature validation** with secret key
- **Role-based endpoint** protection

### Error Handling
- **Global exception handling** middleware
- **Consistent error responses** in JSON format
- **Appropriate HTTP status codes** (401, 403, 404, 500)
- **Detailed error messages** for development environment

## 📊 Logging & Monitoring

### Structured Logging
- **Console logging** for development
- **Debug logging** for detailed troubleshooting
- **Request logging** with `RequestLogHelper`
- **User action tracking** for audit trails

### Log Categories
- API request/response details
- Authentication attempts
- Database operations
- Exception handling

## 🚀 Deployment

### Prerequisites
- .NET 8.0 Runtime
- SQL Server instance
- IIS or hosting platform

### Configuration
1. Update connection strings for production database
2. Configure JWT settings with production secrets
3. Set appropriate logging levels
4. Configure CORS policies if needed

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 🆘 Support

For issues and questions:
- Create an issue on GitHub
- Review the Postman collection for API examples

##
<div align="center">

**Built with ❤️ using ASP.NET Core**

*Developed by [Maduranga Wimalarathne](mailto:madhuranganw@gmail.com)*

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat-square&logo=dotnet)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core%209.0-512BD4?style=flat-square&logo=microsoft)](https://docs.microsoft.com/en-us/ef/)
[![JWT](https://img.shields.io/badge/JWT-Authentication-000000?style=flat-square&logo=JSON%20web%20tokens)](https://jwt.io/)

</div>