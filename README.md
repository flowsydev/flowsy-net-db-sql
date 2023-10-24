# Flowsy Db Sql

This package includes implementations for interfaces defined in [Flows.Db.Abstractions](https://www.nuget.org/packages/Flowsy.Db.Abstractions)
oriented to SQL databases.


## Connection Factory
The class **DbConnectionFactory** implements the interface **IDbConnectionFactory** to
create **IDbConnection** objects from a list of registered configurations identified by unique keys.


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