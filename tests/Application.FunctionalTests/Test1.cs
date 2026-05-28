namespace Hydron.Application.FunctionalTests;

using Hydron.Application.Orders.Commands;

[TestClass]
public sealed class Test1 : TestBase
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        testContext.WriteLine("Test1 ClassInitialize: Context is now available.");
    }

    [ClassCleanup]
    public static void ClassCleanup(TestContext testContext)
    {
        testContext.WriteLine("Test1 ClassCleanup: Context is now available.");
    }

    [TestInitialize]
    public void TestInitialize()
    {
        this.TestContext.WriteLine("Test1 TestInitialize: Context is now available.");
    }

    [TestCleanup]
    public void TestCleanup()
    {
        this.TestContext.WriteLine("Test1 TestCleanup: Context is now available.");
    }

    [TestMethod]
    public async Task TestMethod1()
    {
        // Arrange
        var mediator = this.ServiceProvider.GetRequiredService<IMediator>();

        var orderId = new OrderId(Guid.NewGuid());
        var customerId = new CustomerId(Guid.NewGuid());

        var command = new CreateOrder
        {
            CustomerId = customerId,
            Id = orderId,
        };

        this.TestContext.WriteLine($"OrderId: {orderId}");
        this.TestContext.WriteLine($"CustomerId: {customerId}");

        // Act
        var result = await mediator.Send(command, this.TestContext.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailed.ShouldBeFalse();
    }

    [TestMethod]
    public async Task TestMethod2()
    {
        // Arrange
        var mediator = this.ServiceProvider.GetRequiredService<IMediator>();

        var orderId = new OrderId(Guid.NewGuid());
        var customerId = new CustomerId(Guid.NewGuid());

        var createOrder = new CreateOrder
        {
            CustomerId = customerId,
            Id = orderId,
        };

        var confirmOrderPayment = new ConfirmOrderPayment
        {
            Id = orderId,
        };

        var dispatchOrder = new DispatchOrder
        {
            Id = orderId,
        };

        var deliverOrder = new DeliverOrder
        {
            Id = orderId,
        };

        // Act
        _ = await mediator.Send(createOrder, this.TestContext.CancellationToken);
        _ = await mediator.Send(confirmOrderPayment, this.TestContext.CancellationToken);
        _ = await mediator.Send(dispatchOrder, this.TestContext.CancellationToken);
        var result = await mediator.Send(deliverOrder, this.TestContext.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailed.ShouldBeFalse();
    }
}
