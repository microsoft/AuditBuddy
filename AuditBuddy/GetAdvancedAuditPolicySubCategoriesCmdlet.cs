using AuditBuddy.Library;
using System.Linq;
using System.Management.Automation;

namespace AuditBuddy
{
    /// <summary>
    /// Get Advanced Audit policy categories and subcategories
    /// </summary>
    /// <example>
    ///   <code>
    ///     Get-AdvancedAuditPolicySubCategories
    ///   </code>
    ///   <remarks>
    ///     Gets the Advanced Audit categories and subcategories
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     Get-AdvancedAuditPolicySubCategories -Category 'Object Access'
    ///   </code>
    ///   <remarks>
    ///     Gets the Advanced Audit subcategories for Object Access
    ///   </remarks>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "AdvancedAuditPolicySubCategories")]
    [OutputType(typeof(AdvancedAuditSubCategory))]
    public class GetAdvancedAuditPolicySubCategoriesCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">An advanced audit category name.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = false, ParameterSetName = "Category")]
        [ValidateSet("System", "Logon/Logoff", "Object Access", "Privilege Use", "Detailed Tracking", "Policy Change", "Account Management", "DS Access", "Account Logon")]
        public string Category { get; set; }

        private IAdvancedAuditPolicyManager _advancedAuditPolicyManager;

        public GetAdvancedAuditPolicySubCategoriesCmdlet() : this(advancedAuditPolicyManager: new AdvancedAuditPolicyManager()) { }

        public GetAdvancedAuditPolicySubCategoriesCmdlet(IAdvancedAuditPolicyManager advancedAuditPolicyManager)
        {
            _advancedAuditPolicyManager = advancedAuditPolicyManager;
        }

        protected override void ProcessRecord()
        {
            ProcessInternal();
        }

        public void ProcessInternal()
        {
            var subcategories = _advancedAuditPolicyManager.GetAdvancedAuditSubCategories();

            if (Category != null)
            {
                subcategories.Where(s => s.Category == Category).ToList().ForEach(WriteObject);
            }
            else
            {
                subcategories.ForEach(WriteObject);
            }
        }
    }
}
