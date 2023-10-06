using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Management.Automation;

namespace AuditBuddy.Cmdlets.Tests
{
    public class describe_GetAuditPolicyCmdlet
    {
        private GetAuditPolicyCmdlet _cmdlet;
        private Mock<IAuditPolicyManager> _policyManager;
        private Mock<ICommandRuntime> _runtime;

        private List<AuditPolicy> policies = new List<AuditPolicy>
        {
            new AuditPolicy() { Category = "Foo1", Setting = AuditingFlags.Success },
            new AuditPolicy() { Category = "Foo2", Setting = AuditingFlags.Success },
            new AuditPolicy() { Category = "Foo3", Setting = AuditingFlags.Success },
            new AuditPolicy() { Category = "Foo4", Setting = AuditingFlags.Success }
        };

        [TestInitialize]
        public void before_each()
        {
            _policyManager = new Mock<IAuditPolicyManager>();
            _cmdlet = new GetAuditPolicyCmdlet(_policyManager.Object);
            _runtime = new Mock<ICommandRuntime>();
            _cmdlet.CommandRuntime = _runtime.Object;
            _policyManager.Setup(n => n.GetAuditPolicies()).Returns(policies);
            _policyManager.Setup(n => n.GetAuditCategories()).Returns(new List<string> { "Foo1", "Foo2"});
        }

        [TestClass]
        public class when_GettingAuditPolicies : describe_GetAuditPolicyCmdlet
        {
            [TestMethod]
            public void when_getting_audit_policy_GetAuditPolicy_is_called()
            {
                _cmdlet.ProcessInternal();
                _policyManager.Verify(n => n.GetAuditPolicies(), Times.Once());
            }

            [TestMethod]
            public void when_getting_audit_policy_audit_policies_are_written()
            {
                _cmdlet.ProcessInternal();
                _runtime.Verify(r => r.WriteObject(It.IsAny<AuditPolicy>()), Times.Exactly(policies.Count));
            }

            [TestMethod]
            public void when_category_is_provided_results_are_filtered()
            {
                var policy = new AuditPolicy() { Category = "Foo1", Setting = AuditingFlags.Success };
                _policyManager.Setup(n => n.GetAuditPolicy("Foo1")).Returns(policy);
                _cmdlet.Category = "Foo1";
                _cmdlet.ProcessInternal();
                _runtime.Verify(r => r.WriteObject(It.IsAny<AuditPolicy>()), Times.Exactly(1));
            }
        }
    }
}
