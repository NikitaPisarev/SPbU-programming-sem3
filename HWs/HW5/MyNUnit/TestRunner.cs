namespace MyNUnit;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using MyNUnit.Attributes;

public static class TestRunner
{
    public static void RunTestsFromPath(string path)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"File not found: {path}");
            return;
        }

        var assemblies = LoadAssembliesFromPath(path);
        Parallel.ForEach(assemblies, RunTestsInAssembly);
    }

    private static IEnumerable<Assembly> LoadAssembliesFromPath(string path)
    {
        var assemblies = new List<Assembly>();
        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                assemblies.Add(assembly);
            }
            catch (Exception)
            {
                throw;
            }
        }
        return assemblies;
    }

    private static void RunTestsInAssembly(Assembly assembly)
    {
        var results = new ConcurrentBag<TestResult>();

        foreach (var type in assembly.GetTypes())
        {
            var testMethods = type.GetMethods()
                                .Where(m => m.GetCustomAttributes(typeof(Test), false).Any())
                                .ToArray();

            if (testMethods.Length > 0)
            {
                InvokeMethodsWithAttribute(type, typeof(BeforeClass), null);

                Parallel.ForEach(testMethods, method =>
                {
                    var result = RunTest(type, method);
                    results.Add(result);
                });

                InvokeMethodsWithAttribute(type, typeof(AfterClass), null);
            }
        }

        PrintResults(results.ToList());
    }

    private static TestResult RunTest(Type type, MethodInfo testMethod)
    {
        var result = new TestResult(testMethod.Name);
        var stopwatch = new Stopwatch();

        var instance = Activator.CreateInstance(type);
        InvokeMethodsWithAttribute(type, typeof(Before), instance);

        stopwatch.Start();
        try
        {
            testMethod.Invoke(instance, null);
        }
        catch (Exception e)
        {
            result.IsPassed = false;
            result.ExceptionMessage = e.InnerException?.Message ?? "Unknown exception.";
        }
        stopwatch.Stop();

        InvokeMethodsWithAttribute(type, typeof(After), instance);

        result.Duration = stopwatch.Elapsed;
        return result;
    }

    private static void InvokeMethodsWithAttribute(Type type, Type attributeType, object? instance)
    {
        foreach (var method in type.GetMethods())
        {
            if (method.GetCustomAttributes(attributeType, false).Length > 0)
            {
                if (attributeType == typeof(BeforeClass) || attributeType == typeof(AfterClass))
                {
                    method.Invoke(null, null);
                }
                else
                {
                    method.Invoke(instance, null);
                }
            }
        }
    }

    private static void PrintResults(List<TestResult> results)
    {
        foreach (var result in results)
        {
            Console.WriteLine($"Test: {result.TestName}, Passed: {result.IsPassed}, Time: {result.Duration.TotalMilliseconds} ms");
            if (!result.IsPassed)
            {
                Console.WriteLine($"    Exception: {result.ExceptionMessage}");
            }
        }
    }
}

