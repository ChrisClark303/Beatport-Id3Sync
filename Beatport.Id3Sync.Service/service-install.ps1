$exePath = "C:\Program Files\Beatport Sync Service"
$user = "desktop-16c6ral\fredd"
$acl = Get-Acl $exePath
$aclRuleArgs = $user, "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
$acl.SetAccessRule($accessRule)
$acl | Set-Acl $exePath

New-Service -Name Beatport.Id3Sync.Service -BinaryPathName "C:\Program Files\Beatport Sync Service\Beatport.Id3Sync.Service.exe --SourcePath D:\Beatport\New --OutputPath D:\Beatport\Processed" -Credential $user -Description "Automatically syncs Id3 Tags for Beatport Mp3s" -DisplayName "Beatport Id3Sync Service" -StartupType Automatic