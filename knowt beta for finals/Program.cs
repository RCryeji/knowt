using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

enum NoteType
{
    Task,
    Text,
    List
}

abstract class Note
{
    public int NoteId { get; set; }
    public NoteType Type { get; set; }
    public string Content { get; set; }
    public string Deadline { get; set; }
    public List<string> ListItems { get; set; }

    public virtual string GetNotePreview()
    {
        string previewContent = Content.Length <= 18 ? Content : Content.Substring(0, 18) + "...";
        string preview = $"{NoteId}\t{Type}\t{previewContent}\t";
        if (!string.IsNullOrEmpty(Deadline))
        {
            preview += Deadline;
        }
        return preview;
    }
}

class TextNote : Note
{
    public string AdditionalInfo { get; set; }

    public override string GetNotePreview()
    {
        string previewContent = Content.Length <= 18 ? Content : Content.Substring(0, 18) + "...";
        string preview = $"({NoteId}) : {previewContent}";
        if (!string.IsNullOrEmpty(Deadline))
        {
            preview += $", Deadline: {Deadline}";
        }
        if (!string.IsNullOrEmpty(AdditionalInfo))
        {
            preview += $"  ||  Additional Info: [{AdditionalInfo}]";
        }
        return preview;
    }
}

class TaskNote : Note
{
    public bool IsCompleted { get; set; }
    public bool IsPastDeadline { get; set; }

    public override string GetNotePreview()
    {
        string previewContent = Content.Length <= 18 ? Content : Content.Substring(0, 18) + "...";
        string preview = $"({NoteId}) : {previewContent}";
        if (!string.IsNullOrEmpty(Deadline))
        {
            preview += $"  ||  Deadline: [{Deadline}]";
        }
        preview += IsPastDeadline ? " -> (Past Deadline)" : (IsCompleted ? " -> (Completed)" : " -> (Incomplete)");
        return preview;
    }
}

class ListNote : Note
{
    private List<string> listItems;

    public ListNote()
    {
        listItems = new List<string>();
    }

    public void AddListItem(string item)
    {
        listItems.Add(item);
    }

    public void ClearListItems()
    {
        listItems.Clear();
    }

    public List<string> GetListItems()
    {
        return listItems;
    }

    public override string GetNotePreview()
    {
        string previewContent = Content.Length <= 18 ? Content : Content.Substring(0, 18) + "...";
        string preview = $"({NoteId}) : {previewContent}";
        preview += listItems.Count > 0 ? "  ||  First Item: " + listItems.First() : "  ||  (No items)";
        return preview;
    }
}

class ProgramSettings
{
    public void Menu()
    {
        bool backToMainMenu = false;

        while (!backToMainMenu)
        {
            Console.Clear();
            Console.WriteLine("\t[Program Settings]");
            Console.WriteLine("[1] Change Font Color");
            Console.WriteLine("[2] Back to Main Menu");

            Console.Write("Enter your choice: ");
            string settingsChoice = Console.ReadLine();

            switch (settingsChoice)
            {
                case "1":
                    ChangeFontColor();
                    break;

                case "2":
                    backToMainMenu = true;
                    break;

                default:
                    Console.WriteLine("Invalid choice. Please enter a valid option.");
                    break;
            }
        }
    }

