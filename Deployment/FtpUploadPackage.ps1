# Parameters
param(
	[string]$appdirectory = "../CacheService/bin/Debug/netcoreapp3.1",
	[string]$username = "sdlfly2000",
	[string]$password = "sdl@1215",
	[string]$url = "ftp://192.168.31.250/Projects/CacheServer",
	[string]$serverIP = "192.168.31.250",
	[string]$root="root"
)

# Requre Module
Import-Module Posh-SSH

# Setup Credentail for Ssh
$secure = $password | ConvertTo-SecureString -AsPlainText -Force 
$cred = New-Object System.Management.Automation.PSCredential($username,$secure) 

# Upload files recursively 
$webclient = New-Object -TypeName System.Net.WebClient
$webclient.Credentials = New-Object System.Net.NetworkCredential($username,$password)
$files = Get-ChildItem -Path $appdirectory -Recurse | Where-Object{!($_.PSIsContainer)}
foreach ($file in $files)
{
    $uri = New-Object System.Uri("$url/$file")
    "Uploading to " + $uri.AbsoluteUri
    $webclient.UploadFile($uri, $file.FullName)
} 
$webclient.Dispose()

# Establishing Ssh connection
New-SSHSession -ComputerName $serverIP -Credential $cred -AcceptKey
Write-Host ""

# Execute Restart Systemd
Write-Host "Restart Service"
Write-Host "echo '$password' | sudo -S systemctl restart CacheServer.service"
Try 
{
	Invoke-SSHCommand -SessionId 0 -Command "echo '$password' | sudo -S systemctl restart CacheServer.service"
}
Catch
{
	Write-Host "echo $password | sudo -S systemctl restart CacheServer.service"
	exit 0
}