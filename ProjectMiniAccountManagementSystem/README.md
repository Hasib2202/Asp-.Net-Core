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

- Login Page  
- Dashboard with Role-Based Access  
- Chart of Accounts UI  
- Voucher Entry Form  
- Export to Excel Button

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




