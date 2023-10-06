using System;
using System.Runtime.InteropServices;

namespace AuditBuddy.Library
{
    internal static class Native
    {
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        internal const int TOKEN_QUERY = 0x00000008;
        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const string SE_SECURITY_NAME = "SeSecurityPrivilege";

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct AUDIT_POLICY_INFORMATION
        {
            public Guid AuditSubCategoryGuid;
            public AuditingFlags AuditingInformation;
            public Guid AuditCategoryGuid;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LSA_OBJECT_ATTRIBUTES
        {
            public int Length;
            public IntPtr RootDirectory;
            public LSA_UNICODE_STRING ObjectName;
            public UInt32 Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LSA_UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POLICY_AUDIT_EVENTS_INFO
        {
            public bool AuditingMode;
            public IntPtr EventAuditingOptions;
            public Int32 MaximumAuditEventCount;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct LUID
        {
            public UInt32 LowPart;
            public int HighPart;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct TOKEN_PRIVILEGES
        {
            public UInt32 PrivilegeCount;
            public LUID Luid;
            public UInt32 Attributes;
        }

        internal enum POLICY_INFORMATION_CLASS
        {
            PolicyAuditLogInformation = 1,
            PolicyAuditEventsInformation,
            PolicyPrimaryDomainInformation,
            PolicyPdAccountInformation,
            PolicyAccountDomainInformation,
            PolicyLsaServerRoleInformation,
            PolicyReplicaSourceInformation,
            PolicyDefaultQuotaInformation,
            PolicyModificationInformation,
            PolicyAuditFullSetInformation,
            PolicyAuditFullQueryInformation,
            PolicyDnsDomainInformation
        }

        //under no circumstances should this be reordered
        internal enum AuditCategories : int
        {
            System = 0,
            Logon = 1,
            ObjectAccess = 2,
            PrivilegeUse = 3,
            DetailedTracking = 4,
            PolicyChange = 5,
            AccountManagement = 6,
            DirectoryServiceAccess = 7,
            AccountLogon = 8
        }

        internal enum LSA_AccessPolicy : long
        {
            POLICY_VIEW_LOCAL_INFORMATION = 0x00000001L,
            POLICY_VIEW_AUDIT_INFORMATION = 0x00000002L,
            POLICY_GET_PRIVATE_INFORMATION = 0x00000004L,
            POLICY_TRUST_ADMIN = 0x00000008L,
            POLICY_CREATE_ACCOUNT = 0x00000010L,
            POLICY_CREATE_SECRET = 0x00000020L,
            POLICY_CREATE_PRIVILEGE = 0x00000040L,
            POLICY_SET_DEFAULT_QUOTA_LIMITS = 0x00000080L,
            POLICY_SET_AUDIT_REQUIREMENTS = 0x00000100L,
            POLICY_AUDIT_LOG_ADMIN = 0x00000200L,
            POLICY_SERVER_ADMIN = 0x00000400L,
            POLICY_LOOKUP_NAMES = 0x00000800L,
            POLICY_NOTIFICATION = 0x00001000L
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, int disableAllPrivileges, ref TOKEN_PRIVILEGES newState, UInt32 bufferLength, IntPtr previousState, IntPtr previousBufferLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AuditEnumerateCategories(out IntPtr ppAuditCategoriesArray,out int pCountReturned);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AuditFree(IntPtr auditPolicyArrayPtr);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AuditLookupCategoryName(Guid catGuid, out string catName);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AuditEnumerateSubCategories(Guid catGuid, bool all, out IntPtr subList, out uint count);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AuditLookupSubCategoryName(Guid subGuid, out String subName);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool AuditQuerySystemPolicy(Guid subGuid, uint count, out IntPtr policy);

        [DllImport("Advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool AuditSetSystemPolicy(AUDIT_POLICY_INFORMATION[] auditPolicyArray, UInt32 policyCount);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LookupPrivilegeValue(string systemName, string privilegeName, ref LUID pluid);

        [DllImport("advapi32.dll")]
        public static extern uint LsaSetInformationPolicy(LSA_HANDLE PolicyHandle, POLICY_INFORMATION_CLASS InformationClass, IntPtr Buffer);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint LsaClose(IntPtr ObjectHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint LsaFreeMemory(IntPtr Buffer);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        internal static extern uint LsaOpenPolicy(ref LSA_UNICODE_STRING SystemName, ref LSA_OBJECT_ATTRIBUTES ObjectAttributes, uint DesiredAccess, out IntPtr PolicyHandle);

        [DllImport("advapi32.dll")]
        internal static extern uint LsaQueryInformationPolicy(IntPtr PolicyHandle, uint InformationClass, out IntPtr Buffer);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenProcessToken(IntPtr processHandle, UInt32 desiredAccess, ref IntPtr tokenHandle);

        internal interface IHandle
        {
            /// <summary>Returns the value of the handle field.</summary>
            /// <returns>An IntPtr representing the value of the handle field.</returns>
            IntPtr DangerousGetHandle();
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LSA_HANDLE : IHandle
        {
            private readonly IntPtr handle;

            /// <summary>Initializes a new instance of the <see cref="LSA_HANDLE"/> struct.</summary>
            /// <param name="preexistingHandle">An <see cref="IntPtr"/> object that represents the pre-existing handle to use.</param>
            public LSA_HANDLE(IntPtr preexistingHandle) => handle = preexistingHandle;

            /// <summary>Returns an invalid handle by instantiating a <see cref="LSA_HANDLE"/> object with <see cref="IntPtr.Zero"/>.</summary>
            public static LSA_HANDLE NULL => new LSA_HANDLE(IntPtr.Zero);

            /// <summary>Gets a value indicating whether this instance is a null handle.</summary>
            public bool IsNull => handle == IntPtr.Zero;

            /// <summary>Performs an explicit conversion from <see cref="LSA_HANDLE"/> to <see cref="IntPtr"/>.</summary>
            /// <param name="h">The handle.</param>
            /// <returns>The result of the conversion.</returns>
            public static explicit operator IntPtr(LSA_HANDLE h) => h.handle;

            /// <summary>Performs an implicit conversion from <see cref="IntPtr"/> to <see cref="LSA_HANDLE"/>.</summary>
            /// <param name="h">The pointer to a handle.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator LSA_HANDLE(IntPtr h) => new LSA_HANDLE(h);

            /// <summary>Implements the operator !=.</summary>
            /// <param name="h1">The first handle.</param>
            /// <param name="h2">The second handle.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator !=(LSA_HANDLE h1, LSA_HANDLE h2) => !(h1 == h2);

            /// <summary>Implements the operator ==.</summary>
            /// <param name="h1">The first handle.</param>
            /// <param name="h2">The second handle.</param>
            /// <returns>The result of the operator.</returns>
            public static bool operator ==(LSA_HANDLE h1, LSA_HANDLE h2) => h1.Equals(h2);

            /// <inheritdoc/>
            public override bool Equals(object obj) => obj is LSA_HANDLE h ? handle == h.handle : false;

            /// <inheritdoc/>
            public override int GetHashCode() => handle.GetHashCode();

            /// <inheritdoc/>
            public IntPtr DangerousGetHandle() => handle;
        }
    }
}
