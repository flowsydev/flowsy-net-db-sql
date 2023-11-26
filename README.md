# Flowsy Db Sql

This package includes implementations for interfaces defined in [Flows.Db.Abstractions](https://www.nuget.org/packages/Flowsy.Db.Abstractions)
oriented to SQL databases.

## Connection Factory

The class **DbConnectionFactory** implements the interface **IDbConnectionFactory** to
create **IDbConnection** objects from a list of registered configurations identified by unique keys.


## Repositories

The class DbRepository offers the foundation to implement the repository pattern.

Let's create an interface for a fictitious user repository:

```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
```

Now we need to implement our interface:

```csharp
public class UserRepository : DbRepository, IUserRepository
{
    public UserRepository(IDbConnectionFactory connectionFactory, DbExceptionHandler? exceptionHandler = null) 
        : base(connectionFactory, exceptionHandler)
    {
    }

    public UserRepository(IDbConnection connection, DbExceptionHandler? exceptionHandler = null)
        : base(connection, exceptionHandler)
    {
    }

    public UserRepository(IDbTransaction transaction, DbExceptionHandler? exceptionHandler = null)
        : base(transaction, exceptionHandler)
    {
    }

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return GetSingleOrDefaultAsync<User>(
            "UserGetById", // stored function sf_user_get_by_id(uuid) (see configuration below)
            new
            {
                UserId = userId
            },
            cancellationToken
            );
    }
  
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return GetSingleOrDefaultAsync<User>(
            "UserGetByEmail", // stored function sf_user_get_by_email(varchar) (see configuration below)
            new
            {
                Email = email
            },
            cancellationToken
        );
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
        "MetadataSchema": "public", // Schema containing the table for migration metadata
        "MetadataTable": "migration", // Table for migration metadata
        "InitializationStatement": "call public.populate_tables();" // Optional statement to execute after running migrations
      }
    },
    "Database2": { // Key to identify this connection
      "ProviderInvariantName": "MySql.Data.MySqlClient",
      "ConnectionString": "Server=mysql.example.com;Port=3306;Database=db2;User Id=user2;Password=m3gaS3cr3t;",
      "Migration": { // Optional section to configure database migrations
        "SourceDirectory": "Some/Path/To/Migrations/Database2", // Path with migration scripts for 'Database2'
        "MetadataTable": "migration", // Table for migration metadata
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
        options.Conventions.Routines.Casing = CaseConvention.LowerSnakeCase;
    
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
        options.Schema = "auth";
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

The class **DbUnitOfWork** implements the interface **IUnitOfWork** to create **IDbTransaction** objects from a given **IDbConnection**.

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
  
    public SalesUnitOfWork(IDbConnection connection) : base(connection)
    {
    }
  
    public IInvoiceRepository InvoiceRepository
        => _invoiceRepository ??= new InvoiceRepository(Transaction);
  
    public IInvoiceItemRepository InvoiceItemRepository
        => _invoiceItemRepository ??= new InvoiceItemRepository(Transaction);
}
```

```csharp
public class CreateInvoiceCommandHandler
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
  
    public CreateInvoiceCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }
  
    public async Task<CreateInvoiceCommandResult> HandleAsync(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        // Begin operation
        // IUnitOfWork inherits from IDisposable and IAsyncDisposable, if any exception is thrown, the current operation shall be rolled back
        await using var unitOfWork = _unitOfWorkFactory.Create<ISalesUnitOfWork>();

        var invoice = new Invoice();
        // Populate invoice object from properties of command object 
      
        // Create the Invoice entity
        var invoiceId = await unitOfWork.InvoiceRepository.CreateAsync(invoice, cancellationToken);
      
        // Create all the InvoiceItem entities
        foreach (var item in command.Items)
        {
            var invoiceItem = new InvoiceItem();
            // Populate invoiceItem object from properties of item object
          
            // Create each InvoiceItem entity
            await unitOfWork.InvoiceItemRepository.CreateAsync(invoiceItem, cancellationToken); 
        }

        // Commit the current operation      
        await unitOfWork.SaveAsync(cancellationToken);
      
        // Return the result of the operation
        return new CreateInvoiceCommandResult
        {
            InvoiceId = invoiceId
        };
    }
}
```
