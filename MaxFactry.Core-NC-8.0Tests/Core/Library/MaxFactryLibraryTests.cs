using Microsoft.VisualStudio.TestTools.UnitTesting;
using MaxFactry.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxFactry.Core.Tests
{
    [TestClass()]
    public class MaxFactryLibraryTests
    {


        [TestMethod()]
        public void SetSettingTest()
        {
            try
            {
                MaxSettingsStructure loSetting = new MaxSettingsStructure("TestValue", typeof(object));
                MaxFactryLibrary.SetSetting("TestKey", loSetting);
            }
            catch (Exception loE)
            {
                Assert.Fail(loE.Message);
            }            
        }

        [TestMethod()]
        public void GetSettingTest()
        {
            try
            {
                MaxSettingsStructure loSetting = new MaxSettingsStructure("TestValue", typeof(object));
                MaxFactryLibrary.SetSetting("TestKey", loSetting);
                MaxSettingsStructure loSettingReturn = MaxFactryLibrary.GetSetting("TestKey");
                Assert.IsNotNull(loSettingReturn);
            }
            catch (Exception loE)
            {
                Assert.Fail(loE.Message);
            }
        }

        [TestMethod()]
        public void SetValueTest()
        {
            try
            {
                MaxFactryLibrary.SetValue("TestKey", "TestValue");
                object loValue= MaxFactryLibrary.GetValue("TestKey");
                Assert.AreEqual("TestValue", loValue);
            }
            catch (Exception loE)
            {
                Assert.Fail(loE.Message);
            }
        }

        [TestMethod()]
        public void GetValueTest()
        {
            try
            {
                MaxFactryLibrary.SetValue("TestKey", "TestValue");
                object loValue = MaxFactryLibrary.GetValue("TestKey");
                Assert.IsNotNull(loValue);
                Assert.AreEqual("TestValue", loValue);
            }
            catch (Exception loE)
            {
                Assert.Fail(loE.Message);
            }
        }

        [TestMethod()]
        public void GetValueStringTest()
        {
            try
            {
                MaxFactryLibrary.SetValue("TestKey", "TestValue");
                string lsValue = MaxFactryLibrary.GetValueString("TestKey", "none");
                Assert.IsNotNull(lsValue);
                Assert.AreEqual("TestValue", lsValue);
            }
            catch (Exception loE)
            {
                Assert.Fail(loE.Message);
            }
        }

        [TestMethod()]
        public void ResetTest()
        {
            try
            {
                MaxFactryLibrary.SetValue("TestKey", "TestValue");
                MaxFactryLibrary.Reset();
                object loValue = MaxFactryLibrary.GetValue("TestKey");
                Assert.IsNull(loValue);
            }
            catch (Exception loE)
            {
                Assert.Fail(loE.Message);
            }
        }

        [TestMethod()]
        public void CreateTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateTest4()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateSingletonTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateSingletonTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateProviderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateProviderTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateProviderTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateProviderTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateProviderTest4()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateSingletonProviderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateSingletonProviderTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateSingletonProviderTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateSingletonProviderTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateSingletonProviderTest4()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveSingletonProviderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveSingletonProviderTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetConstructorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBaseTypeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetMethodsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAssemblyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetStringResourceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPropertyListValueTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPropertyListTest()
        {
            Assert.Fail();
        }
    }
}