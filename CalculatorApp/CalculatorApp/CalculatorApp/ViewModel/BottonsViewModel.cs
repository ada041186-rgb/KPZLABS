using calculator.Command;
using calculator.Enumes;
using calculator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace calculator.ViewModel
{
    public class BottonsViewModel : BaseViewModel
    {
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

        public ICommand PlusCommand { get; }
        public ICommand MinusCommand { get; }
        public ICommand MultiplicationCommand { get; }
        public ICommand DivisionCommand { get; }
        public ICommand EqualsCommand { get; }
        public ICommand OneCommand { get; }
        public ICommand TwoCommand { get; }
        public ICommand ThreeCommand { get; }
        public ICommand FourCommand { get; }
        public ICommand FiveCommand { get; }
        public ICommand SixCommand { get; }
        public ICommand SevenCommand { get; }
        public ICommand EightCommand { get; }
        public ICommand NineCommand { get; }
        public ICommand ZeroCommand { get; }
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

            OneCommand = new RelayCommand(_ => addNumber(1));
            TwoCommand = new RelayCommand(_ => addNumber(2));
            ThreeCommand = new RelayCommand(_ => addNumber(3));
            FourCommand = new RelayCommand(_ => addNumber(4));
            FiveCommand = new RelayCommand(_ => addNumber(5));
            SixCommand = new RelayCommand(_ => addNumber(6));
            SevenCommand = new RelayCommand(_ => addNumber(7));
            EightCommand = new RelayCommand(_ => addNumber(8));
            NineCommand = new RelayCommand(_ => addNumber(9));
            ZeroCommand = new RelayCommand(_ => addNumber(0));

            PointCommand = new RelayCommand(_ => { Calculationarea += ","; }); 
            ClearCommand = new RelayCommand(_ => Reset());
            DelOneCommand = new RelayCommand(_ => DeleteLast());
            WhatCommand = new RelayCommand(_ => MessageBox.Show("Курсова робота: Калькулятор"));
            HiCommand = new RelayCommand(_ => MessageBox.Show("Привіт!"));
        }

        public void addNumber(int number)
        {
            Calculationarea += number.ToString();

            if (!Numbers.ContainsKey(CurrentIndex))
            {
                Numbers[CurrentIndex] = new Number { Value = number };
            }
            else
            {
                Numbers[CurrentIndex].Value = Numbers[CurrentIndex].Value * 10 + number;
            }
        }

        public void addOperator(Operators operatorSymbol)
        {
            if (!Numbers.ContainsKey(CurrentIndex)) return; 

            Numbers[CurrentIndex].Operation = operatorSymbol;

            switch (operatorSymbol)
            {
                case Operators.Add: Calculationarea += "+"; break;
                case Operators.Subtract: Calculationarea += "-"; break;
                case Operators.Multiply: Calculationarea += "*"; break;
                case Operators.Divide: Calculationarea += "/"; break;
            }

            CurrentIndex++;
        }

        private void Calculate()
        {
            if (Numbers.Count < 2 && !Numbers.ContainsKey(CurrentIndex)) return;

            try
            {
                var items = Numbers.OrderBy(k => k.Key)
                                   .Select(k => new Number
                                   {
                                       Value = k.Value.Value,
                                       Operation = k.Value.Operation
                                   }).ToList();

                for (int i = 0; i < items.Count - 1;)
                {
                    var op = items[i].Operation;

                    if (op == Operators.Multiply || op == Operators.Divide)
                    {
                        double val1 = items[i].Value;
                        double val2 = items[i + 1].Value;
                        double stepResult = 0;

                        if (op == Operators.Multiply)
                        {
                            stepResult = val1 * val2;
                        }
                        else 
                        {
                            if (val2 == 0) throw new DivideByZeroException();
                            stepResult = val1 / val2;
                        }

                        items[i].Value = stepResult;
                        items[i].Operation = items[i + 1].Operation;

                        items.RemoveAt(i + 1);

                    }
                    else
                    {
                        i++;
                    }
                }

                double finalResult = items[0].Value;
                for (int i = 0; i < items.Count - 1; i++)
                {
                    var op = items[i].Operation;
                    double nextVal = items[i + 1].Value;

                    if (op == Operators.Add) finalResult += nextVal;
                    else if (op == Operators.Subtract) finalResult -= nextVal;
                }

                Calculationarea = finalResult.ToString();

                Numbers.Clear();
                CurrentIndex = 0;
                Numbers[CurrentIndex] = new Number { Value = finalResult };
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

        private void Reset()
        {
            Calculationarea = "";
            Numbers.Clear();
            CurrentIndex = 0;
        }

        private void DeleteLast()
        {
            if (!string.IsNullOrEmpty(Calculationarea))
            {
                Calculationarea = Calculationarea.Substring(0, Calculationarea.Length - 1);
                
            }
        }
    }
}