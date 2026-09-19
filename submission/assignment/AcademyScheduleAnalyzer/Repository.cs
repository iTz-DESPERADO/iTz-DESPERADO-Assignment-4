using System.Globalization;

namespace AcademyScheduleAnalyzer;

internal partial class Program
{
    public static void DisplayAllSessions() => ShowSessionsTable(sessionNames, sessionDates, sessionDurations);

    public static void SearchSession(string sessionName)
    {
        if (!TryGetSession(sessionName, out int index, out _))
        {
            Console.WriteLine("Session not found.");
            return;
        }
        ShowSessionsTable([sessionNames[index]], [sessionDates[index]], [sessionDurations[index]]);
    }

    public static void SortSessionNames() => DisplayNames(CopySessionNames(sessionNames, TableDisplayMode.Sorted));
    public static void ReverseSessions() => DisplayNames(CopySessionNames(sessionNames, TableDisplayMode.Reversed));

    private static void DisplayNames(string[] names)
    {
        foreach (string name in names)
            Console.WriteLine(name);
    }

    public static void SessionIndex(string sessionName)
    {
        IndexOfSession(sessionName, out int index);
        Console.WriteLine(index < 0 ? "Session not found." : $"Index: {index}");
    }

    public static void IsSessionExists(string sessionName)
        => Console.WriteLine(IsExists(sessionName) ? "Session exists." : "Session does not exist.");

    public static void DisplaySessionByIndex(int index)
    {
        try
        {
            var session = GetSessionByIndex(index);
            ShowSessionsTable([session.SessionName], [session.Date], [session.Duration]);
        }
        catch (IndexOutOfRangeException)
        {
            Console.WriteLine("The selected session index is out of range.");
        }
        finally
        {
            Console.WriteLine("Input operation finished.");
        }
    }

    public static void ShowDurationStatistics()
    {
        Console.WriteLine($"Total Duration: {GetTotalDuration(sessionDurations)} minutes");
        Console.WriteLine($"Average Duration: {GetAverageDuration(sessionDurations):0.##} minutes");
        Console.WriteLine($"Shortest Duration: {GetShortestDuration(sessionDurations)} minutes");
        Console.WriteLine($"Longest Duration: {GetLongestDuration(sessionDurations)} minutes");
        Console.WriteLine($"Sorted durations: {string.Join(", ", GetSortedDurations(sessionDurations))}");
    }

    public static void DisplaySessionDetails(string name)
    {
        if (!TryGetSession(name, out int index, out int duration))
        {
            Console.WriteLine("Session not found.");
            return;
        }
        DateTime date = sessionDates[index];
        Console.WriteLine($"Session: {sessionNames[index]}");
        Console.WriteLine($"Date: {date.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Day: {date.DayOfWeek}");
        Console.WriteLine($"Year: {date.Year}");
        Console.WriteLine($"Month: {date.Month}");
        Console.WriteLine($"Day Number: {date.Day}");
        Console.WriteLine($"Start Time: {date.ToString("hh:mm tt", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Duration: {duration} minutes");
        Console.WriteLine($"End Time: {GetSessionEndTime(date, duration).ToString("hh:mm tt", CultureInfo.InvariantCulture)}");
        Console.WriteLine();
        Console.WriteLine("Date and time formats:");
        string[] formats = ["yyyy-MM-dd", "dd/MM/yyyy", "dd MMMM yyyy", "dddd, dd MMMM yyyy", "hh:mm tt"];
        foreach (string format in formats)
            Console.WriteLine(date.ToString(format, CultureInfo.InvariantCulture));
    }

    public static void ShowPastAndUpcomingSessions()
    {
        DateTime now = DateTime.Now;
        for (int index = 0; index < sessionNames.Length; index++)
            Console.WriteLine($"{sessionNames[index],-22} {GetSessionStatus(sessionDates[index], now)}");
    }

