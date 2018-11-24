using NUnit.Framework;
using Serilog;

namespace SemanticVersioning.Acceptance
{
    public abstract class TestBase
    {
        [OneTimeSetUp]
        public virtual void FixtureSetup()
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.NUnitOutput().CreateLogger();
        }
    }
}