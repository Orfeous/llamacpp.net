using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LlamaCpp.Net.Abstractions;
using NUnit.Framework;

namespace LlamaCpp.Net.Test
{
    [TestFixture]
    public class test
    {
        [Test]
        public void CanLoadModel()
        {
            var modelFileName = "wizardLM-7B.ggmlv3.q4_0.bin";
            var modelPath = Path.Join(Constants.ModelDirectory, modelFileName);
            var model = new LanguageModel(modelPath);
        }
    }

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
}
