$PowershellScript = "MyPowershellScript.ps1"
$ResGrp = "my-resource-group"
$VMName = "my-vmname"
$ComputerName = "my-computername"

#Join domain using "RunPowerShellScript" Run Command
Invoke-AzVMRunCommand -ResourceGroupName $ResGrp -Name $VMName -CommandId 'RunPowerShellScript' -ScriptPath $PowershellScript -Verbose

# "MyPowershellScript.ps1" Contents

#Domain Cred
$Domain = 'my-domain'
$DomainUsername = "my-domainusername" 
$DomainPassword = 'my-domainpassword'
$DomainUser = $Domain + "\" + $DomainUsername
$DomainSecurePass = ConvertTo-SecureString $DomainPassword -AsPlainText -Force
$DomainCred = New-Object System.Management.Automation.PSCredential($DomainUser, $DomainSecurePass)

Add-Computer -ComputerName $env:COMPUTERNAME -DomainName $Domain -Credential $DomainCred -Restart -Verbose 
