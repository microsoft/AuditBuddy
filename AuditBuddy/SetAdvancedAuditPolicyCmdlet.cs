using AuditBuddy.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace AuditBuddy
{
    /// <summary>
    /// Sets advanced audit settings
    /// </summary>
    /// <example>
    ///   <code>
    ///     Set-AdvancedAuditPolicy -CategoryName 'Object Access' -Setting Success
    ///   </code>
    ///   <remarks>
    ///     Enables Success auditing on the Object Access legacy audit policy
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     Set-AdvancedAuditPolicy -SubCategoryName 'File System' -Setting Success
    ///   </code>
    ///   <remarks>
    ///     Enables Success auditing on the File System subcategory of Object Access
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     Get-AdvancedAuditPolicy | Set-AdvancedAuditPolicy -Setting Both
    ///   </code>
    ///   <remarks>
    ///     Enable Success and Audit on all advanced audit policies
    ///   </remarks>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "AdvancedAuditPolicy", DefaultParameterSetName = "ByCategoryName")]
    public class SetAdvancedAuditPolicyCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">An advanced audit category name.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "ByCategoryName")]
        [ValidateNotNullOrEmpty()]
        [ValidateSet("System", "Logon/Logoff", "Object Access", "Privilege Use", "Detailed Tracking", "Policy Change", "Account Management", "DS Access", "Account Logon")]
        public string[] CategoryName { get; set; }

        /// <summary>
        /// <para type="description">An advanced audit subcategory name.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "BySubCategoryName")]
        [ValidateNotNullOrEmpty()]
        public string[] SubCategoryName { get; set; }

        /// <summary>
        /// <para type="description">An array of AdvancedAuditPolicy objects used for pipelining.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "ByAuditPolicies")]
        [ValidateNotNullOrEmpty()]
        public AdvancedAuditPolicy[] Policy { get; set; }

        /// <summary>
        /// <para type="description">The audit setting.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = false, ParameterSetName = "ByAuditPolicies")]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = false, ParameterSetName = "ByCategoryName")]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = false, ParameterSetName = "BySubCategoryName")]
        [ValidateNotNullOrEmpty()]
        [ValidateSet("None", "Success", "Failure", "Both", IgnoreCase = true)]
        public string Setting { get; set; }

        private IAdvancedAuditPolicyManager _advancedAuditPolicyManager;

        public SetAdvancedAuditPolicyCmdlet() : this(advancedAuditPolicyManager: new AdvancedAuditPolicyManager()) { }

        public SetAdvancedAuditPolicyCmdlet(IAdvancedAuditPolicyManager advancedAuditPolicyManager)
        {
            _advancedAuditPolicyManager = advancedAuditPolicyManager;
        }

        protected override void ProcessRecord()
        {
            ProcessInternal();
        }

        public void ProcessInternal()
        {
            var setting = new AuditingFlags();

            switch (Setting)
            {
                case "Both":
                    setting = AuditingFlags.Success | AuditingFlags.Failure;
                    break;
                default:
                    setting = (AuditingFlags)Enum.Parse(typeof(AuditingFlags), Setting);
                    break;
            }

            if (Policy != null)
            {
                var policyGuidList = _advancedAuditPolicyManager.GetAdvancedAuditSubCategories().Select(p => p.SubCategoryId).ToList();

                foreach (var policy in Policy)
                {
                    if (!policyGuidList.Contains(policy.SubCategoryId))
                    {
                        throw new InvalidOperationException($"{policy.SubCategoryId} is not a valid subcategory Guid");
                    }

                    policy.Setting = setting;
                }

                _advancedAuditPolicyManager.SetAdvancedAuditPolicy(Policy.ToList());
            }
            else
            {
                var allPolicies = _advancedAuditPolicyManager.GetAdvancedAuditPolicies();
                var targetPolicies = new List<AdvancedAuditPolicy>();

                if (CategoryName != null)
                {
                    var policyNameList = allPolicies.Select(p => p.Category).ToList();

                    foreach (var name in CategoryName)
                    {
                        if (!policyNameList.Contains(name))
                        {
                            throw new ArgumentException($"{name} is not a valid category name");
                        }
                    }

                    targetPolicies = allPolicies.Where(p => CategoryName.Contains(p.Category)).ToList();
                }
                else if (SubCategoryName != null)
                {
                    var policyNameList = allPolicies.Select(p => p.SubCategory).ToList();

                    foreach (var name in SubCategoryName)
                    {
                        if (!policyNameList.Contains(name))
                        {
                            throw new ArgumentException($"{name} is not a valid subcategory name");
                        }
                    }

                    targetPolicies = allPolicies.Where(p => SubCategoryName.Contains(p.SubCategory)).ToList();
                }
                else
                {
                    //should not be reachable
                    throw new ArgumentNullException("Invalid ParameterSet used");
                }

                foreach (var policy in targetPolicies)
                {
                    policy.Setting = setting;
                }

                _advancedAuditPolicyManager.SetAdvancedAuditPolicy(targetPolicies);
            }
        }
    }
}
