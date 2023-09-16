using System;
using System.Collections.Generic;

namespace IntegralCalculator
{
    // Интерфейс функции
    public interface IFunction
    {
        double Calculate(double x);
    }

    // Конкретная реализация функции
    public class PolynomialFunction : IFunction
    {
        private double[] coefficients;

        public PolynomialFunction(double[] coefficients)
        {
            this.coefficients = coefficients;
        }

        public double Calculate(double x)
        {
            double result = 0;
            for (int i = 0; i < coefficients.Length; i++)
            {
                result += coefficients[i] * Math.Pow(x, i);
            }
            return result;
        }
    }

    // Интерфейс операции интегрирования
    public interface IIntegrationOperation
    {
        double Integrate(IFunction function, double lowerBound, double upperBound);
    }

    // Конкретная реализация операции интегрирования методом прямоугольников
    public class RectangleIntegrationOperation : IIntegrationOperation
    {
        public double Integrate(IFunction function, double lowerBound, double upperBound)
        {
            double result = 0;
            double step = 0.001; // Шаг интегрирования

            for (double x = lowerBound; x < upperBound; x += step)
            {
                result += function.Calculate(x) * step;
            }

            return result;
        }
    }

    // Фабрика функций
    public interface IFunctionFactory
    {
        IFunction CreateFunction(double[] coefficients);
    }

    public class PolynomialFunctionFactory : IFunctionFactory
    {
        public IFunction CreateFunction(double[] coefficients)
        {
            return new PolynomialFunction(coefficients);
        }
    }

    // Фабрика операций интегрирования
    public interface IIntegrationOperationFactory
    {
        IIntegrationOperation CreateOperation();
    }

    public class RectangleIntegrationOperationFactory : IIntegrationOperationFactory
    {
        public IIntegrationOperation CreateOperation()
        {
            return new RectangleIntegrationOperation();
        }
    }

    // Класс калькулятора
    public class Calculator
    {
        private static Calculator instance;
        private IIntegrationOperation integrationOperation;

        private List<Action<double>> resultSubscribers;

        private Calculator()
        {
            resultSubscribers = new List<Action<double>>();
        }

        public static Calculator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Calculator();
                }
                return instance;
            }
        }

        public void SubscribeToResults(Action<double> handler)
        {
            resultSubscribers.Add(handler);
        }

        public void UnsubscribeFromResults(Action<double> handler)
        {
            resultSubscribers.Remove(handler);
        }

        private void NotifySubscribers(double result)
        {
            foreach (Action<double> handler in resultSubscribers)
            {
                handler(result);
            }
        }

        public void SetIntegrationOperation(IIntegrationOperation integrationOperation)
        {
            this.integrationOperation = integrationOperation;
        }

        public double Calculate(IFunction function, double lowerBound, double upperBound)
        {
            if (integrationOperation == null)
            {
                throw new InvalidOperationException("Integration operation is not set.");
            }
            double result = integrationOperation.Integrate(function, lowerBound, upperBound);
            NotifySubscribers(result);
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Calculator calculator = Calculator.Instance;

            IFunctionFactory functionFactory = new PolynomialFunctionFactory();
            IIntegrationOperationFactory integrationOperationFactory = new RectangleIntegrationOperationFactory();

            Console.WriteLine("Enter the coefficients of the polynomial function (separated by spaces):");
            string[] coefficientsInput = Console.ReadLine().Split(' ');
            double[] coefficients = Array.ConvertAll(coefficientsInput, double.Parse);
            IFunction function = functionFactory.CreateFunction(coefficients);

            Console.WriteLine("Enter the lower bound:");
            double lowerBound = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Enter the upper bound:");
            double upperBound = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Select an integration method:");
            Console.WriteLine("1. Rectangle Integration");

            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    calculator.SetIntegrationOperation(integrationOperationFactory.CreateOperation());
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }

            calculator.SubscribeToResults(result => Console.WriteLine("Result: " + result));

            try
            {
                double result = calculator.Calculate(function, lowerBound, upperBound);
                Console.WriteLine("Result: " + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.ReadKey();
        }
    }
}