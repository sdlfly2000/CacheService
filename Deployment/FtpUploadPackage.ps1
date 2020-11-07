# Parameters
param(
	[string]$appdirectory = "../CacheService/bin/Debug/netcoreapp3.1",
	[string]$username = "sdlfly2000",
	[string]$password = "sdl@1215",
	[string]$url = "ftp://192.168.31.250/Projects/CacheServer",
	[string]$serverIP = "192.168.31.250"
)

# Requre Module
Install-Module Posh-SSH

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

# Execute Restart Systemd
Invoke-SSHCommand -SessionId 0 -Command "echo 'sdl@1215' | sudo -S systemctl restart CacheServer.service"  