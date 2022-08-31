$PowershellScript = "MyPowershellScript.ps1"
$ResGrp = "my-resource-group"
$VMName = "my-vmname"
$NewComputerName = "my-computername"

#Rename Computer Name using "RunPowerShellScript" Run Command
Invoke-AzVMRunCommand -ResourceGroupName $ResGrp -Name $VMName -CommandId 'RunPowerShellScript' -ScriptPath $MyPowershellScript -Parameter @{NewComputerName = $NewComputerName;} -Verbose

# "MyPowershellScript.ps1" Contents
param(
	$NewComputerName
)

#Local Cred
$LocalUser = 'my-localuser'
$LocalPassword = 'my-localpassword'
$LocalSecurePass = ConvertTo-SecureString $LocalPassword -AsPlainText -Force
$LocalCred = New-Object System.Management.Automation.PsCredential($LocalUser, $LocalSecurePass)

Rename-Computer -ComputerName $env:COMPUTERNAME -NewName $NewComputerName -LocalCredential $LocalCred - Restart -Verbose 
