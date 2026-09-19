using System.Globalization;

namespace AcademyScheduleAnalyzer;


public enum TableDisplayMode
{
    Original,
    Sorted,
    Reversed
}

internal partial class Program
{
    public static int ShowMenu(string title, string[] options, int selected = 0,
        string footer = "Up/Down: Move   Enter: Open   Escape: Back")
    {
        while (true)
        {
            string[] lines = new string[options.Length];
            for (int i = 0; i < options.Length; i++)
                lines[i] = (i == selected ? "> " : "  ") + options[i];

            int firstLine = Math.Max(0, selected - ContentHeight + 1);
            ClearAndDraw(title, lines, footer, firstLine);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    selected = (selected + options.Length - 1) % options.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selected = (selected + 1) % options.Length;
                    break;
                case ConsoleKey.Enter:
                    return selected;
                case ConsoleKey.Escape:
                    return -1;
            }
        }
    }

    public static void ShowScreen(string title, Action displayContent)
    {
       
        TextWriter originalOutput = Console.Out;
        using var output = new StringWriter();
        try
        {
            Console.SetOut(output);
            displayContent();
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        string content = output.ToString().Replace("\r\n", "\n").TrimEnd('\n');
        int firstLine = 0;
        while (true)
        {
            
            string[] lines = WrapLines(content, Math.Max(1, Console.WindowWidth - 1));
            int lastStart = Math.Max(0, lines.Length - ContentHeight);
            firstLine = Math.Clamp(firstLine, 0, lastStart);
            string footer = lastStart > 0
                ? "Up/Down: Scroll   Escape: Back"
                : "Press Escape to go back.";
            ClearAndDraw(title, lines, footer, firstLine);

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.UpArrow:
                    firstLine = Math.Max(0, firstLine - 1);
                    break;
                case ConsoleKey.DownArrow:
                    firstLine = Math.Min(lastStart, firstLine + 1);
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    public static string? ReadText(string title, string prompt)
    {
        string value = string.Empty;
        while (true)
        {
            int visibleLength = Math.Max(1, Console.WindowWidth - 4);
            string visibleValue = value.Length > visibleLength ? value[^visibleLength..] : value;
            ClearAndDraw(title, [prompt, "> " + visibleValue], "Enter: Submit   Escape: Back");

            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    return null;
                case ConsoleKey.Enter:
                    return value;
                case ConsoleKey.Backspace:
                    if (value.Length > 0)
                        value = value[..^1];
                    break;
                default:
                    if (!char.IsControl(key.KeyChar))
                        value += key.KeyChar;
                    break;
            }
        }
    }

    public static int? ReadNumber(string title, string prompt)
    {
        string value = "";
        string error = "";

        while (true)
        {
            int visibleLength = Math.Max(1, Console.WindowWidth - 4);

            string visibleValue =
                value.Length > visibleLength
                    ? value[^visibleLength..]
                    : value;

            ClearAndDraw(
                title,
                [prompt, "> " + visibleValue, error],
                "Enter: Submit   Escape: Back"
            );

            ConsoleKeyInfo key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    return null;

                case ConsoleKey.Enter:
                    try
                    {
                        return int.Parse(value);
                    }
                    catch (FormatException)
                    {
                        error = "Invalid input. Enter a whole number.";
                    }
                    catch (OverflowException)
                    {
                        error = $"Enter a number from {int.MinValue} to {int.MaxValue}.";
                    }
                    break;

                case ConsoleKey.Backspace:
                    if (value.Length > 0)
                        value = value[..^1];
                    error = "";
                    break;

                default:
                    if (!char.IsControl(key.KeyChar))
                    {
                        value += key.KeyChar;
                        error = "";
                    }

                    break;
            }
        }
    }

    private static int ContentHeight => Math.Max(1, Console.WindowHeight - 6);

    private static void ClearAndDraw(string title, string[] lines, string footer, int firstLine = 0)
    {
        Console.Clear();
        int width = Math.Max(1, Console.WindowWidth - 1);
        int height = Math.Max(1, Console.WindowHeight);
        int count = Math.Min(ContentHeight, lines.Length - firstLine);
        int top = Math.Max(0, (height - count - 4) / 2);
        int blockWidth = 0;
        foreach (string line in lines)
            blockWidth = Math.Max(blockWidth, Math.Min(width, line.Length));

        CenterText(title, top, width);
        int left = Math.Max(0, (width - blockWidth) / 2);
        for (int i = 0; i < count; i++)
            WriteAt(lines[firstLine + i], left, top + 2 + i, width, height);

        CenterText(footer, top + count + 3, width);
    }

    private static void CenterText(string text, int row, int width)
    {
        int left = Math.Max(0, (width - Math.Min(width, text.Length)) / 2);
        WriteAt(text, left, row, width, Math.Max(1, Console.WindowHeight));
    }

    private static void WriteAt(string text, int left, int row, int width, int height)
    {
        if (row >= height)
            return;

        int length = Math.Min(text.Length, width - left);
        Console.SetCursorPosition(left, row);
        Console.Write(text[..length]);
    }

    private static string[] WrapLines(string content, int width)
    {
        var lines = new List<string>();
        foreach (string line in content.Split('\n'))
        {
            if (line.Length == 0)
                lines.Add(string.Empty);
            else
                for (int start = 0; start < line.Length; start += width)
                    lines.Add(line.Substring(start, Math.Min(width, line.Length - start)));
        }
        return lines.ToArray();
    }

    public static void ShowSessionsTable(string[] names, DateTime[] dates, int[] durations)
    {
        ValidateScheduleArrays(names, dates, durations);
        const int nameWidth = 22;
        const int dateWidth = 14;
        const int timeWidth = 12;
        const int durationWidth = 14;
        Console.WriteLine($"┌{new string('─', nameWidth)}┬{new string('─', dateWidth)}┬{new string('─', timeWidth)}┬{new string('─', durationWidth)}┐");
        Console.WriteLine($"│{"Session Name",-nameWidth}│{"Date",-dateWidth}│{"Start Time",-timeWidth}│{"Duration",-durationWidth}│");
        Console.WriteLine($"├{new string('─', nameWidth)}┼{new string('─', dateWidth)}┼{new string('─', timeWidth)}┼{new string('─', durationWidth)}┤");
        for (int i = 0; i < names.Length; i++)
        {
            string date = dates[i].ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            string time = dates[i].ToString("hh:mm tt", CultureInfo.InvariantCulture);
            string duration = $"{durations[i]} m";
            Console.WriteLine($"│{names[i],-nameWidth}│{date,-dateWidth}│{time,-timeWidth}│{duration,-durationWidth}│");
        }
        Console.WriteLine($"└{new string('─', nameWidth)}┴{new string('─', dateWidth)}┴{new string('─', timeWidth)}┴{new string('─', durationWidth)}┘");
    }

}
