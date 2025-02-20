Clothing E-Commerce API
A robust Web API providing a complete e-commerce backend for a clothing store. 
Built with EF Core and SQL Server, featuring secure JWT authentication, 
role-based authorization (admin/user), 
and concurrency-safe checkouts using EF Core transactions and RowVersion.

Features
JWT Authentication & Role-Based Authorization: Ensures secure endpoints for both admins and users.
Concurrency Handling: Manages checkouts with EF Core transactions and RowVersion to prevent conflicts.
Product/Category/Variation CRUD: Supports soft-delete and reactivation for seamless product management.
User Carts & Orders: Users can add items to cart, place orders, and track order history.
Address Management: Users can maintain multiple addresses for deliveries.
OTP-Based Email Confirmation & Password Resets: Securely handle account confirmations and credential recovery.
Clean, Scalable Architecture: Designed for easy maintenance and extension.

Tech Stack
ASP.NET Core Web API (C#)
Entity Framework Core
SQL Server
JWT for authentication
