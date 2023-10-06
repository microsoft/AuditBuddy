using AuditBuddy.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace AuditBuddy
{
    /// <summary>
    /// Sets legacy audit settings
    /// </summary>
    /// <example>
    ///   <code>
    ///     Set-AuditPolicy -CategoryName 'ObjectAccess' -Setting Success
    ///   </code>
    ///   <remarks>
    ///     Enables Success auditing on the Object Access legacy audit policy
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     'ObjectAccess','DetailedTracking' | Set-AuditPolicy -Setting Success
    ///   </code>
    ///   <remarks>
    ///     Enables Success auditing on ObjectAccess and DetailedTracking legacy audit policies
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     Get-AuditPolicy | Set-AuditPolicy -Setting Both
    ///   </code>
    ///   <remarks>
    ///     Enable Success and Audit on all legacy audit policies
    ///   </remarks>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "AuditPolicy", DefaultParameterSetName = "ByCategoryName")]
    public class SetAuditPolicyCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">A legacy audit category name.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "ByCategoryName")]
        [ValidateNotNullOrEmpty()]
        [ValidateSet("System", "Logon", "ObjectAccess", "PrivilegeUse", "DetailedTracking", "PolicyChange", "AccountManagement", "DirectoryServiceAccess", "AccountLogon")]
        public string[] CategoryName { get; set; }

        /// <summary>
        /// <para type="description">An array of legacy AuditPolicy objects used for pipelining.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "ByAuditPolicies")]
        [ValidateNotNullOrEmpty()]
        public AuditPolicy[] Policy { get; set; }

        /// <summary>
        /// <para type="description">The audit setting.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = false, ParameterSetName = "ByAuditPolicies")]
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = false, ParameterSetName = "ByCategoryName")]
        [ValidateNotNullOrEmpty()]
        [ValidateSet("None", "Success", "Failure", "Both", IgnoreCase = true)]
        public string Setting { get; set; }

        private IAuditPolicyManager _auditPolicyManager;

        public SetAuditPolicyCmdlet() : this(auditPolicyManager: new AuditPolicyManager()) { }

        public SetAuditPolicyCmdlet(IAuditPolicyManager auditPolicyManager)
        {
            _auditPolicyManager = auditPolicyManager;
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

            var categories = _auditPolicyManager.GetAuditCategories();

            if (Policy != null)
            {
                foreach (var policy in Policy)
                {
                    if (!categories.Contains(policy.Category))
                    {
                        throw new InvalidOperationException($"{policy.Category} is not a valid audit category name");
                    }

                    policy.Setting = setting;
                }

                _auditPolicyManager.SetAuditPolicy(Policy.ToList());
            }
            else
            {
                var targetPolicies = new List<AuditPolicy>();

                if (CategoryName != null)
                {
                    foreach (var name in CategoryName)
                    {
                        if (!categories.Contains(name))
                        {
                            throw new ArgumentException($"{name} is not a valid audit category name");
                        }
                    }

                    var allPolicies = _auditPolicyManager.GetAuditPolicies();
                    targetPolicies = allPolicies.Where(p => CategoryName.Contains(p.Category)).ToList();

                    foreach (var policy in targetPolicies)
                    {
                        policy.Setting = setting;
                    }

                    _auditPolicyManager.SetAuditPolicy(targetPolicies);
                }
                else
                {
                    //should not be reachable
                    throw new ArgumentNullException("Invalid ParameterSet used");
                }
            }
        }
    }
}
