using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace AuditBuddy.Cmdlets.Tests
{
    public class describe_GetAdvancedAuditPolicyCategoriesCmdlet
    {
        private GetAdvancedAuditPolicyCategoriesCmdlet _cmdlet;
        private Mock<IAdvancedAuditPolicyManager> _policyManager;
        private Mock<ICommandRuntime> _runtime;

        private List<AdvancedAuditCategory> policyCategories = new List<AdvancedAuditCategory>
        {
            new AdvancedAuditCategory() { Category = "Foo1", CategoryId = new Guid() },
            new AdvancedAuditCategory() { Category = "Foo2", CategoryId = new Guid() }
        };

        [TestInitialize]
        public void before_each()
        {
            _policyManager = new Mock<IAdvancedAuditPolicyManager>();
            _cmdlet = new GetAdvancedAuditPolicyCategoriesCmdlet(_policyManager.Object);
            _runtime = new Mock<ICommandRuntime>();
            _cmdlet.CommandRuntime = _runtime.Object;
            _policyManager.Setup(n => n.GetAdvancedAuditCategories()).Returns(policyCategories);
        }

        [TestClass]
        public class when_GettingCategories : describe_GetAdvancedAuditPolicyCategoriesCmdlet
        {
            [TestMethod]
            public void when_getting_categories_GetCategories_is_used()
            {
                _cmdlet.ProcessInternal();
                _policyManager.Verify(n => n.GetAdvancedAuditCategories(), Times.Once());
            }

            [TestMethod]
            public void when_getting_audit_categories_they_are_written()
            {
                _cmdlet.ProcessInternal();
                _runtime.Verify(r => r.WriteObject(It.IsAny<AdvancedAuditCategory>()), Times.Exactly(policyCategories.Count));
            }
        }
    }
}
