using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ApprovalTests;
using ApprovalTests.Reporters;
using DeepEqual.Syntax;
using NUnit.Framework;

namespace FBS.Builder.Tests
{
    [TestFixture]
    public class FormulaParserTest
    {
        [Test]
        public void TestParser1()
        {
            // Arrange
            var input = @"<request><expression><operation>plus</operation><operand><const>20</const></operand><operand>
                        <expression><operation>minus</operation><operand><const>10</const></operand><operand><const>5</const></operand>
                        </expression></operand></expression></request>";
            var expected = new FormulaRequest
            {
                Expression = new Expression
                {
                    Operation = "plus",
                    Operands = new [] {
                        new Operand{Value = 20},
                        new Operand
                        {
                            Expression = new Expression
                            {
                                Operation = "minus",
                                Operands = new [] {
                                    new Operand{Value = 10},
                                    new Operand{Value = 5}
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var parser = new XmlParser();
            var result = parser.Parse<FormulaRequest>(input);

            // Assert
            result.ShouldDeepEqual(expected);
        }

        [TestCase("plus")]
        [TestCase("minus")]
        [TestCase("mul")]
        [TestCase("div")]
        public void TestParser2(string operation)
        {
            // Arrange
            var input = $@"<request><expression><operation>{operation}</operation><operand><const>20</const></operand><operand>
                        <expression><operation>minus</operation><operand><const>10</const></operand><operand><const>5</const></operand>
                        </expression></operand></expression></request>";
            var expected = new FormulaRequest
            {
                Expression = new Expression
                {
                    Operation = operation,
                    Operands = new[] {
                        new Operand{Value = 20},
                        new Operand
                        {
                            Expression = new Expression
                            {
                                Operation = "minus",
                                Operands = new [] {
                                    new Operand{Value = 10},
                                    new Operand{Value = 5}
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var parser = new XmlParser();
            var result = parser.Parse<FormulaRequest>(input);

            // Assert
            result.ShouldDeepEqual(expected);
        }

        [Test]
        public void TestParser3()
        {
            // Arrange
            var input = @"<request><expression><operation>plus</operation><operand><const>qewqe</const></operand><operand>
                        <expression><operation>minus</operation><operand><const>10</const></operand><operand><const>5</const></operand>
                        </expression></operand></expression></request>";

            // Act
            var parser = new XmlParser();

            // Assert
            Assert.Throws<InvalidOperationException>(() => parser.Parse<FormulaRequest>(input));
        }

        [Test]
        public void TestParserError()
        {
            // Arrange
            var input = @"<request><expression><operation>plus</operation><operand><const>20</const></operand><operand>
                        <expression><operation>minus</operation><operand><const>10</const></operand><operand><const>5</const></operand>
                        </expression><operand></expression></request>";

            // Act
            var parser = new XmlParser();

            // Assert
            Assert.Throws<InvalidOperationException>(() => parser.Parse<FormulaRequest>(input));
        }

        [Test]
        [UseReporter(typeof(VisualStudioReporter))]
        public void TestStringifyResult()
        {
            // Arrange
            var input = new FormulaResponse
            {
                Result = "20 + (10 - 5)",
                Errors = new Errors()
            };

            // Act
            var parser = new XmlParser();
            var result = parser.Stringify(input);

            // Assert
            Approvals.Verify(result);
        }

        [Test]
        [UseReporter(typeof(VisualStudioReporter))]
        public void TestStringifyError()
        {
            // Arrange
            var input = new FormulaResponse
            {
                Result = string.Empty,
                Errors = new Errors
                {
                    Error = new string[]
                    {
                        "Error1",
                        "Error2"
                    }
                }
            };

            // Act
            var parser = new XmlParser();
            var result = parser.Stringify(input);

            // Assert
            Approvals.Verify(result);
        }
    }
}
