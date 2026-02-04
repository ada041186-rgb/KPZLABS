# Принципи програмування 
[Example of Single Responsibility Principle (SRP)](CalculatorApp/CalculatorApp/CalculatorApp/ViewModel)
Each class has one clearly defined responsibility:

ButtonsViewModel - responsible only for the calculator logic, command processing and calculation management
RelayCommand - responsible only for implementing the Command pattern for WPF
Number - responsible only for storing a numeric value and operations
BaseViewModel - responsible only for implementing the property change notification mechanism (INotifyPropertyChanged)
MainViewModel - responsible only for managing the current ViewModel

[Example of Dependency Inversion Principle (DIP)](CalculatorApp/CalculatorApp/CalculatorApp/ViewModel/BottonsViewModel.cs)
High-level modules depend on abstractions, not concrete implementations:

ButtonsViewModel depends on the ICommand interface, not the concrete RelayCommand class
All commands (PlusCommand, MinusCommand, etc.) are declared as ICommand, which makes it easy to replace the implementation without changing the ViewModel
RelayCommand implements the ICommand interface, providing loose coupling between components

[Example of Open/Closed Principle (OCP)](CalculatorApp/CalculatorApp/CalculatorApp/ViewModel/BaseViewModel.cs)
Classes are open for extension, but closed for modification:

BaseViewModel can be extended through inheritance (BottonsViewModel and MainViewModel inherit from it) without changing the base class
New ViewModels can be added by inheriting BaseViewModel, without modifying existing code
[Enum Operators](CalculatorApp/CalculatorApp/CalculatorApp/Enumes) can be extended with new operations without changing existing logic
