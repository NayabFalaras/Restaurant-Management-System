🍽️ Restaurant Management System (RMS)
A Comprehensive POS & Operations Suite for Modern Terminal Environments

📖 Project Overview
The Restaurant Management System is an interactive, console-based application designed to streamline the daily operations of a food establishment. It features a robust multi-user role system and utilizes File-Based Persistence to ensure that all business data remains intact between sessions.

Developed with a focus on clean logic and user experience, this tool serves as a complete Point of Sale (POS) solution for small to medium-scale restaurants.

🌟 Key Features
👤 Role-Based Portals
Admin Panel: Full control over the establishment. Manage the menu catalog (Add/Remove items), oversee staff records, and analyze customer feedback.

Staff Portal: Operational hub for employees. Monitor active orders, update live order statuses (Pending → Preparing → Served), and view total sales reports.

Customer Panel: A seamless user interface for diners to browse the digital menu, manage a virtual cart, place orders, and track their preparation status in real-time.

💾 Technical Excellence
Advanced File Handling: Implements System.IO for persistent data management. All transactions, menu updates, and user credentials are saved in structured .txt files (CSV format).

Interactive UI: Features a custom ASCII branding header and clear navigational menus to enhance the terminal experience.

Data Integrity: Includes robust input validation and error handling to ensure the application remains stable during incorrect user inputs.

🛠️ Tech Stack
Language: C# 10.0 / .NET Framework

Data Management: Flat-file system (File Handling)

IDE: Visual Studio 2022

Deployment: Docker-ready (Dockerfile included)

📂 Repository Structure
Bash
├── Restaurant Management System/
│   ├── Program.cs          # Core Business Logic & UI
│   ├── menu.txt            # Menu Data Storage
│   ├── users.txt           # Secure User Credentials
│   ├── orders.txt          # Transactional History
│   ├── feedback.txt        # Customer Reviews
│   └── Dockerfile          # Container Configuration
└── README.md               # Documentation
🚀 Getting Started
Prerequisites
Windows OS

.NET Framework 4.7.2+

Visual Studio 2022

Installation
Clone the Repository:

Bash
git clone https://github.com/NayabFalaras/Restaurant-Management-System.git
Open the Solution:
Launch the .sln file in Visual Studio.

Run the Project:
Press F5 or click the Start button to launch the terminal application.

👨‍💻 Developer
Nayab Falaras

Computer Science Student

Specializing in Full-Stack Development (Web, Mobile, & Desktop)

GitHub: @NayabFalaras
