using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace AuditBuddy.Cmdlets.Tests
{
    public class describe_SetAuditPolicyCmdlet
    {
        private SetAuditPolicyCmdlet _cmdlet;
        private Mock<IAuditPolicyManager> _policyManager;
        private Mock<ICommandRuntime> _runtime;

        private List<AuditPolicy> policies = new List<AuditPolicy>
        {
            new AuditPolicy() { Category = "Foo1", Setting = AuditingFlags.Success },
            new AuditPolicy() { Category = "Foo2", Setting = AuditingFlags.Success },
            new AuditPolicy() { Category = "Foo3", Setting = AuditingFlags.Success },
            new AuditPolicy() { Category = "Foo4", Setting = AuditingFlags.Success }
        };

        private List<string> auditCategories = new List<string>
        {
            "Foo1",
            "Foo2",
            "Foo3",
            "Foo4"
        };

        [TestInitialize]
        public void before_each()
        {
            _policyManager = new Mock<IAuditPolicyManager>();
            _cmdlet = new SetAuditPolicyCmdlet(_policyManager.Object);
            _runtime = new Mock<ICommandRuntime>();
            _cmdlet.CommandRuntime = _runtime.Object;
            _policyManager.Setup(n => n.GetAuditPolicies()).Returns(policies);
            _policyManager.Setup(n => n.GetAuditCategories()).Returns(auditCategories);
        }

        [TestClass]
        public class when_SettingAuditPolicies : describe_SetAuditPolicyCmdlet
        {
            [TestMethod]
            public void when_setting_audit_policies_SetAuditPolicy_is_used()
            {
                _cmdlet.Policy = policies.ToArray();
                _cmdlet.Setting = "Success";
                _cmdlet.ProcessInternal();
                _policyManager.Verify(n => n.SetAuditPolicy(policies), Times.Once());
            }

            [TestMethod]
            public void when_setting_audit_policies_the_setting_is_updated()
            {
                AuditingFlags setting = AuditingFlags.Failure;
                _cmdlet.Setting = setting.ToString();
                _cmdlet.Policy = policies.ToArray();
                _cmdlet.ProcessInternal();

                List<AuditPolicy> updatedPolicies = policies;

                foreach (AuditPolicy policy in updatedPolicies)
                {
                    policy.Setting = setting;
                }

                _policyManager.Verify(n => n.SetAuditPolicy(updatedPolicies), Times.Once());
            }

            [TestMethod]
            public void when_invalid_category_is_provided_the_cmdlet_throws()
            {
                _cmdlet.Setting = "Success";
                _cmdlet.CategoryName = new string[] { "foo" };
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

                List<AuditPolicy> updatedPolicies = policies;

                foreach (AuditPolicy policy in updatedPolicies)
                {
                    policy.Setting = AuditingFlags.Success | AuditingFlags.Failure;
                }

                _policyManager.Verify(n => n.SetAuditPolicy(updatedPolicies), Times.Once());
            }

        }
    }
}
