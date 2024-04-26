using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

[Serializable]
public class Singleton
{
    private static readonly object lockObject = new object();
    private static Singleton? instance;

    public static Singleton Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
            }
            return instance;
        }
    }

    private Singleton() { }

    public void Serialize(string filePath)
    {
        lock (lockObject)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Singleton));
                serializer.Serialize(fileStream, this);
            }
        }
    }

    public static Singleton Deserialize(string filePath)
    {
        lock (lockObject)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Singleton));
                var deserializedInstance = (Singleton)serializer.Deserialize(fileStream);
                if (instance == null)
                {
                    instance = deserializedInstance;
                }
                return instance;
            }
        }
    }
}

class Program1
{
    static void Main(string[] args)
    {
        Singleton singleton = Singleton.Instance;
        string filePath = "singleton.xml";
        singleton.Serialize(filePath);

        // Removing the current instance - simulating absence of instance in memory
        singleton = null;

        // Deserializing Singleton instance
        Singleton deserializedSingleton = Singleton.Deserialize(filePath);

        // Checking if deserialization preserved the single instance
        Console.WriteLine(ReferenceEquals(Singleton.Instance, deserializedSingleton)); // Returns true

        RunPerformanceTests();

        Console.ReadLine();
    }

    static void RunPerformanceTests()
    {
        Console.WriteLine("Starting performance tests...");

        Stopwatch serializationTimer = new Stopwatch();
        serializationTimer.Start();

        for (int i = 0; i < 1000; i++)
        {
            Singleton singleton = Singleton.Instance;
            singleton.Serialize($"test_{i}.xml");
        }

        serializationTimer.Stop();
        Console.WriteLine($"Serialization time: {serializationTimer.ElapsedMilliseconds} ms");

        Stopwatch deserializationTimer = new Stopwatch();
        deserializationTimer.Start();

        for (int i = 0; i < 1000; i++)
        {
            Singleton deserializedSingleton = Singleton.Deserialize($"test_{i}.xml");
        }

        deserializationTimer.Stop();
        Console.WriteLine($"Deserialization time: {deserializationTimer.ElapsedMilliseconds} ms");

        Console.WriteLine("Performance tests completed.");
    }
}
