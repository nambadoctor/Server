using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.GenericRepository.Interfaces;
using System;
using System.Threading.Tasks;

namespace LayerTests
{
    [TestClass]
    public class SettingsConfigurationTest
    {
        ISettingsConfigurationRepository? settingsConfigurationRepository;

        [TestInitialize]
        public void InitProvider()
        {
            var provider = new DIProvider();
            settingsConfigurationRepository = provider.GetRequiredService<ISettingsConfigurationRepository>() ?? throw new ArgumentNullException(nameof(ISettingsConfigurationRepository));
        }

        [TestMethod]
        public async Task AddUserConfig()
        {
            var globalUserConfig = DataGenerator.GetSampleSettingsConfiguration();

            await settingsConfigurationRepository!.Add(globalUserConfig);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetUserConfigs()
        {
            var notifConfigs = await settingsConfigurationRepository.GetAll();

            Assert.IsNotNull(notifConfigs);
        }
    }
}