# AshFile Editor
<img src="res/icon.png" width="200"/>
Simple editor for AshFiles so no code is necessary to edit them

## What is an AshFile?
An AshFile is a file format that has **.ash extension**, but it also represents a structure of data.  
This structure is based in multiple pockets of information. These pockets are called **camps**.  
Each camp has a **name** and a **value**. This value can be many things. (Type typelist in-program for a full list)  
For example, you could have a camp named "debt", with its value being a number and being 12000.  
This structure can be used in programming projects with the [AshLib nuget package](https://github.com/siljamdev/AshLib).  
This Editor is just a way to make it possible to edit AshFiles without having to code.  

## How to use this editor
You can directly open .ash files with the executable, or open the executable first and then loading the file. Or you can create new files and then save them.  
The editor is console-based. You type commands to do things.  
To get a list of all the available command, you can type `help` in the editor.  

Once you have a file, there is a bunch of things you can do.  
You can see the whole data of the file with the command `see`. This is very useful for actually visualizing what is in the file.  
You can set the value of a camp, or create a new one with the command `set`.  
You can get the value of a camp in great detail with the command `get`.  
You can delete a camp with the command `delete`.  
You can change the name of a camp with the command `rename`.  

The Editor is in the version v3.X.X and supports v3 AshFiles.

## Installation
Download the portable windows executable or built it youself for linux/mac.

## NO_COLOR
This software follows the [NO_COLOR standard](https://no-color.org/).
