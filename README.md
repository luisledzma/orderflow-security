# OrderFlow - Security Service

## Description
The **Security Service** is responsible for handling user authentication and authorization. It provides endpoints for user registration, login, and token validation.

## Features
- User authentication using **JWT tokens**
- Role-based access control (RBAC)
- Secure password storage with hashing
- Token validation for protected routes

## Technologies
- .NET 8 Web API
- IdentityServer4 / JWT Authentication
- PostgreSQL for user management

## Running the Service
1. Clone the repository:  
   ```sh
   git clone https://github.com/your-username/orderflow-security.git
   cd orderflow-security
