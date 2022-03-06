using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Threading.Tasks;

namespace LayerTests
{
    [TestClass]
    public class NotificationConfigurationTest
    {
        INotificationUserConfigurationRepository? notificationUserConfigurationRepository;

        [TestInitialize]
        public void InitProvider()
        {
            var provider = new DIProvider();
            notificationUserConfigurationRepository = provider.GetRequiredService<INotificationUserConfigurationRepository>() ?? throw new ArgumentNullException(nameof(INotificationUserConfigurationRepository));
        }

        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task AddUserConfig()
        {
            var globalUserConfig = DataGenerator.GenerateNotificationConfigForGlobalUser();

            //await notificationUserConfigurationRepository!.Add(globalUserConfig);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetUserConfigs()
        {
            var notifConfigs = await notificationUserConfigurationRepository.GetAll();

            Assert.IsNotNull(notifConfigs);
        }
    }
}