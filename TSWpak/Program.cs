using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSWpak
{
    class Program
    {
        static bool StartedFromGui =
           !Console.IsOutputRedirected
        && !Console.IsInputRedirected
        && !Console.IsErrorRedirected
        && Environment.UserInteractive
        && Environment.CurrentDirectory == System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
        && Console.CursorTop == 0 && Console.CursorLeft == 0
        && Console.Title == Environment.GetCommandLineArgs()[0]
        && Environment.GetCommandLineArgs()[0] == System.Reflection.Assembly.GetEntryAssembly().Location;

        static void Main(string[] args)
        {
            string usage = "Packaging Syntax:\n[Mod content path] [Pak file name (optional)] [UnrealPak.exe path (optional)]\n\n" +
                "Commands:\n" +
                "RESET  Deletes saved UnrealPak.exe path\n" +
                "HELP   Opens this very list\n" +
                "INFO   Programm info\n";
            string info = System.Reflection.Assembly.GetEntryAssembly().GetName().Name + " " 
                + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString() + " made by Foobian";

            string savedUnrealPakPath = ReadSetting("UnrealPakPath");

            string unrealPakPath = null;
            string contentPath = null;
            string pakName = null;

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "help" | args[0] == "?")
                {
                    Console.WriteLine(usage);
                    Exit();
                }
                else if (args[0].ToLower() == "info" | args[0].ToLower() == "ver" | args[0].ToLower() == "version")
                {
                    WriteColoredLine(info, ConsoleColor.Blue);
                    Exit();
                }
                else if (args[0].ToLower() == "reset")
                {
                    // Resets config file
                    UpdateAppSetting("UnrealPakPath", "");
                    Exit();
                }
                else if (args[0].ToLower() == "readout")
                {
                    // Readout of all the keys from the config file
                    NameValueCollection sAll;
                    sAll = ConfigurationManager.AppSettings;

                    foreach (string s in sAll.AllKeys)
                        Console.WriteLine("Key: " + s + " Value: " + sAll.Get(s));
                    Exit();
                }
                else
                {
                    contentPath = args[0];

                    if (args.Length == 2)
                        pakName = args[1];

                    if (args.Length == 3)
                    {
                        unrealPakPath = args[2];
                        WriteColoredLine("If the inputed UnrealPak path is confirmed working it will be stores as the defaut path", ConsoleColor.DarkGray);
                    }

                    if(String.IsNullOrWhiteSpace(savedUnrealPakPath) && String.IsNullOrWhiteSpace(unrealPakPath))
                    {
                        WriteColoredLine("It looks like there is no default UnrealPak.exe path specified, please enter one bellow! If confirmed working, " +
                            "it will be used as the default path if no path is given. You can use the command \"reset\" to reset the path if anything goes wrong.", ConsoleColor.Blue);

                        Console.Write("UnrealPak.exe file path: ");
                        unrealPakPath = Console.ReadLine();
                    }
                    else
                    {
                        unrealPakPath = savedUnrealPakPath;
                        WriteColoredLine("Using stored UnrealPak.exe path", ConsoleColor.DarkGray);
                    }

                    if (args.Length > 3)
                    {
                        WriteColoredLine("Invalid number of arguments", ConsoleColor.Red);
                        Console.WriteLine(usage);
                        Exit(0);
                    }
                }
            }
            else
            {
                if(StartedFromGui)
                    Console.WriteLine("Please run TSWpak.exe from a cmd or PowerShell window with following commands:");

                Console.WriteLine(usage);
                Exit(0);

                /*
                Console.Write("Mod content file path: ");
                contentPath = Console.ReadLine();
                Console.Write("pak name/file path (optional): ");
                pakName = Console.ReadLine();
                Console.Write("UnrealPak.exe file path: ");
                unrealPakPath = Console.ReadLine();
                */
            }

            #region Idiot Protection
            if(!unrealPakPath.Contains(".exe"))
            {
                WriteColoredLine("Your selected path to UnrealPak.exe (" + unrealPakPath + ") does not end in .exe! " +
                    "Please enter the path to UnrealPak.exe including the exe-file.", ConsoleColor.Red);
                Exit(2);
            }

            TestDir(contentPath);

            if (contentPath.EndsWith("TS2Prototype") | contentPath.EndsWith("TS2Prototype\\"))
            {
                WriteColoredLine("Please select the folder above your \"TS2Prototype\" Folder, otherwise Train Sim World will not see the mod. " +
                    "This folder should be empty", ConsoleColor.Red);

                ContinueQuestion:
                WriteColoredLine("Do you still want to continue? Y/N", ConsoleColor.Yellow);

                char questionAnswer = Console.ReadKey().KeyChar;
                switch (questionAnswer.ToString().ToLower())
                {
                    case "y":
                        Console.Write("\n");
                        break;
                    case "n":
                        Environment.Exit(0);
                        break;
                    default:
                        goto ContinueQuestion;
                }
            }

            // Check if "TS2Prototype" sub-folder exists
            DirectoryInfo contentDirInfo = new DirectoryInfo(contentPath);
            DirectoryInfo[] contentDirSubs = contentDirInfo.GetDirectories();
            bool ts2DirExists = false;
            foreach (DirectoryInfo dir in contentDirSubs)
            {
                if(dir.Name == "TS2Prototype")
                {
                    ts2DirExists = true;
                }
            }

            FileInfo[] contentFiles = contentDirInfo.GetFiles();
            foreach (FileInfo file in contentFiles)
            {
                if(file.Name.ToLower().Contains(".pak"))
                {
                    WriteColoredLine("This ain't a converter silly!", ConsoleColor.Red);
                    Exit();
                }
            }

            if(!ts2DirExists)
            {
                WriteColoredLine("In your selected folder there was no sub-directory found with the name of \"TS2Prototype\"!", ConsoleColor.Red);
                Exit(3);
            }

            if (pakName == null || !pakName.ToLower().Contains(".pak")) 
            {
                pakName = "TSWpak-output.pak";
            }
            
            if (!pakName.Contains(@":\")) // Check if filepath has the part after the drive letter ":\" in it, if so assume it#s a full file path
            {
                if (pakName.Contains("\""))
                {
                    pakName.Replace("\"", ""); // Remove quatation marks around the file path as they will be added again a few lines down
                }

                string pakFilePath = "\"" + Directory.GetCurrentDirectory() + @"\" + pakName + "\"";
                WriteColoredLine("No full/invalid pak file path (" + pakName + ") specified! Output location: " + pakFilePath, ConsoleColor.DarkGray);
                pakName = pakFilePath;
            }
            #endregion

            string responseFileContent = "\"" + contentPath + "\\*.*\" \"..\\..\\..\\*.*\"";
            string responseFilePath = Path.GetTempPath() + "TSWpak_responseFile.txt";
            File.WriteAllText(responseFilePath, responseFileContent);

            string unrealPakArguments = pakName + " -create=" + responseFilePath + " -compress";

            Console.Write("\nUnrealPak.exe output:\n");

            using (Process process = new Process())
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.UseShellExecute = false;
                process.StartInfo = processStartInfo;

                process.StartInfo.FileName = unrealPakPath;
                process.StartInfo.Arguments = unrealPakArguments;

                process.OutputDataReceived += (sender, outArgs) => { PrintUnrealPakOutput(outArgs.Data); };
                process.ErrorDataReceived += (sender, outArgs) => { PrintUnrealPakOutput(outArgs.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Console.WriteLine(process.StandardOutput.ReadToEnd());
                process.WaitForExit();
            }

            // UnrealPak path is knwon working by now, it can be saved
            if (unrealPakPath.Contains("\""))
                unrealPakPath.Replace("\"", "");
            UpdateAppSetting("UnrealPakPath", unrealPakPath);

            #region Cleanup
            File.Delete(responseFilePath);

            if (StartedFromGui)
                Exit(0);
            #endregion
        }

        private static void PrintUnrealPakOutput(string unrealPakOutput)
        {
            if(unrealPakOutput != null) // If the garbage collector does garbage itself this should be a inappropriate fail-safe, asnyc stuff sucks and I'm too lazy to do it right
            {
                if (unrealPakOutput.Contains("Error: "))
                    WriteColoredLine(unrealPakOutput, ConsoleColor.Red);
                else if (unrealPakOutput.Contains("LogDerivedDataCache: Display: "))
                    WriteColoredLine(unrealPakOutput, ConsoleColor.DarkGray);
                else if (unrealPakOutput.Contains("LogPakFile: Display: Creating pak "))
                    WriteColoredLine(unrealPakOutput, ConsoleColor.Green);
                else if (unrealPakOutput.Contains("LogPakFile: Display: Added "))
                    WriteColoredLine(unrealPakOutput, ConsoleColor.Green);
                else if (unrealPakOutput.Contains("LogPakFile: Display: Using command line for crypto configuration"))
                    WriteColoredLine(unrealPakOutput, ConsoleColor.DarkGray);
                else
                    Console.WriteLine(unrealPakOutput);
            }
        }

        static void WriteColoredLine(string text, ConsoleColor color)
        {
            ConsoleColor userForeground = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = userForeground;
        }

        static void TestDir(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path); // TODO: Filter input. This is a base System.IO method and it can cause BSODs with the wrong inputs lol
            if (!dir.Exists)
            {
                WriteColoredLine(path + " not found!", ConsoleColor.Red);
                Exit(3);
            }
        }

        static void Exit(int exitcode = 0)
        {
            if (StartedFromGui)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            Environment.Exit(exitcode);
        }

        static void UpdateAppSetting(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        static string ReadSetting(string key)
        {
            if(File.Exists(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile))
            { 
                try
                {
                    var appSettings = ConfigurationManager.AppSettings;
                    string result = appSettings[key] ?? "Not Found";
                    return result;
                }
                catch (ConfigurationErrorsException)
                {
                    WriteColoredLine("Error reading app settings", ConsoleColor.Red);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}