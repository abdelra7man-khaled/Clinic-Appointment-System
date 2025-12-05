🏥 Clinic Management System

A complete backend system built using ASP.NET Core Web API, implementing clean architecture, design patterns, secure authentication, robust appointment management, payment handling, doctor–patient workflows, and more.

🚀 Features
👤 Authentication & Roles

Register & Login using JWT

Roles:

Admin

Doctor

Patient

🧑‍⚕️ Patient Features

View personal profile (/patient/me)

Book appointments using Appointment Factory (Regular, Follow-Up, Emergency)

Cancel appointments

Make payments (Cash / Credit Card)

View payment receipts

Payment history

Each new patient receives a random wallet balance

🩺 Doctor Features

Manage personal profile

View schedule / appointments

View assigned specialties

Update specialties

Doctor dashboard APIs

🛠 Admin Features

Add / remove doctors

Add specialties

View all doctors

Manage appointments

Manage payments (dashboard)

Global audit logging using Singleton Logger

🧠 Applied Design Patterns
✔ Singleton Pattern

Used to implement Logger.Instance to centralize logging across all controllers.
Logs display in Console only, no files used.

✔ Strategy Pattern (Payments)

Used to handle different payment strategies:

CashPaymentStrategy

CreditCardPaymentStrategy (validated through proxy)

Allows flexible extension of payment types.

✔ Proxy Pattern (Credit Card Validation)

CreditCardProxy validates:

Card number

CVV

Expiry date
Using dummy stored card list.

The real payment strategy is executed only if the proxy approves.

✔ Simple Factory Pattern (Appointments)

AppointmentFactory creates appointment objects based on type:

Regular

Emergency (adds fee multiplier)

Follow-Up (discount applied)

Ensures consistent creation logic across the system.

📦 Technologies Used

ASP.NET Core Web API (.NET 9)

Entity Framework Core

SQL Server

JWT Authentication

Design Patterns (Factory, Strategy, Proxy, Singleton)

LINQ, Repository Pattern, Unit of Work

📁 Project Architecture

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
  AppDbContext
  /Migrations

💳 Payment Flow Summary

Patient sends payment request

PaymentContext selects proper strategy

If credit card → request passes to CreditCardProxy

Proxy validates card

If valid:
✔ Amount deducted
✔ Appointment status updated
✔ Payment saved

Receipt returned  
