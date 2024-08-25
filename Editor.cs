using System;
using System.Globalization;
using AshLib;

public class Editor{
	static string? path;
	static AshFile af;
	static bool hasBeenSaved;
	
	public static void Main(string[] args){
		hasBeenSaved = true;
		af = new AshFile();
		if(args.Length > 0){
			path = removeQuotes(args[0]); //Load path if arguments
			if(!loadFromPath()){
				newBlank();
			} else {
				Console.WriteLine(Path.GetFileName(path) + " loaded succesfully.");
				see();
			}
		} else {
			newBlank();
		}
		
		string command;
		while(true){
			try{
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
			} catch(Exception e){
				Console.WriteLine("An error occured! Here is more info:");
				Console.WriteLine("Message: " + e.Message);
				Console.WriteLine("Stack Trace: " + e.StackTrace);
			}
		}
	}
	
	public static void setCamp(){
		string name = askName();
		AshLib.Type t = askType();
		CampValue o = askValue(t);
		af.SetCamp(name, o);
		hasBeenSaved = false;
	}
	
	public static void getCamp(){
		string name = askName();
		CampValue o;
		if(!af.CanGetCampValue(name, out o)){
			Console.WriteLine("There is no camp named \"" + name + "\".");
			return;
		}
		AshLib.Type t = o.type;
		Console.WriteLine("Camp name: " + name + " | Type: " + getTypeName(t) + " | Value: " + o);
	}
	
	public static void deleteCamp(){
		string name = askName();
		if(!af.CanDeleteCamp(name)){
			Console.WriteLine("There is no camp named \"" + name + "\".");
			return;
		}
		hasBeenSaved = false;
	}
	
	public static void renameCamp(){
		string oldName = askOldName();
		if(!af.ExistsCamp(oldName)){
			Console.WriteLine("There is no camp named \"" + oldName + "\".");
			return;
		}
		string newName = askNewName();
		af.RenameCamp(oldName, newName);
		hasBeenSaved = false;
	}
	
