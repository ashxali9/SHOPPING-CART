using System;

class Product
{
    public int Id;
    public string Name;
    public double Price;
    public int RemainingStock;

    public void DisplayProduct()
    {
        Console.WriteLine($"ID: {Id}, Name: {Name}, Price: {Price}, Remaining Stock: {RemainingStock}");
    }

    public double GetItemTotal(int quantity)
    {
        return Price * quantity;
    }

    public bool HasEnoughStock(int quantity)
    {
        return RemainingStock >= quantity;
    }

    public void DeductStock(int quantity)
    {
        if (HasEnoughStock(quantity))
        {
            RemainingStock -= quantity;
        }
        else
        {
            Console.WriteLine("Not enough stock to deduct.");
        }
    }
}

class CartItem
{
    public Product Product;
    public int Quantity;
    public double SubTotal;

    public void ComputeSubTotal()
    {
        SubTotal = Product.GetItemTotal(Quantity);
    }

    public void AddQuantity(int quantity)
    {
        if (Product.HasEnoughStock(quantity))
        {
            Quantity += quantity;
            ComputeSubTotal();
            Product.DeductStock(quantity);
        }
        else
        {
            Console.WriteLine("Not enough stock to add to cart.");
        }
    }
}

class Program
{
    // STORE MENU
    static void Main(string[] args)
    {
        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Laptop", Price = 30000, RemainingStock = 100 },
            new Product { Id = 2, Name = "Smartphone", Price = 17000, RemainingStock = 200 },
            new Product { Id = 3, Name = "Charger", Price = 1000, RemainingStock = 150 },
            new Product { Id = 4, Name = "Headphones", Price = 1500, RemainingStock = 250 },
            new Product { Id = 5, Name = "Tablet", Price = 20000, RemainingStock = 150 }
        };

        // CART (FIXED SIZE)
        CartItem[] cart = new CartItem[99];
        int cartIndex = 0;

        string choice = "";

        do
        {
            Console.WriteLine("Welcome to the Store!");
            Console.WriteLine("=== STORE MENU ===");
            foreach (var product in products)
            {
                product.DisplayProduct();
            }

            // INPUT PRODUCT ID
            Console.Write("Enter the Product ID to add to cart: ");
            if (!int.TryParse(Console.ReadLine(), out int productId) || productId < 1 || productId > products.Length)
            {
                Console.WriteLine("Invalid Product ID. Please try again.");
                continue;
            }

            Product selectedProduct = products[productId - 1];

            // CHECK OUT OF STOCK
            if (selectedProduct.RemainingStock < 1)
            {
                Console.WriteLine("Sorry, this product is out of stock.");
                continue;
            }

            // INPUT QUANTITY
            Console.Write("Enter the quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 1)
            {
                Console.WriteLine("Invalid quantity. Please try again.");
                continue;
            }

            // CHECK IF ENOUGH STOCK
            if (!selectedProduct.HasEnoughStock(quantity))
            {
                Console.WriteLine("Not enough stock available. Please try again.");
                continue;
            }

            // CHECK IF PRODUCT ALREADY IN CART
            bool found = false;
            for (int i = 0; i < cartIndex; i++)
            {
                if (cart[i].Product.Id == selectedProduct.Id)
                {
                    cart[i].ComputeSubTotal();
                    found = true;
                    break;
                }
            }

            // ADD NEW ITEM IF NOT FOUND
            if (!found)
            {
                if (cartIndex >= cart.Length)
                {
                    Console.WriteLine("Cart is full. Cannot add more items.");
                    continue;
                }
                cart[cartIndex] = new CartItem
                {
                    Product = selectedProduct,
                    Quantity = quantity,
                    SubTotal = selectedProduct.GetItemTotal(quantity)
};
                cartIndex++;
                
                // DEDUCT STOCK
                selectedProduct.DeductStock(quantity);

                Console.WriteLine("Item added to cart successfully!");
                Console.Write("\n Do you want to add more items? (Y/N): ");
                choice = Console.ReadLine().ToUpper();
            }
        }
        while (choice != "N");

        // DISPLAY RECEIPT
        Console.WriteLine("\n=== RECEIPT ===");
        double totalAmount = 0;
        for (int i = 0; i < cartIndex; i++)
        {
            Console.WriteLine($"Product: {cart[i].Product.Name}, Quantity: {cart[i].Quantity}, Subtotal: {cart[i].SubTotal}");
            totalAmount += cart[i].SubTotal;
        }
        Console.WriteLine($"Total Amount: {totalAmount}");

        // APPLY DISCOUNT
        double discount = 0;
        if (totalAmount > 5000)
        {
            discount = totalAmount * 0.10;
            Console.WriteLine($"Discount Applied: {discount}");
        }
        double finalAmount = totalAmount - discount;
        Console.WriteLine($"Final Amount to Pay: {finalAmount}");

        // DISPLAY UPDATED STOCK
        Console.WriteLine("\n=== UPDATED STOCK ===");
        foreach (var product in products)
        {
            Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Remaining Stock: {product.RemainingStock}");
        }
        Console.WriteLine("Thank you for shopping with us!");
    }
}