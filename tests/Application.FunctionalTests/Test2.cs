namespace Hydron.Application.FunctionalTests;

using Hydron.Application.Orders.Commands;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

[TestClass]
public sealed class Test2 : ExtendedTestBase
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        // testContext.WriteLine("Test2 ClassInitialize: Context is now available.");
    }

    [ClassCleanup]
    public static void ClassCleanup(TestContext testContext)
    {
        // testContext.WriteLine("Test2 ClassCleanup: Context is now available.");
    }

    [TestInitialize]
    public void TestInitialize()
    {
        var timeProvider = new FakeTimeProvider();

        timeProvider.SetUtcNow(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        this.Services.Replace(ServiceDescriptor.Singleton<TimeProvider>(timeProvider));
        this.TestContext.WriteLine("Test2 TestInitialize: Context is now available.");
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // this.TestContext.WriteLine("Test2 TestCleanup: Context is now available.");
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
        var timeProvider = this.ServiceProvider.GetRequiredService<TimeProvider>();

        // Act
        var now = timeProvider.GetUtcNow();

        // Assert
        now.Year.ShouldBe(2024);
        now.Month.ShouldBe(1);
        now.Day.ShouldBe(1);
    }
}
