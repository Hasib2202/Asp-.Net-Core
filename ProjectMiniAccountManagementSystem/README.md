# 💼 Mini Account Management System

This is a submission for the Dot.Net task provided by **Qtec Solution Limited**. The project is built using **ASP.NET Core with Razor Pages** and **MS SQL Server** utilizing **stored procedures only**, and implements a simple but functional account management system.

---

## 📌 Project Overview

This system is designed to manage accounts, vouchers, and user roles within a small-scale accounting context. The key features are implemented without using LINQ and instead rely on stored procedures for all database operations.

---

## 🛠 Technologies Used

- **ASP.NET Core (Razor Pages)**
- **MS SQL Server**
- **Stored Procedures Only**
- **ASP.NET Identity with Custom Roles**
- **No LINQ used**

---

## 🔐 Features Implemented

### 1. User Roles & Permissions
- Role-based access (Admin, Accountant, Viewer)
- Users can be assigned roles with specific access rights to modules
- Access rights controlled using stored procedures

### 2. Chart of Accounts
- Create, Update, Delete accounts such as Cash, Bank, Receivable
- Hierarchical account structure (Parent/Child)
- Stored Procedure Used: `sp_ManageChartOfAccounts`

### 3. Voucher Entry Module
- Support for:
  - Journal Vouchers
  - Payment Vouchers
  - Receipt Vouchers
- Form includes:
  - Date
  - Reference No.
  - Multiple Debit & Credit entries
  - Dropdown-based Account Selection
- Stored Procedure Used: `sp_SaveVoucher`

---

## 🎁 Bonus Feature (Implemented)

- **Export Reports to Excel** – Users can export voucher and account reports for offline analysis.

---


---

## 🖼️ Screenshots

> Screenshots are provided below to demonstrate functionality (include actual images in your repo):

- Login Page ![Image](https://github.com/user-attachments/assets/f4506a32-f15d-481f-9a68-bb357e08afdb)
- Register Page ![Image](https://github.com/user-attachments/assets/a8d5c5c0-3f83-4356-b411-9f45a023db18)
- Admin Dahboard ![Image](https://github.com/user-attachments/assets/ba484d16-a01d-4a03-86d2-f45180d75a38)
- Dashboard with Role-Based Access  ![Image](https://github.com/user-attachments/assets/c5bd63f8-92a2-4c1f-8a8c-c8923968d2af)
- Chart of Accounts UI  ![Image](https://github.com/user-attachments/assets/003affb1-bc72-4733-bb53-d9bc8498e0fb)
- Voucher Entry Form  ![Image](https://github.com/user-attachments/assets/246fa402-da58-4309-8575-2a2efe26700b)
- Export to Excel Button ![Image](https://github.com/user-attachments/assets/2c3131d1-d82e-46da-b224-6d49ac429bf2)
- hh ![Image](https://github.com/user-attachments/assets/67519f22-cf44-4f8d-8371-bedc492526db)


---

## 🚀 How to Run

1. Clone the repository
2. Set up the database using the provided stored procedures
3. Configure connection string in `appsettings.json`
4. Run the project using Visual Studio or `dotnet run`
5. Login using seeded Admin credentials or register a new user

---


---

## 👤 Author

**Md. Mostofa Hasib**  
📧 Email: hasibammostofahasib@gmail.com
📱 Phone: 01747496866
🔗 LinkedIn: https://www.linkedin.com/in/md-mostofa-hasib-5b4027184/

---




