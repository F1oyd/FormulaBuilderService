using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBS.Builder
{
    public class FormulaBuilder : IFormulaBuilder
    {
        private readonly string inputString;

        private readonly IParser parser;

        private readonly Dictionary<string, string> availableOperations = new Dictionary<string, string>
        {
            {"plus", "+"},
            {"minus", "-"},
            {"mul", "*"},
            {"div", "/"}
        };

        public FormulaBuilder(string inputString, IParser parser)
        {
            this.inputString = inputString;
            this.parser = parser;
        }

        public FormulaBuilder(string inputString) : this(inputString, new XmlParser())
        {
        }

        public string Build()
        {
            var response = this.BuildInner();
            return parser.Stringify(response);
        }

        private FormulaResponse BuildInner()
        {
            var response = new FormulaResponse();

            FormulaRequest formulaRequest;
            try
            {
                formulaRequest = parser.Parse<FormulaRequest>(inputString);
            }
            catch (Exception e)
            {
                response.Result = string.Empty;
                response.Errors = new Errors
                {
                    Error = new[]
                    {
                        e.ToString()
                    }
                };
                return response;
            }

            var errors = this.ValidateExpression(formulaRequest.Expression);
            if (errors.Length > 0)
            {
                response.Result = string.Empty;
                response.Errors = new Errors
                {
                    Error = errors
                };
            }
            else
            {
                response.Result = this.BuildExpression(formulaRequest.Expression);
                response.Errors = new Errors
                {
                    Error = new string[0]
                };
            }

            return response;
        }

        private string BuildExpression(Expression expr)
        {
            var sb = new StringBuilder();
            if (expr.Operands[0].Value != null)
            {
                sb.Append(expr.Operands[0].Value);
            }
            else if (expr.Operands[0].Expression != null)
            {
                sb.Append($"({this.BuildExpression(expr.Operands[0].Expression)})");
            }
            sb.Append($" {availableOperations[expr.Operation]} ");
            if (expr.Operands[1].Value != null)
            {
                sb.Append(expr.Operands[1].Value);
            }
            else if (expr.Operands[1].Expression != null)
            {
                sb.Append($"({this.BuildExpression(expr.Operands[1].Expression)})");
            }
            return sb.ToString();
        }

        private string[] ValidateExpression(Expression expr)
        {
            var errors = new List<string>();

            if (!availableOperations.Keys.Contains(expr.Operation))
            {
                errors.Add($"Unknown operator: {expr.Operation}");
            }

            if (expr.Operands == null || expr.Operands.Length == 0)
            {
                errors.Add("Operands must be specified");
                return errors.ToArray();
            }

            if (expr.Operands.Length < 2)
            {
                errors.Add("Operand is missing");
            }

            if (expr.Operands.Length > 2)
            {
                errors.Add("Too many operands");
            }

            foreach (var operand in expr.Operands)
            {
                if (operand.Value == null && operand.Expression == null)
                {
                    errors.Add("Operand value must be valid");
                }

                if (operand.Expression != null)
                {
                    errors.AddRange(this.ValidateExpression(operand.Expression));
                }
            }

            return errors.ToArray();
        }
    }
}
