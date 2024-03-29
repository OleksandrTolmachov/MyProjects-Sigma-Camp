﻿using System.Reflection;
using Task_14.Enums;

namespace Task_14.Factories.PatternStratehy
{
    public class ConsoleDialogue : IDialogueStrategy
    {
        private MethodInfo[] methods;
        private List<string> lables;

        public void StartProcess(Type type, object obj)
        {
            methods = type.BaseType.GetMethods();
            lables = new List<string>();

            int i = 0;
            foreach (var method in methods)
            {
                var label = method.GetCustomAttribute<NameLabel>();
                if (label != null)
                {
                    lables.Add($"{i} - " + label.Name);
                    i++;
                }
            }
            startDialogue(type, obj);
        }

        private void startDialogue(Type type, object obj)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("What do you want to buy? press number");
                    foreach (var lable in lables)
                    {
                        Console.WriteLine(lable);
                    }

                    if (!int.TryParse(Console.ReadLine(), out int variant))
                    {
                        Console.WriteLine("exit");
                        return;
                    }
                    object? prod = null;
                    if (methods[variant].GetParameters().Length == 0)
                    {
                        prod = methods[variant].Invoke(obj, default);
                    }
                    else
                    {
                        string? output = Console.ReadLine();
                        object en = Enum.Parse(methods[variant].GetParameters()[0].ParameterType, output);
                        prod = methods[variant].Invoke(obj, new[] { en });
                    }
                    Console.WriteLine(prod);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }
    }
}

