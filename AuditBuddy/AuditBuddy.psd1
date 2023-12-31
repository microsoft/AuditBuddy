@{
RootModule = 'AuditBuddy.dll'
ModuleVersion = '1.1'
GUID = '22e5f58e-fa38-4903-ac5b-8ef417f0f11b'
Author = 'Microsoft Corp'
CompanyName = 'Microsoft Corp'
Copyright = 'Copyright (c) Microsoft Corporation. All rights reserved.'
Description = 'PowerShell Cmdlets to manage Windows audit setting (instead of using Auditpol)'
PowerShellVersion = '3.0'
DotNetFrameworkVersion = '4.7.2'
FunctionsToExport = @()
CmdletsToExport = @('Get-AdvancedAuditPolicy','Get-AdvancedAuditPolicyCategories','Get-AdvancedAuditPolicySubCategories','Get-AuditPolicy','Get-AuditPolicyCategories','Set-AdvancedAuditPolicy','Set-AuditPolicy')
AliasesToExport = @()
ModuleList = @("AuditBuddy")
# HelpInfo URI of this module
HelpInfoURI = 'https://github.com/microsoft/AuditBuddy'
PrivateData = @{
    PSData = @{
        Tags = 'Audit','AuditPol','AuditPol.exe','WindowsAudit'
        LicenseUri = 'https://github.com/microsoft/AuditBuddy/blob/main/LICENSE'
        ProjectUri = 'https://github.com/microsoft/AuditBuddy'
        }
    } 
}