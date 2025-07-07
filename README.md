# 🏆 Luxury & Gold - E-commerce Platform

A sophisticated e-commerce platform specialized in gold products, featuring dynamic pricing based on real-time gold market rates and comprehensive administrative capabilities.

## 🌟 Overview

**Luxury & Gold** is a full-stack web application that enables users to browse, purchase, and manage gold products including jewelry, coins, and bars. The platform integrates real-time gold pricing from international markets and provides a complete e-commerce experience with advanced features for both customers and administrators.

### Key Features

- 🥇 **Dynamic Gold Pricing** - Real-time price updates based on international gold market rates
- 🛒 **Complete Shopping Experience** - Cart management, order processing, and user accounts
- 📊 **Advanced Analytics** - Sales reports, revenue analysis, and customer purchase patterns
- 🔐 **Secure Authentication** - JWT-based authentication with email verification
- 📱 **Responsive Design** - Optimized for all devices using Angular and PrimeNG
- ☁️ **Cloud Integration** - Azure-hosted with automated daily price updates

## 🏗️ Architecture

### Backend (ShopBackEnd)
- **Framework**: ASP.NET Core 8.0
- **Database**: Entity Framework Core with SQL Server/Azure SQL
- **Authentication**: JWT Bearer tokens
- **API Documentation**: Swagger/OpenAPI
- **Validation**: FluentValidation
- **Security**: BCrypt password hashing

### Frontend (ShopFrontEnd)
- **Framework**: Angular with TypeScript
- **UI Components**: PrimeNG
- **Charts**: CanvasJS for gold price visualization
- **Animations**: AOS (Animate On Scroll)
- **Styling**: Custom CSS with responsive design

### Cloud Services
- **Database**: Azure SQL Database
- **Automation**: Azure Timer Functions for daily gold price updates
- **Email**: SendGrid integration for notifications

## 🛠️ Technology Stack

### Backend Technologies
- ASP.NET Core 8.0 (C#)
- Entity Framework Core
- FluentValidation
- BCrypt.Net-Next
- JWT Bearer Authentication
- FluentMigrator
- SendGrid
- System.Text.Json

### Frontend Technologies
- Angular
- TypeScript
- HTML5 & CSS3
- PrimeNG
- CanvasJS
- AOS Animation Library

### Database & Cloud
- SQL Server (Local Development)
- Azure SQL Database (Production)
- Azure Timer Functions
- Azure App Service
