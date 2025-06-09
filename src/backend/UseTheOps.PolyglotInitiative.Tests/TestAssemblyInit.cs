using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UseTheOps.PolyglotInitiative.Models;

namespace UseTheOps.PolyglotInitiative.Tests
{
    [TestClass]
    public class TestAssemblyInit
    {
        public static TestContainerFixture Fixture;
        public static bool Initialized = false;

        [AssemblyInitialize]
        public static async Task MyAssemblyInit(TestContext context)
        {
            Fixture = new TestContainerFixture();
            await Fixture.InitializeAsync();
            Initialized = true;
        }

        [AssemblyCleanup]
        public static async Task MyAssemblyCleanup()
        {
            if (Initialized)
                await Fixture.DisposeAsync();
        }
    }
}
