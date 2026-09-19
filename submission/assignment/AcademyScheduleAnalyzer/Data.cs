using System.Globalization;
using System.Text;

namespace AcademyScheduleAnalyzer;

internal partial class Program
{
    public static string[] sessionNames =
    ["C# Basics", "Arrays", "Functions", "Date and Time", "Exception Handling"];

    public static DateTime[] sessionDates =
    [
        new(2026, 9, 10, 18, 0, 0),
        new(2026, 9, 13, 18, 0, 0),
        new(2026, 9, 17, 18, 0, 0),
        new(2026, 9, 20, 18, 0, 0),
        new(2026, 9, 24, 18, 0, 0)
    ];

    public static int[] sessionDurations = [180, 240, 180, 240, 180];

    public static int FindSessionIndex(string? sessionName)
        => Array.FindIndex(sessionNames,
            name => string.Equals(name, sessionName?.Trim(), StringComparison.OrdinalIgnoreCase));

    public static bool TryGetSession(string? sessionName, out int sessionIndex, out int sessionDuration)
    {
        sessionIndex = FindSessionIndex(sessionName);
        sessionDuration = sessionIndex < 0 ? 0 : sessionDurations[sessionIndex];
        return sessionIndex >= 0;
    }

    public static (string SessionName, DateTime Date, int Duration) GetSessionByName(string sessionName)
    {
        int index = FindSessionIndex(sessionName);
        if (index < 0)
            throw new ArgumentException("Session not found.", nameof(sessionName));
        return GetSessionByIndex(index);
    }

    public static (string SessionName, DateTime Date, int Duration) GetSessionByIndex(int sessionIndex)
        => (sessionNames[sessionIndex], sessionDates[sessionIndex], sessionDurations[sessionIndex]);

    public static bool IsExists(string sessionName)
        => Array.Exists(sessionNames,
            name => string.Equals(name, sessionName.Trim(), StringComparison.OrdinalIgnoreCase));

    public static void IndexOfSession(string? sessionName, out int sessionIndex)
        => sessionIndex = Array.IndexOf(sessionNames, sessionName);

    public static string[] CopySessionNames(string[] names, TableDisplayMode mode)
    {
        string[] copy = new string[names.Length];
        Array.Copy(names, copy, names.Length);
        switch (mode)
        {
            case TableDisplayMode.Sorted:
                Array.Sort(copy, StringComparer.OrdinalIgnoreCase);
                break;
            case TableDisplayMode.Reversed:
                Array.Reverse(copy);
                break;
        }
        return copy;
    }

    public static int GetTotalDuration(int[] durations)
    {
        int total = 0;
        foreach (int duration in durations)
            total = checked(total + duration);
        return total;
    }

    public static double GetAverageDuration(int[] durations)
    {
        RequireDurations(durations);
        return (double)GetTotalDuration(durations) / durations.Length;
    }

    public static int GetShortestDuration(int[] durations)
    {
        RequireDurations(durations);
        int shortest = durations[0];
        foreach (int duration in durations)
            if (duration < shortest)
                shortest = duration;
        return shortest;
    }

    public static int GetLongestDuration(int[] durations)
    {
        RequireDurations(durations);
        int longest = durations[0];
        foreach (int duration in durations)
            if (duration > longest)
                longest = duration;
        return longest;
    }

    private static void RequireDurations(int[] durations)
    {
        if (durations.Length == 0)
            throw new ArgumentException("At least one duration is required.", nameof(durations));
    }

    public static int[] GetSortedDurations(int[] durations)
    {
        int[] copy = new int[durations.Length];
        Array.Copy(durations, copy, durations.Length);
        Array.Sort(copy);
        return copy;
    }

    public static int CalculateTotalDuration(params int[] durations) => GetTotalDuration(durations);

    public static void IncreaseDuration(ref int duration) => duration += 30;

    public static void ChangeFirstSessionName(string[] names)
    {
        if (names.Length > 0)
            names[0] = "Updated session";
    }

    public static DateTime GetSessionEndTime(DateTime start, int duration) => start.AddMinutes(duration);

    public static TimeSpan GetDateDifference(DateTime first, DateTime second) => (second - first).Duration();

    public static string GetSessionStatus(DateTime date, DateTime now) => date > now ? "Upcoming" : "Past";

    public static int FindNextSessionIndex(DateTime[] dates, DateTime now)
    {
        int next = -1;
        for (int i = 0; i < dates.Length; i++)
            if (dates[i] > now && (next == -1 || dates[i] < dates[next]))
                next = i;
        return next;
    }

    public static bool TryReadSessionDate(string? text, out DateTime date)
        => DateTime.TryParseExact(text, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date);

    public static void ValidateSessionDuration(int duration)
    {
        if (duration <= 0)
            throw new ArgumentException("Duration must be greater than zero.");
    }

    private static void ValidateScheduleArrays(string[] names, DateTime[] dates, int[] durations)
    {
        if (names.Length != dates.Length || names.Length != durations.Length)
            throw new ArgumentException("The schedule arrays must have the same length.");
    }

    private static string FormatReportLine(string name, DateTime date, int duration)
        => $"{name} - {date.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture)} - {duration} minutes";

    public static string BuildReportUsingString(string[] names, DateTime[] dates, int[] durations)
    {
        ValidateScheduleArrays(names, dates, durations);
        string result = "";
        for (int i = 0; i < names.Length; i++)
            result += FormatReportLine(names[i], dates[i], durations[i]) + Environment.NewLine;
        return result;
    }

    public static string BuildReportUsingStringBuilder(string[] names, DateTime[] dates, int[] durations)
    {
        ValidateScheduleArrays(names, dates, durations);
        var result = new StringBuilder();
        for (int i = 0; i < names.Length; i++)
            result.AppendLine(FormatReportLine(names[i], dates[i], durations[i]));
        return result.ToString();
    }
}
