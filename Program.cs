using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CustomerConsoleApp
{
    public class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            client.BaseAddress = new Uri("http://localhost:5089/api/"); // Update port if necessary
            bool running = true;

            while (running)
            {
                Console.WriteLine("\nChoose an operation:");
                Console.WriteLine("1. Create Customer");
                Console.WriteLine("2. Read All Customers");
                Console.WriteLine("3. Read Customer by ID");
                Console.WriteLine("4. Update Customer");
                Console.WriteLine("5. Delete Customer");
                Console.WriteLine("6. Exit");

                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await CreateCustomer();
                        break;
                    case "2":
                        await ReadAllCustomers();
                        break;
                    case "3":
                        await ReadCustomerById();
                        break;
                    case "4":
                        await UpdateCustomer();
                        break;
                    case "5":
                        await DeleteCustomer();
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        static async Task CreateCustomer()
        {
            Console.Write("Enter First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine();

            var newCustomer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone
            };

            var response = await client.PostAsJsonAsync("Customers", newCustomer);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Customer added successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to add customer. Status code: {response.StatusCode}");
            }
        }

        static async Task ReadAllCustomers()
        {
            var customers = await client.GetFromJsonAsync<List<Customer>>("Customers");

            Console.WriteLine("\nCustomer List:");
            foreach (var customer in customers)
            {
                Console.WriteLine($"ID: {customer.CustomerID}, Name: {customer.FirstName} {customer.LastName}, Email: {customer.Email}, Phone: {customer.Phone}");
            }
        }

        static async Task ReadCustomerById()
        {
            Console.Write("Enter Customer ID: ");
            int id = int.Parse(Console.ReadLine());

            var response = await client.GetAsync($"Customers/{id}");

            if (response.IsSuccessStatusCode)
            {
                var customer = await response.Content.ReadFromJsonAsync<Customer>();
                Console.WriteLine($"ID: {customer.CustomerID}, Name: {customer.FirstName} {customer.LastName}, Email: {customer.Email}, Phone: {customer.Phone}");
            }
            else
            {
                Console.WriteLine($"Customer with ID {id} not found. Status code: {response.StatusCode}");
            }
        }

        static async Task UpdateCustomer()
        {
            Console.Write("Enter Customer ID to update: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter New Email: ");
            string email = Console.ReadLine();

            var customer = await client.GetFromJsonAsync<Customer>($"Customers/{id}");

            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            customer.Email = email;

            var response = await client.PutAsJsonAsync($"Customers/{id}", customer);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Customer updated successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to update customer. Status code: {response.StatusCode}");
            }
        }

        static async Task DeleteCustomer()
        {
            Console.Write("Enter Customer ID to delete: ");
            int id = int.Parse(Console.ReadLine());

            var response = await client.DeleteAsync($"Customers/{id}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Customer deleted successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to delete customer. Status code: {response.StatusCode}");
            }
        }
    }

    public class Customer
    {
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
