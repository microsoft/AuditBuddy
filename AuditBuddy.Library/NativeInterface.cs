using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using static AuditBuddy.Library.Native;

namespace AuditBuddy.Library
{
    public interface INativeInterface
    {
        /// <summary>
        /// Gets a list of all legacy Audit Categories
        /// </summary>
        List<string> GetAuditCategories();
        /// <summary>
        /// Gets a List of Legacy Audit Policy settings
        /// </summary>
        List<AuditPolicy> GetAuditPolicy();
        /// <summary>
        /// Sets a list of Legacy Audit policy settings
        /// </summary>
        void SetAuditPolicy(IEnumerable<AuditPolicy> policies);
        /// <summary>
        /// Gets a List of Advanced Audit Policy categories
        /// </summary>
        List<AdvancedAuditCategory> GetAdvancedAuditCategories();
        /// <summary>
        /// Gets a List of Advanced Audit Policy subcategories including their parent categories
        /// </summary>
        List<AdvancedAuditSubCategory> GetAdvancedAuditSubCategories();
        /// <summary>
        /// Gets a specific advanced audit policy by sub category Guid
        /// </summary>
        AdvancedAuditPolicy GetAdvancedAuditPolicy(Guid subCategoryGuid);
        /// <summary>
        /// Gets a List of all Advanced Audit Policy settings
        /// </summary>
        List<AdvancedAuditPolicy> GetAdvancedAuditPolicies();
        /// <summary>
        /// Sets a specific advanced audit policy setting
        /// </summary>
        void SetAdvancedAuditPolicy(AdvancedAuditPolicy policy);
        /// <summary>
        /// Sets an array of AdvancedAuditPolicy objects
        /// </summary>
        void SetAdvancedAuditPolicy(AdvancedAuditPolicy[] policy);
    }

    internal class NativeInterface : INativeInterface
    {
        internal NativeInterface() { }

        private List<AdvancedAuditCategory> _advancedAuditCategories = new List<AdvancedAuditCategory>();
        private List<AdvancedAuditSubCategory> _advancedAuditSubCategories = new List<AdvancedAuditSubCategory>();
        private const string accessDeniedError = "This action requires Administrator permissions.";

        public List<string> GetAuditCategories()
        {
            return Enum.GetNames(typeof(AuditCategories)).ToList();
        }

        public List<AuditPolicy> GetAuditPolicy()
        {
            List<string> auditCategories = GetAuditCategories();

            IntPtr LsaPolicyHandle = GetLsaPolicyHandle((uint)LSA_AccessPolicy.POLICY_VIEW_AUDIT_INFORMATION);
            IntPtr outBuffer = IntPtr.Zero;

            try
            {
                var result = Native.LsaQueryInformationPolicy(LsaPolicyHandle, (uint)POLICY_INFORMATION_CLASS.PolicyAuditEventsInformation, out outBuffer);

                if (result != 0)
                {
                    throw new Exception(string.Format("Failed to query LSA policy information with '0x{0:X8}'", result));
                }

                // copy the raw values returned by LsaQueryPolicyInformation() to a local array;
                var auditEventsInfo = Marshal.PtrToStructure<POLICY_AUDIT_EVENTS_INFO>(outBuffer);
                var values = new int[auditEventsInfo.MaximumAuditEventCount];
                Marshal.Copy(auditEventsInfo.EventAuditingOptions, values, 0, auditEventsInfo.MaximumAuditEventCount);

                var policies = new List<AuditPolicy>();

                for (int i = 0; i < values.Length; i++)
                {

                    if ((AuditingFlags)values[i] == AuditingFlags.Unchanged)
                    {
                        values[i] = (int)AuditingFlags.None;
                    }

                    policies.Add(
                        new AuditPolicy
                        {
                            Category = auditCategories[i],
                            Setting = (AuditingFlags)values[i]
                        }
                    );
                }

                return policies;
            }
            finally
            {
                Native.LsaClose(LsaPolicyHandle);

                if (outBuffer != IntPtr.Zero)
                {
                    Native.LsaFreeMemory(outBuffer);
                }
            }
        }

