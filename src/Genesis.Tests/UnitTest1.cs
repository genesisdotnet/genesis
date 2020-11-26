using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Genesis.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }

        [TestMethod, TestCategory("Unit")]
        public void ExampleTest()
        {
            this.TestContext.WriteLine("Hello World!");
            Assert.IsTrue(true);
        }


        [DataTestMethod, TestCategory("Unit")]
        [DataRow("Some Data")]
        [DataRow("More Data")]
        [DataRow("Yet More Test Data")]
        public void Example2Test(string input)
        {
            this.TestContext.WriteLine($"Hello World! {input}");
            Assert.IsTrue(true);
        }
    }
}
