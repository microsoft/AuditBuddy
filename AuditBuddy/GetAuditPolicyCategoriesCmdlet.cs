using AuditBuddy.Library;
using System.Management.Automation;

namespace AuditBuddy
{
    /// <summary>
    /// Get legacy Audit policy categories
    /// </summary>
    /// <example>
    ///   <code>
    ///     Get-AuditPolicyCategories
    ///   </code>
    ///   <remarks>
    ///     Gets the legacy Audit categories
    ///   </remarks>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "AuditPolicyCategories")]
    [OutputType(typeof(string[]))]
    public class GetAuditPolicyCategoriesCmdlet : PSCmdlet
    {
        private IAuditPolicyManager _auditPolicyManager;

        public GetAuditPolicyCategoriesCmdlet() : this(auditPolicyManager: new AuditPolicyManager()) { }

        public GetAuditPolicyCategoriesCmdlet(IAuditPolicyManager auditPolicyManager)
        {
            _auditPolicyManager = auditPolicyManager;
        }

        protected override void ProcessRecord()
        {
            ProcessInternal();
        }

        public void ProcessInternal()
        {
            _auditPolicyManager.GetAuditCategories().ForEach(WriteObject);
        }
    }
}