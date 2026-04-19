using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantSystem
{
    class Program
    {
        static List<MenuItem> menu = new List<MenuItem>();
        static List<Order> orders = new List<Order>();
        static int orderCounter = 1;

        static void Main(string[] args)
        {
            SeedMenu();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== RESTAURANT POS SYSTEM ====");
                Console.WriteLine("1. Admin Login");
                Console.WriteLine("2. Staff Login");
                Console.WriteLine("3. Customer");
                Console.WriteLine("4. Exit");

                int choice = GetInt();

                switch (choice)
                {
                    case 1: AdminPanel(); break;
                    case 2: StaffPanel(); break;
                    case 3: CustomerPanel(); break;
                    case 4: return;
                }
            }
        }

        // ================= ADMIN =================
        static void AdminPanel()
        {
            if (!Login("admin", "1234")) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== ADMIN PANEL ====");
                Console.WriteLine("1. Add Item");
                Console.WriteLine("2. View Menu");
                Console.WriteLine("3. Update Item");
                Console.WriteLine("4. Delete Item");
                Console.WriteLine("5. Reports");
                Console.WriteLine("6. Back");

                int ch = GetInt();

                if (ch == 1)
                {
                    Console.Write("Enter ID: ");
                    int id = GetInt();

                    if (menu.Any(x => x.Id == id))
                    {
                        Console.WriteLine("❌ ID already exists!");
                        Pause();
                        continue;
                    }

                    Console.Write("Enter Name: ");
                    string name = Console.ReadLine();

                    Console.Write("Enter Price: ");
                    double price = GetDouble();

                    menu.Add(new MenuItem(id, name, price));
                    Console.WriteLine("✅ Item Added Successfully!");
                    Pause();
                }
                else if (ch == 2)
                {
                    ShowMenu();
                    Pause();
                }
                else if (ch == 3)
                {
                    Console.Write("Enter ID to update: ");
                    int id = GetInt();

                    var item = menu.Find(x => x.Id == id);

                    if (item != null)
                    {
                        Console.Write("Enter New Name: ");
                        item.Name = Console.ReadLine();

                        Console.Write("Enter New Price: ");
                        item.Price = GetDouble();

                        Console.WriteLine("✅ Item Updated!");
                    }
                    else
                        Console.WriteLine("❌ Item not found!");

                    Pause();
                }
                else if (ch == 4)
                {
                    Console.Write("Enter ID to delete: ");
                    int id = GetInt();

                    var item = menu.Find(x => x.Id == id);

                    if (item != null)
                    {
                        Console.Write("Are you sure? (y/n): ");
                        string confirm = Console.ReadLine().ToLower();

                        if (confirm == "y")
                        {
                            menu.Remove(item);
                            Console.WriteLine("✅ Item Deleted!");
                        }
                        else
                        {
                            Console.WriteLine("❌ Delete Cancelled.");
                        }
                    }
                    else
                        Console.WriteLine("❌ Item not found!");

                    Pause();
                }
                else if (ch == 5)
                {
                    double sales = orders.Sum(o => o.Total);
                    Console.WriteLine($"💰 Total Sales: Rs.{sales}");
                    Pause();
                }
                else break;
            }
        }

        // ================= STAFF =================
        static void StaffPanel()
        {
            if (!Login("staff", "1234")) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== STAFF PANEL ====");
                Console.WriteLine("1. View Menu");
                Console.WriteLine("2. View Orders");
                Console.WriteLine("3. Update Order Status");
                Console.WriteLine("4. Back");

                int ch = GetInt();

                if (ch == 1)
                {
                    ShowMenu();
                    Pause();
                }
                else if (ch == 2)
                {
                    ViewOrders();
                }
                else if (ch == 3)
                {
                    UpdateStatus();
                }
                else break;
            }
        }

        // ================= CUSTOMER =================
        static void CustomerPanel()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==== CUSTOMER PANEL ====");
                Console.WriteLine("1. View Menu");
                Console.WriteLine("2. Place Order");
                Console.WriteLine("3. Check Order Status");
                Console.WriteLine("4. Back");

                int ch = GetInt();

                if (ch == 1)
                {
                    ShowMenu();
                    Pause();
                }
                else if (ch == 2)
                {
                    PlaceOrder();
                }
                else if (ch == 3)
                {
                    CheckOrderStatus();
                }
                else break;
            }
        }

        // ================= ORDER =================
        static void PlaceOrder()
        {
            if (menu.Count == 0)
            {
                Console.WriteLine("❌ Menu is empty!");
                Pause();
                return;
            }

            Order order = new Order { OrderId = orderCounter++ };

            while (true)
            {
                Console.Clear();
                ShowMenu();

                Console.Write("\nEnter Item ID (0 to finish): ");
                int id = GetInt();

                if (id == 0) break;

                var item = menu.Find(x => x.Id == id);

                if (item == null)
                {
                    Console.WriteLine("❌ Invalid item!");
                    Pause();
                    continue;
                }

                Console.Write("Enter Quantity: ");
                int qty = GetInt();

                if (qty <= 0)
                {
                    Console.WriteLine("❌ Invalid quantity!");
                    Pause();
                    continue;
                }

                order.AddItem(item, qty);
                Console.WriteLine("✅ Added to cart!");
                Pause();
            }

            if (order.Items.Count == 0)
            {
                Console.WriteLine("❌ No order placed!");
                Pause();
                return;
            }

            orders.Add(order);

            Console.Clear();
            Console.WriteLine("🎉 ORDER PLACED SUCCESSFULLY!");
            Console.WriteLine($"Order ID: {order.OrderId}");
            Console.WriteLine($"Total Bill: Rs.{order.Total}");
            Pause();
        }

        // ================= VIEW ORDERS =================
        static void ViewOrders()
        {
            Console.Clear();

            if (orders.Count == 0)
            {
                Console.WriteLine("❌ No orders found!");
                Pause();
                return;
            }

            Console.WriteLine("==== ALL ORDERS ====");

            foreach (var o in orders)
            {
                Console.WriteLine($"\n📦 Order #{o.OrderId} - {o.Status}");

                foreach (var i in o.Items)
                    Console.WriteLine($"   {i.Item.Name} x{i.Quantity} = Rs.{i.SubTotal}");

                Console.WriteLine($"   💰 Total: Rs.{o.Total}");
            }

            Pause();
        }

        // ================= CHECK STATUS (NEW) =================
        static void CheckOrderStatus()
        {
            Console.Write("Enter your Order ID: ");
            int id = GetInt();

            var order = orders.Find(o => o.OrderId == id);

            if (order == null)
            {
                Console.WriteLine("❌ Order not found!");
                Pause();
                return;
            }

            Console.WriteLine($"\n📦 Order #{order.OrderId}");
            Console.WriteLine($"Status: {order.Status}");

            Console.WriteLine("\nItems:");
            foreach (var i in order.Items)
                Console.WriteLine($"{i.Item.Name} x{i.Quantity}");

            Console.WriteLine($"\n💰 Total: Rs.{order.Total}");

            Pause();
        }

        // ================= STATUS =================
        static void UpdateStatus()
        {
            Console.Write("Enter Order ID: ");
            int id = GetInt();

            var order = orders.Find(o => o.OrderId == id);

            if (order == null)
            {
                Console.WriteLine("❌ Order not found!");
                Pause();
                return;
            }

            Console.WriteLine("1. Pending\n2. Preparing\n3. Served");
            int s = GetInt();

            if (s == 1) order.Status = "Pending";
            else if (s == 2) order.Status = "Preparing";
            else if (s == 3) order.Status = "Served";
            else
            {
                Console.WriteLine("❌ Invalid option!");
                Pause();
                return;
            }

            Console.WriteLine("✅ Status Updated!");
            Pause();
        }

        // ================= HELPERS =================
        static bool Login(string u, string p)
        {
            Console.Write("Username: ");
            string user = Console.ReadLine();

            Console.Write("Password: ");
            string pass = Console.ReadLine();

            if (user == u && pass == p) return true;

            Console.WriteLine("❌ Invalid login!");
            Pause();
            return false;
        }

        static int GetInt()
        {
            string input = Console.ReadLine();
            int n;

            while (!int.TryParse(input, out n))
            {
                Console.Write("Enter valid number: ");
                input = Console.ReadLine();
            }

            return n;
        }

        static double GetDouble()
        {
            string input = Console.ReadLine();
            double n;

            while (!double.TryParse(input, out n))
            {
                Console.Write("Enter valid number: ");
                input = Console.ReadLine();
            }

            return n;
        }

        static void Pause()
        {
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n==== MENU ====");
            if (menu.Count == 0)
            {
                Console.WriteLine("No items available!");
                return;
            }

            foreach (var i in menu)
                Console.WriteLine($"{i.Id} - {i.Name} - Rs.{i.Price}");
        }

        static void SeedMenu()
        {
            menu.Add(new MenuItem(1, "Burger", 500));
            menu.Add(new MenuItem(2, "Pizza", 1200));
            menu.Add(new MenuItem(3, "Fries", 300));
        }
    }

    class MenuItem
    {
        public int Id;
        public string Name;
        public double Price;

        public MenuItem(int id, string name, double price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }

    class OrderItem
    {
        public MenuItem Item;
        public int Quantity;
        public double SubTotal => Item.Price * Quantity;
    }

    class Order
    {
        public int OrderId;
        public List<OrderItem> Items = new List<OrderItem>();
        public double Total = 0;
        public string Status = "Pending";

        public void AddItem(MenuItem item, int qty)
        {
            Items.Add(new OrderItem { Item = item, Quantity = qty });
            Total += item.Price * qty;
        }
    }
}