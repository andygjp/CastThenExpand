using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddOData(options =>
    {
        options.AddRouteComponents("default", GetModel());
    });

var app = builder.Build();

app.UseODataRouteDebug().UseRouting().UseEndpoints(routeBuilder => routeBuilder.MapControllers());

app.Run();

static IEdmModel GetModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Customer>("Customers").EntityType.HasKey(x => x.ID);
    builder.EntitySet<VipCustomer>("VipCustomers").EntityType.DerivesFrom<Customer>();
    builder.EntitySet<Email>("Emails").EntityType.HasKey(x => x.ID);
    return builder.GetEdmModel();
}

public class Customer
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;

    public int EmailID { get; set; }
    public Email? Email { get; set; }
}

public class VipCustomer : Customer
{
    public int VipNumber { get; set; }
}

public class Email
{
    public int ID { get; set; }
    public string EmailAddress { get; set; } = null!;

    public int CustomerID { get; set; }
    public Customer? Customer { get; set; }
}

public static class Data
{
    public static IEnumerable<Customer> Customers()
    {
        return new[]
        {
            new Customer
            {
                ID = 1,
                Name = "normal customer",
                EmailID = 11,
                Email = new Email
                {
                    ID = 11,
                    EmailAddress = "normal@email.com",
                    CustomerID = 1
                }
            },
            new VipCustomer
            {
                ID = 2,
                Name = "vip customer",
                VipNumber = 007,
                EmailID = 21,
                Email = new Email
                {
                    ID = 21,
                    EmailAddress = "vip@email.com",
                    CustomerID = 2
                }
            }
        };
    }
    
    public static IEnumerable<Email> Emails()
    {
        return new[]
        {
            new Email
            {
                ID = 11,
                EmailAddress = "normal@email.com",
                CustomerID = 1,
                Customer = new Customer
                {
                    ID = 1,
                    Name = "normal customer",
                    EmailID = 11
                }
            },
            new Email
            {
                ID = 21,
                EmailAddress = "vip@email.com",
                CustomerID = 2,
                Customer = new VipCustomer
                {
                    ID = 2,
                    Name = "vip customer",
                    VipNumber = 007,
                    EmailID = 21
                }
            }
        };
    }
}

public class CustomersController : ODataController
{
    public IQueryable<Customer> Get()
    {
        return Data.Customers().AsQueryable();
    }
    
    public IQueryable<VipCustomer> GetFromVipCustomer()
    {
        return Data.Customers().AsQueryable().OfType<VipCustomer>();
    }
}

public class VipCustomersController : ODataController
{
    public IQueryable<VipCustomer> Get()
    {
        return Data.Customers().AsQueryable().OfType<VipCustomer>();
    }
}

public class EmailsController : ODataController
{
    public IQueryable<Email> Get()
    {
        return Data.Emails().AsQueryable();
    }
}