using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace AuditBuddy.Cmdlets.Tests
{
    public class describe_GetAdvancedAuditPolicyCmdlet
    {
        private GetAdvancedAuditPolicyCmdlet _cmdlet;
        private Mock<IAdvancedAuditPolicyManager> _policyManager;
        private Mock<ICommandRuntime> _runtime;

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
            _cmdlet = new GetAdvancedAuditPolicyCmdlet(_policyManager.Object);
            _runtime = new Mock<ICommandRuntime>();
            _cmdlet.CommandRuntime = _runtime.Object;
            _policyManager.Setup(n => n.GetAdvancedAuditPolicies()).Returns(policies);
        }

        [TestClass]
        public class when_GettingAdvancedAuditPolicies : describe_GetAdvancedAuditPolicyCmdlet
        {
            [TestMethod]
            public void when_getting_advanced_audit_policy_GetAuditPolicy_is_called()
            {
                _cmdlet.ProcessInternal();
                _policyManager.Verify(n => n.GetAdvancedAuditPolicies(), Times.Once());
            }

            [TestMethod]
            public void when_getting_advanced_audit_policy_audit_policies_are_written()
            {
                _cmdlet.ProcessInternal();
                _runtime.Verify(r => r.WriteObject(It.IsAny<AdvancedAuditPolicy>()), Times.Exactly(policies.Count));
            }

            [TestMethod]
            public void when_category_name_is_provided_results_are_filtered()
            {
                _cmdlet.CategoryName = "Foo1";
                _cmdlet.ProcessInternal();

                _runtime.Verify(r => r.WriteObject(It.IsAny<AdvancedAuditPolicy>()), Times.Exactly(1));
            }

            [TestMethod]
            public void when_subcategory_objects_are_provided_results_are_filtered()
            {
                AdvancedAuditSubCategory[] subcategories = new AdvancedAuditSubCategory[] 
                { 
                    new AdvancedAuditSubCategory() 
                    { 
                        Category = "Foo1", 
                        CategoryId = new Guid("11111111-1111-1111-1111-111111111111"), 
                        SubCategory = "Foo1", 
                        SubCategoryId = new Guid("11111111-1111-1111-1111-111111111111") } 
                };

                _cmdlet.SubCategory = subcategories;
                _cmdlet.ProcessInternal();

                _runtime.Verify(r => r.WriteObject(It.IsAny<AdvancedAuditPolicy>()), Times.Exactly(1));
            }
        }
    }
}
