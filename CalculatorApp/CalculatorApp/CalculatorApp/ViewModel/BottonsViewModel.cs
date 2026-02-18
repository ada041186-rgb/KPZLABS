using calculator.Command;
using calculator.Enumes;
using calculator.Model;
using calculator.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace calculator.ViewModel
{
    public class BottonsViewModel : BaseViewModel
    {
        private readonly CalculatorService _calculatorService = new CalculatorService();

        private string _calculationarea = "";
        public string Calculationarea
        {
            get { return _calculationarea; }
            set
            {
                _calculationarea = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<int, Number> Numbers { get; set; } = new Dictionary<int, Number>();
        public int CurrentIndex { get; set; } = 0;

        private bool _isDecimalMode = false;
        private double _decimalMultiplier = 0.1;

        public ICommand PlusCommand { get; }
        public ICommand MinusCommand { get; }
        public ICommand MultiplicationCommand { get; }
        public ICommand DivisionCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand DigitCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand DelOneCommand { get; }
        public ICommand PointCommand { get; }
        public ICommand WhatCommand { get; }
        public ICommand HiCommand { get; }

        public BottonsViewModel()
        {
            PlusCommand = new RelayCommand(_ => addOperator(Operators.Add));
            MinusCommand = new RelayCommand(_ => addOperator(Operators.Subtract));
            MultiplicationCommand = new RelayCommand(_ => addOperator(Operators.Multiply));
            DivisionCommand = new RelayCommand(_ => addOperator(Operators.Divide));
            EqualsCommand = new RelayCommand(_ => Calculate());
            DigitCommand = new RelayCommand(param => addNumber(Convert.ToInt32(param)));
            PointCommand = new RelayCommand(_ => AddDecimalPoint());
            ClearCommand = new RelayCommand(_ => Reset());
            DelOneCommand = new RelayCommand(_ => DeleteLast());
            WhatCommand = new RelayCommand(_ => MessageBox.Show("Курсова робота: Калькулятор"));
            HiCommand = new RelayCommand(_ => MessageBox.Show("Привіт!"));
        }

        public void addNumber(int number)
        {
            if (!Numbers.ContainsKey(CurrentIndex))
            {
                Numbers[CurrentIndex] = new Number { Value = number };
            }
            else if (_isDecimalMode)
            {
                Numbers[CurrentIndex].Value += number * _decimalMultiplier;
                _decimalMultiplier *= 0.1;
            }
            else
            {
                Numbers[CurrentIndex].Value = Numbers[CurrentIndex].Value * 10 + number;
            }

            Calculationarea += number.ToString();
        }

        private void AddDecimalPoint()
        {
            if (_isDecimalMode || !Numbers.ContainsKey(CurrentIndex)) return;

            _isDecimalMode = true;
            _decimalMultiplier = 0.1;
            Calculationarea += ",";
        }

        public void addOperator(Operators operatorSymbol)
        {
            if (!Numbers.ContainsKey(CurrentIndex)) return;

            Numbers[CurrentIndex].Operation = operatorSymbol;

            _isDecimalMode = false;
            _decimalMultiplier = 0.1;

            switch (operatorSymbol)
            {
                case Operators.Add: Calculationarea += "+"; break;
                case Operators.Subtract: Calculationarea += "-"; break;
                case Operators.Multiply: Calculationarea += "*"; break;
                case Operators.Divide: Calculationarea += "/"; break;
            }

            CurrentIndex++;
        }

        private void DeleteLast()
        {
            if (string.IsNullOrEmpty(Calculationarea)) return;

            char removed = Calculationarea[Calculationarea.Length - 1];
            Calculationarea = Calculationarea.Substring(0, Calculationarea.Length - 1);

            if (char.IsDigit(removed))
            {
                HandleDigitDeletion();
            }
            else if (removed == ',')
            {
                _isDecimalMode = false;
                _decimalMultiplier = 0.1;
            }
            else
            {
                HandleOperatorDeletion();
            }
        }

        private void HandleDigitDeletion()
        {
            if (!Numbers.ContainsKey(CurrentIndex)) return;

            string segment = ExtractCurrentSegment();

            if (string.IsNullOrEmpty(segment))
            {
                Numbers.Remove(CurrentIndex);
                _isDecimalMode = false;
                _decimalMultiplier = 0.1;
            }
            else
            {
                string normalized = segment.Replace(',', '.');
                if (double.TryParse(normalized,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out double parsed))
                {
                    Numbers[CurrentIndex].Value = parsed;

                    _isDecimalMode = segment.Contains(',');
                    if (_isDecimalMode)
                    {
                        int decimals = segment.Length - segment.IndexOf(',') - 1;
                        _decimalMultiplier = Math.Pow(0.1, decimals + 1);
                    }
                    else
                    {
                        _decimalMultiplier = 0.1;
                    }
                }
            }
        }

        private void HandleOperatorDeletion()
        {
            if (CurrentIndex == 0) return;

            CurrentIndex--;

            if (Numbers.ContainsKey(CurrentIndex))
                Numbers[CurrentIndex].Operation = Operators.None;

            if (Numbers.ContainsKey(CurrentIndex + 1))
                Numbers.Remove(CurrentIndex + 1);
        }

        private string ExtractCurrentSegment()
        {
            int start = Calculationarea.Length;

            for (int i = Calculationarea.Length - 1; i >= 0; i--)
            {
                char c = Calculationarea[i];
                if (c == '+' || c == '-' || c == '*' || c == '/')
                {
                    start = i + 1;
                    break;
                }
                if (i == 0) start = 0;
            }

            return Calculationarea.Substring(start);
        }

        private void Calculate()
        {
            if (Numbers.Count < 2) return;

            try
            {
                double result = _calculatorService.Evaluate(Numbers);
                ApplyResult(result);
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Помилка: Ділення на нуль!");
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка в обчисленнях: {ex.Message}");
                Reset();
            }
        }

        private void ApplyResult(double result)
        {
            Calculationarea = result.ToString();

            Numbers.Clear();
            CurrentIndex = 0;
            _isDecimalMode = false;
            _decimalMultiplier = 0.1;
            Numbers[CurrentIndex] = new Number { Value = result };
        }

        private void Reset()
        {
            Calculationarea = "";
            _isDecimalMode = false;
            _decimalMultiplier = 0.1;
            Numbers.Clear();
            CurrentIndex = 0;
        }
    }
}