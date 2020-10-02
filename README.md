# RegistryProvider
Registry tool for local and remote machines.

## Description

Allows you to query/set/get Registry keys, permissions and values within the Windows Registry.

I don't like the standard .NET Registry class because I cannot easily access the Registry of a remote machine.

This uses the preinstalled StdRegProv WMI class to query a remote or local registry.


## Installation

Download the source code and build the class library.

Add the DLL which is located in the "..\Petersilie.ManagementTools.RegistryProvider\bin\Release" to your project.


## Usage 

**This project is free to use in any way for anyone**

### Classes

* RegistryBase
* Registry32
* Registry64
* RegistryNode


**RegistryBase:**
The RegistryBase class can be used to write your own Registry implementation.
The base class contains all the logic, the derived Registry32 and Registry64 classes simply set the RegView flag to either x64 or x86.

**Registry32 & Registry64:**
Use the Registry32 class to access the 32-bit Registry view and the Registry64 class to access the 64-bit Registry view.
The Registry64 class probably won't work on a 32-bit System.
I did not test that though!

**RegistryNode**
Implementation of a tree data structure which allows you to load all keys of a specified hive or only a defined subset of keys within that hive.


### Getting Registry keys

**Using the Registry class:**
``` csharp
// 32-bit Registry.
var reg32 = new Registry32();

/* Returns all subkeys on the first level 
** of the HKEY_CURRENT_USER hive. */
string[] k1 = reg32.GetSubKeys(RegHive.HKEY_CURRENT_USER,
                               string.Empty);

/* Returns all subkeys within the
** Software\Classes key which is found
** in the HKEY_CURRENT_USER hive. */
string[] k2 = reg32.GetSubKeys(RegHive.HKEY_CURRENT_USER, 
                               "Software\\Classes");
``` 

**Using the RegistryNode class:**
``` csharp
// Access HKEY_CURRENT_USER 32-bit Registry view.
var allCurrentUserNodes = new RegistryNode(
    RegView.x86, RegHive.HKEY_CURRENT_USER);

// Load ALL registry keys.
allCurrentUserNodes.BuildHive();


// Access HKEY_CURRENT_USER 32-bit Registry view.
var limitedCurrentUserNodes = new RegistryNode(
    RegView.x86, RegHive.HKEY_CURRENT_USER);

// Load 3 levels of subkeys.
limitedCurrentUserNodes.BuildHive(3);
```


### Permission check

``` csharp
var reg32 = new Registry32();
                        
string uninstallKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

bool canDelete;
// Check if that can be deleted by the current user.
int retVal = reg32.HasPermission(
    RegHive.HKEY_LOCAL_MACHINE,
    uninstallKey,
    RegAccessFlags.Delete, 
    out canDelete);
```


### Create new Registry key
``` csharp
var reg32 = new Registry32();

string newKey = "SOFTWARE\\MySoftwareKey";

/* Creates all subkeys specified in the path
** that do not exist. */
bool created = CreateKey(RegHive.HKEY_LOCAL_MACHINE, newKey);
```

### RegistryBase overview
![ClassDiagram1](https://user-images.githubusercontent.com/50140896/94928651-4d2fa000-04c4-11eb-9021-60fa1de71f47.png)



