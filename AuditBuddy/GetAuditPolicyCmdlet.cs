using AuditBuddy.Library;
using System.Management.Automation;

namespace AuditBuddy
{
    /// <summary>
    /// <para type="synopsis">Get legacy Audit policy settings.</para>
    /// </summary>
    /// <example>
    ///   <code>
    ///     Get-AuditPolicy
    ///   </code>
    ///   <remarks>
    ///     Gets the legacy Audit policies
    ///   </remarks>
    /// </example>
    /// <example>
    ///   <code>
    ///     Get-AuditPolicy -Category 'Object Access'
    ///   </code>
    ///   <remarks>
    ///     Gets the Audit policy for Object Access
    ///   </remarks>
    /// </example>
    [Cmdlet (VerbsCommon.Get, "AuditPolicy")]
    [OutputType(typeof(AuditPolicy))]
    public class GetAuditPolicyCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">A legacy audit policy category name.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = false, ParameterSetName = "Category")]
        [ValidateSet("System", "Logon", "Object Access", "Privilege Use", "Detailed Tracking", "Policy Change", "Account Management", "Directory Service Access", "Account Logon")]
        public string Category { get; set; }

        private IAuditPolicyManager _auditPolicyManager;

        public GetAuditPolicyCmdlet() : this(auditPolicyManager: new AuditPolicyManager()) { }

        public GetAuditPolicyCmdlet(IAuditPolicyManager auditPolicyManager)
        {
            _auditPolicyManager = auditPolicyManager;
        }

        protected override void ProcessRecord()
        {
            ProcessInternal();
        }

        public void ProcessInternal()
        {
            if (Category == null)
            {
                _auditPolicyManager.GetAuditPolicies().ForEach(WriteObject);
            }
            else
            {
                WriteObject(_auditPolicyManager.GetAuditPolicy(Category));
            }
        }
    }
}