# 🏥 Clinic Management System – ASP.NET Core Web API

A production-ready backend for managing clinic operations, built with ASP.NET Core Web API following clean architecture principles.

The system includes authentication, doctor–patient workflow, appointment booking, payment processing, auditing, and more.

---

## 🚀 Core Features

### 👤 Authentication & Role-Based Access
* **Secure JWT Authentication**
* **Supported roles:**
    * Admin
    * Doctor
    * Patient

### 🧑‍⚕️ Patient Features
* **Profile:** View personal profile (`/patient/me`)
* **Appointments:** Book appointments using **Appointment Factory**:
    * Regular
    * Follow-Up
    * Emergency
* **Management:** Cancel appointments
* **Payments:**
    * Make payments (Cash / Credit Card)
    * View payment history & receipts
* **Wallet:** New patients receive a random wallet balance from preset values: `(5000, 3700, 1500, 900, 250)`

### 🩺 Doctor Features
* Manage personal profile
* View schedule & appointments
* View/update specialties
* Doctor dashboard APIs

### 🛠 Admin Features
* Add / remove doctors
* Add specialties
* View all doctors & assigned specialties
* Manage all appointments
* Payment & reports dashboard
* Centralized audit logging via **Singleton Logger**

---

## 🧠 Design Patterns Used

### ✔ Singleton Pattern
* Global `Logger.Instance` used across the system.
* Logs written to console.

### ✔ Strategy Pattern (Payment Handling)
* Handles different payment types:
    * `CashPaymentStrategy`
    * `CreditCardPaymentStrategy`
* Allows adding new payment methods easily.

### ✔ Proxy Pattern (Credit Card Validation)
* The `CreditCardProxy` validates:
    * Card number
    * CVV
    * Expiry date
* **Logic:** Only if valid → real credit card strategy executes.

### ✔ Simple Factory Pattern (Appointments)
* `AppointmentFactory` generates:
    * **Regular Appointments**
    * **Emergency** (fee multiplier)
    * **Follow-Up** (discount applied)
* Ensures consistent appointment creation logic.

---

## 📦 Technologies Used
* ASP.NET Core Web API (.NET 9)
* Entity Framework Core
* SQL Server
* JWT Authentication
* Repository Pattern
* Unit of Work Pattern
* LINQ
* Dependency Injection

---

## 📁 Project Structure

```text
/Controllers
    AdminController.cs
    PatientController.cs
    DoctorController.cs
    AppointmentController.cs
    PaymentController.cs

/Services
    /Payments
        IPaymentStrategy.cs
        CashPaymentStrategy.cs
        CreditCardPaymentStrategy.cs
        CreditCardProxy.cs
        PaymentContext.cs
    /Factory
        AppointmentFactory.cs
    /Logging
        Logger.cs (Singleton)

/Models
    /DTOs
    /Enums

/Data
    /Repositories
    AppDbContext.cs
    /Migrations
