using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace AuditBuddy.Cmdlets.Tests
{
    public class describe_SetAdvancedAuditPolicyCmdlet
    {
        private SetAdvancedAuditPolicyCmdlet _cmdlet;
        private Mock<IAdvancedAuditPolicyManager> _policyManager;
        private Mock<ICommandRuntime> _runtime;

        private List<AdvancedAuditSubCategory> categories = new List<AdvancedAuditSubCategory>
        {
            new AdvancedAuditSubCategory() { Category = "Foo1", CategoryId = new Guid("11111111-1111-1111-1111-111111111111"), SubCategory = "Foo1", SubCategoryId = new Guid("11111111-1111-1111-1111-111111111111") },
            new AdvancedAuditSubCategory() { Category = "Foo2", CategoryId = new Guid("22222222-2222-2222-2222-222222222222"), SubCategory = "Foo2", SubCategoryId = new Guid("22222222-2222-2222-2222-222222222222") },
            new AdvancedAuditSubCategory() { Category = "Foo3", CategoryId = new Guid("33333333-3333-3333-3333-333333333333"), SubCategory = "Foo3", SubCategoryId = new Guid("33333333-3333-3333-3333-333333333333") },
            new AdvancedAuditSubCategory() { Category = "Foo4", CategoryId = new Guid("44444444-4444-4444-4444-444444444444"), SubCategory = "Foo4", SubCategoryId = new Guid("44444444-4444-4444-4444-444444444444") }
        };

        private List<AdvancedAuditPolicy> policies = new List<AdvancedAuditPolicy>
        {
            new AdvancedAuditPolicy() { Category = "Foo1", Setting = AuditingFlags.Success, CategoryId = new Guid("11111111-1111-1111-1111-111111111111"), SubCategory = "Foo1", SubCategoryId = new Guid("11111111-1111-1111-1111-111111111111") },
            new AdvancedAuditPolicy() { Category = "Foo2", Setting = AuditingFlags.Success, CategoryId = new Guid("22222222-2222-2222-2222-222222222222"), SubCategory = "Foo2", SubCategoryId = new Guid("22222222-2222-2222-2222-222222222222") },
            new AdvancedAuditPolicy() { Category = "Foo3", Setting = AuditingFlags.Success, CategoryId = new Guid("33333333-3333-3333-3333-333333333333"), SubCategory = "Foo3", SubCategoryId = new Guid("33333333-3333-3333-3333-333333333333") },
            new AdvancedAuditPolicy() { Category = "Foo4", Setting = AuditingFlags.Success, CategoryId = new Guid("44444444-4444-4444-4444-444444444444"), SubCategory = "Foo4", SubCategoryId = new Guid("44444444-4444-4444-4444-444444444444") }
        };

        [TestInitialize]
        public void before_each()
        {
            _policyManager = new Mock<IAdvancedAuditPolicyManager>();
            _cmdlet = new SetAdvancedAuditPolicyCmdlet(_policyManager.Object);
            _runtime = new Mock<ICommandRuntime>();
            _cmdlet.CommandRuntime = _runtime.Object;
            _policyManager.Setup(n => n.GetAdvancedAuditPolicies()).Returns(policies);
            _policyManager.Setup(n => n.GetAdvancedAuditSubCategories()).Returns(categories);
        }

        [TestClass]
        public class when_SettingAdvancedAuditPolicies : describe_SetAdvancedAuditPolicyCmdlet
        {
            [TestMethod]
            public void when_setting_advanced_policies_SetAdvancedAuditPolicy_is_used()
            {
                _cmdlet.Policy = policies.ToArray();
                _cmdlet.Setting = "Success";
                _cmdlet.ProcessInternal();
                _policyManager.Verify(n => n.SetAdvancedAuditPolicy(policies), Times.Once());
            }

            [TestMethod]
            public void when_setting_advanced_policies_the_setting_is_updated()
            {
                AuditingFlags setting = AuditingFlags.Failure;
                _cmdlet.Setting = setting.ToString();
                _cmdlet.Policy = policies.ToArray();
                _cmdlet.ProcessInternal();

                List<AdvancedAuditPolicy> updatedPolicies = policies;

                foreach(AdvancedAuditPolicy policy in updatedPolicies)
                {
                    policy.Setting = setting;
                }

                _policyManager.Verify(n => n.SetAdvancedAuditPolicy(updatedPolicies), Times.Once());
            }

            [TestMethod]
            public void when_invalid_category_name_is_provided_the_cmdlet_throws()
            {
                _cmdlet.Setting = "Success";
                //random guid will not be in the setup
                _cmdlet.CategoryName = new string[] { "Bar" };
                Assert.ThrowsException<ArgumentException>(() => _cmdlet.ProcessInternal());
            }

            [TestMethod]
            public void when_invalid_sub_category_name_is_provided_the_cmdlet_throws()
            {
                _cmdlet.Setting = "Success";
                //random name will not be in the setup
                _cmdlet.SubCategoryName = new string[] { "Bar" };
                Assert.ThrowsException<ArgumentException>(() => _cmdlet.ProcessInternal());
            }

            [TestMethod]
            public void when_no_input_values_are_provided_the_cmdlet_throws()
            {
                Assert.ThrowsException<ArgumentNullException>(() => _cmdlet.ProcessInternal());
            }

            [TestMethod]
            public void when_both_is_provided_the_correct_flag_is_set()
            {   
                _cmdlet.Setting = "Both";
                _cmdlet.Policy = policies.ToArray();
                _cmdlet.ProcessInternal();

                List<AdvancedAuditPolicy> updatedPolicies = policies;

                foreach (AdvancedAuditPolicy policy in updatedPolicies)
                {
                    policy.Setting = AuditingFlags.Success | AuditingFlags.Failure;
                }

                _policyManager.Verify(n => n.SetAdvancedAuditPolicy(updatedPolicies), Times.Once());
            }

        }
    }
}
