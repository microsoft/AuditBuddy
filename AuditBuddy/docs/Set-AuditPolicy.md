---
external help file: AuditBuddy.Cmdlets.dll-Help.xml
Module Name: AuditBuddy.Cmdlets
online version:
schema: 2.0.0
---

# Set-AuditPolicy

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### ByCategoryName (Default)
```
Set-AuditPolicy [-CategoryName] <String[]> [-Setting] <String> [<CommonParameters>]
```

### ByAuditPolicies
```
Set-AuditPolicy [-Policy] <AuditPolicy[]> [-Setting] <String> [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -CategoryName
{{ Fill CategoryName Description }}

```yaml
Type: String[]
Parameter Sets: ByCategoryName
Aliases:
Accepted values: System, Logon, ObjectAccess, PrivilegeUse, DetailedTracking, PolicyChange, AccountManagement, DirectoryServiceAccess, AccountLogon

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Policy
{{ Fill Policy Description }}

```yaml
Type: AuditPolicy[]
Parameter Sets: ByAuditPolicies
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Setting
{{ Fill Setting Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: None, Success, Failure, Both

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

### AuditBuddy.Library.AuditPolicy[]

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
