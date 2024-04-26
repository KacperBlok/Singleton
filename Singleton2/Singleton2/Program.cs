using System;
using System.Diagnostics;
using System.Threading;

public sealed class ThreadLocalSingleton
{
    private static readonly ThreadLocal<ThreadLocalSingleton> threadInstance =
        new ThreadLocal<ThreadLocalSingleton>(() => new ThreadLocalSingleton(), trackAllValues: true);

    private ThreadLocalSingleton() { }

    // Właściwość dostępu do instancji ThreadLocalSingleton dla danego wątku
    public static ThreadLocalSingleton Instance => threadInstance.Value;

    public void DisplayHashCode()
    {
        // Wykorzystanie właściwości Instance, aby uzyskać dostęp do instancji dla bieżącego wątku
        Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}, Singleton hash code: {Instance.GetHashCode()}");
    }

    public static void RunPerformanceTest()
    {
        const int numThreads = 10; 
        const int numIterations = 100; 

        Stopwatch stopwatch = new Stopwatch(); 
        stopwatch.Start(); // Rozpoczęcie pomiaru czasu

        Thread[] threads = new Thread[numThreads]; 

        // Tworzenie i uruchamianie wątków
        for (int i = 0; i < numThreads; i++)
        {
            Thread thread = new Thread(() =>
            {
                // Wykonanie określonej liczby iteracji
                for (int j = 0; j < numIterations; j++)
                {
                    Instance.DisplayHashCode(); 
                }
            });

            threads[i] = thread; 
            thread.Start(); 
        }

        // Oczekiwanie na zakończenie wszystkich wątków
        for (int i = 0; i < numThreads; i++)
        {
            threads[i].Join(); 
        }

        stopwatch.Stop(); 
        Console.WriteLine($"Performance test completed in: {stopwatch.ElapsedMilliseconds} ms"); 
    }
}
class Program
{
    static void Main(string[] args)
    {
        ThreadLocalSingleton.RunPerformanceTest(); 
    }
}
