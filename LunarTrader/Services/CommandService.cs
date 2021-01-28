using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Alpaca.Markets;
using Microsoft.Extensions.DependencyInjection;
using LunarTrader.Attributes;
using LunarTrader.Interfaces;
using LunarTrader.Utils;
using WebSocket4Net.Common;

namespace LunarTrader.Services
{
    public partial class CommandService : ICommandService
    {
        public Dictionary<string, Type> Enums { get; private set; }
        public ILoggerService LoggerService { get; }
        public Dictionary<string, CommandInfo> Commands { get; }
        public List<string> Sorted { get; }
        public List<string> CommandHistory { get; }
        public string CurrentCommand = "";

        public CommandService(ILoggerService loggerService)
        {
            LoggerService = loggerService;
            Enums = new Dictionary<string, Type>();
            Commands = new Dictionary<string, CommandInfo>();
            Sorted = new List<string>();
            CommandHistory = new List<string>();
            
            buildCommandsList();
            
        }
        
        public void Listen()
        {
            CurrentCommand = Console.ReadLine();
//
            enterCommand();
//
            //return true;
            //var typedKey = Console.ReadKey();
            ////if we hit enter, run the command
            //if (typedKey.Key == ConsoleKey.Enter)
            //{
            //    Console.WriteLine(CurrentCommand);
            //    enterCommand();
            //}
            //else if (typedKey.Key == ConsoleKey.Backspace)
            //{
            //    //Console.Write("\b \b");
            //    CurrentCommand.Remove(CurrentCommand.Length - 1);
            //}
            //else if(char.IsNumber(typedKey.KeyChar) || char.IsLetter(typedKey.KeyChar))
            //{
            //    CurrentCommand += typedKey.KeyChar.ToString();
            //}
            //add typed key to the current command
            
            
            
            Listen();
        }
        

        private void enterCommand()
        {
            var data = CurrentCommand.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (CommandHistory.Count == 0 || CommandHistory[0] != CurrentCommand)
                CommandHistory.Insert(0, CurrentCommand);
            CurrentCommand = "";

            if (data.Length == 0)
                return;

            var args = new string[data.Length - 1];
            for (var i = 1; i < data.Length; i++)
                args[i - 1] = data[i];
            executeCommand(data[0], args);
        }
        
        private void executeCommand(string command, string[] args)
        {
            if (Commands.ContainsKey(command))
            {
                Commands[command].Action(args);
            }
                
            else
                LoggerService.LogInfo($"Command {command} not found! Type 'help' for a list of commands");
        }
        private void buildCommandsList()
        {
            Enums = Assembly.GetAssembly(typeof(LimitOrder))?.GetTypes().Where(t => t.IsEnum).
                ToDictionary(x => x.ToString().Split('.').Last(), x => x);
            foreach (var item in Enums)
            {
                LoggerService.LogTrace(item.ToString());
            }
            LoggerService.LogInfo("Building Commands");
            // this will get us the current assembly
            processAssembly(Assembly.GetAssembly(typeof(CommandService)));

            // Maintain the sorted command list
            foreach (var command in Commands)
                Sorted.Add(command.Key);
            Sorted.Sort();
        }
        private void processAssembly(Assembly assembly)
        {
            //Get the methods that have a commandAttribute
            var methods = assembly.DefinedTypes.SelectMany(t => t.DeclaredMethods)
                .Where(m => m.GetCustomAttribute<CommandAttribute>(false) != null);
            
            //Get Command attribute from each method and then process the method
            foreach(var method in methods)
            {
                CommandAttribute attr = null;
                attr = method.GetCustomAttribute<CommandAttribute>();
                processMethod(method, attr);
            }

        }
        private void processMethod(MethodBase method, CommandAttribute attr)
        {
            if (!method.IsStatic)
            {
                if (method.DeclaringType is not null)
                    throw new Exception(method.DeclaringType.Name + "." + method.Name +
                                        " is marked as a command but is not static");
            }
            else
            {
                var info = new CommandInfo();

                var parameters = method.GetParameters();
                var defaults = new object[parameters.Length];
                var usage = new string[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    usage[i] = p.Name + " : ";

                    //usage[i] += p.ParameterType.ToString().Split('.').Last();
                    
                   if (p.ParameterType == typeof(string))
                       usage[i] += "string";
                   else if (p.ParameterType == typeof(int))
                       usage[i] += "int";
                   else if (p.ParameterType == typeof(decimal))
                       usage[i] += "decimal";
                   else if (p.ParameterType == typeof(bool))
                       usage[i] += "bool";
                   else if (Enums.Values.Contains(p.ParameterType))
                       usage[i] += p.ParameterType.ToString().Split('.').Last();
                   else if (method.DeclaringType is not null)
                       throw new Exception(method.DeclaringType.Name + "." + method.Name +
                                            " is marked as a command, but has an invalid parameter type. Allowed types are: string, int, decimal, bool and enums");

                    // no System.DBNull in PCL so we fake it
                    if (p.DefaultValue != null && p.DefaultValue.GetType().FullName == "System.DBNull")
                    {
                        defaults[i] = null;
                    }
                    else if (p.DefaultValue != null)
                    {
                        defaults[i] = p.DefaultValue;
                        if (p.ParameterType == typeof(string))
                            usage[i] += "=\"" + p.DefaultValue.ToString() + "\"";
                        else
                            usage[i] += "=" + p.DefaultValue.ToString();
                    }
                    else
                    {
                        defaults[i] = null;
                    }
                }

                if (usage.Length == 0)
                    info.Usage = "";
                else
                    info.Usage = "[" + string.Join(" | ", usage) + "]";

                info.Help = attr.Help;
                
                info.Action = args =>
                {
                    if (parameters.Length == 0)
                    {
                        method.Invoke(null, null);
                    }
                    else
                    {
                        var param = (object[]) defaults.Clone();

                        for (var i = 0; i < param.Length && i < args.Length; i++)
                        {
                            if (parameters[i].ParameterType == typeof(string))
                                param[i] = argString(args[i]);
                            else if (parameters[i].ParameterType == typeof(int))
                                param[i] = argInt(args[i]);
                            else if (parameters[i].ParameterType == typeof(decimal))
                                param[i] = argDecimal(args[i]);
                            else if (parameters[i].ParameterType == typeof(bool))
                                param[i] = argBool(args[i]);
                            else if (Enums.ContainsValue(parameters[i].ParameterType))
                                param[i] = Enum.Parse(parameters[i].ParameterType, args[i]);
                        }

                        try
                        {
                            method.Invoke(null, param);
                        }
                        catch (Exception e)
                        {
                            LoggerService.LogError(e);
                        }
                    }
                };

                Commands[attr.Name] = info;
                LoggerService.LogTrace($"Command Added: {attr.Name}");
            }
        }
        #region Parsing arguments

        private static string argString(string arg)
        {
            if (arg == null)
                return "";
            else
                return arg;
        }

        private static bool argBool(string arg)
        {
            if (arg != null)
                return !(arg == "0" || arg.ToLower() == "false" || arg.ToLower() == "f");
            else
                return false;
        }


        private static int argInt(string arg)
        {
            try
            {
                return Convert.ToInt32(arg);
            }
            catch
            {
                return 0;
            }
        }


        private static decimal argDecimal(string arg)
        {
            try
            {
                return Convert.ToDecimal(arg);
            }
            catch
            {
                return 0;
            }
        }

        
        


        #endregion
    }

    public class CommandInfo
    {
        public Action<string[]> Action;
        public string Help;
        public string Usage;
    }
    
}