using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuditBuddyLibrary.Tests
{
    public class describe_AdvancedAuditPolicyManager
    {
        private AdvancedAuditPolicyManager _manager;
        private Mock<INativeInterface> _nativeInterface;

        [TestInitialize]
        public void before_each()
        {
            _nativeInterface = new Mock<INativeInterface>();
            _manager = new AdvancedAuditPolicyManager(_nativeInterface.Object);
        }

        [TestClass]
        public class when_GettingCategories : describe_AdvancedAuditPolicyManager
        {
            [TestMethod]
            public void when_getting_audit_categories_NativeInterface_is_called()
            {
                var policy = _manager.GetAdvancedAuditCategories();
                _nativeInterface.Verify(n => n.GetAdvancedAuditCategories(), Times.Once());
            }
        }

        [TestClass]
        public class when_GettingAdvancedAuditPolicies : describe_AdvancedAuditPolicyManager
        {
            [TestMethod]
            public void when_getting_audit_policies_NativeInterface_is_called()
            {
                var policy = _manager.GetAdvancedAuditPolicies();
                _nativeInterface.Verify(n => n.GetAdvancedAuditPolicies(), Times.Once());
            }
        }

        [TestClass]
        public class when_GetAdvancedAuditPolicy : describe_AdvancedAuditPolicyManager
        {
            [TestMethod]
            public void when_getting_an_audit_policies_the_correct_one_is_returned()
            {
                List<AdvancedAuditPolicy> advancedAuditPolicies = GetFakePolicies();
                _nativeInterface.Setup(n => n.GetAdvancedAuditPolicy(It.IsAny<Guid>())).Returns(advancedAuditPolicies.First());
                var policy = _manager.GetAdvancedAuditPolicy(new Guid("11111111-1111-1111-1111-111111111111"));

                Assert.AreEqual(new Guid("11111111-1111-1111-1111-111111111111"), policy.CategoryId);
            }
        }

        [TestClass]
        public class when_SettingAdvancedAuditPolicy : describe_AdvancedAuditPolicyManager
        {
            [TestMethod]
            public void when_setting_an_advanced_audit_policy_NativeInterface_is_called()
            {
                List<AdvancedAuditPolicy> advancedAuditPolicies = GetFakePolicies();
                _manager.SetAdvancedAuditPolicy(advancedAuditPolicies[1]);

                _nativeInterface.Verify(n => n.SetAdvancedAuditPolicy(advancedAuditPolicies[1]), Times.Once());
            }
        }

        [TestClass]
        public class when_SettingAdvancedAuditPolicies : describe_AdvancedAuditPolicyManager
        {
            [TestMethod]
            public void when_setting_advanced_audit_policies_NativeInterface_is_called()
            {
                List<AdvancedAuditPolicy> advancedAuditPolicies = GetFakePolicies();
                _manager.SetAdvancedAuditPolicy(advancedAuditPolicies);
                _nativeInterface.Verify(n => n.SetAdvancedAuditPolicy(It.IsAny<AdvancedAuditPolicy[]>()), Times.Exactly(1));
            }
        }

        private List<AdvancedAuditPolicy> GetFakePolicies()
        {
            return new List<AdvancedAuditPolicy>
                {
                    new AdvancedAuditPolicy { Category = "foo", CategoryId = new Guid("11111111-1111-1111-1111-111111111111"), Setting = AuditingFlags.Success, SubCategory = "Foo", SubCategoryId = new Guid("11111111-1111-1111-1111-111111111111") },
                    new AdvancedAuditPolicy { Category = "foo2", CategoryId = new Guid("22222222-2222-2222-2222-222222222222"), Setting = AuditingFlags.Success, SubCategory = "Foo2", SubCategoryId = new Guid("22222222-2222-2222-2222-222222222222") },
                    new AdvancedAuditPolicy { Category = "foo3", CategoryId = new Guid("33333333-3333-3333-3333-333333333333"), Setting = AuditingFlags.Success, SubCategory = "Foo3", SubCategoryId = new Guid("33333333-3333-3333-3333-333333333333") }
                };
        }
    }
}