    public static void ShowNextSession()
    {
        DateTime now = DateTime.Now;
        int index = FindNextSessionIndex(sessionDates, now);
        if (index < 0)
        {
            Console.WriteLine("No upcoming sessions.");
            return;
        }
        TimeSpan remaining = sessionDates[index] - now;
        Console.WriteLine($"Next Session: {sessionNames[index]}");
        Console.WriteLine(sessionDates[index].ToString("dd MMMM yyyy hh:mm tt", CultureInfo.InvariantCulture));
        Console.WriteLine($"Time Remaining: {remaining.Days} days, {remaining.Hours} hours, {remaining.Minutes} minutes");
    }

    public static void CompareSessionDates(string first, string second)
    {
        if (!TryGetSession(first, out int firstIndex, out _) ||
            !TryGetSession(second, out int secondIndex, out _))
        {
            Console.WriteLine("Session not found.");
            return;
        }
        TimeSpan difference = GetDateDifference(sessionDates[firstIndex], sessionDates[secondIndex]);
        Console.WriteLine($"First Session: {sessionNames[firstIndex]}");
        Console.WriteLine($"Second Session: {sessionNames[secondIndex]}");
        Console.WriteLine($"Difference: {difference.TotalDays:0.##} days");
        Console.WriteLine($"Difference: {difference.TotalHours:0.##} hours");
    }

    public static DateTime ReadSessionDate()
    {
        string prompt = "Enter date (yyyy-MM-dd HH:mm):";
        while (true)
        {
            string? input = ReadText("Read and Validate a Date", prompt);
            if (input == null)
                throw new OperationCanceledException();
            if (TryReadSessionDate(input, out DateTime date))
                return date;
            prompt = "Invalid date. Use yyyy-MM-dd HH:mm (example: 2026-10-15 18:30):";
        }
    }

    public static void ShowDurationValidation(int duration)
    {
        try
        {
            ValidateSessionDuration(duration);
            Console.WriteLine("Duration accepted.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.WriteLine("Input operation finished.");
        }
    }

    public static void FindSessionByCondition(string text)
    {
        string? found = Array.Find(sessionNames,
            name => name.Contains(text, StringComparison.OrdinalIgnoreCase));
        Console.WriteLine(found == null ? "Session not found." : $"Matched session: {found}");
    }

    public static void FindIndexByCondition(string text)
    {
        int index = Array.FindIndex(sessionNames,
            name => name.Contains(text, StringComparison.OrdinalIgnoreCase));
        Console.WriteLine($"Index: {index}");
    }

    public static void DemonstrateArrayCopy()
    {
        string[] copy = new string[sessionNames.Length];
        Array.Copy(sessionNames, copy, sessionNames.Length);
        copy[0] = "Changed only in the copy";
        Console.WriteLine("Original array:");
        DisplayNames(sessionNames);
        Console.WriteLine("\nCopied array:");
        DisplayNames(copy);
    }

    public static void DemonstrateRef()
    {
        int duration = sessionDurations[0];
        Console.WriteLine($"Before: {duration}");
        IncreaseDuration(ref duration);
        Console.WriteLine($"After: {duration}");
    }

    public static void DemonstrateOut(string name)
    {
        if (TryGetSession(name, out int index, out int duration))
        {
            Console.WriteLine($"Index: {index}");
            Console.WriteLine($"Duration: {duration} minutes");
        }
        else
            Console.WriteLine("Session not found.");
    }

    public static void DemonstrateReferenceType()
    {
        string[] example = CopySessionNames(sessionNames, TableDisplayMode.Original);
        Console.WriteLine("Before:");
        DisplayNames(example);
        ChangeFirstSessionName(example);
        Console.WriteLine("\nAfter passing the array without ref:");
        DisplayNames(example);
    }

    public static void DemonstrateParams()
    {
        Console.WriteLine($"(120, 180): {CalculateTotalDuration(120, 180)} minutes");
        Console.WriteLine($"(120, 180, 240): {CalculateTotalDuration(120, 180, 240)} minutes");
        Console.WriteLine($"(60, 90, 120, 180, 240): {CalculateTotalDuration(60, 90, 120, 180, 240)} minutes");
    }
}
