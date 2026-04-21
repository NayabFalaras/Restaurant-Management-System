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

        static void Main(string[] args)
        {
            LoadData();
            SeedMenu();

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("======================================");
                Console.WriteLine("      🍽️  RESTAURANT MANAGEMENT      ");
                Console.WriteLine("======================================");
                Console.ResetColor();

                Console.WriteLine("1. Admin Login");
                Console.WriteLine("2. Staff Login");
                Console.WriteLine("3. Customer Panel");
                Console.WriteLine("4. Exit");
                Console.WriteLine("--------------------------------------");
                Console.Write("Select Option: ");

                int choice = GetInt();

                if (choice == 1) AdminPanel();
                else if (choice == 2) StaffPanel();
                else if (choice == 3) CustomerPanel();
                else if (choice == 4) break;
            }
        }

        static void LoadData()
        {
            if (File.Exists(menuFile))
            {
                foreach (var line in File.ReadAllLines(menuFile))
                {
                    var d = line.Split(',');
                    int id;
                    double price;
                    if (d.Length >= 3 && int.TryParse(d[0], out id) && double.TryParse(d[2], out price))
                    {
                        menu.Add(new MenuItem(id, d[1], price));
                    }
                }
            }

            if (File.Exists(orderFile))
            {
                foreach (var line in File.ReadAllLines(orderFile))
                {
                    var d = line.Split(',');
                    int id;
                    double total;
                    if (d.Length >= 3 && int.TryParse(d[0], out id) && double.TryParse(d[1], out total))
                    {
                        orders.Add(new Order { OrderId = id, Total = total, Status = d[2] });
                        orderCounter = Math.Max(orderCounter, id + 1);
                    }
                }
            }
        }

        static void SaveMenu() => File.WriteAllLines(menuFile, menu.Select(m => $"{m.Id},{m.Name},{m.Price}"));
        static void SaveOrders() => File.WriteAllLines(orderFile, orders.Select(o => $"{o.OrderId},{o.Total},{o.Status}"));

        static void AdminPanel()
        {
            if (!Login("admin", "1234")) return;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== ADMIN PANEL ====");
                Console.WriteLine("1. Add Item\n2. View Menu\n3. Delete Item\n4. Manage Staff\n5. View Feedback\n6. Back");
                int ch = GetInt();
                if (ch == 1)
                {
                    Console.Write("ID: "); int id = GetInt();
                    if (menu.Any(x => x.Id == id)) { Console.WriteLine("❌ ID exists!"); Pause(); continue; }
                    Console.Write("Name: "); string name = Console.ReadLine();
                    Console.Write("Price: "); double price = GetDouble();
                    menu.Add(new MenuItem(id, name, price));
                    SaveMenu();
                    Console.WriteLine("✅ Item Added!"); Pause();
                }
                else if (ch == 2) { ShowMenu(); Pause(); }
                else if (ch == 3)
                {
                    Console.Write("Enter ID to Delete: "); int id = GetInt();
                    menu.RemoveAll(x => x.Id == id);
                    SaveMenu();
                    Console.WriteLine("✅ Deleted!"); Pause();
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
                Console.Clear();
                Console.WriteLine("==== STAFF PANEL ====");
                Console.WriteLine("1. View Orders\n2. Update Order Status\n3. Search Order\n4. Total Sales\n5. Back");
                int ch = GetInt();
                if (ch == 1) ViewOrders();
                else if (ch == 2) UpdateStatus();
                else if (ch == 3) SearchOrder();
                else if (ch == 4) ShowSales();
                else break;
            }
        }

        // ================= UPDATED CUSTOMER PANEL WITH STATUS OPTION =================
        static void CustomerPanel()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("==== CUSTOMER PANEL ====");
                Console.ResetColor();
                Console.WriteLine("1. View Menu");
                Console.WriteLine("2. Place Order");
                Console.WriteLine("3. Track Order Status"); // Naya Option
                Console.WriteLine("4. Give Feedback");
                Console.WriteLine("5. Back");
                int ch = GetInt();
                if (ch == 1) { ShowMenu(); Pause(); }
                else if (ch == 2) PlaceOrder();
                else if (ch == 3) TrackOrderStatus(); // Naya function call
                else if (ch == 4) GiveFeedback();
                else break;
            }
        }

        static void TrackOrderStatus()
        {
            Console.Clear();
            Console.Write("Enter your Order ID: ");
            int id = GetInt();
            var order = orders.FirstOrDefault(x => x.OrderId == id);

            if (order != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n----------------------------");
                Console.WriteLine($"Order ID: {order.OrderId}");
                Console.WriteLine($"Current Status: {order.Status}");
                Console.WriteLine("----------------------------");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("❌ Order ID not found!");
            }
            Pause();
        }

        static void PlaceOrder()
        {
            Console.Clear();
            ShowMenu();

            var currentCart = new List<(MenuItem Item, int Qty)>();
            double billTotal = 0;

            while (true)
            {
                Console.WriteLine("\n--- Add Items to Cart ---");
                Console.Write("Enter Product ID (or 0 to Finish Order): ");
                int pid = GetInt();
                if (pid == 0) break;

                var item = menu.FirstOrDefault(x => x.Id == pid);
                if (item == null)
                {
                    Console.WriteLine("❌ Invalid Product ID! Try again.");
                    continue;
                }

                Console.Write($"Enter Quantity for {item.Name}: ");
                int qty = GetInt();
                if (qty <= 0)
                {
                    Console.WriteLine("❌ Invalid Quantity!");
                    continue;
                }

                currentCart.Add((item, qty));
                billTotal += (item.Price * qty);
                Console.WriteLine($"🛒 Added {qty}x {item.Name} to cart.");
            }

            if (currentCart.Count == 0)
            {
                Console.WriteLine("No items selected. Returning to menu...");
                Pause();
                return;
            }

            Order newOrder = new Order { OrderId = orderCounter++, Total = billTotal, Status = "Pending" };
            orders.Add(newOrder);
            SaveOrders();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("==========================================");
            Console.WriteLine("          🌟 OFFICIAL RECEIPT 🌟         ");
            Console.WriteLine("==========================================");
            Console.ResetColor();

            Console.WriteLine($"Order ID  : {newOrder.OrderId}");
            Console.WriteLine($"Date/Time : {DateTime.Now.ToString("f")}");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("{0,-18} {1,-7} {2,-15}", "Item Name", "Qty", "Subtotal");
            Console.WriteLine("------------------------------------------");

            foreach (var cartItem in currentCart)
            {
                double subtotal = cartItem.Item.Price * cartItem.Qty;
                Console.WriteLine("{0,-18} {1,-7} Rs.{2,-15}",
                    cartItem.Item.Name, cartItem.Qty, subtotal);
            }

            Console.WriteLine("------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"GRAND TOTAL:            Rs.{newOrder.Total}");
            Console.ResetColor();
            Console.WriteLine("==========================================");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("      🙏 THANK YOU FOR ORDERING! 🙏      ");
            Console.WriteLine("       We hope to see you again soon!     ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("==========================================");
            Console.ResetColor();

            Pause();
        }

        static void ManageStaff()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("==== STAFF MANAGEMENT ====");
                Console.ResetColor();
                Console.WriteLine("1. Add New Staff member");
                Console.WriteLine("2. View All Staff members");
                Console.WriteLine("3. Remove a Staff member");
                Console.WriteLine("4. Back");
                Console.Write("\nChoose an option: ");

                int choice = GetInt();

                if (choice == 1)
                {
                    Console.Write("Enter Staff Name: ");
                    string name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        File.AppendAllText(staffFile, name + "\n");
                        Console.WriteLine("✅ Staff Added!");
                    }
                    Pause();
                }
                else if (choice == 2)
                {
                    Console.Clear();
                    Console.WriteLine("--- Staff List ---");
                    if (File.Exists(staffFile))
                    {
                        var staff = File.ReadAllLines(staffFile);
                        if (staff.Length == 0) Console.WriteLine("List is empty.");
                        else foreach (var s in staff) Console.WriteLine("- " + s);
                    }
                    else Console.WriteLine("No staff file found.");
                    Pause();
                }
                else if (choice == 3)
                {
                    if (File.Exists(staffFile))
                    {
                        var staffList = File.ReadAllLines(staffFile).ToList();
                        Console.Write("Enter name to remove: ");
                        string toRemove = Console.ReadLine();
                        if (staffList.Remove(toRemove))
                        {
                            File.WriteAllLines(staffFile, staffList);
                            Console.WriteLine("✅ Removed successfully.");
                        }
                        else Console.WriteLine("❌ Name not found.");
                    }
                    Pause();
                }
                else break;
            }
        }

        static void ViewOrders()
        {
            Console.Clear();
            if (orders.Count == 0) Console.WriteLine("No orders found!");
            else foreach (var o in orders) Console.WriteLine($"ID: {o.OrderId} | Rs.{o.Total} | Status: {o.Status}");
            Pause();
        }

        static void UpdateStatus()
        {
            Console.Write("Order ID: "); int id = GetInt();
            var order = orders.FirstOrDefault(x => x.OrderId == id);
            if (order == null) { Console.WriteLine("❌ Not found!"); Pause(); return; }
            Console.WriteLine("1.Pending 2.Preparing 3.Served");
            int s = GetInt();
            order.Status = s == 1 ? "Pending" : s == 2 ? "Preparing" : "Served";
            SaveOrders();
            Console.WriteLine("✅ Status Updated!"); Pause();
        }

        static void SearchOrder()
        {
            Console.Write("Order ID: "); int id = GetInt();
            var o = orders.FirstOrDefault(x => x.OrderId == id);
            if (o != null) Console.WriteLine($"ID: {o.OrderId} | Total: Rs.{o.Total} | Status: {o.Status}");
            else Console.WriteLine("❌ Not found!");
            Pause();
        }

        static void ShowSales() { Console.WriteLine($"💰 Total Sales: Rs.{orders.Sum(x => x.Total)}"); Pause(); }

        static void GiveFeedback()
        {
            Console.Write("Your Feedback: ");
            File.AppendAllText(feedbackFile, Console.ReadLine() + "\n");
            Console.WriteLine("✅ Thank you for your feedback!"); Pause();
        }

        static void ViewFeedback()
        {
            if (File.Exists(feedbackFile))
                foreach (var f in File.ReadAllLines(feedbackFile)) Console.WriteLine("- " + f);
            else Console.WriteLine("No feedback yet.");
            Pause();
        }

        static bool Login(string u, string p)
        {
            Console.Write("Username: "); string user = Console.ReadLine();
            Console.Write("Password: "); string pass = Console.ReadLine();
            if (user == u && pass == p) return true;
            Console.WriteLine("❌ Access Denied!"); Pause(); return false;
        }

        static int GetInt() { int n; while (!int.TryParse(Console.ReadLine(), out n)) Console.Write("Enter valid number: "); return n; }
        static double GetDouble() { double n; while (!double.TryParse(Console.ReadLine(), out n)) Console.Write("Enter valid price: "); return n; }
        static void Pause() { Console.WriteLine("\nPress any key to continue..."); Console.ReadKey(); }

        static void ShowMenu()
        {
            Console.WriteLine("\n--- MENU CARD ---");
            Console.WriteLine("{0,-5} {1,-15} {2,-10}", "ID", "Name", "Price");
            foreach (var i in menu) Console.WriteLine("{0,-5} {1,-15} Rs.{2,-10}", i.Id, i.Name, i.Price);
        }

        static void SeedMenu()
        {
            if (menu.Count == 0)
            {
                menu.Add(new MenuItem(1, "Burger", 500));
                menu.Add(new MenuItem(2, "Pizza", 1200));
                menu.Add(new MenuItem(3, "Pasta", 800));
                menu.Add(new MenuItem(4, "Coke", 150));
                SaveMenu();
            }
        }
    }

    class MenuItem
    {
        public int Id; public string Name; public double Price;
        public MenuItem(int id, string name, double price) { Id = id; Name = name; Price = price; }
    }

    class Order { public int OrderId; public double Total; public string Status = "Pending"; }
}