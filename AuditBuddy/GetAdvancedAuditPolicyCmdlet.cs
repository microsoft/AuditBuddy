using AuditBuddy.Library;
using System.Linq;
using System.Management.Automation;

namespace AuditBuddy
{
    /// <summary>
    /// Get Advanced Audit policy settings
    /// </summary>
    /// <example>
    ///   <code>
    ///     Get-AdvancedAuditPolicy
    ///   </code>
    ///   <remarks>
    ///     Gets all of the Advanced Audit policy settings
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     Get-AdvancedAuditPolicy -Category 'Object Access'
    ///   </code>
    ///   <remarks>
    ///     Gets only the Object Access related audit policy settings
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     Get-AdvancedAuditPolicySubCategories -Category 'Object Access' | Get-AdvancedAuditPolicy
    ///   </code>
    ///   <remarks>
    ///     Gets only the Object Access related audit policy settings
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     Get-AdvancedAuditPolicy -SubCategoryName 'File System'
    ///   </code>
    ///   <remarks>
    ///     Gets only the File System SubCategory audit policy settings
    ///   </remarks>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "AdvancedAuditPolicy", DefaultParameterSetName = "ByCategoryName")]
    [OutputType(typeof(AdvancedAuditPolicy))]
    public class GetAdvancedAuditPolicyCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">An advanced audit category name.</para>
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = false, ParameterSetName = "ByCategoryName")]
        [ValidateNotNullOrEmpty()]
        [ValidateSet("System", "Logon/Logoff", "Object Access", "Privilege Use", "Detailed Tracking", "Policy Change", "Account Management", "DS Access", "Account Logon")]
        public string CategoryName { get; set; }

        /// <summary>
        /// <para type="description">An advanced audit subcategory name.</para>
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = false, ParameterSetName = "BySubCategoryName")]
        [ValidateNotNullOrEmpty()]
        public string SubCategoryName { get; set; }

        /// <summary>
        /// <para type="description">An array of AdvancedAuditSubCategory objects used in a pipeline.</para>
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "BySubCategory")]
        [ValidateNotNullOrEmpty()]
        public AdvancedAuditSubCategory[] SubCategory { get; set; }

        private IAdvancedAuditPolicyManager _advancedAuditPolicyManager;

        public GetAdvancedAuditPolicyCmdlet() : this(advancedAuditPolicyManager: new AdvancedAuditPolicyManager()) { }

        public GetAdvancedAuditPolicyCmdlet(IAdvancedAuditPolicyManager advancedAuditPolicyManager)
        {
            _advancedAuditPolicyManager = advancedAuditPolicyManager;
        }

        protected override void ProcessRecord()
        {
            ProcessInternal();
        }

        public void ProcessInternal()
        {
            if (SubCategoryName != null)
            {
                WriteObject(_advancedAuditPolicyManager.GetAdvancedAuditPolicy(SubCategoryName));
            }
            else
            {
                var policies = _advancedAuditPolicyManager.GetAdvancedAuditPolicies();

                if (CategoryName != null)
                {
                    policies.Where(p => p.Category == CategoryName).ToList().ForEach(WriteObject);
                }
                else if (SubCategory != null)
                {
                    var subCategoryGuidList = SubCategory.Select(s => s.SubCategoryId).ToList();
                    policies.Where(p => subCategoryGuidList.Contains(p.SubCategoryId)).ToList().ForEach(WriteObject);
                }
                else
                {
                    policies.ForEach(WriteObject);
                }
            }
        }
    }
}