	public static CampValue askValue(AshLib.Type t){
		while(true){
			start:
			Console.Write("Please enter the value of the camp: ");
			string answer = Console.ReadLine();
			
			switch(t){
				case AshLib.Type.ByteArray:
					string[] a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((byte[]) null);
					}
					byte[] b = new byte[a.Length];
					for(int i = 0; i < a.Length; i++){
						byte c;
						if(!byte.TryParse(a[i], out c)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						b[i] = c;
					}
					return new CampValue(b);
				case AshLib.Type.String:
					return new CampValue(answer);
				case AshLib.Type.Byte:
					byte d;
					if(!byte.TryParse(answer, out d)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(d);
				case AshLib.Type.Ushort:
					ushort e;
					if(!ushort.TryParse(answer, out e)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(e);
				case AshLib.Type.Uint:
					uint f;
					if(!uint.TryParse(answer, out f)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(f);
				case AshLib.Type.Ulong:
					ulong g;
					if(!ulong.TryParse(answer, out g)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(g);
				case AshLib.Type.Sbyte:
					sbyte h;
					if(!sbyte.TryParse(answer, out h)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(h);
				case AshLib.Type.Short:
					short j;
					if(!short.TryParse(answer, out j)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(j);
				case AshLib.Type.Int:
					int k;
					if(!int.TryParse(answer, out k)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(k);
				case AshLib.Type.Long:
					long l;
					if(!long.TryParse(answer, out l)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(l);
				case AshLib.Type.Color:
					if(answer[0] == '#'){
						return new CampValue(new Color3(answer));
					}
					a = answer.Split(",");
					if(a.Length != 3){
						Console.WriteLine("There is not the correct amount of arguments. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					byte r1, r2, r3;
					if(!byte.TryParse(a[0], out r1)){
						Console.WriteLine("The first number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!byte.TryParse(a[1], out r2)){
						Console.WriteLine("The second number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!byte.TryParse(a[2], out r3)){
						Console.WriteLine("The third number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(new Color3(r1, r2, r3));
				case AshLib.Type.Float:
					float m;
					if(!float.TryParse(answer, out m)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(m);
				case AshLib.Type.Double:
					double n;
					if(!double.TryParse(answer, out n)){
						Console.WriteLine("That number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(n);
				case AshLib.Type.Vec2:
					a = answer.Split(",");
					if(a.Length != 2){
						Console.WriteLine("There is not the correct amount of arguments. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					float s1, s2, s3, s4;
					if(!float.TryParse(a[0], out s1)){
						Console.WriteLine("The first number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!float.TryParse(a[1], out s2)){
						Console.WriteLine("The second number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(new Vec2(s1, s2));
				case AshLib.Type.Vec3:
					a = answer.Split(",");
					if(a.Length != 3){
						Console.WriteLine("There is not the correct amount of arguments. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!float.TryParse(a[0], out s1)){
						Console.WriteLine("The first number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!float.TryParse(a[1], out s2)){
						Console.WriteLine("The second number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!float.TryParse(a[2], out s3)){
						Console.WriteLine("The second number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(new Vec3(s1, s2, s3));
				case AshLib.Type.Vec4:
					a = answer.Split(",");
					if(a.Length != 4){
						Console.WriteLine("There is not the correct amount of arguments. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!float.TryParse(a[0], out s1)){
						Console.WriteLine("The first number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!float.TryParse(a[1], out s2)){
						Console.WriteLine("The second number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!float.TryParse(a[2], out s3)){
						Console.WriteLine("The third number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					if(!float.TryParse(a[3], out s4)){
						Console.WriteLine("The forth number is in an invalid format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					return new CampValue(new Vec4(s1, s2, s3, s4));
				case AshLib.Type.Bool:
					answer = answer.ToLower();
					if(answer == "true" || answer == "t"){
						return new CampValue(true);
					} else if(answer == "false" || answer == "f"){
						return new CampValue(false);
					}
					Console.WriteLine("That boolean value is in an invalid format. Please enter true/false value.");
					Console.WriteLine();
					goto start;
				case AshLib.Type.UbyteArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((byte[]) null);
					}
					b = new byte[a.Length];
					for(int i = 0; i < a.Length; i++){
						byte c;
						if(!byte.TryParse(a[i], out c)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						b[i] = c;
					}
					return new CampValue(b);
				case AshLib.Type.UshortArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((ushort[]) null);
					}
					ushort[] o = new ushort[a.Length];
					for(int i = 0; i < a.Length; i++){
						ushort p;
						if(!ushort.TryParse(a[i], out p)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						o[i] = p;
					}
					return new CampValue(o);
				case AshLib.Type.UintArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((uint[]) null);
					}
					uint[] q = new uint[a.Length];
					for(int i = 0; i < a.Length; i++){
						uint r;
						if(!uint.TryParse(a[i], out r)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						q[i] = r;
					}
					return new CampValue(q);
				case AshLib.Type.UlongArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((ulong[]) null);
					}
					ulong[] s = new ulong[a.Length];
					for(int i = 0; i < a.Length; i++){
						ulong u;
						if(!ulong.TryParse(a[i], out u)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						s[i] = u;
					}
					return new CampValue(s);
				case AshLib.Type.SbyteArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((sbyte[]) null);
					}
					sbyte[] aa = new sbyte[a.Length];
					for(int i = 0; i < a.Length; i++){
						sbyte r;
						if(!sbyte.TryParse(a[i], out r)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						aa[i] = r;
					}
					return new CampValue(aa);
				case AshLib.Type.ShortArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((short[]) null);
					}
					short[] ab = new short[a.Length];
					for(int i = 0; i < a.Length; i++){
						short r;
						if(!short.TryParse(a[i], out r)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						ab[i] = r;
					}
					return new CampValue(ab);
				case AshLib.Type.IntArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((int[]) null);
					}
					int[] ac = new int[a.Length];
					for(int i = 0; i < a.Length; i++){
						int r;
						if(!int.TryParse(a[i], out r)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						ac[i] = r;
					}
					return new CampValue(ac);
				case AshLib.Type.LongArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((long[]) null);
					}
					long[] ad = new long[a.Length];
					for(int i = 0; i < a.Length; i++){
						long r;
						if(!long.TryParse(a[i], out r)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						ad[i] = r;
					}
					return new CampValue(ad);
				case AshLib.Type.FloatArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((float[]) null);
					}
					float[] ae = new float[a.Length];
					for(int i = 0; i < a.Length; i++){
						float r;
						if(!float.TryParse(a[i], out r)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						ae[i] = r;
					}
					return new CampValue(ae);
				case AshLib.Type.DoubleArray:
					a = answer.Split(",");
					if(a.Length < 1){
						return new CampValue((long[]) null);
					}
					double[] af = new double[a.Length];
					for(int i = 0; i < a.Length; i++){
						double r;
						if(!double.TryParse(a[i], out r)){
							Console.WriteLine("The element number " + i + " is not in a correct format. Please enter again.");
							Console.WriteLine();
							goto start;
						}
						af[i] = r;
					}
					return new CampValue(af);
				case AshLib.Type.Date:
					string format = "HH:mm:ss dd/MM/yyyy";
					DateTime dt;
					if(!DateTime.TryParseExact(answer, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)){
						Console.WriteLine("The date is not in a correct format. Please enter again.");
						Console.WriteLine();
						goto start;
					}
					Date da = (Date) dt;
					return new CampValue(da);
				default:
					Console.WriteLine();
					goto start;
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
	
	public static AshLib.Type askType(){
		while(true){
			Console.Write("Please enter the type (type help for a list of valid types): ");
			string answer = Console.ReadLine();
			if(answer == "help" || answer == "typehelp" || answer == "typelist"){
				typeHelp();
				Console.WriteLine();
				continue;
			}
			AshLib.Type t = getTypeFromString(answer);
			if(t != AshLib.Type.Invalid){
				return t;
			}
			Console.WriteLine("That is not a valid type. Please enter a valid type.");
			Console.WriteLine();
		}
	}
	
	public static AshLib.Type getTypeFromString(string s){
		s = s.ToLower();
		if(byte.TryParse(s, out byte t) && t > -1 && t < 28){
			return (AshLib.Type) t;
		}
		switch(s){
			case "bytearray":
				return AshLib.Type.ByteArray;
			case "text":
			case "string":
				return AshLib.Type.String;
			case "byte":
				return AshLib.Type.Byte;
			case "ushort":
			case "unsigned short":
				return AshLib.Type.Ushort;
			case "uint":
			case "unsigned int":
				return AshLib.Type.Uint;
			case "ulong":
			case "unsigned long":
				return AshLib.Type.Ulong;
			case "sbyte":
			case "signed byte":
				return AshLib.Type.Sbyte;
			case "short":
				return AshLib.Type.Short;
			case "number":
			case "int":
				return AshLib.Type.Int;
			case "long":
				return AshLib.Type.Long;
			case "color":
			case "color3":
			case "colorrgb":
			case "rgb":
				return AshLib.Type.Color;
			case "float":
				return AshLib.Type.Float;
			case "double":
				return AshLib.Type.Double;
			case "vec2":
				return AshLib.Type.Vec2;
			case "vec3":
				return AshLib.Type.Vec3;
			case "vec4":
				return AshLib.Type.Vec4;
			case "bool":
			case "boolean":
				return AshLib.Type.Bool;
			case "ubytearray":
			case "ubyte array":
				return AshLib.Type.UbyteArray;
			case "ushortarray":
			case "ushort array":
				return AshLib.Type.UshortArray;
			case "uintarray":
			case "uint array":
				return AshLib.Type.UintArray;
			case "ulongarray":
			case "ulong array":
				return AshLib.Type.UlongArray;
			case "sbytearray":
			case "sbyte array":
				return AshLib.Type.SbyteArray;
			case "shortarray":
			case "short array":
				return AshLib.Type.ShortArray;
			case "intarray":
			case "int array":
				return AshLib.Type.IntArray;
			case "longarray":
			case "long array":
				return AshLib.Type.LongArray;
			case "floatarray":
			case "float array":
				return AshLib.Type.FloatArray;
			case "doublearray":
			case "double array":
				return AshLib.Type.DoubleArray;
			case "date":
			case "time":
				return AshLib.Type.Date;
			default:
				return AshLib.Type.Invalid;
		}
	}
	
	public static string getTypeName(AshLib.Type t){
		switch (t){
			case AshLib.Type.ByteArray:
				return "Byte Array";
			case AshLib.Type.String:
				return "Text";
			case AshLib.Type.Byte:
				return "Byte";
			case AshLib.Type.Ushort:
				return "Unsigned 2-byte number";
			case AshLib.Type.Uint:
				return "Unsigned 4-byte number";
			case AshLib.Type.Ulong:
				return "Unsigned 8-byte number";
			case AshLib.Type.Sbyte:
				return "Signed byte";
			case AshLib.Type.Short:
				return "Signed 2-byte number";
			case AshLib.Type.Int:
				return "Integer number";
			case AshLib.Type.Long:
				return "Signed 8-byte number";
			case AshLib.Type.Color:
				return "Color";
			case AshLib.Type.Float:
				return "Floating point number";
			case AshLib.Type.Double:
				return "Double precision number";
			case AshLib.Type.Vec2:
				return "2D Vector";
			case AshLib.Type.Vec3:
				return "3D Vector";
			case AshLib.Type.Vec4:
				return "4D Vector";
			case AshLib.Type.Bool:
				return "Boolean";
			case AshLib.Type.UbyteArray:
				return "Unsigned Byte Array";
			case AshLib.Type.UshortArray:
				return "Unsigned 2-byte number array";
			case AshLib.Type.UintArray:
				return "Unsigned 4-byte number array";
			case AshLib.Type.UlongArray:
				return "Unsigned 8-byte number array";
			case AshLib.Type.SbyteArray:
				return "Signed byte array";
			case AshLib.Type.ShortArray:
				return "Signed 2-byte number array";
			case AshLib.Type.IntArray:
				return "Integer number array";
			case AshLib.Type.LongArray:
				return "Signed 8-byte number array";
			case AshLib.Type.FloatArray:
				return "Floating point number array";
			case AshLib.Type.DoubleArray:
				return "Double precision number array";
			case AshLib.Type.Date:
				return "Date";
			case AshLib.Type.Invalid:
			default:
				return "Unknown Type: " + (int)t;
		}
	}

	
	public static void reload(){
		if(path == null){
			Console.WriteLine("There is no path asociated with this file");
			return;
		}
		if(!loadFromPath()){
			return;
		}
		Console.WriteLine(Path.GetFileName(path) + " reloaded succesfully.");
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
		if(!loadFromPath()){
			return;
		}
		Console.WriteLine(Path.GetFileName(path) + " loaded succesfully.");
		hasBeenSaved = true;
	}
	
	public static void askPath(){
		Console.Write("Please enter the path: ");
		string answer = Console.ReadLine();
		path = removeQuotes(answer);
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
		
		handleAshFileErrors();
	}
	
	public static bool loadFromPath(){
		try{
			if(!File.Exists(path)){
				path = null;
				Console.WriteLine("Could not load from path becaue the file doesnt exist.");
				return false;
			}
			af.Load(path);
			
			handleAshFileErrors();
			
			return true;
		} catch(Exception e){
			Console.WriteLine("An error occured trying to open the file. Here is more info:");
			Console.WriteLine("Message: " + e.Message);
			Console.WriteLine("Stack Trace: " + e.StackTrace);
			
			return false;
		}
	}
	
	
	public static void see(){
		Console.WriteLine(af.AsString());
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
			Console.WriteLine("Errors occured while reading or saving the file, here is more info:");
			Console.WriteLine("Error count: " + AshFile.GetErrorCount());
			Console.WriteLine("Error log: " + AshFile.GetErrorLog());
		}
		AshFile.EmptyErrors();
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
		Console.WriteLine("\"number\" for natural positive numbers.");
		Console.WriteLine("\"bool\" for true/false values.");
		Console.WriteLine("\"float\" for 32-bit floating-point numbers (numbers with decimals).");
		Console.WriteLine("\"color\" for RGB color values.");
		Console.WriteLine("\"date\" for date/time values.");
		Console.WriteLine("\"vec2\" for 2D vectors.");
		Console.WriteLine("\"vec3\" for 3D vectors.");
		Console.WriteLine("\"vec4\" for 4D vectors.");
		Console.WriteLine();
		Console.WriteLine("\"byte\" for 8-bit signed integers.");
		Console.WriteLine("\"sbyte\" for 8-bit signed integers.");
		Console.WriteLine("\"short\" for 16-bit signed integers.");
		Console.WriteLine("\"ushort\" for 16-bit unsigned integers.");
		Console.WriteLine("\"int\" for 32-bit signed integers.");
		Console.WriteLine("\"uint\" for 32-bit unsigned integers.");
		Console.WriteLine("\"long\" for 64-bit signed integers.");
		Console.WriteLine("\"ulong\" for 64-bit unsigned integers.");
		Console.WriteLine("\"double\" for 64-bit floating-point numbers (numbers with decimals).");
		Console.WriteLine();
		Console.WriteLine("\"bytearray\" for arrays of bytes.");
		Console.WriteLine("\"ubytearray\" for arrays of unsigned bytes.");
		Console.WriteLine("\"shortarray\" for arrays of signed shorts.");
		Console.WriteLine("\"ushortarray\" for arrays of unsigned shorts.");
		Console.WriteLine("\"intarray\" for arrays of signed integers.");
		Console.WriteLine("\"uintarray\" for arrays of unsigned integers.");
		Console.WriteLine("\"longarray\" for arrays of signed longs.");
		Console.WriteLine("\"ulongarray\" for arrays of unsigned longs.");
		Console.WriteLine("\"floatarray\" for arrays of floating-point numbers.");
		Console.WriteLine("\"doublearray\" for arrays of double-precision floating-point numbers.");
		Console.WriteLine();
		Console.WriteLine("Note: Type names are case-insensitive.");
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
		Console.WriteLine("The current version of AshFile Editor is v2.0.0");
		Console.WriteLine("This version is prepared to support v2 AshFiles");
		Console.WriteLine();
		Console.WriteLine("It was made by Dumbelfo for the AshProject");
	}
	
	public static void exit(){
		askToSave();
		Console.WriteLine("Bye bye honey pie!");
		Environment.Exit(0);
	}
}