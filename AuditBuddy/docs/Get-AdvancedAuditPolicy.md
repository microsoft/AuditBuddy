---
external help file: AuditBuddy.Cmdlets.dll-Help.xml
Module Name: AuditBuddy.Cmdlets
online version:
schema: 2.0.0
---

# Get-AdvancedAuditPolicy

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### ByCategoryName (Default)
```
Get-AdvancedAuditPolicy [-CategoryName <String>] [<CommonParameters>]
```

### BySubCategoryName
```
Get-AdvancedAuditPolicy [-SubCategoryName <String>] [<CommonParameters>]
```

### BySubCategory
```
Get-AdvancedAuditPolicy [-SubCategory <AdvancedAuditSubCategory[]>] [<CommonParameters>]
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
Type: String
Parameter Sets: ByCategoryName
Aliases:
Accepted values: System, Logon/Logoff, Object Access, Privilege Use, Detailed Tracking, Policy Change, Account Management, DS Access, Account Logon

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SubCategory
{{ Fill SubCategory Description }}

```yaml
Type: AdvancedAuditSubCategory[]
Parameter Sets: BySubCategory
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -SubCategoryName
{{ Fill SubCategoryName Description }}

```yaml
Type: String
Parameter Sets: BySubCategoryName
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### AuditBuddy.Library.AdvancedAuditSubCategory[]

## OUTPUTS

### AuditBuddy.Library.AdvancedAuditPolicy

## NOTES

## RELATED LINKS
