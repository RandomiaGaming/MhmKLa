using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Drawing2D;

public static class Program
{
	private const int MOUSEEVENTF_LEFTDOWN = 0x02;
	private const int MOUSEEVENTF_LEFTUP = 0x04;

	[DllImport("user32.dll")]
	private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

	public static void MouseDown()
	{
		mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
	}
	public static void MouseUp()
	{
		mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
	}
	private const int MOUSEEVENTF_WHEEL = 0x0800;
	public static void ScrollMouseWheel(int linesToScroll)
	{
		int scrollAmount = linesToScroll * 120; // Each line is typically 120 units of scroll
		mouse_event(MOUSEEVENTF_WHEEL, 0, 0, scrollAmount, 0);
	}
	public static bool enabled = false;
	public static void Main()
	{
		string[] folders = Directory.GetDirectories("D:\\Nintendo\\ROMs");
		foreach (string folder in folders)
		{
			string launchCommand;
			string romPath;

			if (ConsoleIs(folder, "NS"))
			{
				launchCommand = GenerateStartCommand("Yuzu");

				romPath = SelectFile(folder, ".xci", ".nsp");
			}
			else if (ConsoleIs(folder, "NDS"))
			{
				launchCommand = GenerateStartCommand("DeSmuMe");

				romPath = SelectFile(folder, ".nds");
			}
			else if (ConsoleIs(folder, "3DS"))
			{
				launchCommand = GenerateStartCommand("Citra", "Citra-QT.exe");

				romPath = SelectFile(folder, ".3ds", ".cia");
			}
			else if (ConsoleIs(folder, "NES", "SNES", "GB"))
			{
				launchCommand = GenerateStartCommand("Mesen");

				romPath = SelectFile(folder, ".nes", ".sfc", ".gb");
			}
			else if (ConsoleIs(folder, "GBA"))
			{
				launchCommand = GenerateStartCommand("mGBA");

				romPath = SelectFile(folder, ".gba");
			}
			else
			{
				Console.WriteLine("Invalid folder at " + folder);
				goto SkipMarker;
			}

			File.WriteAllText(Path.Combine($"D:\\Nintendo\\{new DirectoryInfo(folder).Name}.bat"), launchCommand + $" \"{romPath.Substring("D:\\Nintendo\\".Length)}\"");

		SkipMarker:;
		}

		Console.ReadLine();
	}
	public static bool ConsoleIs(string folderPath, params string[] consoles)
	{
		try
		{
			string targetConsole = folderPath;
			targetConsole = new DirectoryInfo(folderPath).Name;
			targetConsole = targetConsole.Split('-')[0];
			targetConsole = targetConsole.Substring(0, targetConsole.Length - 1);
			targetConsole = targetConsole.ToUpper();

			foreach (string console in consoles)
			{
				if (targetConsole == console.ToUpper())
				{
					return true;
				}
			}
			return false;
		}
		catch
		{
			return false;
		}
	}
	public static string SelectFile(string folderPath, params string[] extensions)
	{
		foreach (string extension in extensions)
		{
			try
			{
				return Directory.GetFiles(folderPath, "*" + extension)[0];
			}
			catch
			{

			}
		}
		throw new Exception("No file with desired extension was found.");
	}
	public static string GenerateStartCommand(string emulatorName, string executableName = null)
	{
		if (executableName is null)
		{
			return $"start \"{emulatorName}\" \"Emulators\\{emulatorName}\\{emulatorName}.exe\"";
		}
		else
		{
			return $"start \"{emulatorName}\" \"Emulators\\{emulatorName}\\{executableName}\"";
		}
	}
}