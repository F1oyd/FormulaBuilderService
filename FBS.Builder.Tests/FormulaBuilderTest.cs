using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using NUnit.Framework;

namespace FBS.Builder.Tests
{
    [TestFixture]
    public class FormulaBuilderTest
    {
        [Test]
        [UseReporter(typeof(VisualStudioReporter))]
        public void TestBuilder()
        {
            // Arrange
            var input = @"<request><expression><operation>plus</operation><operand><const>20</const></operand><operand>
                        <expression><operation>minus</operation><operand><const>10</const></operand><operand><const>5</const></operand>
                        </expression></operand></expression></request>";
            // Act
            var builder = new FormulaBuilder(input);
            var result = builder.Build();

            // Assert
            Approvals.Verify(result);
        }

        [Test]
        [UseReporter(typeof(VisualStudioReporter))]
        public void TestOperationValidation()
        {
            // Arrange
            var input = @"<request><expression><operation>plus</operation><operand><const>20</const></operand><operand>
                        <expression><operation>bla</operation><operand><const>10</const></operand><operand><const>5</const></operand>
                        </expression></operand></expression></request>";
            // Act
            var builder = new FormulaBuilder(input);
            var result = builder.Build();

            // Assert
            Approvals.Verify(result);
        }


        [Test]
        [UseReporter(typeof(VisualStudioReporter))]
        public void TestExpression()
        {
            // Arrange
            var input = @"<request><expression><operation>mul</operation><operand><const>20</const></operand><operand>
                        <expression><operation>plus</operation><operand><const>10</const></operand><operand><expression>
                        <operation>div</operation><operand><const>100</const></operand><operand><const>50</const></operand>
                        </expression></operand></expression></operand></expression></request>";
            // Act
            var builder = new FormulaBuilder(input);
            var result = builder.Build();

            // Assert
            Approvals.Verify(result);
        }
    }
}
