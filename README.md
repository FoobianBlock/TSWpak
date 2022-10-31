# TSWpak

C# console application to ease the packing process of mods for Dovetail Games' Train Sim World® on Unreal Engine 4.26 (TSW 3 + TSW 2 post Rush Hour update)

## Requirements

- You have an install of Unreal Engine 4.26 (needed for `UnrealPak.exe`)
- You can use a computer that can execute exe-files

## How to use

### Arguments:
- Mod content path
  - Folder above the `TS2Prototype` Folder, this folder should otherwise be empty
- Pak filename/path *(optional, will default to `TSWpak-output.pak` in the current directory)*
- UnrealPak.exe path *(optional)*

### Example:
Input:  
```Shell
TSWpak.exe "G:\TSWMods\Mod\Cooked Files" "G:\TSWMods\Mod\TSWpakExample.pak"
```
  
***Pro-tip:***
*Add TSWpak as an enviorment variable for execution from any folder*

Output:  
```
It looks like there is no default UnrealPak.exe path specified, please enter one bellow! If confirmed working, it will be used as the default path if no path is given. You can use the command "reset" to reset the path if anything goes wrong.
UnrealPak.exe file path: [INPUT PATH TO UNREALPAK.EXE]

UnrealPak.exe output:
LogPakFile: Display: Using command line for crypto configuration
LogPakFile: Display: Loading response file C:\Users\[USERNAME]\AppData\Local\Temp\TSWpak_responseFile.txt
LogPakFile: Display: Added 1 entries to add to pak file.
LogPakFile: Display: Collecting files to add to pak file...
LogPakFile: Display: Collected 90 files in 0.01s.
LogPakFile: Display: Creating pak G:\TSWMods\Mod\TSWpakExample.pak.
LogDerivedDataCache: Display: Pak cache opened for reading ../../../Engine/DerivedDataCache/Compressed.ddp.
LogDerivedDataCache: Display: Performance to C:/Users/[USERNAME]/AppData/Local/UnrealEngine/Common/DerivedDataCache: Latency=0.00ms. RandomReadSpeed=999.00MBs, RandomWriteSpeed=999.00MBs. Assigned SpeedClass 'Local'
LogPakFile: Display: Added 90 files, 15850490 bytes total, time 1.27s.
LogPakFile: Display: PrimaryIndex size: 11843 bytes
LogPakFile: Display: PathHashIndex size: 4138 bytes
LogPakFile: Display: FullDirectoryIndex size: 6546 bytes
LogPakFile: Display: Compression summary: 9.35% of original size. Compressed Size 15730055 bytes, Original Size 168245225 bytes.
LogPakFile: Display: Used compression formats (in priority order) 'Zlib, '
LogPakFile: Display: Encryption - DISABLED
LogPakFile: Display: Unreal pak executed in 1.291050 seconds
```  
The path to the `UnrealPak.exe` will be remembered in the `TSWpak.exe.config`, saved in the program directory.

### Commands:
- `HELP`  - Displays help list, basically what you got in this readme anyway
- `INFO`  - Displays verison info
- `RESET` - Resets saved UnrealPak.exe path
