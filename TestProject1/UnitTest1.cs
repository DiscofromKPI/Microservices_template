using ClassLibrary1;
using Moq;
using Xunit;

namespace TestProject1;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var Moq = new Mock<Dish>();
        Moq.Setup(x => x.GetDish()).Returns("Dish");    
        var dish = Moq.Object;
        Assert.Equal("Dish", dish.GetDish());
    }
}