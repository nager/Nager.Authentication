using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.Authentication.Helpers;
using System.Threading.Tasks;

namespace Nager.Authentication.UnitTest
{
    [TestClass]
    public class RoleHelperTest
    {
        [TestMethod]
        public async Task AddRoleToRoleData_RoleDataNull_Successful()
        {
            var roleData = RoleHelper.AddRoleToRoleData(null, "test");

            Assert.AreEqual(roleData, "test");
        }

        [TestMethod]
        public async Task AddRoleToRoleData_RoleDataStringEmpty_Successful()
        {
            var roleData = RoleHelper.AddRoleToRoleData(string.Empty, "test");

            Assert.AreEqual(roleData, "test");
        }

        [TestMethod]
        public async Task AddRoleToRoleData_DuplicateRoleTest_Successful()
        {
            var roleData = RoleHelper.AddRoleToRoleData("test", "test");

            Assert.AreEqual(roleData, "test");
        }

        [TestMethod]
        public async Task AddRoleToRoleData_DuplicateRoleTestPascalCase_Successful()
        {
            var roleData = RoleHelper.AddRoleToRoleData("test", "Test");

            Assert.AreEqual(roleData, "test");
        }
    }
}
