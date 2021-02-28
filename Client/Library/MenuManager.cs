using System;

namespace Library
{
    public static class MenuManager
    {
        public static int Menu(string header, int menuWidth, params string[] options)
        {
            Console.Clear();
            int chosen = 0;
            while (true)
            {
                Console.SetCursorPosition(0, 0);
                DrawHeader(header, menuWidth);

                foreach (var option in options)
                {
                    if (option == options[chosen])
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(TableBuilder.AlignCentre(option, menuWidth));
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write(TableBuilder.AlignCentre(option, menuWidth));
                    }
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }
                
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        Console.Clear();
                        return chosen;
                    case ConsoleKey.UpArrow:
                        if (chosen == 0) chosen = options.Length - 1;
                        else chosen--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (chosen == options.Length - 1) chosen = 0;
                        else chosen++;
                        break;
                    //case ConsoleKey.Escape: return -1;
                }
            }
        }
        public static void DrawHeader(string header,int width)
        {
            header = header.ToUpper();          
            Console.WriteLine("".PadLeft(width,'='));
            Console.WriteLine("|" + TableBuilder.AlignCentre(header, width - 2) + "|");
            Console.WriteLine("".PadLeft(width, '='));
        }        
    }
}
