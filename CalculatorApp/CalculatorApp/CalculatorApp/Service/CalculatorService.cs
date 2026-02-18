using calculator.Enumes;
using calculator.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace calculator.Service
{
    public class CalculatorService
    {
        public double Evaluate(Dictionary<int, Number> numbers)
        {
            var items = numbers
                .OrderBy(k => k.Key)
                .Select(k => new Number { Value = k.Value.Value, Operation = k.Value.Operation })
                .ToList();

            ApplyHighPrecedenceOperators(items);
            return ApplyLowPrecedenceOperators(items);
        }

        private void ApplyHighPrecedenceOperators(List<Number> items)
        {
            for (int i = 0; i < items.Count - 1;)
            {
                var op = items[i].Operation;
                if (op == Operators.Multiply || op == Operators.Divide)
                {
                    double val1 = items[i].Value;
                    double val2 = items[i + 1].Value;

                    if (op == Operators.Divide && val2 == 0)
                        throw new DivideByZeroException();

                    items[i].Value = op == Operators.Multiply ? val1 * val2 : val1 / val2;
                    items[i].Operation = items[i + 1].Operation;
                    items.RemoveAt(i + 1);
                }
                else { i++; }
            }
        }

        private double ApplyLowPrecedenceOperators(List<Number> items)
        {
            double result = items[0].Value;
            for (int i = 0; i < items.Count - 1; i++)
            {
                double next = items[i + 1].Value;
                switch (items[i].Operation)
                {
                    case Operators.Add: result += next; break;
                    case Operators.Subtract: result -= next; break;
                }
            }
            return result;
        }
    }
}