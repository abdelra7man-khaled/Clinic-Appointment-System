🏥 Clinic Management System – ASP.NET Core Web API

A powerful, scalable backend system for clinic operations, built using ASP.NET Core Web API, applying clean architecture, design patterns, secure authentication, appointment handling, payments, and complete doctor–patient workflows.

🚀 Core Features
👤 Authentication & Role-Based Access

Secure JWT authentication

Supported roles:

Admin

Doctor

Patient

🧑‍⚕️ Patient Features

View personal profile (/patient/me)

Book appointments via Appointment Factory:

Regular

Follow-Up

Emergency

Cancel appointments

Make payments (Cash / Credit Card)

View receipts & payment history

New patients get a random wallet balance (from preset values)

🩺 Doctor Features

Manage personal profile

View personal schedule & appointments

View & update specialties

Doctor dashboard endpoints

🛠 Admin Features

Add / remove doctors

Add specialties

View doctor list & specialties

Manage all appointments

Payment & reports dashboard

Centralized audit logging (Singleton Logger)

🧠 Design Patterns Used
✔ Singleton Pattern

Logger.Instance used across controllers

Logs to console

✔ Strategy Pattern (Payment Handling)

Payment strategies:

CashPaymentStrategy

CreditCardPaymentStrategy

Allows adding new payment types easily.

✔ Proxy Pattern (Credit Card Validation)

CreditCardProxy validates:

Card number

CVV

Expiry date
(using dummy stored card data)

Executes the real strategy only when validated.

✔ Simple Factory Pattern (Appointments)

AppointmentFactory creates:

Regular appointments

Emergency (fee multiplier)

Follow-Up (discount applied)

Ensures consistent appointment creation logic.

📦 Technologies Used

ASP.NET Core Web API (.NET 9)

Entity Framework Core

SQL Server

JWT Authentication

Repository Pattern + Unit of Work

LINQ

📁 Project Structure

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
        Logger.cs  (Singleton)

/Models
    /DTOs
    /Enums

/Data
    /Repositories
    AppDbContext
    /Migrations

💳 Payment Flow Summary

Patient sends a payment request

PaymentContext selects appropriate strategy

If payment is credit card:

Request is validated using CreditCardProxy

If card is valid:

Amount deducted from wallet

Appointment status updated

Payment saved to database

A receipt DTO is returned

