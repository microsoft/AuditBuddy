using System;
using System.Collections.Generic;
using System.Linq;

namespace AuditBuddy.Library
{
    public class AuditPolicy
    {
        public string Category { get; set; }
        public AuditingFlags Setting { get; set; }
    }

    public interface IAuditPolicyManager
    {
        /// <summary>
        /// Gets a list of all legacy Audit Categories
        /// </summary>
        List<string> GetAuditCategories();
        /// <summary>
        /// Gets a List of Legacy Audit Policy settings
        /// </summary>
        List<AuditPolicy> GetAuditPolicies();
        /// <summary>
        /// Gets a specific audit policy
        /// </summary>
        AuditPolicy GetAuditPolicy(string category);
        /// <summary>
        /// Sets a specific Legacy Audit policy
        /// </summary>
        void SetAuditPolicy(AuditPolicy policy);
        /// <summary>
        /// Sets a list of Legacy Audit policy settings
        /// </summary>
        void SetAuditPolicy(IEnumerable<AuditPolicy> policies);
    }

    public class AuditPolicyManager : IAuditPolicyManager
    {
        private INativeInterface _nativeInterface;

        public AuditPolicyManager() : this(nativeInterface: new NativeInterface()) { }

        public AuditPolicyManager(INativeInterface nativeInterface)
        {
            _nativeInterface = nativeInterface ?? throw new ArgumentNullException(nameof(nativeInterface));
        }

        public List<string> GetAuditCategories()
        {
            return _nativeInterface.GetAuditCategories();
        }

        public AuditPolicy GetAuditPolicy (string category)
        {
            var categories = _nativeInterface.GetAuditCategories();
            var auditCategory = categories.Where(p => p.Equals(category));

            if (!auditCategory.Any())
                return null;

            return _nativeInterface.GetAuditPolicy().Where(p => p.Category == category).First();
        }

        public List<AuditPolicy> GetAuditPolicies()
        {
            return _nativeInterface.GetAuditPolicy();
        }

        public void SetAuditPolicy(AuditPolicy policy)
        {
            List<AuditPolicy> policies = new List<AuditPolicy> { policy };
            _nativeInterface.SetAuditPolicy(policies);
        }

        public void SetAuditPolicy(IEnumerable<AuditPolicy> policies)
        {
            _nativeInterface.SetAuditPolicy(policies);
        }
    }
}