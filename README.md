# 🛍️ ECommerceApp

A **modern full-stack e-commerce application** built with **.NET 8.0**, featuring a RESTful Web API backend and Razor Pages frontend. Seamlessly manage products, handle shopping carts, and apply time-based discounts — all within a beautiful, responsive UI styled with **Tailwind CSS**. Docker support makes deployment effortless.

![ECommerceApp Screenshot](https://via.placeholder.com/1000x400?text=ECommerceApp+Preview)

---

## 🚀 Features

- 🔍 **Product Management** — Search, sort, and paginate product listings.
- 🛒 **Shopping Cart** — Add, update, or remove items per user session.
- ⏰ **Dynamic Pricing** — Real-time time-based discounts.
- 🎨 **Modern UI** — Built with **Tailwind CSS** for clean, responsive design.
- 🐳 **Docker-Ready** — Containerized for consistent deployment across environments.

---

## 🛠️ Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

---

## 📁 Project Structure

ECommerceApp/
├── ECommerceApp.Api/         ──▶ [Backend Web API (.NET 8.0)](./ECommerceApp.Api)
│   ├── Controllers/          ──▶ [API endpoints](./ECommerceApp.Api/Controllers)
│   ├── Models/               ──▶ [Entity models](./ECommerceApp.Api/Models)
│   ├── DTOs/                 ──▶ [Data transfer objects](./ECommerceApp.Api/DTOs)
│   ├── Services/             ──▶ [Business logic](./ECommerceApp.Api/Services)
│   └── appsettings.json      ──▶ [App config](./ECommerceApp.Api/appsettings.json)
│
├── ECommerceApp.Web/         ──▶ [Frontend Razor Pages](./ECommerceApp.Web)
│   ├── Pages/                ──▶ [Razor views](./ECommerceApp.Web/Pages)
│   ├── wwwroot/              ──▶ [Static files](./ECommerceApp.Web/wwwroot)
│   └── tailwind.config.js    ──▶ [Tailwind config](./ECommerceApp.Web/tailwind.config.js)
│
├── docker-compose.yml        ──▶ [Docker Orchestration](./docker-compose.yml)
├── README.md                 ──▶ [This file](./README.md)
└── ECommerceApp.sln          ──▶ [Solution file](./ECommerceApp.sln)




---

## 🐳 Running with Docker

1. Clone the repository  
2. Open a terminal in the root directory  
3. Run the app:

```bash
docker-compose build
docker-compose up
🌐 Access the apps:

Web UI: https://localhost:7240

API: https://localhost:7241

Swagger UI: https://localhost:7241/swagger

---

💻 Running Locally (Visual Studio)
Clone the repository

Open the solution (.sln) in Visual Studio 2022

Update the appsettings.json connection string if necessary

Set both projects (Api and Web) as startup projects

Press F5 to launch

---

📡 API Endpoints


### 🧾 Products

| Method | Endpoint                    | Description          |
|--------|-----------------------------|----------------------|
| GET    | `/api/products`             | List with filters    |
| GET    | `/api/products/{id}`        | Get specific product |
| POST   | `/api/products`             | Create a product     |
| PUT    | `/api/products/{id}`        | Update a product     |
| DELETE | `/api/products/{id}`        | Delete a product     |


### 🛍️ Cart

| Method | Endpoint                                  | Description          |
|--------|-------------------------------------------|----------------------|
| GET    | `/api/cart/{userId}`                      | Get user's cart      |
| POST   | `/api/cart/{userId}/items`                | Add item to cart     |
| PUT    | `/api/cart/{userId}/items/{itemId}`       | Update item quantity |
| DELETE | `/api/cart/{userId}/items/{itemId}`       | Remove an item       |
| DELETE | `/api/cart/{userId}`                      | Clear the cart       |


---

🔧 Development Guide
🔄 Migrations
To generate and apply EF Core migrations:

```bash
cd ECommerceApp.Api
dotnet ef migrations add InitialCreate
dotnet ef database update

---

➕ Adding Features
Define models & DTOs

Update or create API endpoints / Razor Pages

Add services and configurations

Test functionality

Update the README!

---

🤝 Contributing
We welcome contributions! Here's how to get started:

Fork the repository

Create a new feature branch

Commit your changes

Push and submit a pull request

---

📄 License
This project is licensed under the MIT License — see the LICENSE file for details.

---

💬 Let's Connect
Have questions, ideas, or feedback? Feel free to open an Issue or start a discussion.

Made with ❤️ using .NET 8, Razor Pages & Docker

---
