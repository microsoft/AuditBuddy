using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Management.Automation;

namespace AuditBuddy.Cmdlets.Tests
{
    public class describe_GetAuditPolicyCategoriesCmdlet
    {
        private GetAuditPolicyCategoriesCmdlet _cmdlet;
        private Mock<IAuditPolicyManager> _policyManager;
        private Mock<ICommandRuntime> _runtime;

        private List<string> auditCategories = new List<string>
        {
            "Foo",
            "Bar"
        };

        [TestInitialize]
        public void before_each()
        {
            _policyManager = new Mock<IAuditPolicyManager>();
            _cmdlet = new GetAuditPolicyCategoriesCmdlet(_policyManager.Object);
            _runtime = new Mock<ICommandRuntime>();
            _cmdlet.CommandRuntime = _runtime.Object;
            _policyManager.Setup(n => n.GetAuditCategories()).Returns(auditCategories);
        }

        [TestClass]
        public class when_GettingCategories : describe_GetAuditPolicyCategoriesCmdlet
        {
            [TestMethod]
            public void when_getting_categories_GetAuditCategoryNames_is_used()
            {
                _cmdlet.ProcessInternal();
                _policyManager.Verify(n => n.GetAuditCategories(), Times.Once());
            }

        [TestMethod]
        public void when_getting_audit_categories_they_are_written()
        {
            _cmdlet.ProcessInternal();
            _runtime.Verify(r => r.WriteObject(It.IsAny<string>()), Times.Exactly(auditCategories.Count));
        }
        }
    }
}
