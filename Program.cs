using System;

class Product
{
    private int Id;
    private string Name = "";
    private string Category = "";
    private double Price;
    private int RemainingStock;

    public int Id{
        get { return Id; }
        set { Id = value; }
    }

    public string Name{
        get { return Name; }
        set { Name = value; }
    }

    public string Category{
        get { return Category; }
        set { Category = value; }
    }

    public double Price{
        get { return Price; }
        set { Price = value; }
    }

    public int RemainingStock{
        get { return RemainingStock; }
        set { RemainingStock = value; }
    }

    public void Display()
    {
        Console.WriteLine($"{Id}. {Name} ({Category}) - P{Price} | Stock: {RemainingStock}");
    }

    public bool HasEnoughStock(int qty)
    {
        return RemainingStock >= qty;
    }

    public void DeductStock(int qty)
    {
        RemainingStock -= qty;
    }

    public void AddStock(int qty)
    {
        RemainingStock += qty;
    }
}

class CartItem
{
    private Product product = new Product();
    private int quantity;
    private double subtotal;

    public Product Product{
        get { return product; }
        set { product = value; }
    }

    public int Quantity{
        get { return quantity; }
        set { quantity = value; }
    }

    public double SubTotal{
        get { return subtotal; }
        set { subtotal = value; }
    }

    public void UpdateSubtotal()
    {
        SubTotal = Product.Price * Quantity;
    }
}

class Order
{
    private string ReceiptNumber = "";
    private DateTime Date;
    private double FinalTotal;

    public string ReceiptNumber{
        get { return ReceiptNumber; }
        set { ReceiptNumber = value; }
    }

    public DateTime Date{
        get { return Date; }
        set { Date = value; }
    }

    public double FinalTotal{
        get { return FinalTotal; }
        set { FinalTotal = value; }
    }
}

class Program
{
    static int receiptCounter = 1;

