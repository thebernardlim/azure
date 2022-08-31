###########################################################
## VARIABLES

# Snapshot Details
$ResGroup = "my-res-grp"
$SnapshotName = "my-snapshot"
$Location = "eastasia"

# VNet 
$VNetName = "my-vnet"
$SubnetName = "my-subnet"
$NSGName = "my-nsg"

# VM
$VMName = "my-vm"
$VMSize = "Standard_D2as_v4" 
$NewDiskName = "hk-" + $VMName + "-disk"
$NICName = $VMName + "NIC"

###########################################################

# Get resource group 
$NewRG = Get-AzResourceGroup -Name $ResGroup -Location $Location -ErrorAction SilentlyContinue

# Get snapshot
$Snapshot = Get-AzSnapshot -SnapshotName $SnapshotName

# Create the new disk config
$NewDiskConfig = New-AzDiskConfig -Location $Location -CreateOption "Copy" -SourceResourceId $Snapshot.Id

# Create the new OS disk
$NewDisk = New-AzDisk -DiskName $NewDiskName -Disk $NewDiskConfig -ResourceGroupName $ResGroup

#Get Network details
$virtualNetwork = Get-AzVirtualNetwork -Name $VNetName -ResourceGroupName $ResGroup
$subnet = Get-AzVirtualNetworkSubnetConfig -Name $SubnetName -VirtualNetwork $virtualNetwork
$nsg = Get-AzNetworkSecurityGroup -Name $NSGName -ResourceGroupName $ResGroup

#Create NIC
$NetworkInterface = New-AzNetworkInterface -Name $NICName -ResourceGroupName $ResGroup -Location $Location -SubnetId $subnet.Id -NetworkSecurityGroupId $nsg.Id

#VM Config (Note: I am creating a "Spot" Machine type here as my example)
$VirtualMachine = New-AzVMConfig -VMName $VMName -VMSize $VMSize -Priority "Spot" -MaxPrice 0.0400 -EvictionPolicy Deallocate 

# Add NIC to VM Config
$VirtualMachine = Add-AzVMNetworkInterface -Id $NetworkInterface.Id -VM $VirtualMachine

# Apply the OS disk properties
$VirtualMachine = Set-AzVMOSDisk -VM $VirtualMachine -ManagedDiskId $NewDisk.Id -StorageAccountType Standard_LRS -CreateOption "Attach" -Windows

# Create the virtual machine
$NewVM = New-AzVM -ResourceGroupName $ResGroup -Location $Location -VM $VirtualMachine

# Run 'Disable NLA' Command to allow RDP access
Invoke-AzVMRunCommand -ResourceGroupName $ResGroup -Name $VMName -CommandId 'DisableNLA' 

# Restart VM for changes to take effect
Restart-AzVM -ResourceGroupName $ResGroup -Name $VMName
