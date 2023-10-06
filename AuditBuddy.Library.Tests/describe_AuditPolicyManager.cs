using AuditBuddy.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace AuditBuddyLibrary.Tests
{
    public class describe_AuditPolicyManager
    {
        private AuditPolicyManager _manager;
        private Mock<INativeInterface> _nativeInterface;

        [TestInitialize]
        public void before_each()
        {
            _nativeInterface = new Mock<INativeInterface>();
            _nativeInterface.Setup(n => n.GetAuditCategories()).Returns(new List<string>() { "Foo1", "Foo2"});
            List<AuditPolicy> auditPolicies = GetFakePolicies();
            _nativeInterface.Setup(n => n.GetAuditPolicy()).Returns(auditPolicies);
            _manager = new AuditPolicyManager(_nativeInterface.Object);
        }

        [TestClass]
        public class when_GettingAuditPolicy : describe_AuditPolicyManager
        {
            [TestMethod]
            public void when_getting_audit_policy_NativeInterface_is_called()
            {
                var policy = _manager.GetAuditPolicy("Foo1");
                _nativeInterface.Verify(n => n.GetAuditPolicy(), Times.Once());
            }

            [TestMethod]
            public void when_getting_audit_policy_the_correct_policy_is_returned()
            {
                var policy = _manager.GetAuditPolicy("Foo2");
                Assert.AreEqual("Foo2", policy.Category);
            }
        }


        [TestClass]
        public class when_GettingAuditPolicies : describe_AuditPolicyManager
        {
            [TestMethod]
            public void when_getting_audit_policies_NativeInterface_is_called()
            {
                var policy = _manager.GetAuditPolicies();
                _nativeInterface.Verify(n => n.GetAuditPolicy(), Times.Once());
            }
        }

        private List<AuditPolicy> GetFakePolicies()
        {
            return new List<AuditPolicy>
            {
                new AuditPolicy {  Category= "Foo1", Setting = AuditingFlags.Success},
                new AuditPolicy {  Category= "Foo2", Setting = AuditingFlags.Success},
                new AuditPolicy {  Category= "Foo3", Setting = AuditingFlags.Success},
            };
        }
    }
}
