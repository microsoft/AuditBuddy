using System;

namespace AuditBuddy.Library
{
    [Flags]
    public enum AuditingFlags : uint
    {
        Unchanged = 0x00000000,
        Success = 0x00000001,
        Failure = 0x00000002,
        None = 0x00000004,
    }
}