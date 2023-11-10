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
        // Invoke a stored routine: user_get_by_id(uuid)
        return GetSingleOrDefaultAsync<User>(
            "user_get_by_id",
            new
            {
                UserId = userId
            },
            cancellationToken
            );
    }
  
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        // Invoke a stored routine: user_get_by_email(varchar)
        return GetSingleOrDefaultAsync<User>(
            "user_get_by_email",
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

### Repositories & Dependency Injection

```csharp

var builder = WebApplication.CreateBuilder(args);

// Register a DbConnectionFactory service with the required configurations taken from the application settings
builder.Services.AddDbConnectionFactory(serviceProvider =>
    {
      var configuration = serviceProvider.GetRequiredService<IConfiguration>();
      var sqlConfiguration = configuration.GetRequiredSection("Databases");
      var dbConnectionConfigurations = 
          sqlConfiguration.GetChildren().Select(databaseConfiguration => new DbConnectionConfiguration
          {
              Key = databaseConfiguration.Key,
              ProviderInvariantName = databaseConfiguration["ProviderInvariantName"],
              ConnectionString = databaseConfiguration["ConnectionString"]
          });
  
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
```

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
