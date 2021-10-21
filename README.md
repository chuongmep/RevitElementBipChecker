# RevitElementBipChecker
![Revit API](https://img.shields.io/badge/Revit%20API-2021-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgray.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
<a href="https://twitter.com/intent/follow?screen_name=chuongmep">
        <img src="https://img.shields.io/twitter/follow/chuongmep?style=social&logo=twitter"
            alt="follow on Twitter"></a>
            
Project Update From  <a href="https://github.com/ottosson">ottosson</a> and <a href="https://github.com/jeremytammik">jeremytammik</a> with WPF Solution And Extend Funtion, fix Error for package nuget dependent old with .NET.

### Download 

<a href="https://github.com/chuongmep/RevitElementBipChecker/releases" target="_blank">RevitElementBipChecker</a> 

### Solution Check Full Parameter : 

- Search Parameter Of Element And Snoop All Value For Developer
- Support Parameter Type And Instance
- Export Parameter to Excel (*csv format*)
- Export Parameter to Json (*json format*)
- Interactive Live With Revit Project 
- Select Quickly Change Snoop Element
- Support Snoop LinkedElement
- Support Copy Parameter Infomation

### Select First To Snoop :

![](doc/_Image_f3e6247d-ff00-4624-8424-8498d3f69d7e.png)

### Main Form : 

![](doc/_Image_bb0e2d66-3c9d-4c3d-ad86-6a77987124be.png)

### Live Snoop

![](doc/Demo.gif)

### Data Export Excel 

![](doc/_Image_f1aac13a-394a-4b91-87d3-02ecf8bfd3ef.png)

### Data Export Json 

![](doc/_Image_8818052f-5f71-46f9-8d4c-314997031280.png)


### Copy Info By Right Click

![](doc/_Image_d275515e-7661-4d53-aed8-6624fec689d9.png)

### How To Use

#### Clone project from :

```
git clone https://github.com/chuongmep/RevitElementBipChecker.git
```
#### Restore nuget : 

1 [https://github.com/chuongmep/RevitAPI-Nuget](https://github.com/chuongmep/RevitAPI-Nuget)

2.[https://github.com/JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

### Get Datagrid library from:

<a href="https://github.com/macgile/DataGridFilter" target="_blank">DataGridFilter</a> 

#### Build solution: 

1. _RevitElementBipChecker.dll_
2. _BipChecker.addin_

### Reference

<a href="https://github.com/jeremytammik/BipChecker">jeremytammik</a> 

<a href="https://github.com/ottosson/BipChecker-WPF">ottosson</a> 


### Log Change

1.0.1 : First Release

1.0.2 : Fix Parameter Value String

1.0.3 :

- [x] Add Check Snoop Associated Global Parameter

1.0.4 :
- [x] Add Sort ClickHeader
- [x] Export correct with sort
- [x] Fix Export Character _"_ Unit Inch 