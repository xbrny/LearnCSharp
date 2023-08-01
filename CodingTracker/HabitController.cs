namespace CodingTracker;

public class HabitController
{
    private readonly HabitView _habitView;
    private readonly HabitDb _habitDb;
    private bool _endSession;

    public HabitController(HabitView habitView, HabitDb habitDb)
    {
        _habitView = habitView;
        _habitDb = habitDb;
    }

    public void Run()
    {
        while (!_endSession)
        {
            var selection = _habitView.ShowMenu();

            switch (selection)
            {
                case MenuOption.ViewAll:
                    ViewAllEntry();
                    break;
                case MenuOption.LogEntry:
                    LogEntry();
                    break;
                case MenuOption.DeleteEntry:
                    DeleteEntry();
                    break;
                case MenuOption.UpdateEntry:
                    UpdateEntry();
                    break;
                case MenuOption.Exit:
                    Exit();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Console.Clear();
        }
    }

    private void ViewAllEntry()
    {
        var entries = _habitDb.GetAllEntries();

        _habitView.ShowAllEntries(entries);
    }

    private void LogEntry()
    {
        var entry = _habitView.ShowCreateEntryForm();

        try
        {
            _habitDb.CreateEntry(entry);

            _habitView.ShowSuccessfulMessage();
        }
        catch (Exception e)
        {
            _habitView.ShowErrorMessage(e);
        }
    }

    private void DeleteEntry()
    {
        var entries = _habitDb.GetAllEntries();

        var toDelete = _habitView.PromptDeleteEntry(entries);

        if (toDelete == null) return;

        try
        {
            _habitDb.DeleteEntry(toDelete.Id);

            _habitView.ShowSuccessfulMessage();
        }
        catch (Exception e)
        {
            _habitView.ShowErrorMessage(e);
        }
    }

    private void UpdateEntry()
    {
        var entries = _habitDb.GetAllEntries();

        var toUpdate = _habitView.PromptUpdateEntry(entries);

        if (toUpdate == null) return;

        try
        {
            _habitDb.UpdateEntry(toUpdate.Id, toUpdate);

            _habitView.ShowSuccessfulMessage();
        }
        catch (Exception e)
        {
            _habitView.ShowErrorMessage(e);
        }
    }

    private void Exit()
    {
        _endSession = true;
    }
}