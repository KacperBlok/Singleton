using System;
using System.Diagnostics;
using System.Text;
using System.Threading;



//Solving problem: Resilience to concurrent code usage while maintaining maximum performance
public sealed class Singleton
{

    private static volatile Singleton instance = new Singleton();

    private static readonly object syncRoot = new object();

    private Singleton() { }

    public static Singleton Instance
    {
        get
        {
            return instance;
        }
    }


    public void DoSomething(StringBuilder results)
    {
        lock (syncRoot)
        {
            int sum = 0; // Initializes the sum variable to store the sum result
            for (int i = 1; i <= 1000; i++)
            {
                sum += i;
            }
            results.AppendLine($"Value for thread nr. {Thread.CurrentThread.ManagedThreadId}: {sum}");
        }
    }


    public class SingletonConcurrencyTest
    {

        public void TestSingletonConcurrency()
        {
            int numThreads = 20;
            int numIterations = 30;

            Thread[] threads = new Thread[numThreads]; // Creates an array of threads
            StringBuilder results = new StringBuilder(); // Creates a StringBuilder object to store results

            // Loop to create and start threads
            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < numIterations; j++) // Loop executed 1000 times
                    {
                        Singleton.Instance.DoSomething(results); // Calls the DoSomething method for the Singleton object
                    }
                });
                threads[i].Start(); // Starts the thread
            }


            foreach (Thread t in threads)
            {
                t.Join();
            }


            Console.WriteLine(results.ToString());

            // Checks the correctness of sums in the results buffer
            int expectedSum = 500500; // Specifies the expected sum

            if (results.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(':')[1].Trim())
                .Select(int.Parse)
                .All(sum => sum == expectedSum)) // Checks if all sums are equal to the expected sum
            {
                Console.WriteLine("All sums are correct.");
            }
            else
            {
                Console.WriteLine("At least one sum is incorrect.");
            }
        }
        public void TestPerformance()
        {
            Console.WriteLine("Starting performance test...");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 1000; i++)
            {
                Singleton singleton = Singleton.Instance;
                singleton.DoSomething(new StringBuilder());
            }

            stopwatch.Stop();
            Console.WriteLine($"Performance test completed in: {stopwatch.ElapsedMilliseconds} ms");
        }
    }


    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Task 1");
            Console.WriteLine("Resilience to concurrent code usage while maintaining maximum performance");

            SingletonConcurrencyTest test = new SingletonConcurrencyTest();
            test.TestSingletonConcurrency();

            test.TestPerformance();
        }
    }
}
