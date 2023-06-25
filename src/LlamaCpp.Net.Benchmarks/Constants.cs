namespace LlamaCpp.Net.Benchmarks;

public static class Constants
{
    public static string ModelDirectory => Path.Join(GetRepositoryRoot(), "models");

    private static string GetRepositoryRoot()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;


        while (!Directory.Exists(Path.Join(baseDirectory, ".git")))
        {
            var directoryInfo = new DirectoryInfo(baseDirectory).Parent;
            if (directoryInfo != null)
            {
                baseDirectory = directoryInfo.FullName;
            }
        }

        return baseDirectory;
    }
}
