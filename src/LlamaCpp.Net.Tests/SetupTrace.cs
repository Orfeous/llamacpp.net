﻿using NUnit.Framework;
using System.Diagnostics;

namespace LlamaCpp.Net.Tests;

[SetUpFixture]
public class SetupTrace
{
    [OneTimeSetUp]
    public void StartTest()
    {
        Trace.Listeners.Add(new ConsoleTraceListener()
        {

        });
    }

    [OneTimeTearDown]
    public void EndTest()
    {
        Trace.Flush();
    }
}