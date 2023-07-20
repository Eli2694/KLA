# Define the LDAP path
$ldapPath = "LDAP://DC=KLA,DC=domain,DC=com"

# Define the user and password for connection
$user = "adminUser"
$password = ConvertTo-SecureString "adminPassword" -AsPlainText -Force

# Create the directory entry
$directoryEntry = New-Object System.DirectoryServices.DirectoryEntry($ldapPath, $user, $password)

# Create a searcher object
$searcher = New-Object System.DirectoryServices.DirectorySearcher($directoryEntry)

# Define the search filter
$searcher.Filter = "(objectClass=user)"

# Define properties to load
$searcher.PropertiesToLoad.Add("distinguishedName") > $null
$searcher.PropertiesToLoad.Add("samAccountName") > $null
$searcher.PropertiesToLoad.Add("userPrincipalName") > $null

# Find all objects
$results = $searcher.FindAll()

# Loop through the results
foreach ($result in $results) {
    # Get the properties
    $distinguishedName = $result.Properties["distinguishedName"]
    $samAccountName = $result.Properties["samAccountName"]
    $userPrincipalName = $result.Properties["userPrincipalName"]

    # Output the user
    Write-Host "Distinguished Name: $distinguishedName"
    Write-Host "SAM Account Name: $samAccountName"
    Write-Host "User Principal Name: $userPrincipalName"
    Write-Host "`n"
}

# Dispose of the searcher and results to free up resources
$searcher.Dispose()
$results.Dispose()
