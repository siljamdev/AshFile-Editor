using System;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using AshLib.Formatting;
using AshLib.AshFiles;
using AshLib.Folders;
using AshLib.Dates;
using AshLib;

public class Editor{	
	static string? path;
	static AshFile af;
	static bool hasBeenSaved;
	static bool isFile;
	
	static Color3 red = new Color3("E7484B");
	static Color3 paleRed = new Color3("E57272");
	static Color3 purple = new Color3("9F60C1");
	static Color3 error = new Color3("7D60E5");
	
	static Dependencies dep;
	
	public static void Main(string[] args){
		string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		dep = new Dependencies(appDataPath + "/ashproject/ashfileeditor", true, null, null);
		
		initializeConfig();
		
		hasBeenSaved = true;
		af = new AshFile();
		if(args.Length > 0){
			path = removeQuotes(args[0]); //Load path if arguments
			if(!loadFromPath()){
				newBlank();
			} else {
				writeLine(Path.GetFileName(path) + " loaded succesfully.", purple);
				see();
			}
		} else {
			newBlank();
		}
		
		string command;
		while(true){
			try{
			writeLine();
			writeLine("What do you want to do?: ", red);
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
				case "open":
				case "load":
					load();
					break;
				case "save":
					save();
					break;
				case "import":
					import();
					break;
				case "export":
					export();
					break;
				case "config":
					loadConfig();
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
				case "tostring":
				case "tostr":
					toString();
					break;
				case "tojson":
					toJson();
					break;
				case "parse":
					parse();
					break;
				case "exit":
				case "x":
					exit();
					break;
				default:
					writeLine("Command not found. Write help for list of commands.", error);
					break;
			}
			} catch(Exception e){
				writeLine("An error occured! Here is more info:", error);
				writeLine("Message: " + e.Message);
				writeLine("Stack Trace: " + e.StackTrace);
			}
		}
	}
	
	public static void initializeConfig(){
		AshFileModel afm = new AshFileModel(new ModelInstance(ModelInstanceOperation.Type, "visualizeAsTree", false),
		new ModelInstance(ModelInstanceOperation.Type, "useColors", true),
		new ModelInstance(ModelInstanceOperation.Type, "successColor", purple),
		new ModelInstance(ModelInstanceOperation.Type, "questionColor", red),
		new ModelInstance(ModelInstanceOperation.Type, "questionColor2", paleRed),
		new ModelInstance(ModelInstanceOperation.Type, "errorColor", error));
		
		afm.deleteNotMentioned = true;
		
		dep.config *= afm;
		
		dep.config.Save();
		
		purple = dep.config.GetCamp<Color3>("successColor");
		red = dep.config.GetCamp<Color3>("questionColor");
		paleRed = dep.config.GetCamp<Color3>("questionColor2");
		error = dep.config.GetCamp<Color3>("errorColor");
	}
	
	public static void setCamp(){
		string name = askName();
		AshFileType t = askType(out bool array);
		if(isFile){
			string p = askFilePath();
			if(!File.Exists(p)){
				writeLine("That file does not exist.", error);
				isFile = false;
				return;
			}
			af.SetCamp(name, File.ReadAllText(p));
			hasBeenSaved = false;
			isFile = false;
			return;
		}
		object o;
		if(array){
			o = askValueArray(t);
		}else{
			o = askValue(t);
		}
		af.SetCamp(name, o);
		hasBeenSaved = false;
	}
	
	public static void getCamp(){
		string name = askName();
		object o;
		if(!af.CanGetCamp(name, out o)){
			writeLine("There is no camp named '" + name + "'.", error);
			return;
		}
		if(o.GetType().IsArray){
			AshFileType t = getFileTypeFromType(o.GetType().GetElementType());
			StringBuilder sb = new StringBuilder();
			bool f = true;
			foreach(var j in (Array) o){
				if(!f){
					sb.Append(",");
				}
				f = false;
				
				sb.Append(j.ToString());
				writeLine("Camp name: " + name + " | Type: " + getTypeName(t) + " Array | Values: [" + sb.ToString() + "]");
			}
		}else{
			AshFileType t = getFileTypeFromType(o.GetType());
			writeLine("Camp name: " + name + " | Type: " + getTypeName(t) + " | Value: " + o.ToString());
		}
		
		//writeLine("Camp name: " + name + " | Type: " + getTypeName(t) + " | Value: " + o.ToString());
	}
	
	public static void deleteCamp(){
		string name = askName();
		if(!af.CanDeleteCamp(name)){
			writeLine("There is no camp named \"" + name + "\".");
			return;
		}
		hasBeenSaved = false;
	}
	
	public static void renameCamp(){
		string oldName = askOldName();
		if(!af.ExistsCamp(oldName)){
			writeLine("There is no camp named \"" + oldName + "\".", error);
			return;
		}
		string newName = askNewName();
		af.RenameCamp(oldName, newName);
		hasBeenSaved = false;
	}
	
	public static Type getTypeFromEnum(AshFileType fileType){
		switch (fileType){
			case AshFileType.String: return typeof(string);
			case AshFileType.Byte: return typeof(byte);
			case AshFileType.Ushort: return typeof(ushort);
			case AshFileType.Uint: return typeof(uint);
			case AshFileType.Ulong: return typeof(ulong);
			case AshFileType.Sbyte: return typeof(sbyte);
			case AshFileType.Short: return typeof(short);
			case AshFileType.Int: return typeof(int);
			case AshFileType.Long: return typeof(long);
			case AshFileType.Color3: return typeof(Color3); // Example for Color3 (need a proper type here)
			case AshFileType.Float: return typeof(float);
			case AshFileType.Double: return typeof(double);
			case AshFileType.Vec2: return typeof(Vec2); // Example for Vec2
			case AshFileType.Vec3: return typeof(Vec3); // Example for Vec3
			case AshFileType.Vec4: return typeof(Vec4); // Example for Vec4
			case AshFileType.Bool: return typeof(bool);
			case AshFileType.Date: return typeof(Date);
			default: return typeof(object); // Default case if no matching type
		}
	}
	
	public static AshFileType getFileTypeFromType(Type type){
		if (type == typeof(string)) return AshFileType.String;
		if (type == typeof(byte)) return AshFileType.Byte;
		if (type == typeof(ushort)) return AshFileType.Ushort;
		if (type == typeof(uint)) return AshFileType.Uint;
		if (type == typeof(ulong)) return AshFileType.Ulong;
		if (type == typeof(sbyte)) return AshFileType.Sbyte;
		if (type == typeof(short)) return AshFileType.Short;
		if (type == typeof(int)) return AshFileType.Int;
		if (type == typeof(long)) return AshFileType.Long;
		if (type == typeof(Color3)) return AshFileType.Color3;
		if (type == typeof(float)) return AshFileType.Float;
		if (type == typeof(double)) return AshFileType.Double;
		if (type == typeof(Vec2)) return AshFileType.Vec2;
		if (type == typeof(Vec3)) return AshFileType.Vec3;
		if (type == typeof(Vec4)) return AshFileType.Vec4;
		if (type == typeof(bool)) return AshFileType.Bool;
		if (type == typeof(Date)) return AshFileType.Date;
	
		return AshFileType.Default; // Default case if no matching type
	}
	
	public static object askValueArray(AshFileType t){
		Type listType = typeof(List<>).MakeGenericType(getTypeFromEnum(t));
		dynamic list = Activator.CreateInstance(listType);
		
		writeLine("Enter 'end' to stop adding elements to the array", purple);
		
		int i = 0;
		
		bool flag = true;
		
		while(flag){
			write("Please enter the " + i + "th element of the " + getTypeName(t) + "array: ", paleRed);
			string answer = Console.ReadLine();
			
			if(answer.ToUpper() == "END"){
				break;
			}
			
			object o = null;
			
			while(!parseValue(answer, t, out o)){
				write("Please enter the " + i + "th element of the " + getTypeName(t) + "array: ", paleRed);
				answer = Console.ReadLine();
				
				if(answer.ToUpper() == "END"){
					flag = false;
					break;
				}
			}
			
			if(flag){
				dynamic d = o;
				list.Add(d);
				i++;
			}
		}
		
		return list.ToArray();
	}
	
	public static object askValue(AshFileType t){
			write("Please enter the value of the " + getTypeName(t) + " camp: ", paleRed);
			string answer = Console.ReadLine();
			
			object o = null;
			
			while(!parseValue(answer, t, out o)){
				write("Please enter the value of the " + getTypeName(t) + " camp: ", paleRed);
				answer = Console.ReadLine();
			}
			
			return o;
	}
	
	public static bool parseValue(string answer, AshFileType t, out object o){
		switch(t){
			case AshFileType.String:
				o = answer;
				return true;
			case AshFileType.Byte:
				byte d;
				if(!byte.TryParse(answer, out d)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = d;
				return true;
			case AshFileType.Ushort:
				ushort e;
				if(!ushort.TryParse(answer, out e)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = e;
				return true;
			case AshFileType.Uint:
				uint f;
				if(!uint.TryParse(answer, out f)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = f;
				return true;
			case AshFileType.Ulong:
				ulong g;
				if(!ulong.TryParse(answer, out g)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = g;
				return true;
			case AshFileType.Sbyte:
				sbyte h;
				if(!sbyte.TryParse(answer, out h)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = h;
				return true;
			case AshFileType.Short:
				short j;
				if(!short.TryParse(answer, out j)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = j;
				return true;
			case AshFileType.Int:
				int k;
				if(!int.TryParse(answer, out k)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = k;
				return true;
			case AshFileType.Long:
				long l;
				if(!long.TryParse(answer, out l)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = l;
				return true;
			case AshFileType.Color3:
				if(Color3.TryParse(answer, out Color3 ccc)){
					o = ccc;
					return true;
				}
				string[] a = answer.Split(",");
				if(a.Length != 3){
					writeLine("There is not the correct amount of arguments. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				byte r1, r2, r3;
				if(!byte.TryParse(a[0], out r1)){
					writeLine("The first number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!byte.TryParse(a[1], out r2)){
					writeLine("The second number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!byte.TryParse(a[2], out r3)){
					writeLine("The third number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = new Color3(r1, r2, r3);
				return true;
			case AshFileType.Float:
				float m;
				if(!float.TryParse(answer, out m)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = m;
				return true;
			case AshFileType.Double:
				double n;
				if(!double.TryParse(answer, out n)){
					writeLine("That number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = n;
				return true;
			case AshFileType.Vec2:
				a = answer.Split(",");
				if(a.Length != 2){
					writeLine("There is not the correct amount of arguments. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				float s1, s2, s3, s4;
				if(!float.TryParse(a[0], out s1)){
					writeLine("The first number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!float.TryParse(a[1], out s2)){
					writeLine("The second number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = new Vec2(s1, s2);
				return true;
			case AshFileType.Vec3:
				a = answer.Split(",");
				if(a.Length != 3){
					writeLine("There is not the correct amount of arguments. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!float.TryParse(a[0], out s1)){
					writeLine("The first number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!float.TryParse(a[1], out s2)){
					writeLine("The second number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!float.TryParse(a[2], out s3)){
					writeLine("The second number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = new Vec3(s1, s2, s3);
				return true;
			case AshFileType.Vec4:
				a = answer.Split(",");
				if(a.Length != 4){
					writeLine("There is not the correct amount of arguments. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!float.TryParse(a[0], out s1)){
					writeLine("The first number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!float.TryParse(a[1], out s2)){
					writeLine("The second number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!float.TryParse(a[2], out s3)){
					writeLine("The third number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				if(!float.TryParse(a[3], out s4)){
					writeLine("The forth number is in an invalid format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = new Vec4(s1, s2, s3, s4);
				return true;
			case AshFileType.Bool:
				answer = answer.ToLower();
				if(answer == "true" || answer == "t"){
					o = true;
					return true;
				} else if(answer == "false" || answer == "f"){
					o = false;
					return true;
				}
				writeLine("That boolean value is in an invalid format. Please enter true/false value.", error);
				writeLine();
				o = null;
				return false;
			case AshFileType.Date:
				string format = "HH:mm:ss dd/MM/yyyy";
				DateTime dt;
				if(!DateTime.TryParseExact(answer, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)){
					writeLine("The date is not in a correct format. Please enter again.", error);
					writeLine();
					o = null;
					return false;
				}
				o = (Date) dt;
				return true;
			default:
				writeLine();
				o = null;
				return false;
		}
	}
	
	public static string askName(){
		write("Please enter the name of the camp: ", red);
		string answer = Console.ReadLine();
		return answer;
	}
	
	public static string askOldName(){
		write("Please enter the old name of the camp to rename: ", red);
		string answer = Console.ReadLine();
		return answer;
	}
	
	public static string askNewName(){
		write("Please enter the new updated name of the camp: ", red);
		string answer = Console.ReadLine();
		return answer;
	}
	
	public static AshFileType askType(out bool array){
		while(true){
			write("Please enter the type (type help for a list of valid types): ", paleRed);
			string answer = Console.ReadLine();
			if(answer == "help" || answer == "typehelp" || answer == "typelist"){
				typeHelp();
				writeLine();
				continue;
			}
			if(answer == "file"){
				isFile = true;
				array = false;
				return AshFileType.String;
			}
			AshFileType t = getTypeFromString(answer, out array);
			if(t != AshFileType.Default){
				return t;
			}
			writeLine("That is not a valid type. Please enter a valid type.", error);
			writeLine();
		}
	}
	
	public static AshFileType getTypeFromString(string s, out bool array){
		s = s.ToLower().Trim();
		if(s.EndsWith("[]")){
			array = true;
			s = s.Substring(0, s.Length - 2);
		}else{
			array = false;
		}
		
		if(byte.TryParse(s, out byte t) && t > 0 && t < 18){
			return (AshFileType) t;
		}
		
		switch(s){
			case "text":
			case "s":
			case "string":
				return AshFileType.String;
			case "byte":
				return AshFileType.Byte;
			case "ushort":
			case "unsigned short":
				return AshFileType.Ushort;
			case "uint":
			case "unsigned int":
				return AshFileType.Uint;
			case "ulong":
			case "unsigned long":
				return AshFileType.Ulong;
			case "sbyte":
			case "signed byte":
				return AshFileType.Sbyte;
			case "short":
				return AshFileType.Short;
			case "number":
			case "num":
			case "n":
			case "int":
				return AshFileType.Int;
			case "long":
				return AshFileType.Long;
			case "color":
			case "color3":
			case "c":
			case "colorrgb":
			case "rgb":
				return AshFileType.Color3;
			case "float":
			case "f":
				return AshFileType.Float;
			case "double":
				return AshFileType.Double;
			case "vec2":
			case "v2":
				return AshFileType.Vec2;
			case "vec3":
			case "v3":
				return AshFileType.Vec3;
			case "vec4":
			case "v4":
				return AshFileType.Vec4;
			case "bool":
			case "boolean":
			case "b":
				return AshFileType.Bool;
			case "date":
			case "time":
				return AshFileType.Date;
			default:
				return AshFileType.Default;
		}
	}
	
	public static string getTypeName(AshFileType t){
		switch (t){
			case AshFileType.String:
				return "Text";
			case AshFileType.Byte:
				return "Byte";
			case AshFileType.Ushort:
				return "Unsigned 2-byte number";
			case AshFileType.Uint:
				return "Unsigned 4-byte number";
			case AshFileType.Ulong:
				return "Unsigned 8-byte number";
			case AshFileType.Sbyte:
				return "Signed byte";
			case AshFileType.Short:
				return "Signed 2-byte number";
			case AshFileType.Int:
				return "Integer number";
			case AshFileType.Long:
				return "Signed 8-byte number";
			case AshFileType.Color3:
				return "Color";
			case AshFileType.Float:
				return "Floating point number";
			case AshFileType.Double:
				return "Double precision number";
			case AshFileType.Vec2:
				return "2D Vector";
			case AshFileType.Vec3:
				return "3D Vector";
			case AshFileType.Vec4:
				return "4D Vector";
			case AshFileType.Bool:
				return "Boolean";
			case AshFileType.Date:
				return "Date";
			case AshFileType.Default:
			default:
				return "Unknown Type: " + (int)t;
		}
	}

	
	public static void reload(){
		if(path == null){
			writeLine("There is no path asociated with this file", error);
			return;
		}
		if(!loadFromPath()){
			return;
		}
		writeLine(Path.GetFileName(path) + " reloaded succesfully.", purple);
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
		writeLine("File saved succesfully.", purple);
		hasBeenSaved = true;
	}
	
	public static void export(){
		writeLine("Here is the text: ", purple);
		writeLine(af.ToString());
	}
	
	public static void load(){
		askToSave();
		askPath();
		if(!loadFromPath()){
			return;
		}
		writeLine(Path.GetFileName(path) + " loaded succesfully.", purple);
		hasBeenSaved = true;
	}
	
	public static void import(){
		askToSave();
		write("Please enter the text: ", paleRed);
		string answer = Console.ReadLine();
		if(AshFile.TryParse(answer, out AshFile a)){
			af = a;
			writeLine("Text imported succesfully.", purple);
			hasBeenSaved = true;
		}else{
			writeLine("Failed to parse the text.", error);
		}
	}
	
	public static string askFilePath(){
		write("Please enter the path to the file: ", paleRed);
		string answer = Console.ReadLine();
		return removeQuotes(answer);
	}
	
	public static void askPath(){
		write("Please enter the path: ", paleRed);
		string answer = Console.ReadLine();
		path = removeQuotes(answer);
	}
	
	public static void askToSave(){
		if(!hasBeenSaved){
			while(true){
				write("The current file has not been saved. Do you want to save? (Y/N): ", paleRed);
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
						writeLine("Invalid answer. Please answer Y/N.", error);
						writeLine();
						continue;
				}
			}
		}
	}
	
	public static void saveToPath(){
		af.Save(path);
		
		handleAshFileErrors();
	}
	
	public static bool loadFromPath(){
		try{
			if(!File.Exists(path)){
				path = null;
				writeLine("Could not load from path becaue the file doesnt exist.", error);
				return false;
			}
			af.Load(path);
			
			handleAshFileErrors();
			
			return true;
		} catch(Exception e){
			writeLine("An error occured trying to open the file. Here is more info:", error);
			writeLine("Message: " + e.Message);
			writeLine("Stack Trace: " + e.StackTrace);
			
			return false;
		}
	}
	
	
	public static void see(){
		if(dep.config.GetCamp<bool>("visualizeAsTree")){
			if(!dep.config.GetCamp<bool>("useColors")){
				writeLine(af.VisualizeAsTree());
			}else{
				writeLine(af.VisualizeAsFormattedTree(CharFormat.ResetAll, new CharFormat(purple), CharFormat.ResetAll));
			}
		}else{
			if(!dep.config.GetCamp<bool>("useColors")){
				writeLine(af.Visualize());
			}else{
				writeLine(af.VisualizeFormatted(new CharFormat(purple), CharFormat.ResetAll));
			}
		}
	}
	
	public static void toString(){
		writeLine(af.ToStringCompact());
	}
	
	public static void toJson(){
		writeLine(af.ToJson());
	}
	
	public static void parse(){
		askToSave();
		write("Please enter the AshFile string format: ", paleRed);
		string answer = Console.ReadLine();
		
		if(!AshFile.TryParse(answer, out AshFile a)){
			handleAshFileParseErrors();
			return;
		}
		
		af = a;
		path = null;
		
		writeLine("Parsed succesfully.", purple);
		hasBeenSaved = false;
	}
	
	public static string removeQuotes(string p){
		if(p.Length < 1){
			return p;
		}
		char[] c = p.ToCharArray();
		if(c[0] == '\"' && c[c.Length - 1] == '\"'){
			if(c.Length < 2){
				return "";
			}
			return removeQuotes(p.Substring(1, p.Length - 2));
		}
		return p;
	}
	
	public static void handleAshFileErrors(){
		if(AshFile.GetErrorCount() > 0){
			writeLine("Errors occured while reading or saving the file, here is more info:", error);
			writeLine("Error count: " + AshFile.GetErrorCount());
			writeLine("Error log: " + AshFile.GetErrorLog());
		}
		AshFile.EmptyErrors();
	}
	
	public static void handleAshFileParseErrors(){
		if(AshFile.GetErrorCount() > 0){
			writeLine("Errors occured while parsing, here is more info:", error);
			writeLine("Error count: " + AshFile.GetErrorCount());
			writeLine("Error log: " + AshFile.GetErrorLog());
		}
		AshFile.EmptyErrors();
	}
	
	public static void printHelp(){
		writeLine("The list of commands is:", purple);
		writeLine();
		write("'");
		write("help", paleRed);
		writeLine("' for getting help.");
		
		write("'");
		write("typelist", paleRed);
		writeLine("' for getting a list of valid types.");
		
		write("'");
		write("tutorial", paleRed);
		writeLine("' for getting a tutorial on how AshFiles work.");
		
		write("'");
		write("info", paleRed);
		writeLine("' for getting information about the Editor (version, author...)");
		
		write("'");
		write("load", paleRed);
		writeLine("' for loading a file.");
		
		write("'");
		write("save", paleRed);
		writeLine("' for saving the file.");
		
		write("'");
		write("import", paleRed);
		writeLine("' for importing the file as text.");
		
		write("'");
		write("export", paleRed);
		writeLine("' for exporting the file as text.");
		
		write("'");
		write("new", paleRed);
		writeLine("' for creating a new blank file.");
		
		write("'");
		write("reload", paleRed);
		writeLine("' for reloading the file from where it was loaded/saved.");
		
		write("'");
		write("see", paleRed);
		writeLine("' for seeing the whole file. The camp names will display first, and then the values");
		
		write("'");
		write("set", paleRed);
		writeLine("' for setting the value of a camp.");
		
		write("'");
		write("get", paleRed);
		writeLine("' for getting and seeing in detail the value of a camp.");
		
		write("'");
		write("delete", paleRed);
		writeLine("' for deleting a camp.");
		
		write("'");
		write("rename", paleRed);
		writeLine("' for renaming a camp.");
		
		write("'");
		write("tojson", paleRed);
		writeLine("' for getting the AshFile as a JSON.");
		
		write("'");
		write("tostr", paleRed);
		writeLine("' for getting the AshFile string format representation of the file.");
		
		write("'");
		write("parse", paleRed);
		writeLine("' for getting an AshFile from its string format representation.");
		
		write("'");
		write("config", paleRed);
		writeLine("' for opening the application configuration as an AshFile.");
		
		write("'");
		write("exit", paleRed);
		writeLine("' for exiting the application.");
	}
	
	public static void typeHelp(){
		writeLine("The list of valid types is:", purple);
		writeLine();
		
		write("'");
		write("text", paleRed);
		writeLine("' for strings of text.");
		
		write("'");
		write("number", paleRed);
		writeLine("' for natural positive numbers.");
		
		write("'");
		write("bool", paleRed);
		writeLine("' for true/false values.");
		
		write("'");
		write("float", paleRed);
		writeLine("' for 32-bit floating-point numbers (numbers with decimals).");
		
		write("'");
		write("color", paleRed);
		writeLine("' for RGB color values.");
		
		write("'");
		write("file", paleRed);
		writeLine("' for copying the contents of a file(as text) based on its path.");
		
		write("'");
		write("date", paleRed);
		writeLine("' for date/time values.");
		
		write("'");
		write("vec2", paleRed);
		writeLine("' for 2D vectors.");
		
		write("'");
		write("vec3", paleRed);
		writeLine("' for 3D vectors.");
		
		write("'");
		write("vec4", paleRed);
		writeLine("' for 4D vectors.");
		writeLine();
		
		write("'");
		write("byte", paleRed);
		writeLine("' for 8-bit signed integers.");
		
		write("'");
		write("sbyte", paleRed);
		writeLine("' for 8-bit signed integers.");
		
		write("'");
		write("short", paleRed);
		writeLine("' for 16-bit signed integers.");
		
		write("'");
		write("ushort", paleRed);
		writeLine("' for 16-bit unsigned integers.");
		
		write("'");
		write("int", paleRed);
		writeLine("' for 32-bit signed integers.");
		
		write("'");
		write("uint", paleRed);
		writeLine("' for 32-bit unsigned integers.");
		
		write("'");
		write("long", paleRed);
		writeLine("' for 64-bit signed integers.");
		
		write("'");
		write("ulong", paleRed);
		writeLine("' for 64-bit unsigned integers.");
		
		write("'");
		write("double", paleRed);
		writeLine("' for 64-bit floating-point numbers (numbers with decimals).");
		
		writeLine();
		writeLine("Note: Type names are case-insensitive.", purple);
		writeLine();
		writeLine("You can add '[]' to the end of the type name to make an array", purple);
	}

	
	public static void tutorial(){
		writeLine();
		writeLine("An AshFile is a file format that has .ash extension, but it also represents a structure of data.");
		writeLine("This structure is based in multiple pockets of information. This pockets are called camps.");
		writeLine("Each camp has a name and a value. This value can be many things. (Type typelist for a full list)");
		writeLine("For example, you could have a camp named 'debt', with its value being a number and being 12000.");
		writeLine("This structure can be used in programming projects wwith the AshLib nuget package.");
		writeLine("This Editor is just a way to make it possible to edit AshFiles without having to code.");
	}
	
	public static void info(){
		writeLine("The current version of AshFile Editor is v3.1.1", purple);
		writeLine("This version is prepared to support v3 AshFiles", purple);
		writeLine();
		writeLine("It was made by Siljam for the AshProject", red);
	}
	
	public static void exit(){
		askToSave();
		writeLine("Bye bye honey pie!", purple);
		Environment.Exit(0);
	}
	
	public static void loadConfig(){
		writeLine("The config is an AshFile, so it will be loaded. When you finish, save it and restart the application.", purple);
		askToSave();
		path = dep.path + "/config.ash";
		loadFromPath();
	}
	
	public static void writeLine(){
		Console.WriteLine();
	}
	
	public static void writeLine(string s){
		Console.WriteLine(s);
	}
	
	public static void writeLine(object s){
		Console.WriteLine(s);
	}
	
	public static void writeLine(string s, Color3 c){
		if(!dep.config.GetCamp<bool>("useColors")){
			write(s);
			return;
		}
		FormatString fs = new FormatString();
		fs.Append(s, new CharFormat(c));
		Console.WriteLine(fs);
	}
	
	public static void writeLine(object s, Color3 c){
		if(!dep.config.GetCamp<bool>("useColors")){
			write(s);
			return;
		}
		FormatString fs = new FormatString();
		fs.Append(s.ToString(), new CharFormat(c));
		Console.WriteLine(fs);
	}
	
	public static void write(string s){
		Console.Write(s);
	}
	
	public static void write(object s){
		Console.Write(s);
	}
	
	public static void write(string s, Color3 c){
		if(!dep.config.GetCamp<bool>("useColors")){
			write(s);
			return;
		}
		FormatString fs = new FormatString();
		fs.Append(s, new CharFormat(c));
		Console.Write(fs);
	}
	
	public static void write(object s, Color3 c){
		if(!dep.config.GetCamp<bool>("useColors")){
			write(s);
			return;
		}
		FormatString fs = new FormatString();
		fs.Append(s.ToString(), new CharFormat(c));
		Console.Write(fs);
	}
}