    public void ChangeFontColor()
    {
        Console.Clear();
        Console.WriteLine("\t[Change Font Color]");
        Console.WriteLine("[1] Red");
        Console.WriteLine("[2] Green");
        Console.WriteLine("[3] White");
        Console.WriteLine("[4] Blue");

        Console.Write("Enter your choice: ");
        if (int.TryParse(Console.ReadLine(), out int colorChoice) && colorChoice >= 1 && colorChoice <= 4)
        {
            ConsoleColor[] colors = { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.White, ConsoleColor.Blue };
            Console.ForegroundColor = colors[colorChoice - 1];
            Console.WriteLine("Font color changed successfully.");
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter a valid option.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}

class NoteManager
{
    private List<Note> notes = new List<Note>();
    private int noteIdCounter = 1;

    public void AddNote()
    {
        try
        {
            Console.WriteLine("\n\t[Add Note]");
            Console.Write("Enter the type of note (Task/Text/List): ");
            string noteTypeInput = Console.ReadLine();

            if (Enum.TryParse(noteTypeInput, true, out NoteType type))
            {
                Console.Write("Enter the note content: ");
                string content = Console.ReadLine();

                if (type == NoteType.Task)
                {
                    Console.Write("Do you want to add a deadline? (Y/N): ");
                    string addDeadlineChoice = Console.ReadLine();
                    if (addDeadlineChoice.Trim().ToUpper() == "Y")
                    {
                        Console.Write("Enter the deadline: ");
                        string deadline = Console.ReadLine();
                        Console.Write("Is the task completed? (Y/N): ");
                        string isCompletedChoice = Console.ReadLine();
                        bool isCompleted = isCompletedChoice.Trim().ToUpper() == "Y";

                        Note newNote = new TaskNote
                        {
                            NoteId = noteIdCounter++,
                            Type = type,
                            Content = content,
                            Deadline = deadline,
                            IsCompleted = isCompleted
                        };

                        notes.Add(newNote);
                        Console.WriteLine("Task Note added successfully.");
                    }
                    else
                    {
                        Note newNote = new TaskNote
                        {
                            NoteId = noteIdCounter++,
                            Type = type,
                            Content = content
                        };

                        notes.Add(newNote);
                        Console.WriteLine("Task Note added successfully.");
                    }
                }
                else if (type == NoteType.Text)
                {
                    Console.Write("Do you want to add additional info? (Y/N): ");
                    string addAdditionalInfoChoice = Console.ReadLine();
                    if (addAdditionalInfoChoice.Trim().ToUpper() == "Y")
                    {
                        Console.Write("Enter additional info: ");
                        string additionalInfo = Console.ReadLine();

                        Note newNote = new TextNote
                        {
                            NoteId = noteIdCounter++,
                            Type = type,
                            Content = content,
                            AdditionalInfo = additionalInfo
                        };

                        notes.Add(newNote);
                        Console.WriteLine("Text Note added successfully.");
                    }
                    else
                    {
                        Note newNote = new TextNote
                        {
                            NoteId = noteIdCounter++,
                            Type = type,
                            Content = content
                        };

                        notes.Add(newNote);
                        Console.WriteLine("Text Note added successfully.");
                    }
                }
                else if (type == NoteType.List)
                {
                    Console.Write("Enter the list name: ");
                    string listName = Console.ReadLine();

                    List<string> listItems = new List<string>();

                    Console.WriteLine("Enter the list items. Type '/s' on a new line to save and exit.");
                    string listItem;
                    do
                    {
                        Console.Write(" - ");
                        listItem = Console.ReadLine();
                        if (listItem.Trim() != "/s")
                        {
                            listItems.Add(listItem);
                        }
                    } while (listItem.Trim() != "/s");

                    ListNote newNote = new ListNote
                    {
                        NoteId = noteIdCounter++,
                        Type = type,
                        Content = $"{listName} (List)"
                    };

                    // Use the AddListItem method to update the list items
                    foreach (var item in listItems)
                    {
                        newNote.AddListItem(item);
                    }

                    notes.Add(newNote);
                    Console.WriteLine("List Note added successfully.");
                }
            }
            else
            {
                Console.WriteLine("Invalid note type. Use 'Task', 'Text', or 'List'.");
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error adding note: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }


    public void UpdateNote()
    {
        Console.WriteLine("\n\t[Update Note]");
        if (notes.Count == 0)
        {
            Console.WriteLine("No notes to update.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter the type of note to update (Task/Text/List): ");
        string noteTypeInput = Console.ReadLine();

        if (Enum.TryParse(noteTypeInput, true, out NoteType type))
        {
            Console.Write("Enter the note ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int noteId))
            {
                Note noteToUpdate = notes.Find(note => note.NoteId == noteId && note.Type == type);
                if (noteToUpdate != null)
                {
                    Console.WriteLine("Choose what to update:");
                    Console.WriteLine("[1] Update Content");

                    if (type == NoteType.Task)
                    {
                        Console.WriteLine("[2] Update Deadline");
                        Console.WriteLine("[3] Update Completion Status");
                    }
                    else if (type == NoteType.Text)
                    {
                        Console.WriteLine("[2] Update Additional Information");
                    }
                    else if (type == NoteType.List)
                    {
                        Console.WriteLine("[2] Update List Items");
                    }

                    Console.Write("Enter your choice: ");
                    if (int.TryParse(Console.ReadLine(), out int updateChoice))
                    {
                        switch (updateChoice)
                        {
                            case 1:
                                Console.Write("Enter the updated content: ");
                                noteToUpdate.Content = Console.ReadLine();
                                Console.WriteLine("Note updated successfully.");
                                break;

                            case 2 when type == NoteType.Task:
                                if (type == NoteType.Task)
                                {
                                    Console.Write("Enter the updated deadline: ");
                                    noteToUpdate.Deadline = Console.ReadLine();
                                    Console.WriteLine("Deadline updated successfully.");
                                }
                                else if (type == NoteType.Text)
                                {
                                    Console.Write("Enter the updated additional info: ");
                                    (noteToUpdate as TextNote).AdditionalInfo = Console.ReadLine();
                                    Console.WriteLine("Additional Information updated successfully.");
                                }
                                break;

                            case 3:
                                if (type == NoteType.Task)
                                {
                                    Console.Write("Is the task completed? (Y/N): ");
                                    string isCompletedChoice = Console.ReadLine();
                                    (noteToUpdate as TaskNote).IsCompleted = isCompletedChoice.Trim().ToUpper() == "Y";
                                    Console.WriteLine("Completion Status updated successfully.");
                                }
                                break;
                            case 2 when type == NoteType.List:
                                ListNote listNoteToUpdate = noteToUpdate as ListNote;
                                UpdateListItems(listNoteToUpdate);
                                Console.WriteLine("List Note updated successfully.");
                                break;

                            default:
                                Console.WriteLine("Invalid choice. Please enter a valid option.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter a valid option.");
                    }
                }
                else
                {
                    Console.WriteLine("Note not found with the specified ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid note ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid note type. Use 'Task', 'Text', or 'List'.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    private void UpdateListItems(ListNote listNoteToUpdate)
    {
        Console.WriteLine("Enter new items for the list (press Enter to add a new item, type '/s' to save and exit):");
        listNoteToUpdate.ClearListItems();

        string listItem;
        do
        {
            listItem = Console.ReadLine();
            if (listItem != "/s")
            {
                listNoteToUpdate.AddListItem(listItem);
            }
        } while (listItem != "/s");
    }

    public void RemoveNote()
    {
        Console.WriteLine("\n\t[Remove Note]");
        if (notes.Count == 0)
        {
            Console.WriteLine("No notes to remove.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter the type of note to remove (Task/Text/List): ");
        string noteTypeInput = Console.ReadLine();

        if (Enum.TryParse(noteTypeInput, true, out NoteType type))
        {
            Console.Write("Enter the note ID to remove: ");
            if (int.TryParse(Console.ReadLine(), out int noteId))
            {
                Note noteToRemove = notes.Find(note => note.NoteId == noteId && note.Type == type);
                if (noteToRemove != null)
                {
                    notes.Remove(noteToRemove);
                    Console.WriteLine("Note removed successfully.");
                }
                else
                {
                    Console.WriteLine("Note not found with the specified ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid note ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid note type. Use 'Task', 'Text', or 'List'.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public void ViewFullNote()
    {
        Console.WriteLine("\n\t[View Full Note]");
        if (notes.Count == 0)
        {
            Console.WriteLine("No notes available to view.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter the type of note to view (Task/Text/List): ");
        string noteTypeInput = Console.ReadLine();

        if (Enum.TryParse(noteTypeInput, true, out NoteType type))
        {
            Console.Write("Enter the note ID to view: ");
            if (int.TryParse(Console.ReadLine(), out int noteId))
            {
                Note noteToView = notes.Find(note => note.NoteId == noteId && note.Type == type);
                if (noteToView != null)
                {
                    Console.WriteLine("\n\t [Viewing Full Note Content]");
                    Console.WriteLine($"ID: {noteToView.NoteId}\n");
                    Console.WriteLine($"Type: {noteToView.Type}\n");
                    Console.WriteLine($"Content: {noteToView.Content} \n");
                    if (type == NoteType.Task)
                    {
                        Console.WriteLine($"Deadline: {noteToView.Deadline} \n");
                        Console.WriteLine($"Completion Status: {(noteToView as TaskNote).IsCompleted}");
                    }
                    else if (type == NoteType.Text)
                    {
                        Console.WriteLine($"Additional Info: {(noteToView as TextNote).AdditionalInfo}");
                    }
                    else if (type == NoteType.List)
                    {
                        Console.WriteLine("List Items:");
                        foreach (var item in (noteToView as ListNote).GetListItems())
                        {
                            Console.WriteLine($"- {item}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Note not found with the specified ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid note ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid note type. Use 'Task', 'Text', or 'List'.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public void CleanCompletedTaskNotes()
    {
        Console.WriteLine("\n\t[Clean Completed Task Notes]");
        if (notes.Count == 0)
        {
            Console.WriteLine("No task notes available to clean.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return;
        }

        List<Note> completedTaskNotes = notes
            .Where(note => note.Type == NoteType.Task && (note as TaskNote).IsCompleted)
            .ToList();

        if (completedTaskNotes.Count > 0)
        {
            Console.WriteLine("The following completed task notes will be removed:");
            Console.WriteLine("ID\tContent");
            foreach (var note in completedTaskNotes)
            {
                Console.WriteLine($"{note.NoteId}\t{note.Content}");
            }

            Console.Write("Do you want to proceed? Removed Tasks cannot me restored. (Y/N): ");
            string confirmation = Console.ReadLine();
            if (confirmation.Trim().ToUpper() == "Y")
            {
                notes.RemoveAll(note => note.Type == NoteType.Task && (note as TaskNote).IsCompleted);
                Console.WriteLine("Completed task notes removed successfully.");
            }
            else
            {
                Console.WriteLine("Clean operation canceled.");
            }
        }
        else
        {
            Console.WriteLine("No completed task notes found to clean.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public void SaveNotesToFile(string knowt)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(knowt))
            {
                foreach (var note in notes)
                {
                    string noteLine = "";
                    switch (note.Type)
                    {
                        case NoteType.Task:
                            var taskNote = note as TaskNote;
                            noteLine = $"{note.NoteId},{note.Type},{note.Content},{note.Deadline},{taskNote.IsCompleted},{(note is TextNote ? (note as TextNote).AdditionalInfo : "")}";
                            break;
                        case NoteType.Text:
                            noteLine = $"{note.NoteId},{note.Type},{note.Content},{note.Deadline},,{(note as TextNote).AdditionalInfo}";
                            break;
                        case NoteType.List:
                            var listNote = note as ListNote;
                            string listItems = string.Join(",", listNote.GetListItems());
                            noteLine = $"{note.NoteId},{note.Type},{note.Content},{note.Deadline},,{listItems}";
                            break;
                    }
                    writer.WriteLine(noteLine);
                }
            }

            Console.WriteLine("Notes saved to file successfully.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error saving notes to file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    public void LoadNotesFromFile(string knowt)
    {
        try
        {
            notes.Clear();
            int maxNoteId = 0;

            using (StreamReader reader = new StreamReader(knowt))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length >= 3)
                    {
                        NoteType type;
                        if (Enum.TryParse(parts[1], out type))
                        {
                            switch (type)
                            {
                                case NoteType.Task:
                                    bool isCompleted = false;
                                    if (bool.TryParse(parts[4], out isCompleted))
                                    {
                                        var taskNote = new TaskNote
                                        {
                                            NoteId = int.Parse(parts[0]),
                                            Type = type,
                                            Content = parts[2],
                                            Deadline = parts[3],
                                            IsCompleted = isCompleted
                                        };
                                        notes.Add(taskNote);
                                        maxNoteId = Math.Max(maxNoteId, taskNote.NoteId);
                                    }
                                    break;
                                case NoteType.Text:
                                    var textNote = new TextNote
                                    {
                                        NoteId = int.Parse(parts[0]),
                                        Type = type,
                                        Content = parts[2],
                                        Deadline = parts[3],
                                        AdditionalInfo = parts[5]
                                    };
                                    notes.Add(textNote);
                                    maxNoteId = Math.Max(maxNoteId, textNote.NoteId);
                                    break;
                                case NoteType.List:
                                    var listNote = new ListNote
                                    {
                                        NoteId = int.Parse(parts[0]),
                                        Type = type,
                                        Content = parts[2],
                                        Deadline = parts[3],
                                    };
                                    string[] listItems = parts[5].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var item in listItems)
                                    {
                                        listNote.AddListItem(item);
                                    }
                                    notes.Add(listNote);
                                    maxNoteId = Math.Max(maxNoteId, listNote.NoteId);
                                    break;
                            }
                        }
                    }
                }
                noteIdCounter = maxNoteId + 1;
            }

            Console.WriteLine("Notes loaded from file successfully.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error loading notes from file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }


    public void CheckPastDeadlines()
    {
        try
        {
            foreach (var note in notes)
            {
                if (note.Type == NoteType.Task && !string.IsNullOrEmpty(note.Deadline))
                {
                    DateTime deadlineDate;
                    if (DateTime.TryParseExact(note.Deadline, "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out deadlineDate))
                    {
                        if (!(((TaskNote)note).IsCompleted) && deadlineDate < DateTime.Now)
                        {
                            // The deadline has passed
                            ((TaskNote)note).IsPastDeadline = true; // Mark the task as past deadline
                        }
                        else
                        {
                            // The task is completed or the deadline hasn't passed
                            ((TaskNote)note).IsPastDeadline = false;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    public void DisplayNotesPreview()
    {
        try
        {
            Console.WriteLine("DATE:" + DateTime.Now.ToString("MM-dd-yyyy"));
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║  _  __  __  _    __    _   _   _____ ║");
            Console.WriteLine("║ | |/ / |  \\| |  /__\\  | | | | |_   _|║");
            Console.WriteLine("║ |   <  | | ' | | \\/ | | 'V' |   | |  ║");
            Console.WriteLine("║ |_|\\_\\ |_|\\__|  \\__/  !_/ \\_!   |_|  ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Thread.Sleep(55);
            Console.ResetColor();
            Console.WriteLine("NOTES:");
            Thread.Sleep(55);
            Console.WriteLine("(ID)\tContent");
            Thread.Sleep(55);

            CheckPastDeadlines(); // Check and update "Past Deadline" flags

            if (notes.Count == 0)
            {
                Console.WriteLine("No notes available.");
            }
            else
            {
                // Separate notes by type
                var textNotes = notes.OfType<TextNote>().ToList();
                var taskNotes = notes.OfType<TaskNote>().ToList();
                var listNotes = notes.OfType<ListNote>().ToList();

                // Display TextNotes
                if (textNotes.Count > 0)
                {
                    Console.WriteLine("\nText Notes:");
                    foreach (var textNote in textNotes)
                    {
                        Console.WriteLine(" |");
                        Console.WriteLine(textNote.GetNotePreview());
                    }
                }

                // Display TaskNotes
                if (taskNotes.Count > 0)
                {
                    Console.WriteLine("\nTask Notes:");
                    foreach (var taskNote in taskNotes)
                    {
                        Console.WriteLine(" |");
                        Console.WriteLine(taskNote.GetNotePreview());
                    }
                }

                // Display ListNotes
                if (listNotes.Count > 0)
                {
                    Console.WriteLine("\nList Notes:");
                    foreach (var listNote in listNotes)
                    {
                        Console.WriteLine(" |");
                        Console.WriteLine(listNote.GetNotePreview());
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}

class Program
{
    public static void Main()
    {
        NoteManager noteManager = new NoteManager();
        ProgramSettings programSettings = new ProgramSettings();

        noteManager.LoadNotesFromFile("knowt.txt");

        bool exitProgram = false;

        while (!exitProgram)
        {
            Console.Clear();
            noteManager.DisplayNotesPreview();

            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║           Operations           ║");
            Console.WriteLine("║ [1] Add Note                   ║");
            Console.WriteLine("║ [2] Update Note                ║");
            Console.WriteLine("║ [3] Remove Note                ║");
            Console.WriteLine("║ [4] View Full Note             ║");
            Console.WriteLine("║ [5] Clean Completed Task Notes ║");
            Console.WriteLine("║ [6] Program Settings           ║");
            Console.WriteLine("║ [7] Exit                       ║");
            Console.WriteLine("╚════════════════════════════════╝");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    noteManager.AddNote();
                    break;
                case "2":
                    noteManager.UpdateNote();
                    break;

                case "3":
                    noteManager.RemoveNote();
                    break;

                case "4":
                    noteManager.ViewFullNote();
                    break;

                case "5":
                    noteManager.CleanCompletedTaskNotes();
                    break;

                case "6":
                    programSettings.Menu();
                    break;

                case "7":
                    noteManager.SaveNotesToFile("knowt.txt");
                    exitProgram = true;
                    break;

                default:
                    Console.WriteLine("Invalid operation ID. Please enter a valid ID.");
                    break;
            }
        }
    }
}