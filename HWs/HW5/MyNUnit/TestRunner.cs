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

        var testArg = testMethod.GetCustomAttribute<Test>();
        if (testArg?.Ignore is not null)
        {
            result.IsIgnored = true;
            result.Reason = testArg.Ignore;
            return result;
        }

        if (testMethod.GetParameters().Length > 0 || testMethod.ReturnType != typeof(void))
        {
            result.IsIgnored = true;
            result.Reason = "Invalid test method signature.";
            return result;
        }

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
            if (testArg?.Expected is not null && e.InnerException?.GetType() == testArg.Expected)
            {
                result.IsPassed = true;
            }
            else
            {
                result.IsPassed = false;
                result.ExceptionMessage = e.InnerException?.Message ?? "Unknown exception.";
            }
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
        int passedCount = 0;
        int failedCount = 0;
        int ignoredCount = 0;

        foreach (var result in results)
        {
            if (result.IsIgnored)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"IGNORED: {result.TestName}\n    Reason: {result.Reason}");
                ignoredCount++;
            }
            else if (result.IsPassed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"PASSED: {result.TestName}\n    Duration: {result.Duration.TotalMilliseconds} ms");
                passedCount++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"FAILED: {result.TestName}\n    Duration: {result.Duration.TotalMilliseconds} ms");
                Console.WriteLine($"Exception: {result.ExceptionMessage}");
                failedCount++;
            }

            Console.ResetColor();
            Console.WriteLine("\n");
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Total tests: {results.Count}. Passed: {passedCount}. Failed: {failedCount}. Ignored: {ignoredCount}.");
        Console.ResetColor();
    }
}

