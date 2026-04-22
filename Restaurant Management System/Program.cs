using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace RestaurantSystem
{
    class Program
    {
        static List<MenuItem> menu = new List<MenuItem>();
        static List<Order> orders = new List<Order>();
        static int orderCounter = 1;

        static string menuFile = "menu.txt";
        static string orderFile = "orders.txt";
        static string feedbackFile = "feedback.txt";
        static string staffFile = "staff.txt";
        static string userFile = "users.txt";

        static void Main(string[] args)
        {
            Console.Title = "Restaurant Management System";
            LoadData();
            SeedMenu();

            while (true)
            {
                UI.Header("MAIN NAVIGATION");
                Console.WriteLine("\n\t [1] Admin Portal");
                Console.WriteLine("\t [2] Staff Portal");
                Console.WriteLine("\t [3] Customer Panel");
                Console.WriteLine("\t [4] Exit System");
                UI.Line();
                Console.Write("\t Select Choice: ");

                int choice = GetInt();
                if (choice == 1) AdminPanel();
                else if (choice == 2) StaffPanel();
                else if (choice == 3) CustomerAuth();
                else if (choice == 4) break;
            }
        }

        #region UI_HELPER
        static class UI
        {
            public static void Header(string title)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                // Updated branding to RESTAURANT MANAGEMENT
                Console.WriteLine(@"
   █▀█ █▀▀ █▀ ▀█▀ ▄▀█ █░█ █▀█ ▄▀█ █▄░█ ▀█▀   █▀▄▀█ ▄▀█ █▄░█ ▄▀█ █▀▀ █▀▀ █▀▄▀█ █▀▀ █▄░█ ▀█▀
   █▀▄ ██▄ ▄█ ░█░ █▀█ █▄█ █▀▄ █▀█ █░▀█ ░█░   █░▀░█ █▀█ █░▀█ █▀█ █▄█ ██▄ █░▀░█ ██▄ █░▀█ ░█░");
                Console.WriteLine("\t===============================================================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\t   >> {title} <<");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\t===============================================================================");
                Console.ResetColor();
            }

            public static void Line() => Console.WriteLine("\t-------------------------------------------------------------------------------");

            public static void Success(string msg) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("\n\t✅ " + msg); Console.ResetColor(); }
            public static void Error(string msg) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("\n\t❌ " + msg); Console.ResetColor(); }
        }
        #endregion

        static void LoadData()
        {
            if (File.Exists(menuFile))
            {
                foreach (var line in File.ReadAllLines(menuFile))
                {
                    var d = line.Split(',');
                    if (d.Length >= 3 && int.TryParse(d[0], out int id) && double.TryParse(d[2], out double price))
                        menu.Add(new MenuItem(id, d[1], price));
                }
            }

            if (File.Exists(orderFile))
            {
                foreach (var line in File.ReadAllLines(orderFile))
                {
                    var d = line.Split(',');
                    if (d.Length >= 6)
                    {
                        orders.Add(new Order
                        {
                            OrderId = int.Parse(d[0]),
                            Total = double.Parse(d[1]),
                            Status = d[2],
                            CustomerName = d[3],
                            CustomerPhone = d[4],
                            DeliveryAddress = d[5]
                        });
                        orderCounter = Math.Max(orderCounter, int.Parse(d[0]) + 1);
                    }
                }
            }
        }

        static void SaveMenu() => File.WriteAllLines(menuFile, menu.Select(m => $"{m.Id},{m.Name},{m.Price}"));
        static void SaveOrders() => File.WriteAllLines(orderFile, orders.Select(o => $"{o.OrderId},{o.Total},{o.Status},{o.CustomerName},{o.CustomerPhone},{o.DeliveryAddress}"));

        static void CustomerAuth()
        {
            UI.Header("CUSTOMER ACCESS");
            Console.WriteLine("\n\t [1] Login");
            Console.WriteLine("\t [2] Register New Account");
            Console.WriteLine("\t [3] Return Home");
            int ch = GetInt();
            if (ch == 1) { if (CustomerLogin()) CustomerPanel(); }
            else if (ch == 2) CustomerRegister();
        }

        static void CustomerRegister()
        {
            UI.Header("ACCOUNT REGISTRATION");
            Console.Write("\tEnter ID: "); string id = Console.ReadLine();
            Console.Write("\tEnter Username: "); string user = Console.ReadLine();
            Console.Write("\tEnter Password: "); string pass = Console.ReadLine();
            Console.Write("\tEnter Phone: "); string phone = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(pass))
            {
                File.AppendAllText(userFile, $"{id},{user},Customer,{pass},{phone}\n");
                UI.Success("Registration Successful! Please login.");
            }
            else UI.Error("Invalid Input.");
            Pause();
        }

        static bool CustomerLogin()
        {
            UI.Header("CUSTOMER LOGIN");
            Console.Write("\tUsername: "); string user = Console.ReadLine();
            Console.Write("\tPassword: "); string pass = Console.ReadLine();

            if (File.Exists(userFile))
            {
                var users = File.ReadAllLines(userFile);
                foreach (var line in users)
                {
                    var d = line.Split(',');
                    if (d.Length >= 4 && d[1].Trim() == user && d[3].Trim() == pass) return true;
                }
            }
            UI.Error("Invalid Credentials!");
            Pause();
            return false;
        }

        static void AdminPanel()
        {
            if (!Login("admin", "1234")) return;
            while (true)
            {
                UI.Header("ADMINISTRATOR PANEL");
                Console.WriteLine("\t[1] Add New Item\t[2] View Menu Card");
                Console.WriteLine("\t[3] Remove Item\t\t[4] Manage Staff");
                Console.WriteLine("\t[5] View Feedback\t[6] Logout");
                UI.Line();
                int ch = GetInt();
                if (ch == 1)
                {
                    Console.Write("\tItem ID: "); int id = GetInt();
                    if (menu.Any(x => x.Id == id)) { UI.Error("ID already exists!"); Pause(); continue; }
                    Console.Write("\tItem Name: "); string name = Console.ReadLine();
                    Console.Write("\tItem Price: "); double price = GetDouble();
                    menu.Add(new MenuItem(id, name, price));
                    SaveMenu();
                    UI.Success("Item added to catalog."); Pause();
                }
                else if (ch == 2) { ShowMenu(); Pause(); }
                else if (ch == 3)
                {
                    Console.Write("\tID to Delete: "); int id = GetInt();
                    menu.RemoveAll(x => x.Id == id);
                    SaveMenu();
                    UI.Success("Item removed."); Pause();
                }
                else if (ch == 4) ManageStaff();
                else if (ch == 5) ViewFeedback();
                else break;
            }
        }

        static void StaffPanel()
        {
            if (!Login("staff", "1234")) return;
            while (true)
            {
                UI.Header("STAFF OPERATIONS");
                Console.WriteLine("\t[1] Active Orders\t[2] Update Status");
                Console.WriteLine("\t[3] Search Order\t[4] Sales Report");
                Console.WriteLine("\t[5] Logout");
                UI.Line();
                int ch = GetInt();
                if (ch == 1) ViewOrders();
                else if (ch == 2) UpdateStatus();
                else if (ch == 3) SearchOrder();
                else if (ch == 4) ShowSales();
                else break;
            }
        }

        static void CustomerPanel()
        {
            while (true)
            {
                UI.Header("CUSTOMER DASHBOARD");
                Console.WriteLine("\t[1] Browse Menu Card");
                Console.WriteLine("\t[2] Place New Order");
                Console.WriteLine("\t[3] Track My Order");
                Console.WriteLine("\t[4] Send Feedback");
                Console.WriteLine("\t[5] Sign Out");
                UI.Line();
                int ch = GetInt();
                if (ch == 1) { ShowMenu(); Pause(); }
                else if (ch == 2) PlaceOrder();
                else if (ch == 3) TrackOrderStatus();
                else if (ch == 4) GiveFeedback();
                else break;
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n\t┌──────┬─────────────────┬──────────┐");
            Console.WriteLine("\t│  ID  │    ITEM NAME    │  PRICE   │");
            Console.WriteLine("\t├──────┼─────────────────┼──────────┤");
            foreach (var i in menu)
            {
                Console.WriteLine("\t│ {0,-4} │ {1,-15} │ Rs.{2,-5} │", i.Id, i.Name, i.Price);
            }
            Console.WriteLine("\t└──────┴─────────────────┴──────────┘");
        }

        static void ViewOrders()
        {
            UI.Header("ORDER MONITORING");
            if (orders.Count == 0) UI.Error("No records found.");
            else
            {
                foreach (var o in orders)
                {
                    Console.ForegroundColor = o.Status == "Served" ? ConsoleColor.Green : ConsoleColor.Yellow;
                    Console.WriteLine($"\tID: {o.OrderId} | {o.Status} | Bill: Rs.{o.Total}");
                    Console.ResetColor();
                    Console.WriteLine($"\tCustomer: {o.CustomerName} ({o.CustomerPhone})");
                    Console.WriteLine($"\tAddress : {o.DeliveryAddress}");
                    UI.Line();
                }
            }
            Pause();
        }

        static void PlaceOrder()
        {
            UI.Header("CREATE NEW ORDER");
            ShowMenu();
            var cart = new List<(MenuItem Item, int Qty)>();
            double total = 0;

            while (true)
            {
                Console.Write("\n\tEnter Product ID (0 to finish): ");
                int pid = GetInt();
                if (pid == 0) break;
                var item = menu.FirstOrDefault(x => x.Id == pid);
                if (item == null) { UI.Error("Not found!"); continue; }

                Console.Write($"\tQty for {item.Name}: ");
                int qty = GetInt();
                if (qty <= 0) continue;

                cart.Add((item, qty));
                total += (item.Price * qty);
                UI.Success($"{item.Name} added.");
            }

            if (cart.Count > 0)
            {
                Console.Write("\n\tYour Name: "); string name = Console.ReadLine();
                Console.Write("\tPhone Number: "); string phone = Console.ReadLine();
                Console.Write("\tAddress: "); string addr = Console.ReadLine();

                Order newOrder = new Order
                {
                    OrderId = orderCounter++,
                    Total = total,
                    Status = "Pending",
                    CustomerName = name,
                    CustomerPhone = phone,
                    DeliveryAddress = addr
                };
                orders.Add(newOrder);
                SaveOrders();

                UI.Header("ORDER RECEIPT");
                Console.WriteLine($"\tOrder ID  : {newOrder.OrderId}");
                Console.WriteLine($"\tCustomer  : {name}");
                UI.Line();
                foreach (var c in cart) Console.WriteLine($"\t{c.Item.Name,-15} x{c.Qty} = Rs.{c.Item.Price * c.Qty}");
                UI.Line();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\tGRAND TOTAL: Rs.{total}");
                Console.ResetColor();
                UI.Success("Order placed successfully!");
            }
            Pause();
        }

        static void UpdateStatus()
        {
            Console.Write("\tOrder ID: "); int id = GetInt();
            var order = orders.FirstOrDefault(x => x.OrderId == id);
            if (order == null) { UI.Error("Not found!"); Pause(); return; }
            Console.WriteLine("\t1.Pending 2.Preparing 3.Served");
            int s = GetInt();
            order.Status = s == 1 ? "Pending" : s == 2 ? "Preparing" : "Served";
            SaveOrders();
            UI.Success("Status synced."); Pause();
        }

        static bool Login(string u, string p)
        {
            UI.Header("SECURE ACCESS");
            Console.Write("\tUsername: "); string user = Console.ReadLine();
            Console.Write("\tPassword: "); string pass = Console.ReadLine();
            if (user == u && pass == p) return true;
            UI.Error("Access Denied!"); Pause(); return false;
        }

        static void ManageStaff()
        {
            while (true)
            {
                UI.Header("STAFF DIRECTORY");
                Console.WriteLine("\t[1] Add Staff\t[2] View All\t[3] Remove\t[4] Back");
                int ch = GetInt();
                if (ch == 1)
                {
                    Console.Write("\tName: "); string n = Console.ReadLine();
                    File.AppendAllText(staffFile, n + "\n"); UI.Success("Added."); Pause();
                }
                else if (ch == 2)
                {
                    if (File.Exists(staffFile)) foreach (var s in File.ReadAllLines(staffFile)) Console.WriteLine("\t- " + s);
                    Pause();
                }
                else if (ch == 3)
                {
                    var list = File.ReadAllLines(staffFile).ToList();
                    Console.Write("\tName to remove: "); string r = Console.ReadLine();
                    if (list.Remove(r)) { File.WriteAllLines(staffFile, list); UI.Success("Removed."); }
                    Pause();
                }
                else break;
            }
        }

        static void TrackOrderStatus()
        {
            Console.Write("\tOrder ID: "); int id = GetInt();
            var o = orders.FirstOrDefault(x => x.OrderId == id);
            if (o != null) { UI.Success($"Order Status: {o.Status}"); }
            else UI.Error("ID not found.");
            Pause();
        }

        static void ShowSales() { UI.Success($"Total Revenue: Rs.{orders.Sum(x => x.Total)}"); Pause(); }
        static void GiveFeedback() { Console.Write("\tFeedback: "); File.AppendAllText(feedbackFile, Console.ReadLine() + "\n"); UI.Success("Thank you!"); Pause(); }
        static void ViewFeedback() { UI.Header("GUEST FEEDBACK"); if (File.Exists(feedbackFile)) foreach (var f in File.ReadAllLines(feedbackFile)) Console.WriteLine("\t- " + f); Pause(); }
        static void SeedMenu() { if (menu.Count == 0) { menu.Add(new MenuItem(1, "Burger", 500)); menu.Add(new MenuItem(2, "Pizza", 1200)); SaveMenu(); } }
        static int GetInt() { int n; while (!int.TryParse(Console.ReadLine(), out n)) Console.Write("\tEnter valid number: "); return n; }
        static double GetDouble() { double n; while (!double.TryParse(Console.ReadLine(), out n)) Console.Write("\tEnter valid price: "); return n; }
        static void Pause() { Console.WriteLine("\n\tPress any key to continue..."); Console.ReadKey(); }
        static void SearchOrder()
        {
            Console.Write("\tOrder ID: "); int id = GetInt();
            var o = orders.FirstOrDefault(x => x.OrderId == id);
            if (o != null) Console.WriteLine($"\tFound: {o.CustomerName} - Rs.{o.Total} ({o.Status})");
            else UI.Error("No record."); Pause();
        }
    }

    class MenuItem { public int Id; public string Name; public double Price; public MenuItem(int id, string name, double price) { Id = id; Name = name; Price = price; } }
    class Order { public int OrderId; public double Total; public string Status = "Pending"; public string CustomerName; public string CustomerPhone; public string DeliveryAddress; }
}