        public void SetAuditPolicy(IEnumerable<AuditPolicy> policies)
        {
            uint accessMask = (uint)LSA_AccessPolicy.POLICY_VIEW_AUDIT_INFORMATION | (uint)LSA_AccessPolicy.POLICY_SET_AUDIT_REQUIREMENTS;
            IntPtr LsaPolicyHandle = GetLsaPolicyHandle(accessMask);
            IntPtr outBuffer = IntPtr.Zero;

            try
            {
                var result = Native.LsaQueryInformationPolicy(LsaPolicyHandle, (uint)POLICY_INFORMATION_CLASS.PolicyAuditEventsInformation, out outBuffer);

                if (result != 0)
                {
                    throw new Exception(string.Format("Failed to query LSA policy information with '0x{0:X8}'", result));
                }

                // copy the raw values returned by LsaQueryPolicyInformation() to a local array;
                var auditEventsInfo = Marshal.PtrToStructure<POLICY_AUDIT_EVENTS_INFO>(outBuffer);
                var values = new int[auditEventsInfo.MaximumAuditEventCount];
                Marshal.Copy(auditEventsInfo.EventAuditingOptions, values, 0, auditEventsInfo.MaximumAuditEventCount);

                // Set the auditing mode to true
                auditEventsInfo.AuditingMode = true;

                // Set the audit event options for the category in each policy
                foreach (AuditPolicy policy in policies)
                {
                    AuditCategories auditCategory = (AuditCategories)Enum.Parse(typeof(AuditCategories), policy.Category);

                    int category = (int)auditCategory;
                    int setting = (int)policy.Setting;

                    values[category] = setting;
                }

                Marshal.Copy(values, 0, auditEventsInfo.EventAuditingOptions, (int)auditEventsInfo.MaximumAuditEventCount);

                // Set the audit information for the Policy object
                result = LsaSetInformationPolicy(LsaPolicyHandle, POLICY_INFORMATION_CLASS.PolicyAuditEventsInformation, outBuffer);
                if (result != 0)
                {
                    throw new Exception("LsaSetInformationPolicy failed: " + result);
                }
            }
            finally
            {
                Native.LsaClose(LsaPolicyHandle);

                if (outBuffer != IntPtr.Zero)
                {
                    Native.LsaFreeMemory(outBuffer);
                }
            }
        }

        public List<AdvancedAuditCategory> GetAdvancedAuditCategories()
        {
            if (_advancedAuditCategories != null && _advancedAuditCategories.Count > 0)
            {
                return _advancedAuditCategories;
            }

            IntPtr buffer;
            int categoryCount;

            Native.AuditEnumerateCategories(out buffer, out categoryCount);
            var ptr = (Int64)buffer;
            var size = Marshal.SizeOf(typeof(Guid));
            var categories = new List<AdvancedAuditCategory>();

            for (int i = 0; i < categoryCount; i++)
            {
                var guid = (Guid)Marshal.PtrToStructure((IntPtr)ptr, typeof(Guid));
                categories.Add(new AdvancedAuditCategory { Category = LookupCategoryNameByGuid(guid), CategoryId = guid });
                ptr += size;
            }

            AuditFree(buffer);
            _advancedAuditCategories = categories;

            return categories;
        }

        public List<AdvancedAuditSubCategory> GetAdvancedAuditSubCategories()
        {
            if (_advancedAuditSubCategories != null && _advancedAuditSubCategories.Count > 0)
            {
                return _advancedAuditSubCategories;
            }

            var categories = GetAdvancedAuditCategories();
            List<AdvancedAuditSubCategory> subCategories = new List<AdvancedAuditSubCategory>();

            IntPtr buffer;
            uint subCategoryCount;
            var size = Marshal.SizeOf(typeof(Guid));

            foreach (AdvancedAuditCategory category in categories)
            {
                Native.AuditEnumerateSubCategories(category.CategoryId, false, out buffer, out subCategoryCount);
                var ptr = (Int64)buffer;

                for (int i = 0; i < subCategoryCount; i++)
                {
                    var subCategoryGuid = (Guid)Marshal.PtrToStructure((IntPtr)ptr, typeof(Guid));
                    var subCategoryName = LookupSubCategoryNameByGuid(subCategoryGuid);

                    var subcategory = new AdvancedAuditSubCategory { Category = category.Category, CategoryId = category.CategoryId, SubCategory = subCategoryName, SubCategoryId = subCategoryGuid };

                    subCategories.Add(subcategory);
                    ptr += size;
                }

                AuditFree(buffer);
            }

            _advancedAuditSubCategories = subCategories;

            return subCategories;
        }

