# Flowsy Db Sql

This package includes implementations for interfaces defined in [Flows.Db.Abstractions](https://www.nuget.org/packages/Flowsy.Db.Abstractions)
oriented to SQL databases.


## Connection Factory

The class **DbConnectionFactory** creates **IDbConnection** objects from a list of registered configurations identified by unique keys.


## Unit Of Work

The unit of work is necessary to build our data access components.

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

All the operations related to a given invoice and its items must be performed within a single transaction represented by a unit of work.


### Repositories
Data access operations could be organized in separate repositories bound to the same unit of work.

The class DbRepository offers an implementation of the repository pattern focused on database stored routines.
By inheriting from DbRepository we can reuse its methods to execute SQL statements and routines without writing many lines of boilerplate code.

```csharp
public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(string invoiceId, CancellationToken cancellationToken);
    Task CreateAsync(Invoice invoice, CancellationToken cancellationToken);
    // More methods
}

public interface IInvoiceItemRepository
{
    Task<IEnumerable<InvoiceItem>> GetByInvoiceIdAsync(string invoiceId, CancellationToken cancellationToken);
    Task CreateAsync(InvoiceItem invoiceItem, CancellationToken cancellationToken);
    // More methods
}
```

To simplify these examples, let's create only a simplified implementation of the IInvoiceRepository interface:
```csharp
public class InvoiceRepository : DbRepository, IInvoiceRepository
{
    public InvoiceRepository(DbUnitOfWork unitOfWork, DbExceptionHandler? exceptionHandler = null)
        : base(unitOfWork, exceptionHandler)
    {
    }
    
    public async Task<Invoice?> GetByIdAsync(string invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await GetSingleOrDefaultAsync<Invoice>(
            "invoice_get_by_id", // stored function sales.invoice_get_by_id(text) (see configuration below)
            new
            {
                InvoiceId = invoiceId
            },
            cancellationToken
            );
        return invoice;
    }
  
    public Task CreateAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        return ExecuteRoutineAsync(
            "invoice_create", // stored procedure sales.invoice_create(@p_invoice_id, @p_customer_id, ...) (see configuration below)
            DbRoutineType.StoredProcedure,
            new
            {
                InvoiceId = invoiceId,
                CustomerId = customerId,
                // ...
            },
            cancellation
            );
    }
}
```

The InvoiceItemRepository shall be implemented in a similar way but using stored routines related to invoice items.


### Unit Of Work Interface and Implementation

We can define a unit of work interface targeting the two repositories we've created.

```csharp
public interface ISalesUnitOfWork : IUnitOfWork
{
    IInvoiceRepository InvoiceRepository { get; }
    IInvoiceItemRepository InvoiceItemRepository { get; }
}
```

By inheriting from DbUnitOfWork we only need to create repository instances bound to our specific unit of work.
The DbUnitOfWork class will handle the underlying connection and transaction.

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

### Unit Of Work Factory

To create instances of our unit of work, we need to provide an implementation of the **IUnitOfWorkFactory** interface.
We can create a single unit of work factory or a unit of work factory per subdomain of our application. 

```csharp
public class SalesUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DbConnectionFactory _connectionFactory;
    
    public SalesUnitOfWorkFactory(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory; 
    }
    
    public T Create<T>() where T : IUnitOfWork
    {
        IUnitOfWork? unitOfWork = null;
        
        var type = typeof(T);
        if (type == typeof(ISalesUnitOfWork))
            unitOfWork = new SalesUnitOfWork(_connectionFactory);
        // else if (type == typeof(IAnotherUnitOfWork))
        //    unitOfWork = new AnotherUnitOfWork(_connectionFactory);
        
        if (unitOfWork is null)
            throw new NotSupportedException();
        
        return (T) unitOfWork;
    }
}
```


### Using Our Units of Work

The following examples shows how to use the unit of work factory to create a new instance of a unit of work: 

```csharp
public class InvoiceByIdQueryHandler
{
    private readonly ISalesUnitOfWorkFactory _unitOfWorkFactory;
    
    public InvoiceByIdQueryHandler(ISalesUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }
  
    public async Task<InvoiceByIdQueryResult> HandleAsync(InvoiceByIdQuery query, CancellationToken cancellationToken)
    {
        await using var unitOfWork = _unitOfWorkFactory.Create<ISalesUnitOfWork>();
        // The unitOfWork instance will be disposed when execution of HandleAsync is completed or any exception is thrown.
        
        var invoice = await unitOfWork.InvoiceRepository.GetByIdAsync(query.InvoiceId, cancellationToken);
        if (invoice is null)
            throw new EntityNotFoundException(query.InvoiceId);
        
        // Return the fictitious query result
        return new InvoiceByIdQueryResult(invoice);
    }
}

public class CreateInvoiceCommandHandler
{
    private readonly ISalesUnitOfWorkFactory _unitOfWorkFactory;
    
    public CreateInvoiceCommandHandler(ISalesUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }
  
    public async Task<CreateInvoiceCommandResult> HandleAsync(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        await using var unitOfWork = _unitOfWorkFactory.Create<ISalesUnitOfWork>();
        // The unitOfWork instance will be disposed when execution of HandleAsync is completed or any exception is thrown.
        // If the unit of work is disposed without invoking the SaveWork or SaveWorkAsync methods,
        // the inner transaction will be rolled back discarding all the work of the unit.
        
        var invoice = new Invoice();
        // Populate invoice object from properties of command object
        
        // Begin transactional operation
        unitOfWork.BeginWork();
      
        // Create the Invoice entity
        var invoiceId = await unitOfWork.InvoiceRepository.CreateAsync(invoice, cancellationToken);
      
        // Create all the InvoiceItem entities
        foreach (var item in command.Items)
        {
            var invoiceItem = new InvoiceItem();
            // Populate invoiceItem object from properties of the item object
          
            // Create each InvoiceItem entity
            await unitOfWork.InvoiceItemRepository.CreateAsync(invoiceItem, cancellationToken); 
        }

        // Complete the transactional operation      
        await unitOfWork.SaveWorkAsync(cancellationToken);
      
        // Return the result of the operation
        return new CreateInvoiceCommandResult
        {
            InvoiceId = invoiceId
        };
    }
}
```


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
for IConfiguration objects to configure our DbConnectionFactory instance.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register the required database provider
// In this example we're using PostgreSQL (requires the Npgsql Nuget)
DbProviderFactories.RegisterFactory(DbProvider.PostgreSql, NpgsqlFactory.Instance);

// Register a DbConnectionFactory service with the required configurations taken from the application settings
builder.Services.AddDbConnectionFactory(serviceProvider =>
    {
      var configuration = serviceProvider.GetRequiredService<IConfiguration>();
      var connectionConfigurations = configuration.GetConnectionConfigurations("Database"); // "Database" is the section name in the configuration file
      
      return new DbConnectionFactory(dbConnectionConfigurations.ToArray());
    });

// Register unit of work services and repositories with their options
builder.Services
    .AddUnitOfWork(options => options.ConnectionKey = "Database1") // Default options for all units of work
    .UsingFactory<ISalesUnitOfWorkFactory, SalesUnitOfWorkFactory>()
    .WithUnit<SalesUnitOfWork>() // Can pass argument to customize options for each unit of work
    .AddRepositories(options =>
    {
        // Configure default options for all repository types.
        // All settings are optional, they have default values to use if a custom value is not set.
        options.Schema = "public";
    
        options.Conventions.DateTimeOffsetFormat = DbDateTimeOffsetFormat.Utc;
    
        options.Conventions.Routines.DefaultType = DbRoutineType.StoredFunction;
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
        
        options.JsonSerialization = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            Converters = new List<JsonConverter>
            {
                // Custom converters
            },
            // More options
        };
        
        options.LogLevel = LogLevel.Debug;
    })
    .WithRepository<InvoiceRepository>(options =>
    {
        options.Schema = "sales";
    })
    .WithRepository<InvoiceItemRepository>(options =>
    {
        options.Schema = "sales";
    })
    // .WithRepository<AnotherRepository>() // Use default options
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