    static void Main()
    {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 30000, RemainingStock = 5 },
            new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 500, RemainingStock = 10 },
            new Product { Id = 3, Name = "Keyboard", Category = "Electronics", Price = 1500, RemainingStock = 7 },
            new Product { Id = 4, Name = "Headphones", Category = "Electronics", Price = 2000, RemainingStock = 6 },
            new Product { Id = 5, Name = "Monitor", Category = "Electronics", Price = 10000, RemainingStock = 8 }
        };

        CartItem[] cart = new CartItem[20];
        int cartCount = 0;

        Order[] history = new Order[50];
        int historyCount = 0;

        bool running = true;

        while (running)
        {
            Console.WriteLine("\n=== MAIN MENU ===");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. View Cart");
            Console.WriteLine("3. Search Product");
            Console.WriteLine("4. Filter by Category");
            Console.WriteLine("5. View Order History");
            Console.WriteLine("6. Exit");

            int choice = GetInt("Select option: ");
            switch (choice)
            {
                case 1:
                    AddProduct(products, cart, ref cartCount);
                    break;
                case 2:
                    CartMenu(cart, ref cartCount, products, history, ref historyCount);
                    break;
                case 3:
                    SearchProduct(products);
                    break;
                case 4:
                    FilterCategory(products);
                    break;
                case 5:
                    ShowHistory(history, historyCount);
                    break;
                case 6:
                    running = false;
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    // ADD PRODUCT
    static void AddProduct(Product[] products, CartItem[] cart, ref int cartCount)
    {
        foreach (var p in products) p.Display();
        int id = GetInt("Enter product number: ");
        if (id < 1 || id > products.Length) return;
        Product selected = products[id - 1];
        int qty = GetInt("Enter quantity: ");
        if (!selected.HasEnoughStock(qty))
        {
            Console.WriteLine("Not enough stock.");
            return;
        }

        bool found = false;

        for (int i = 0; i < cartCount; i++)
        {
            if (cart[i].Product.Id == selected.Id)
            {
                cart[i].Quantity += qty;
                cart[i].UpdateSubtotal();
                found = true;
                break;
            }
        }

        if (!found)
        {
            cart[cartCount++] = new CartItem
            {
                Product = selected,
                Quantity = qty,
                SubTotal = selected.Price * qty
            };
        }

        selected.DeductStock(qty);
        Console.WriteLine("Product added to cart.");
    }

    // CART MENU
    static void CartMenu(CartItem[] cart, ref int cartCount, Product[] products, Order[] history, ref int historyCount)
    {
        bool back = false;

        while (!back){
            Console.WriteLine("\n=== CART MENU ===");
            Console.WriteLine("1. View Cart");
            Console.WriteLine("2. Update Quantity");
            Console.WriteLine("3. Remove Item");
            Console.WriteLine("4. Clear Cart");
            Console.WriteLine("5. Checkout");
            Console.WriteLine("6. Back to Main Menu");

            int choice = GetInt("Select option: ");

            switch (choice)
            {
                case 1:
                    ShowCart(cart, cartCount);
                    break;
                case 2:
                    UpdateQuantity(cart, cartCount, products);
                    break;
                case 3:
                    RemoveItem(cart, ref cartCount, products);
                    break;
                case 4:
                    ClearCart(cart, ref cartCount, products);
                    break;
                case 5:
                    Checkout(cart, ref cartCount, products, history, ref historyCount);
                    break;
                case 6:
                    back = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    // CART FUNCTIONS
    static void ShowCart(CartItem[] cart, int count)
    {
        double total = 0;

        for (int i = 0; i < count; i++){
            Console.WriteLine($"{i + 1}. {cart[i].Product.Name} x {cart[i].Quantity} = P{cart[i].SubTotal}");
            total += cart[i].SubTotal;
        }
        Console.WriteLine($"Total: P{total}");
    }
    static void UpdateQuantity(CartItem[] cart, int count, Product[] products)
    {
        int index = GetInt("Enter cart item number: ") - 1;
        if (index < 0 || index >= count) return;
        int newQty = GetInt("Enter new quantity: ");
        int diff = newQty - cart[index].Quantity;

        if (diff > 0 && !cart[index].Product.HasEnoughStock(diff))
        {
            Console.WriteLine("Not enough stock.");
            return;
        }

        cart[index].Product.DeductStock(diff);
        cart[index].Quantity = newQty;
        cart[index].UpdateSubtotal();
    }

    static void RemoveItem(CartItem[] cart, ref int count, Product[] products)
    {
        int index = GetInt("Remove item #:") - 1;
        if (index < 0 || index >= count) return;
        cart[index].Product.AddStock(cart[index].Quantity);
        for (int i = index; i < count - 1; i++)
            cart[i] = cart[i + 1];
        count--;
    }

    static void ClearCart(CartItem[] cart, ref int count, Product[] products)
    {
        for (int i = 0; i < count; i++)
            cart[i].Product.AddStock(cart[i].Quantity);
        count = 0;
        Console.WriteLine("Cart cleared.");
    }

    // CHECKOUT
    static void Checkout(CartItem[] cart, ref int count, Product[] products, Order[] history, ref int historyCount)
    {
        double total = 0;

        for (int i = 0; i < count; i++)
            total += cart[i].SubTotal;

            double discount = total >= 5000 ? total * 0.1 : 0;
            double finalTotal = total - discount;
            double payment;
            while (true){
                payment = GetInt ($"Final Total: P{finalTotal}\nEnter payment:");
                if (payment >= finalTotal) break;
                Console.WriteLine("Insufficient payment. Try again.");
            }
            double change = payment - finalTotal;

            string receiptNo = receiptCounter.ToString("D4");

            Console.WriteLine($"\n=== RECEIPT ===");
            Console.WriteLine($"Receipt No: {receiptNo}");
            Console.WriteLine($"Date: {DateTime.Now}");

            foreach (var item in cart){
                if (item != null)
                    Console.WriteLine($"{item.Product.Name} x {item.Quantity} = P{item.SubTotal}");
            }

            Console.WriteLine($"Total: P{total}");
            Console.WriteLine($"Discount: P{discount}");
            Console.WriteLine($"Final Total: P{finalTotal}");
            Console.WriteLine($"Payment: P{payment}");
            Console.WriteLine($"Change: P{change}");

            // SAVE HISTORY
            history[historyCount++] = new Order
            {
                ReceiptNumber = receiptNo,
                Date = DateTime.Now,
                FinalTotal = finalTotal
            };

            receiptCounter++;
            count = 0;

            // LOW STOCK ALERT  
            Console.WriteLine("\n=== LOW STOCK ALERT ===");
            foreach (var p in products){
                if (p.RemainingStock <= 3 )
                    Console.WriteLine($"{p.Name} only {p.RemainingStock} left.");
            }
        }

        // SEARCH & FILTER
        static void SearchProduct(Product[] products){
            Console.Write("Search: ");
            string keyword = Console.ReadLine().ToLower();

            foreach (var p in products){
                if (p.Name.ToLower().Contains(keyword))
                    p.Display();
            }
        }

        static void FilterCategory(Product[] products){
            Console.Write("Enter category: ");
            string cat = Console.ReadLine().ToLower();

            foreach (var p in products){
                if (p.Category.ToLower() == cat)
                    p.Display();
            }
        }

        // HISTORY
        static void ShowHistory(Order[] history, int count){
            Console.WriteLine("\n=== ORDER HISTORY ===");
            for (int i = 0; i < count; i++){
                Console.WriteLine($"Receipt #{history [i].ReceiptNumber} - P{history[i].FinalTotal}");
            }
        }

        // VALIDATION
        static int GetInt(string msg){
            int val;
            while (true){
                Console.Write(msg);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out val))
                    return val;
                Console.WriteLine("Invalid input. Try again.");
            }
        }
    }
