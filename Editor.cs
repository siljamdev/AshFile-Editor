using System;
using AshLib;

public enum Type{
	Text = 1,
	Number = 2,
	Bool = 3
}

public class Editor{
	static string? path;
	static AshFile af;
	static bool hasBeenSaved;
	
	public static void Main(string[] args){
		hasBeenSaved = true;
		af = new AshFile();
		if(args.Length > 0){
			path = args[0]; //Load path if arguments
			loadFromPath(); //Load it
		} else {
			newBlank();
		}
		
		string command;
		while(true){
			Console.WriteLine();
			Console.WriteLine("What do you want to do?: ");
			command = Console.ReadLine();
			command = command.ToLower();
			
			switch(command){
				case "help":
				case "h":
					printHelp();
					break;
				case "typelist":
				case "typehelp":
				case "tl":
				case "th":
					typeHelp();
					break;
				case "tutorial":
					tutorial();
					break;
				case "info":
					info();
					break;
				case "load":
					load();
					break;
				case "save":
					save();
					break;
				case "see":
				case "visualize":
					see();
					break;
				case "new":
				case "newfile":
					newBlank();
					break;
				case "reload":
					reload();
					break;
				case "set":
				case "setcamp":
					setCamp();
					break;
				case "get":
				case "getcamp":
					getCamp();
					break;
				case "delete":
				case "deletecamp":
					deleteCamp();
					break;
				case "rename":
				case "renamecamp":
					renameCamp();
					break;
				case "exit":
				case "x":
					exit();
					break;
				default:
					Console.WriteLine("Command not found. Write help for list of commands.");
					break;
			}
		}
	}
	
	public static void setCamp(){
		string name = askName();
		Type t = askType();
		object o = askValue(t);
		af.SetCamp(name, o);
		hasBeenSaved = false;
	}
	
	public static void getCamp(){
		string name = askName();
		if(!af.data.ContainsKey(name)){
			Console.WriteLine("There is no camp named \"" + name + "\".");
			return;
		}
		object o = af.GetCamp(name);
		Type t = getTypeFromObject(o);
		Console.WriteLine("Camp name: " + name + " | Type: " + getTypeName(t) + " | Value: " + o);
	}
	
	public static void deleteCamp(){
		string name = askName();
		if(!af.data.ContainsKey(name)){
			Console.WriteLine("There is no camp named \"" + name + "\".");
			return;
		}
		af.data.Remove(name);
		hasBeenSaved = false;
	}
	
	public static void renameCamp(){
		string oldName = askOldName();
		if(!af.data.ContainsKey(oldName)){
			Console.WriteLine("There is no camp named \"" + oldName + "\".");
			return;
		}
		string newName = askNewName();
		object o = af.GetCamp(oldName);
		af.data.Remove(oldName);
		af.SetCamp(newName, o);
		hasBeenSaved = false;
	}
	
	public static object askValue(Type t){
		while(true){
			Console.Write("Please enter the value of the camp: ");
			string answer = Console.ReadLine();
			
			switch(t){
				case Type.Text:
					return (string) answer;
				case Type.Number:
					ulong l = 0;
					if(!ulong.TryParse(answer, out l)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						continue;
					}
					return (ulong) l;
				case Type.Bool:
					answer = answer.ToLower();
					if(answer == "true" || answer == "t"){
						return (bool) true;
					} else if(answer == "false" || answer == "f"){
						return (bool) false;
					}
					Console.WriteLine("That boolean value is in an invalid format. Please enter true/false value.");
					Console.WriteLine();
					continue;
				default:
					Console.WriteLine();
					continue;
			}
		}
	}
	
	public static string askName(){
		Console.Write("Please enter the name of the camp: ");
		string answer = Console.ReadLine();
		return answer;
	}
	
	public static string askOldName(){
		Console.Write("Please enter the old name of the camp to rename: ");
		string answer = Console.ReadLine();
		return answer;
	}
	
	public static string askNewName(){
		Console.Write("Please enter the new updated name of the camp: ");
		string answer = Console.ReadLine();
		return answer;
	}
	
	public static Type askType(){
		while(true){
			Console.Write("Please enter the type (type help for a list of valid types): ");
			string answer = Console.ReadLine();
			if(answer == "help" || answer == "typehelp" || answer == "typelist"){
				typeHelp();
				Console.WriteLine();
				continue;
			}
			Type t = getTypeFromString(answer);
			if(t != (Type) 0){
				return t;
			}
			Console.WriteLine("That is not a valid type. Please enter a valid type.");
			Console.WriteLine();
		}
	}
	
