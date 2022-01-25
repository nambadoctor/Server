using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceTests.Services.v1.ScenarioTests.Web.Provider
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            TestInitializer.Instance.Initialize().Wait();
        }

        [AssemblyCleanup]
        static public void AssemblyCleanup()
        {
            TestInitializer.Instance.CleanUp();
        }
    }
}
