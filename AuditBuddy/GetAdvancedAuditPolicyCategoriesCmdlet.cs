using AuditBuddy.Library;
using System.Management.Automation;

namespace AuditBuddy
{
    /// <summary>
    /// Get Advanced Audit policy categories
    /// </summary>
    /// <example>
    ///   <code>
    ///     Get-AdvancedAuditPolicyCategories
    ///   </code>
    ///   <remarks>
    ///     Gets the Advanced Audit categories
    ///   </remarks>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "AdvancedAuditPolicyCategories")]
    [OutputType(typeof(AdvancedAuditCategory))]
    public class GetAdvancedAuditPolicyCategoriesCmdlet : PSCmdlet
    {
        private IAdvancedAuditPolicyManager _advancedAuditPolicyManager;

        public GetAdvancedAuditPolicyCategoriesCmdlet() : this(advancedAuditPolicyManager: new AdvancedAuditPolicyManager()) { }

        public GetAdvancedAuditPolicyCategoriesCmdlet(IAdvancedAuditPolicyManager advancedAuditPolicyManager)
        {
            _advancedAuditPolicyManager = advancedAuditPolicyManager;
        }

        protected override void ProcessRecord()
        {
            ProcessInternal();
        }

        public void ProcessInternal()
        {
            _advancedAuditPolicyManager.GetAdvancedAuditCategories().ForEach(WriteObject);
        }
    }
}
