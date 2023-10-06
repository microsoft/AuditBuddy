using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace AuditBuddy.Cmdlets.Tests
{
    public class describe_GetAdvancedAuditPolicySubCategoriesCmdlet
    {
        private GetAdvancedAuditPolicySubCategoriesCmdlet _cmdlet;
        private Mock<IAdvancedAuditPolicyManager> _policyManager;
        private Mock<ICommandRuntime> _runtime;

        private List<AdvancedAuditSubCategory> policyCategories = new List<AdvancedAuditSubCategory>
        {
            new AdvancedAuditSubCategory() { Category = "Foo1", CategoryId = new Guid(), SubCategory = "Foo1", SubCategoryId = new Guid()},
            new AdvancedAuditSubCategory() { Category = "Foo2", CategoryId = new Guid(), SubCategory = "Foo2", SubCategoryId = new Guid()}
        };

        [TestInitialize]
        public void before_each()
        {
            _policyManager = new Mock<IAdvancedAuditPolicyManager>();
            _cmdlet = new GetAdvancedAuditPolicySubCategoriesCmdlet(_policyManager.Object);
            _runtime = new Mock<ICommandRuntime>();
            _cmdlet.CommandRuntime = _runtime.Object;
            _policyManager.Setup(n => n.GetAdvancedAuditSubCategories()).Returns(policyCategories);
        }

        [TestClass]
        public class when_GettingCategories : describe_GetAdvancedAuditPolicySubCategoriesCmdlet
        {
            [TestMethod]
            public void when_getting_categories_GetCategories_is_used()
            {
                _cmdlet.ProcessInternal();
                _policyManager.Verify(n => n.GetAdvancedAuditSubCategories(), Times.Once());
            }

            [TestMethod]
            public void when_getting_audit_categories_they_are_written()
            {
                _cmdlet.ProcessInternal();
                _runtime.Verify(r => r.WriteObject(It.IsAny<AdvancedAuditSubCategory>()), Times.Exactly(policyCategories.Count));
            }

            [TestMethod]
            public void when_category_name_is_provided_results_are_filtered()
            {
                _cmdlet.Category = "Foo2";
                _cmdlet.ProcessInternal();
                _runtime.Verify(r => r.WriteObject(It.IsAny<AdvancedAuditSubCategory>()), Times.Exactly(1));
            }

        }
    }
}
