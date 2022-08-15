$resourceGroup = "[your-resource-grp]"
$storageAccountName = "[your-storage-acct]"
$containerName = "[your-container-name]"
$daysToKeep = [number-of-days]

$storageAccount = Get-AzStorageAccount -ResourceGroupName $resourceGroup -Name $storageAccountName 
$ctx = $storageAccount.Context 
$listOfBlobs = Get-AzStorageBlob -Container $containerName -Context $ctx
$blobs = Get-AzStorageBlob -Container $containerName -Context $ctx | Where-Object{$_.LastModified.DateTime -lt (Get-Date).AddDays($daysToKeep)} | Remove-AzStorageBlob
