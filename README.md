# AuditBuddy

**AudityBuddy** is a PowerShell Cmdlet used to manage Windows Audit settings.

**AuditBuddy** also includes a .NET library used by the cmdlet that can be used in any .NET applications.

[![PR Build](https://github.com/microsoft/AuditBuddy/actions/workflows/PullRequest.yml/badge.svg)](https://github.com/microsoft/AuditBuddy/actions/workflows/PullRequest.yml)

# Installation
The module can directly be installed from the PSGallery using

```
Install-Module -Name AuditBuddy
```

# Documentation
- [Advanced Audit Policy Settings Documentation](https://learn.microsoft.com/en-us/windows/security/threat-protection/auditing/advanced-security-audit-policy-settings)
- [Legacy Audit Policy Settings Documentation](https://learn.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/audit-policy)
- [Audit Policy Recommendations](https://learn.microsoft.com/en-us/windows-server/identity/ad-ds/plan/security-best-practices/audit-policy-recommendations)

# Community and Contact
Please feel free to file issues through GitHub for bugs and feature requests and we'll respond to them as quickly as we're able.

#  Examples

## Get-AuditPolicy

### Syntax
    Get-AuditPolicy [[-Category] {System | Logon | Object Access | Privilege Use | Detailed Tracking | Policy Change |
    Account Management | Directory Service Access | Account Logon}]  [<CommonParameters>]

## Description
Get legacy Audit policy settings.

## Examples

### Example 1
Gets the legacy Audit policies

    Get-AuditPolicy
### Example 2
Gets the Audit policy for Object Access

    Get-AuditPolicy -Category 'Object Access'

## Get-AuditPolicyCategories
### Syntax
    Get-AuditPolicyCategories  [<CommonParameters>]

## Description
Get legacy Audit policy categories.

## Examples

### Example 1
Gets the legacy Audit policies

    Get-AuditPolicyCategories

## Set-AuditPolicy

### Syntax
    Set-AuditPolicy [-CategoryName] {System | Logon | ObjectAccess | PrivilegeUse | DetailedTracking | PolicyChange |
    AccountManagement | DirectoryServiceAccess | AccountLogon} [-Setting] {None | Success | Failure | Both}
    [<CommonParameters>]

    Set-AuditPolicy [-Policy] <AuditPolicy[]> [-Setting] {None | Success | Failure | Both}  [<CommonParameters>]

## Description
Set legacy Audit policy settings.

## Examples

### Example 1
Enables Success auditing on the Object Access legacy audit policy

    Set-AuditPolicy -CategoryName 'ObjectAccess' -Setting Success

### Example 2
Enables Success auditing on ObjectAccess and DetailedTracking legacy audit policies

    'ObjectAccess','DetailedTracking' | Set-AuditPolicy -Setting Success

### Example 3
Enable Success and Audit on all legacy audit policies

    Get-AuditPolicy | Set-AuditPolicy -Setting Both

## Get-AdvancedAuditPolicy

### Syntax
    Get-AdvancedAuditPolicy [-CategoryName {System | Logon/Logoff | Object Access | Privilege Use | Detailed Tracking
    | Policy Change | Account Management | DS Access | Account Logon}]  [<CommonParameters>]

    Get-AdvancedAuditPolicy [-SubCategoryName <string>]  [<CommonParameters>]

    Get-AdvancedAuditPolicy [-SubCategory <AdvancedAuditSubCategory[]>]  [<CommonParameters>]

## Description
Get Advanced Audit policy settings

## Examples

### Example 1
Gets all of the Advanced Audit policy settings

    Get-AdvancedAuditPolicy

### Example 2
Gets only the Object Access related audit policy settings

    Get-AdvancedAuditPolicy -Category 'Object Access'

### Example 3
Gets only the Object Access related audit policy settings

    Get-AdvancedAuditPolicySubCategories -Category 'Object Access' | Get-AdvancedAuditPolicy

### Example 4
Gets only the File System SubCategory audit policy settings

    Get-AdvancedAuditPolicy -SubCategoryName 'File System'

## Get-AdvancedAuditPolicyCategories
### Syntax
    Get-AdvancedAuditPolicyCategories  [<CommonParameters>]

## Description
Get Advanced Audit policy categories.

## Examples

### Example 1
Gets the Advanced Audit categories

    Get-AdvancedAuditPolicyCategories

## Get-AdvancedAuditPolicySubCategories
### Syntax
    Get-AdvancedAuditPolicySubCategories  [<CommonParameters>]

## Description
Gets the Advanced Audit categories and subcategories.

## Examples

### Example 1
Gets the Advanced Audit categories and subcategories

    Get-AdvancedAuditPolicySubCategories
### Example 2
Gets the Advanced Audit subcategories for Object Access

    Get-AdvancedAuditPolicySubCategories -Category 'Object Access'

## Set-AdvancedAuditPolicy

### Syntax
    Set-AdvancedAuditPolicy [-CategoryName] {System | Logon/Logoff | Object Access | Privilege Use | Detailed Tracking
    | Policy Change | Account Management | DS Access | Account Logon} [-Setting] {None | Success | Failure | Both}
    [<CommonParameters>]

    Set-AdvancedAuditPolicy [-SubCategoryName] <string[]> [-Setting] {None | Success | Failure | Both}
    [<CommonParameters>]

    Set-AdvancedAuditPolicy [-Policy] <AdvancedAuditPolicy[]> [-Setting] {None | Success | Failure | Both}
    [<CommonParameters>]

## Description
Get Advanced Audit policy settings

## Examples

### Example 1
Enables Success auditing on the Object Access legacy audit policy

    Set-AdvancedAuditPolicy -CategoryName 'Object Access' -Setting Success

### Example 2
Enables Success auditing on the File System subcategory of Object Access

    Set-AdvancedAuditPolicy -SubCategoryName 'File System' -Setting Success

### Example 3
Enable Success and Audit on all advanced audit policies

    Get-AdvancedAuditPolicy | Set-AdvancedAuditPolicy -Setting Both

# Trademark Notice
**Trademarks** This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow Microsoft’s Trademark & Brand Guidelines. Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party’s policies.