	public static Type getTypeFromString(string s){
		s = s.ToLower();
		switch(s){
			case "1":
				return Type.Text;
			case "text":
				return Type.Text;
			case "string":
				return Type.Text;
			case "2":
				return Type.Number;
			case "number":
				return Type.Number;
			case "int":
				return Type.Number;
			case "long":
				return Type.Number;
			case "3":
				return Type.Bool;
			case "bool":
				return Type.Bool;
			case "boolean":
				return Type.Bool;
			default:
				return (Type) 0;
		}
	}
	
	public static Type getTypeFromObject(object o){
		if(o is string){
			return Type.Text;
		} else if(o is ulong){
			return Type.Number;
		} else if(o is bool){
			return Type.Bool;
		}
		return (Type) 0;
	}
	
	public static string getTypeName(Type t){
		switch(t){
			case Type.Text:
				return "Text";
			case Type.Number:
				return "Number";
			case Type.Bool:
				return "Boolean";
			default:
				return "Unknown Type";
		}
	}
	
	public static void reload(){
		if(path == null){
			Console.WriteLine("There is no path asociated with this file");
			return;
		}
		loadFromPath();
		Console.WriteLine("File reloaded succesfully.");
		hasBeenSaved = true;
	}
	
	public static void newBlank(){
		askToSave();
		path = null;
		af = new AshFile();
		hasBeenSaved = true;
	}
	
	public static void save(){
		if(path == null){
			askPath();
		}
		saveToPath();
		Console.WriteLine("File saved succesfully.");
		hasBeenSaved = true;
	}
	
	public static void load(){
		askToSave();
		askPath();
		loadFromPath();
		Console.WriteLine("File loaded succesfully.");
		hasBeenSaved = true;
	}
	
	public static void askPath(){
		Console.Write("Please enter the path: ");
		string answer = Console.ReadLine();
		path = answer;
	}
	
	public static void askToSave(){
		if(!hasBeenSaved){
			while(true){
				Console.Write("The current file has not been saved. Do you want to save? (Y/N): ");
				string answer = Console.ReadLine();
				answer = answer.ToLower();
				
				switch(answer){
					case "y":
					case "yes":
						save();
						return;
					case "n":
					case "no":
						return;
					default:
						Console.WriteLine("Invalid answer. Please answer Y/N.");
						Console.WriteLine();
						continue;
				}
			}
		}
	}
	
	public static void saveToPath(){
		af.Save(path);
	}
	
	public static void loadFromPath(){
		af.Load(path);
	}
	
	public static void see(){
		Console.WriteLine(af.AsString());
	}
	
	public static void printHelp(){
		Console.WriteLine("The list of commands is:");
		Console.WriteLine();
		Console.WriteLine("\"help\" for getting help.");
		Console.WriteLine("\"typelist\" for getting a list of valid types.");
		Console.WriteLine("\"tutorial\" for getting a tutorial on how AshFiles work.");
		Console.WriteLine("\"info\" for getting information about the Editor (version, author...)");
		Console.WriteLine("\"load\" for loading a file.");
		Console.WriteLine("\"save\" for saving the file.");
		Console.WriteLine("\"new\" for creating a new blank file.");
		Console.WriteLine("\"reload\" for reloading the file from where it was loaded/saved.");
		Console.WriteLine("\"see\" for seeing the whole file. The camp names will display first, and then the values");
		Console.WriteLine("\"set\" for setting the value of a camp.");
		Console.WriteLine("\"get\" for getting and seeing in detail the value of a camp.");
		Console.WriteLine("\"delete\" for deleting a camp.");
		Console.WriteLine("\"rename\" for renaming a camp.");
		Console.WriteLine("\"exit\" for exiting the application");
	}
	
	public static void typeHelp(){
		Console.WriteLine("The list of valid types is:");
		Console.WriteLine();
		Console.WriteLine("\"text\" for strings of text.");
		Console.WriteLine("\"number\" for an natural positive number.");
		Console.WriteLine("\"bool\" for true/false values.");
	}
	
	public static void tutorial(){
		Console.WriteLine();
		Console.WriteLine("An AshFile is a file format that has .ash extension, but it also represents a structure of data.");
		Console.WriteLine("This structure is based in multiple pockets of information. This pockets are called camps.");
		Console.WriteLine("Each camp has a name and a value. This value can be many things. (Type typelist for a full list)");
		Console.WriteLine("For example, you could have a camp named \"debt\", with its value being a number and being 12000.");
		Console.WriteLine("This structure can be used in programming projects wwith the AshLib nuget package.");
		Console.WriteLine("This Editor is just a way to make it possible to edit AshFiles without having to code.");
	}
	
	public static void info(){
		Console.WriteLine("The current version of AshFile Editor is v1.0");
		Console.WriteLine("This version is prepared to support v1 AshFiles");
		Console.WriteLine();
		Console.WriteLine("It was made by Dumbelfo for the AshProject");
	}
	
	public static void exit(){
		askToSave();
		Console.WriteLine("Bye bye honey pie!");
		Environment.Exit(0);
	}
}