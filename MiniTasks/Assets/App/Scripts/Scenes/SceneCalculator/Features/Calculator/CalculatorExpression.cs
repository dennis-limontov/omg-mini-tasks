using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Scripts.Scenes.SceneCalculator.Features.Calculator
{
    public class CalculatorExpression : ICalculatorExpression
    {
        private Dictionary<string, string> expressions = new Dictionary<string, string>();
        private readonly Dictionary<char, int> operatorPriorities = new Dictionary<char, int>()
            { {'(', 0}, {')', 1}, {'+', 2}, {'-', 2}, {'*', 3}, {'/', 3 } };

        // RPN – reverse Polish notation
        private string ConvertToRPN(string expression)
        {
            Stack<char> expressionParts = new Stack<char>();
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < expression.Length; i++)
            {
                if (IsOperator(expression[i]))
                {
                    if (expression[i] == '(')
                    {
                        expressionParts.Push(expression[i]);
                    }
                    else if (expression[i] == ')')
                    {
                        char symbol = expressionParts.Pop();
                        while (symbol != '(')
                        {
                            result.Append(symbol).Append(' ');
                            symbol = expressionParts.Pop();
                        }
                    }
                    else if ((expression[i] == '-') && ((i == 0) || (expression[i - 1] == '(')))
                    {
                        result.Append("0 ");
                        expressionParts.Push(expression[i]);
                    }
                    else
                    {
                        if (expressionParts.Count > 0)
                        {
                            if (operatorPriorities[expression[i]] <= operatorPriorities[expressionParts.Peek()])
                            {
                                result.Append(expressionParts.Pop()).Append(' ');
                            }
                        }
                        expressionParts.Push(expression[i]);
                    }
                }
                else if (char.IsDigit(expression[i]) || char.IsLetter(expression[i]))
                {
                    CountExpressionPart(result, expression, ref i);
                }
            }

            while (expressionParts.Count > 0)
            {
                result.Append(expressionParts.Pop()).Append(' ');
            }

            return result.ToString();
        }

        private void CountExpressionPart(StringBuilder result, string expression, ref int index)
        {
            do
            {
                result.Append(expression[index++]);
            } while ((index < expression.Length) && !IsOperator(expression[index])
                    && (expression[index] != ' '));
            result.Append(' ');
            index--;
        }
        
        private bool IsOperator(char op)
        {
            const string operators = "()*/+-";
            return (operators.IndexOf(op) != -1);
        }

        public int Execute(string expression)
        {
            string expressionRPN = ConvertToRPN(expression);
            Stack<int> expressionParts = new Stack<int>();
            StringBuilder expressionPart = new StringBuilder();

            for (int i = 0; i < expressionRPN.Length; i++)
            {
                if (char.IsLetter(expressionRPN[i]))
                {
                    expressionPart.Clear();
                    CountExpressionPart(expressionPart, expressionRPN, ref i);
                    expressionParts.Push(Get(expressionPart.ToString()));
                }
                else if (char.IsDigit(expressionRPN[i]))
                {
                    expressionPart.Clear();
                    CountExpressionPart(expressionPart, expressionRPN, ref i);
                    expressionParts.Push(int.Parse(expressionPart.ToString()));
                }
                else if (IsOperator(expressionRPN[i]))
                {
                    int a = expressionParts.Pop();
                    int b = expressionParts.Pop();
                    int localResult = 0;

                    switch (expressionRPN[i])
                    {
                        case '*':
                            localResult = b * a;
                            break;
                        case '/':
                            if (a != 0)
                            {
                                localResult = b / a;
                            }
                            else
                            {
                                throw new ExceptionExecuteExpression($"Dividing by zero in: {expression}!");
                            }
                            break;
                        case '+':
                            localResult = b + a;
                            break;
                        case '-':
                            localResult = b - a;
                            break;
                    }
                    expressionParts.Push(localResult);
                }
            }

            return expressionParts.Peek();
        }

        public void SetExpression(string expressionKey, string expression)
        {
            Regex regex = new Regex(@$"[^\w]{expressionKey}[^\w]");
            if (regex.IsMatch(expression))
            {
                throw new ExceptionExecuteExpression($"Recursive expression: {expression}!");
            }

            expressions[expressionKey.Trim()] = expression;
        }

        public int Get(string expressionKey)
        {
            if (expressions.TryGetValue(expressionKey.Trim(), out string expression))
            {
                return Execute(expression);
            }
            else
            {
                throw new ExceptionExecuteExpression($"Wrong expression key: {expressionKey}!");
            }
        }
    }
}