        public AdvancedAuditPolicy GetAdvancedAuditPolicy(Guid subCategoryGuid)
        {
            var parentCategory = GetAdvancedAuditSubCategories().Where(s => s.SubCategoryId == subCategoryGuid).First();

            if (parentCategory == null)
                return null;

            var policyInfo = GetAdvancedAuditPolicyInfo(subCategoryGuid);
            var policy = new AdvancedAuditPolicy { Category = parentCategory.Category, CategoryId = parentCategory.CategoryId, SubCategory = parentCategory.SubCategory, SubCategoryId = subCategoryGuid, Setting = (AuditingFlags)policyInfo.AuditingInformation };

            return policy;
        }

        public List<AdvancedAuditPolicy> GetAdvancedAuditPolicies()
        {
            var subCategories = GetAdvancedAuditSubCategories();
            List<AdvancedAuditPolicy> advancedAuditPolicies = new List<AdvancedAuditPolicy>();

            foreach (AdvancedAuditSubCategory subCategory in subCategories)
            {
                var policyInfo = GetAdvancedAuditPolicyInfo(subCategory.SubCategoryId);
                var policy = new AdvancedAuditPolicy { Category = subCategory.Category, CategoryId = subCategory.CategoryId, SubCategory = subCategory.SubCategory, SubCategoryId = subCategory.SubCategoryId, Setting = (AuditingFlags)policyInfo.AuditingInformation };

                advancedAuditPolicies.Add(policy);
            }

            return advancedAuditPolicies;
        }

        public void SetAdvancedAuditPolicy(AdvancedAuditPolicy policy)
        {
            AUDIT_POLICY_INFORMATION[] policyArray = new AUDIT_POLICY_INFORMATION[1];
            policyArray[0].AuditCategoryGuid = policy.CategoryId;
            policyArray[0].AuditSubCategoryGuid = policy.SubCategoryId;
            policyArray[0].AuditingInformation = policy.Setting;

            SetAdvancedAuditPolicyInternal(policyArray);
        }

        public void SetAdvancedAuditPolicy(AdvancedAuditPolicy[] policy)
        {
            AUDIT_POLICY_INFORMATION[] policyArray = new AUDIT_POLICY_INFORMATION[policy.Length];
            int i = 0;

            foreach (var policyitem in policy)
            {
                policyArray[i].AuditCategoryGuid = policyitem.CategoryId;
                policyArray[i].AuditSubCategoryGuid = policyitem.SubCategoryId;
                policyArray[i].AuditingInformation = policyitem.Setting;
                i++;
            }

            SetAdvancedAuditPolicyInternal(policyArray);
        }

