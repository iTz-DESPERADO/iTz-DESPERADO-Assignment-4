namespace AcademyScheduleAnalyzer;

internal partial class Program
{
    public static void Main()
    {
        int selected = 0;
        while (true)
        {
            selected = ShowMenu("Academy Schedule Analyzer",
                ["View Sessions", "Analyze Sessions", "Statistics", "Reports", "Array Methods", "Parameter Examples", "Exit"],
                selected, "Up/Down: Move   Enter: Open   Escape: Exit");
            switch (selected)
            {
                case 0:
                    ShowOperations("View Sessions",
                        ("Display all sessions", () => ShowScreen("All Sessions", DisplayAllSessions)),
                        ("Search for a session", () => WithSessionName("Search for a Session", SearchSession)),
                        ("Sort session names", () => ShowScreen("Sorted Session Names", SortSessionNames)),
                        ("Reverse session names", () => ShowScreen("Reversed Session Names", ReverseSessions)),
                        ("Find session index", () => WithSessionName("Find Session Index", SessionIndex)),
                        ("Check if session exists", () => WithSessionName("Check If Session Exists", IsSessionExists)),
                        ("Select session by index", SelectSessionByIndex));
                    break;
                case 1:
                    ShowOperations("Analyze Sessions",
                        ("Session date details and formats", () => WithSessionName("Session Date Details", DisplaySessionDetails)),
                        ("Past and upcoming sessions", () => ShowScreen("Past and Upcoming Sessions", ShowPastAndUpcomingSessions)),
                        ("Find next session", () => ShowScreen("Next Session", ShowNextSession)),
                        ("Compare two session dates", CompareDates),
                        ("Read and validate a custom date", ReadCustomDate),
                        ("Validate session duration", ReadDuration));
                    break;
                case 2:
                    ShowScreen("Duration Statistics", ShowDurationStatistics);
                    break;
                case 3:
                    ShowOperations("Reports",
                        ("Generate report using string", () => ShowScreen("Report Using String",
                            () => Console.Write(BuildReportUsingString(sessionNames, sessionDates, sessionDurations)))),
                        ("Generate report using StringBuilder", () => ShowScreen("Report Using StringBuilder",
                            () => Console.Write(BuildReportUsingStringBuilder(sessionNames, sessionDates, sessionDurations)))));
                    break;
                case 4:
                    ShowOperations("Array Methods",
                        ("Find session matching text (Array.Find)", () => WithSearchText("Array.Find", FindSessionByCondition)),
                        ("Find matching index (Array.FindIndex)", () => WithSearchText("Array.FindIndex", FindIndexByCondition)),
                        ("Copy an array and change the copy", () => ShowScreen("Array.Copy", DemonstrateArrayCopy)));
                    break;
                case 5:
                    ShowOperations("Parameter Examples",
                        ("Change an integer using ref", () => ShowScreen("ref Parameter", DemonstrateRef)),
                        ("Find index and duration using out", () => WithSessionName("out Parameters", DemonstrateOut)),
                        ("Change an array without ref", () => ShowScreen("Reference Type Without ref", DemonstrateReferenceType)),
                        ("Total durations using params", () => ShowScreen("params int[]", DemonstrateParams)));
                    break;
                default:
                    Console.Clear();
                    return;
            }
        }
    }

    private static void ShowOperations(string title, params (string Label, Action Open)[] operations)
    {
        string[] labels = new string[operations.Length];
        for (int i = 0; i < operations.Length; i++)
            labels[i] = operations[i].Label;
        int selection = 0;
        while (true)
        {
            selection = ShowMenu(title, labels, selection);
            if (selection == -1)
                return;
            try { operations[selection].Open(); }
            catch (OperationCanceledException) {  }
        }
    }

    private static void WithSessionName(string title, Action<string> operation)
    {
        string? name = ReadText(title, "Enter session name:");
        if (name != null)
            ShowScreen(title, () => operation(name));
    }

    private static void WithSearchText(string title, Action<string> operation)
    {
        string? text = ReadText(title, "Enter text contained in a session name:");
        if (text != null)
            ShowScreen(title, () => operation(text));
    }

    private static void SelectSessionByIndex()
    {
        int? index = ReadNumber("Select Session by Index", $"Enter index (0 to {sessionNames.Length - 1}):");
        if (index.HasValue)
            ShowScreen("Session Details", () => DisplaySessionByIndex(index.Value));
    }

    private static void CompareDates()
    {
        string? first = ReadText("Compare Session Dates", "First session name:");
        if (first == null)
            return;
        string? second = ReadText("Compare Session Dates", "Second session name:");
        if (second != null)
            ShowScreen("Date Difference", () => CompareSessionDates(first, second));
    }

    private static void ReadCustomDate()
    {
        DateTime date = ReadSessionDate();
        ShowScreen("Valid Date", () => Console.WriteLine(date.ToString("yyyy-MM-dd HH:mm")));
    }

    private static void ReadDuration()
    {
        int? duration = ReadNumber("Validate Session Duration", "Enter duration in minutes:");
        if (duration.HasValue)
            ShowScreen("Duration Validation", () => ShowDurationValidation(duration.Value));
    }
}
