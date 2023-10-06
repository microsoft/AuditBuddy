using System;
using System.Collections.Generic;
using System.Linq;

namespace AuditBuddy.Library
{
    public class AdvancedAuditCategory
    {
        public string Category { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class AdvancedAuditSubCategory
    {
        public string Category { get; set; }
        public Guid CategoryId { get; set; }
        public string SubCategory { get; set; }
        public Guid SubCategoryId { get; set; }
    }

    public class AdvancedAuditPolicy
    {
        public string Category { get; set; }
        public Guid CategoryId { get; set; }
        public string SubCategory { get; set; }
        public Guid SubCategoryId { get; set; }
        public AuditingFlags Setting { get; set; }
    }

    public interface IAdvancedAuditPolicyManager
    {
        /// <summary>
        /// Gets a List of Advanced Audit Policy categories
        /// </summary>
        List<AdvancedAuditCategory> GetAdvancedAuditCategories();
        /// <summary>
        /// Gets a List of Advanced Audit Policy subcategories including their parent categories
        /// </summary>
        List<AdvancedAuditSubCategory> GetAdvancedAuditSubCategories();
        /// <summary>
        /// Gets a specific advanced audit policy setting by sub category Guid
        /// </summary>
        AdvancedAuditPolicy GetAdvancedAuditPolicy(Guid policyGuid);
        /// <summary>
        /// Gets a specific advanced audit policy setting by sub category name
        /// </summary>
        AdvancedAuditPolicy GetAdvancedAuditPolicy(string policyName);
        /// <summary>
        /// Gets a list of all advanced audit policy settings
        /// </summary>
        List<AdvancedAuditPolicy> GetAdvancedAuditPolicies();
        /// <summary>
        /// Sets a specific advanced audit policy setting
        /// </summary>
        void SetAdvancedAuditPolicy(AdvancedAuditPolicy policy);
        /// <summary>
        /// Sets a List of AdvancedAuditPolicy objects
        /// </summary>
        void SetAdvancedAuditPolicy(IEnumerable<AdvancedAuditPolicy> policies);
    }

    public class AdvancedAuditPolicyManager : IAdvancedAuditPolicyManager
    {
        private INativeInterface _nativeInterface;

        public AdvancedAuditPolicyManager() : this(nativeInterface: new NativeInterface()) { }

        public AdvancedAuditPolicyManager(INativeInterface nativeInterface) 
        {
            _nativeInterface = nativeInterface ?? throw new ArgumentNullException(nameof(nativeInterface));
        }

        public List<AdvancedAuditCategory> GetAdvancedAuditCategories()
        {
            return _nativeInterface.GetAdvancedAuditCategories();
        }

        public List<AdvancedAuditSubCategory> GetAdvancedAuditSubCategories()
        {
            return _nativeInterface.GetAdvancedAuditSubCategories();
        }

        public List<AdvancedAuditPolicy> GetAdvancedAuditPolicies()
        {
            return _nativeInterface.GetAdvancedAuditPolicies();
        }

        public AdvancedAuditPolicy GetAdvancedAuditPolicy(Guid policyGuid)
        {
            return _nativeInterface.GetAdvancedAuditPolicy(policyGuid);
        }

        public AdvancedAuditPolicy GetAdvancedAuditPolicy(string policyName)
        {
            var subCategories = _nativeInterface.GetAdvancedAuditSubCategories();
            var category = subCategories.Where(s => s.SubCategory == policyName);

            if (!category.Any())
                return null;

            return _nativeInterface.GetAdvancedAuditPolicy(category.First().SubCategoryId);
        }

        public void SetAdvancedAuditPolicy(AdvancedAuditPolicy policy)
        {
            _nativeInterface.SetAdvancedAuditPolicy(policy);
        }

        public void SetAdvancedAuditPolicy(IEnumerable<AdvancedAuditPolicy> policies)
        {
            _nativeInterface.SetAdvancedAuditPolicy(policies.ToArray());
        }
    }
}