        private void SetAdvancedAuditPolicyInternal(AUDIT_POLICY_INFORMATION[] policies)
        {
            for (int i = 0; i < policies.Length; ++i)
            {
                if (policies[i].AuditingInformation == AuditingFlags.Unchanged)
                {
                    policies[i].AuditingInformation = AuditingFlags.None;
                }
            }

            AdjustPrivilege(SE_SECURITY_NAME);

            //currently there is a bug that will not allow for this code to remove success or failure if both are set
            //there is no problem adding a success or failure audit flags
            // to work around this stash the original policies, create a modified set with none, then 
            AUDIT_POLICY_INFORMATION[] policiesTemp = (AUDIT_POLICY_INFORMATION[])policies.Clone();

            for (int i = 0; i < policiesTemp.Length; ++i)
            {
                policiesTemp[i].AuditingInformation = AuditingFlags.None;
            }

            bool success = Native.AuditSetSystemPolicy(policiesTemp, (uint)policiesTemp.Length);

            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            //end bug workaround code

            success = Native.AuditSetSystemPolicy(policies, (uint)policies.Length);

            if (!success)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private string LookupCategoryNameByGuid(Guid categoryGuid)
        {
            Native.AuditLookupCategoryName(categoryGuid, out string name);
            return name;
        }

        private string LookupSubCategoryNameByGuid(Guid subCategoryGuid)
        {
            Native.AuditLookupSubCategoryName(subCategoryGuid, out string name);
            return name;
        }

        /// <summary>
        /// Adjust current process privilege so it could change the audit policy
        /// </summary>
        /// <param name="privilegeName"></param>
        /// <returns></returns>
        private static bool AdjustPrivilege(string privilegeName)
        {
            TOKEN_PRIVILEGES tokenPrivilege = new TOKEN_PRIVILEGES();

            IntPtr hToken = IntPtr.Zero;

            if (!Native.OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_QUERY | TOKEN_ADJUST_PRIVILEGES, ref hToken))
            {
                throw new ApplicationException("Cannot open process token when enabling privilege");
            }

            if (!Native.LookupPrivilegeValue(null, privilegeName, ref tokenPrivilege.Luid))
            {
                throw new ApplicationException("Failed at LookupPrivilegeValue for " + privilegeName);
            }

            tokenPrivilege.PrivilegeCount = 1;
            tokenPrivilege.Attributes = SE_PRIVILEGE_ENABLED;

            // no need to store the previous state
            if (!AdjustTokenPrivileges(hToken, 0, ref tokenPrivilege, 0, IntPtr.Zero, IntPtr.Zero))
            {
                throw new ApplicationException("Failed at AdjustTokenPrivileges");
            }

            if (hToken != IntPtr.Zero)
            {
                Native.CloseHandle(hToken);
            }

            return true;
        }

        private IntPtr GetLsaPolicyHandle(uint accessMask)
        {
            // Open local machine security policy
            IntPtr LsaPolicyHandle;
            LSA_OBJECT_ATTRIBUTES objectAttributes = new LSA_OBJECT_ATTRIBUTES();
            objectAttributes.Length = 0;
            objectAttributes.RootDirectory = IntPtr.Zero;
            objectAttributes.Attributes = 0;
            objectAttributes.SecurityDescriptor = IntPtr.Zero;
            objectAttributes.SecurityQualityOfService = IntPtr.Zero;

            var localsystem = new LSA_UNICODE_STRING();
            var result = Native.LsaOpenPolicy(ref localsystem, ref objectAttributes, accessMask, out LsaPolicyHandle);

            if (result != 0)
            {
                if (result == 0xC0000022)
                {
                    // Access denied, needs to run as admin
                    throw new UnauthorizedAccessException(accessDeniedError);
                }

                throw new Exception(string.Format("Failed to open LSA policy with return code '0x{0:X8}'", result));
            }

            return LsaPolicyHandle;
        }

        private AUDIT_POLICY_INFORMATION GetAdvancedAuditPolicyInfo(Guid subCategoryId)
        {
            IntPtr buffer;
            bool success = Native.AuditQuerySystemPolicy(subCategoryId, 1, out buffer);

            try
            {
                int win32Error = Marshal.GetLastWin32Error();

                if (win32Error == 1314)
                {
                    // Access denied, needs to run as admin
                    throw new UnauthorizedAccessException(accessDeniedError);
                }
                else if (!success)
                {
                    throw new Win32Exception(win32Error);
                }

                AUDIT_POLICY_INFORMATION policyInfo = (AUDIT_POLICY_INFORMATION)Marshal.PtrToStructure(buffer, typeof(AUDIT_POLICY_INFORMATION));

                if (policyInfo.AuditingInformation == AuditingFlags.Unchanged)
                {
                    policyInfo.AuditingInformation = AuditingFlags.None;
                }

                return (policyInfo);
            }
            finally
            {
                AuditFree(buffer);
            }
        }
    }
}