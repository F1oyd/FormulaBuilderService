using System;
using NUnit.Framework;

namespace FBS.Builder.Tests
{
    [TestFixture]
    public class FormulaBuilderTest
    {
        [Test]
        public void TestBuilder()
        {
            // Arrange
            var input = @"<request><expression><operation>plus</operation><operand><const>20</const></operand><operand>
                        <expression><operation>minus</operation><operand><const>10</const></operand><operand><const>5</const></operand>
                        </expression></operand></expression></request>";
            var expected = "<response>\r\n  <result>20 + (10 - 5)</result>\r\n  <errors>\r\n    <error>Unknown operator: bla</error>\r\n  </errors>\r\n</response>";
            // Act
            var builder = new FormulaBuilder(input);
            var result = builder.Build();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TestOperationValidation()
        {
            // Arrange
            var input = @"<request><expression><operation>plus</operation><operand><const>20</const></operand><operand>
                        <expression><operation>bla</operation><operand><const>10</const></operand><operand><const>5</const></operand>
                        </expression></operand></expression></request>";
            var expected = "<response>\r\n  <errors>\r\n    <error>Unknown operator: bla</error>\r\n  </errors>\r\n</response>";
            // Act
            var builder = new FormulaBuilder(input);
            var result = builder.Build();

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
