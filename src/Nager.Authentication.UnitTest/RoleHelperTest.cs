using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.Authentication.Helpers;
using System.Threading.Tasks;

namespace Nager.Authentication.UnitTest
{
    [TestClass]
    public class RoleHelperTest
    {
        [TestMethod]
        public async Task AddRoleToRoleData_Test_Test()
        {
            var roleData = RoleHelper.AddRoleToRoleData(null, "test");
            roleData = RoleHelper.AddRoleToRoleData("", "test");
            roleData = RoleHelper.AddRoleToRoleData(",", "test");

            //TODO: fix the test logic
        }
    }
}
