using AutoMapper;

namespace WebApiCrudExample.Application.UnitTests
{
    [TestClass]
    public class AutoMapperTests
    {
        [TestMethod]
        public void AssertConfigurationIsValid_Test()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>());
            config.AssertConfigurationIsValid();

        }
    }
}