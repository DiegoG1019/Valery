using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Valery
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }


        public static ConsoleColor[] StarColors =
        {
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Blue,
            ConsoleColor.Cyan,
            ConsoleColor.DarkBlue,
        };
        public class DrawMapClass : Dictionary<Point, CharColor>
        {
            new public CharColor this[Point ind]
            {
                get
                {
                    if (ContainsKey(ind))
                    {
                        return base[ind];
                    }

                    return CharColor.Emtpy;
                }
                set
                {
                    base[ind] = value;
                }
            }
        }
        public struct CharColor
        {
            public char Char;
            public ConsoleColor Color;
            public CharColor(char c, ConsoleColor color)
            {
                Char = c;
                Color = color;
            }
            public static CharColor Emtpy { get; } = new CharColor(' ', ConsoleColor.Black);
        }
        public static DrawMapClass DrawMap { get; } = new DrawMapClass();
        public static void DrawAt(Point p, CharColor cc)
        {
            if (Window.Contains(p))
            {
                if (cc.Char == ' ' || cc.Color == ConsoleColor.Black)
                    return;
                BlackEnableDrawAt(p, cc);
            }
        }
        public static void DrawAt(int x, int y, char c, ConsoleColor color)
        {
            DrawAt(new Point(x, y), new CharColor(c, color));
        }
        public static void BlackEnableDrawAt(int x, int y, char c, ConsoleColor color)
        {
            BlackEnableDrawAt(new Point(x, y), new CharColor(c, color));
        }
        public static void BlackEnableDrawAt(Point p, CharColor cc)
        {
            if (Window.Contains(p))
            {
                Console.SetCursorPosition(p.X, p.Y);
                Console.ForegroundColor = cc.Color;
                Console.Write(cc.Char);
                DrawMap[p] = cc;
            }
        }

        public static Random RandomGen = new Random();
        public interface IBody
        {
            public int Left { get; }
            public int Top { get; }
            public ConsoleColor Color { get; }
            public void Draw();
            public void Update();
        }

        public class ShootingStar : IBody
        {
            public int Left { get; private set; }
            public int Top { get; private set; }
            public ConsoleColor Color { get; private set; }

            public int UpdateStage { get; set; } = 0;
            public int UpdateInterval { get; private set; }
            public int CycleStage { get; private set; } = 0;
            public static string StaticTrail { get; } = "##\"\"``` ";
            public static string StaticCycle { get; } = "+*";
            public string Cycle => StaticCycle;
            public string Trail => StaticTrail;
            public int AngleOffset { get; private set; }

            public ShootingStar(int left, int updateInterval, int color, int angle) : this(left, updateInterval, ConsoleColor.Red, angle)
            {
                Color = StarColors[color];
            }
            public ShootingStar(int left, int updateInterval, ConsoleColor color, int angle)
            {
                Top = 0;
                Left = left;
                UpdateInterval = updateInterval;
                Color = color;
                AngleOffset = angle;
            }

            public void Update()
            {
                UpdateStage++;
                if (UpdateStage == UpdateInterval)
                {
                    UpdateStage = 0;
                    Progress();
                }
            }

            public void Progress()
            {
                CycleStage++;
                if (CycleStage >= Cycle.Length)
                {
                    CycleStage = 0;
                }

                if (Left + Trail.Length < 0)
                {
                    Left = WindowLeft;
                }

                if (Top - Trail.Length > WindowTop)
                {
                    Top = 0;
                }

                Left -= 1;
                Top += 1;
            }
            public void Draw()
            {
                if (Window.Contains(Left, Top))
                {
                    DrawAt(Left, Top, Head, Color);
                }
                for (int ind = 0; ind < Trail.Length; ind++)
                {
                    if (Window.Contains(Left + (ind + AngleOffset), Top - (ind + AngleOffset)))
                    {
                        BlackEnableDrawAt(Left + ind + AngleOffset, Top - (ind + AngleOffset), Trail[ind], Color);
                    }
                }
            }

            public char Head => Cycle[CycleStage];
        }

        public class Star : IBody
        {
            public int Left { get; private set; }

            public int Top { get; private set; }

            public int Type { get; private set; }
            public const int Types = 10;
            public ConsoleColor Color { get; private set; }
            public ConsoleColor ColorOuter1 { get; private set; }
            public ConsoleColor ColorOuter2 { get; private set; }
            public ConsoleColor Inner => ConsoleColor.White;

            public List<string> FirstStageHead => FirstStageHeads[Type];
            public List<string> SecondStageHead => SecondStageHeads[Type];

            public static List<List<string>> FirstStageHeads { get; } = new List<List<string>>()
            {
                new List<string>(){
                    "          _",
                    "      ,  | `.",
                    "--- --+-<#>-+- ---  --  -",
                    "      `._|_,'"
                },
                new List<string>(){
                    "    ;",
                    "- --+-  -",
                    "    !",
                    "    ."
                },
                new List<string>()
                {
                    "*"
                },
                new List<string>()
                {// # %
                    "  /",
                    "% # %",
                    "  /"
                }
            };
            public static List<List<string>> SecondStageHeads { get; } = new List<List<string>>()
            {
                new List<string>(){
                    "          _",
                    "      .  ! `.",
                    " --- -+-<%> +-- -- -- -",
                    "      `,_|_,`"
                },
                new List<string>(){
                    "    :",
                    "-- -+- -",
                    "    ¡",
                    "    ,"
                },
                new List<string>()
                {
                    "+"
                },
                new List<string>()
                {// # % /
                    "  %",
                    "/ # /",
                    "  %"
                }
            };

            public int UpdateStage { get; set; } = 0;
            public int UpdateInterval { get; private set; }
            public bool State { get; private set; } = false;

            public Star(int left, int top, int updateInterval, int type, ConsoleColor color)
            {
                Top = top;
                Left = left;
                UpdateInterval = updateInterval;
                Color = color;
                if (type > 8)
                {
                    Type = 0;
                }
                else
                {
                    Type = 1;
                }
            }
            public Star(int left, int top, int updateInterval, int colorA, int colorB, int colorC) : this(left, top, updateInterval, 0, ConsoleColor.Red)
            {
                Color = StarColors[colorA];
                ColorOuter1 = StarColors[colorB];
                ColorOuter2 = StarColors[colorC];
                Type = 3;
            }
            public Star(int left, int top, int updateInterval, int type, int color) : this(left, top, updateInterval, type, ConsoleColor.Black)
            {
                Color = StarColors[color];
            }
            public Star(int left, int top, int updateInterval, int color, bool sm0l) : this(left, top, updateInterval, 0, color)
            {
                Type = 2;
            }

            public void Update()
            {
                UpdateStage++;
                if (UpdateStage == UpdateInterval)
                {
                    UpdateStage = 0;
                    Switch();
                }
            }

            public void Switch()
            {
                State = !State;
            }

            public void Draw()
            {
                var co = Color;
                if (State)
                {
                    for (int Y = 0; Y < FirstStageHead.Count; Y++)
                    {
                        for (int X = 0; X < FirstStageHead[Y].Length; X++)
                        {   // # % /
                            if (Type == 3)
                            {
                                char c = FirstStageHead[Y][X];
                                switch (c)
                                {
                                    case '#':
                                        co = ColorOuter1;
                                        break;
                                    case '%':
                                        co = ColorOuter2;
                                        break;
                                    case '/':
                                        co = Color;
                                        break;
                                }
                            }
                            DrawAt(Left + X, Top + Y, FirstStageHead[Y][X], co);
                        }
                    }
                    return;
                }
                for (int Y = 0; Y < SecondStageHead.Count; Y++)
                {
                    for (int X = 0; X < SecondStageHead[Y].Length; X++)
                    {
                        if (Type == 3)
                        {
                            char c = SecondStageHead[Y][X];
                            switch (c)
                            {
                                case '#':
                                    co = ColorOuter1;
                                    break;
                                case '%':
                                    co = ColorOuter2;
                                    break;
                                case '/':
                                    co = Color;
                                    break;
                            }
                        }
                        DrawAt(Left + X, Top + Y, SecondStageHead[Y][X], co);
                    }
                }
            }
        }

        public const string Version = "0.0.2";
        public static DEVMODE devMode = default;
        const int Delay = 1200;
        public static int WindowLeft;
        public static int WindowTop;
        public static int WindowSize;
        public static Rectangle Window;
        static async Task Main(string[] args)
        {
            try
            {
                await MainSafe(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION CAUGHT.");
                Console.WriteLine(e);
                Console.WriteLine("Copy this and send it to me (with right click)\nPress any key to continue");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
        static async Task MainSafe(string[] args)
        {
            const int ENUM_CURRENT_SETTINGS = -1;

            devMode.dmSize = (short)Marshal.SizeOf(devMode);
            EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devMode);

            WindowLeft = (devMode.dmPelsWidth / 9);
            WindowTop = (devMode.dmPelsHeight / 18);
            WindowSize = WindowLeft * WindowTop;
            Window = new Rectangle(0, 0, WindowLeft, WindowTop);

            Console.CursorVisible = false;
            Console.Title = "Valery";
            Console.SetWindowPosition(0, 0);
            Console.SetWindowSize(WindowLeft, WindowTop);

            var ShootingStarCount = RandomGen.Next(5, 15);
            var StarCount = RandomGen.Next(15, 25);
            var TinyStarCount = RandomGen.Next(150, 300);
            var GalaxyCount = RandomGen.Next(4, 12);

            Console.WriteLine($"Diego Garcia, 2020, Version: {Version}");
            await Task.Delay(Delay);

            Console.WriteLine("Still technically a surprise");
            await Task.Delay(Delay);

            Console.WriteLine($"The nightsky this time around has {ShootingStarCount} Shooting stars, {StarCount} Stars, {TinyStarCount} distant stars, and {GalaxyCount} spinning galaxies");
            await Task.Delay(Delay * 2);

            Console.WriteLine("All made just for you");
            await Task.Delay(Delay);

            List<IBody> Stars = new List<IBody>();
            for (int a = 0; a < ShootingStarCount; a++)
                Stars.Add(new ShootingStar(RandomGen.Next(0, WindowLeft), RandomGen.Next(1, 4), RandomGen.Next(0, StarColors.Length - 1), RandomGen.Next(0, 2)));
            for (int a = 0; a < StarCount; a++)
                Stars.Add(new Star(RandomGen.Next(0, WindowLeft), RandomGen.Next(0, WindowTop), RandomGen.Next(1, 7), RandomGen.Next(0, Star.Types), RandomGen.Next(0, StarColors.Length - 1)));
            for (int a = 0; a < TinyStarCount; a++)
                Stars.Add(new Star(RandomGen.Next(0, WindowLeft), RandomGen.Next(0, WindowTop), RandomGen.Next(1, 5), RandomGen.Next(0, StarColors.Length - 1), true));
            for (int a = 0; a < GalaxyCount; a++)
                Stars.Add(new Star(RandomGen.Next(0, WindowLeft), RandomGen.Next(0, WindowTop), RandomGen.Next(1, 3), RandomGen.Next(0, StarColors.Length - 1), RandomGen.Next(0, StarColors.Length - 1), RandomGen.Next(0, StarColors.Length - 1)));

            Console.WriteLine("Enjoy");
            await Task.Delay(Delay * 4);
            Console.WriteLine("...");
            await Task.Delay(Delay);
            Console.Clear();

            var sizecheck = Task.Delay(1000);
            while (true)
            {
                foreach (IBody s in Stars)
                {
                    s.Update();
                    s.Draw();
                }
                if (sizecheck.IsCompleted)
                {
                    Console.SetWindowPosition(0, 0);
                    Console.SetWindowSize(WindowLeft, WindowTop);
                    sizecheck = Task.Delay(500);
                }
            }

        }
    }
}