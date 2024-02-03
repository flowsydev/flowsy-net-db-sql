# Flowsy Db Sql

This package includes implementations for interfaces defined in [Flows.Db.Abstractions](https://www.nuget.org/packages/Flowsy.Db.Abstractions)
oriented to SQL databases.

## Connection Factory

The class **DbConnectionFactory** creates **IDbConnection** objects from a list of registered configurations identified by unique keys.


## Repositories

The class DbRepository offers an implementation of the repository pattern focused on database stored routines.

Let's create an interface for a fictitious user repository:

```csharp
public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
}
```

Now we need to implement our interface:

```csharp
public class UserRepository : DbRepository, IUserRepository
{
    public UserRepository(DbConnectionFactory connectionFactory, DbExceptionHandler? exceptionHandler = null) 
        : base(connectionFactory, exceptionHandler)
    {
    }

    public UserRepository(DbUnitOfWork unitOfWork, DbExceptionHandler? exceptionHandler = null)
        : base(unitOfWork, exceptionHandler)
    {
    }
    
    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await GetSingleOrDefaultAsync<User>(
            "user_get_by_id", // stored function sf_user_get_by_id(uuid) (see configuration below)
            new
            {
                UserId = userId
            },
            cancellationToken
            );
        
        user.Roles = await GetManyAsync<UserRole>(
            "user_role_get_by_user_id", // stored function sf_user_role_get_by_user_id(uuid) (see configuration below)
            new
            {
                UserId = userId
            },
            cancellationToken
            );
        
        return user;
    }
  
    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await GetSingleOrDefaultAsync<User>(
            "user_get_by_email", // stored function sf_user_get_by_email(varchar) (see configuration below)
            new
            {
                Email = email
            },
            cancellationToken
            );
        
        user.Roles = await GetManyAsync<UserRole>(
            "user_role_get_by_user_id", // stored function sf_user_role_get_by_user_id(uuid) (see configuration below)
            new
            {
                UserId = user.UserId
            },
            cancellationToken
            );
        
        return user;
    }
}
```

By inheriting from DbRepository we can reuse its methods to execute SQL statements and routines without writing many lines of boilerplate code.

## Dependency Injection

### Recommended Configuration
The following code snippet shows a recommended configuration for database connections:
```json5
{
  // ...
  "Database": { // Root for all database connections
    "Database1": { // Key to identify this connection
      "ProviderInvariantName": "Npgsql",
      "ConnectionString": "Server=pg.example.com;Port=5432;Database=db1;User Id=user1;Password=sup3rS3cr3t;Include Error Detail=True;",
      "Migration": { // Optional section to configure database migrations
        "SourceDirectory": "Some/Path/To/Migrations/Database1", // Path with migration scripts for 'Database1'
        "MetadataTableSchema": "migration", // Schema containing the table for migration metadata
        "MetadataTableName": "changelog", // Table for migration metadata
        "InitializationStatement": "call public.populate_tables();" // Optional statement to execute after running migrations
      }
    },
    "Database2": { // Key to identify this connection
      "ProviderInvariantName": "MySql.Data.MySqlClient",
      "ConnectionString": "Server=mysql.example.com;Port=3306;Database=db2;User Id=user2;Password=m3gaS3cr3t;",
      "Migration": { // Optional section to configure database migrations
        "SourceDirectory": "Some/Path/To/Migrations/Database2", // Path with migration scripts for 'Database2'
        "MetadataTableSchema": "migration", // Schema containing the table for migration metadata
        "MetadataTableName": "changelog", // Table for migration metadata
        "InitializationStatement": "call populate_tables();" // Optional statement to execute after running migrations
      }
    }
  },
  // ...
}
```

### Register Services
The previous configuration will allow us to use the GetConnectionConfigurations extension method
for IConfiguration to configure our DbConnectionFactory instance.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register a DbConnectionFactory service with the required configurations taken from the application settings
builder.Services.AddDbConnectionFactory(serviceProvider =>
    {
      var configuration = serviceProvider.GetRequiredService<IConfiguration>();
      var connectionConfigurations = configuration.GetConnectionConfigurations("Database");
      
      return new DbConnectionFactory(dbConnectionConfigurations.ToArray());
    });

builder.Services
    .AddUnitOfWork(options => {
        // Configure default options for all unit of work types.
        options.ConnectionKey = "Default";
    })
    .Using<ISalesUnitOfWork, SalesUnitOfWork>();

// Register repositories and configure their options
builder.Services
    .AddRepositories(options =>
    {
        // Configure default options for all repository types.
        // All settings are optional, they have default values to use if a custom value is not set.
        options.ConnectionKey = "Default";
        options.Schema = "public";
    
        options.Conventions.DateTimeOffsetFormat = DbDateTimeOffsetFormat.Utc;
    
        options.Conventions.Routines.DefaultType = DbRoutineType.StoredFunction;
        options.Conventions.Routines.Prefix = "sf_";
        options.Conventions.Routines.Casing = null;
    
        options.Conventions.Parameters.Prefix = "p_";
        options.Conventions.Parameters.Casing = CaseConvention.LowerSnakeCase;
        options.Conventions.Parameters.FormatPlaceholder = (_, routineType, parameterName, _) =>
        {
            return routineType switch
            {
                DbRoutineType.StoredFunction => $"{parameterName} => @{parameterName}",
                _ => $"@{parameterName}"
            };
        };

        options.Conventions.Enums.ValueFormat = DbEnumFormat.Name;
    })
    .Using<IUserRepository, UserRepository>(options =>
    {
        options.Schema = "security";
    })
    .Using<IInvoiceRepository, InvoiceRepository>(options =>
    {
        options.Schema = "sales";
    })
    .Using<IInvoiceItemRepository, InvoiceItemRepository>(options =>
    {
        options.Schema = "sales";
    })
    .WithColumnMapping(
        CaseConvention.LowerSnakeCase, // Database column names in lower_snake_case
        t => typeof(IEntity).IsAssignableFrom(t), // Apply this column mapping to entities implementing the fictitious IEntity interface
        Assembly.GetExecutingAssembly() // Scan types from the executing assembly
    )
    .WithTypeHandler(new DbDateOnlyTypeHandler()) // The fictitious DbDateOnlyTypeHandler must inherit from DbTypeHandler
    ;

// Configure more services and run the application...
```

## Database Migrations
Optionally, you can use the DbManager class to run database migrations.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure the IDbConnectionFactory instance and other services...

builder.Services.AddSingleton<DbManager>();

// Configure more services and run the application...
```

```csharp
public class SomeService
{
    private readonly DbManager _dbManager;
    
    public SomeService(DbManager dbManager)
    {
        // The dependency injection system will configure the DbManager instance using the previously registered IDbConnectionFactory object.
        _dbManager = dbManager;
    }
    
    public async Task RunMigrationsAsync(CancellationToken cancellationToken)
    {
        var results = await _dbManager.MigrateAsync(cancellationToken);
        // Do something with results
    }
}
```

Under the hood, this package uses the popular tool named [Evolve](https://www.nuget.org/packages/Evolve), so you should
follow the specifications from [Evolve Concepts](https://evolve-db.netlify.app/concepts/) to create your migration files.

If you decide to take another approach or use another tool, you will need to take care of all the details to manage your database objects.


## Unit Of Work

The class **DbUnitOfWork** implements the interface **IUnitOfWork** and gets connections from a
given **DbConnectionFactory** instance to create a **IDbTransaction** object to handle the whole unit of work.

For example, to create an invoice, we may need to create two kinds of entities:

* Invoice
  * InvoiceId
  * CustomerId
  * CreateDate
  * Total
  * Taxes
  * GrandTotal
* InvoiceItem
  * InvoiceItemId
  * InvoiceId
  * ProductId
  * Quantity
  * Amount

A way of completing such operation from an application-level command handler could be:

```csharp
public interface ISalesUnitOfWork : IUnitOfWork
{
    IInvoiceRepository InvoiceRepository { get; }
    IInvoiceItemRepository InvoiceItemRepository { get; }
}
```

```csharp
public class SalesUnitOfWork : DbUnitOfWork, ISalesUnitOfWork
{
    private readonly IInvoiceRepository? _invoiceRepository;
    private readonly IInvoiceItemRepository? _invoiceItemRepository;
  
    public SalesUnitOfWork(DbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }
    
    // By using this pattern, repositories will be instantiated only when needed
    // InvoiceRepository and InvoiceItemRepository classes shall implement their corresponding methods
    // and those operations will be bounded to the transaction handled by this unit of work
  
    public IInvoiceRepository InvoiceRepository
        => _invoiceRepository ??= new InvoiceRepository(this);
  
    public IInvoiceItemRepository InvoiceItemRepository
        => _invoiceItemRepository ??= new InvoiceItemRepository(this);
}
```

```csharp
public class CreateInvoiceCommandHandler
{
    private readonly ISalesUnitOfWork _unitOfWork;
  
    // The unit of work will be injected here but its connection will be created
    // only when one of its repositories executes the first query, and will be disposed
    // automatically when the unit of work is disposed.
    
    // If the unit of work is disposed without invoking the SaveWork or SaveWorkAsync methods,
    // the inner transaction will be rolled back discarding all the work of the unit.
    public CreateInvoiceCommandHandler(ISalesUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
  
    public async Task<CreateInvoiceCommandResult> HandleAsync(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = new Invoice();
        // Populate invoice object from properties of command object
        
        // Begin operation
        _unitOfWork.BeginWork();
      
        // Create the Invoice entity
        var invoiceId = await _unitOfWork.InvoiceRepository.CreateAsync(invoice, cancellationToken);
      
        // Create all the InvoiceItem entities
        foreach (var item in command.Items)
        {
            var invoiceItem = new InvoiceItem();
            // Populate invoiceItem object from properties of the item object
          
            // Create each InvoiceItem entity
            await _unitOfWork.InvoiceItemRepository.CreateAsync(invoiceItem, cancellationToken); 
        }

        // Commit the current operation      
        await _unitOfWork.SaveWorkAsync(cancellationToken);
      
        // Return the result of the operation
        return new CreateInvoiceCommandResult
        {
            InvoiceId = invoiceId
        };
    }
}
```
