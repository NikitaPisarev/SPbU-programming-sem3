namespace MyNUnit;

using System.Reflection;
using System.Threading.Tasks;

public static class TestRunner
{
    public static void RunTestsFromPath(string path)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Указанный путь не существует: {path}");
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
    }
}

