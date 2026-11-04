
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Shapes;

namespace Mobile_App;

public partial class MainPage : ContentPage
{
    private const string TaskListKey = "MyTaskList";

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        TasksContainer.Children.Clear();

        string savedTask = Preferences.Default.Get(TaskListKey, "");

        if (!String.IsNullOrWhiteSpace(savedTask))
        {
            string[] tasks = savedTask.Split('|');
            foreach (string task in tasks)
            {
                if (!String.IsNullOrWhiteSpace(task))
                {
                    CreateTaskRow(task);
                }
            }
        }
    }

    private void OnActionButtonClicked(object sender, EventArgs e)
    {
        string userText = UserInputEntry.Text;

        if (!string.IsNullOrWhiteSpace(userText) && userText != "Please Enter the task")
        {
            CreateTaskRow(userText);

            string currentSaved = Preferences.Default.Get(TaskListKey, "");

            if (string.IsNullOrEmpty(currentSaved))
                currentSaved = userText;
            else
                currentSaved += "|" + userText;

            Preferences.Default.Set(TaskListKey, currentSaved);

            UserInputEntry.Text = string.Empty;
            UserInputEntry.Placeholder = "Add task";
        }
        else
        {
            UserInputEntry.Placeholder = "Please Enter the task";
        }
    }
   
    private void CreateTaskRow(string userText)
    {
        //create the grid layout for the row
        var grid = new Microsoft.Maui.Controls.Grid
        {
           
            HorizontalOptions = LayoutOptions.Fill,
            Padding = new Thickness(15, 10),
            IsClippedToBounds = true,
            // Automatically fits the page width
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Star }, // Left side stretches
                new ColumnDefinition { Width = GridLength.Auto }  // Right side hugs the button
            }
        };
        //create the rounded row

        var rowBorder = new Border
        {
            BackgroundColor = Color.FromArgb("#2E2E2E"), // Matches your dark theme background (or use Colors.Transparent)
            Stroke = Color.FromArgb("#2E2E2E"),         // Match stroke to background to hide the outline
            StrokeThickness = 1,                         // MUST be at least 1 on Windows for rounding to trigger!                   
            Margin = new Thickness(15, 10),
            HorizontalOptions = LayoutOptions.Fill,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(12)      // Smooth rounded edges
            },
            Content = grid
        };
        //create the text label
        var label = new Label
        {
            Text = $"Tasks: {userText}",
            FontSize = 18,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start
        };

        //create the checkbox 
        var checkBox = new CheckBox
        {
            IsChecked = false,
                
            HorizontalOptions = LayoutOptions.End
           
        };
        //create the delete button
        var button = new Button
        {
            Text = "Remove",
            BackgroundColor = Colors.Red,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };

        // click event to remove the row
        button.Clicked += (s, args) =>
        {
            // FIXED: Remove from the task container layout
            TasksContainer.Children.Remove(grid);
            RemoveTaskFromPreferences(userText);
        };
        checkBox.CheckedChanged += (s, args) =>
            {   
                if(checkBox.IsChecked)
                {

                    grid.Background = Colors.Green;
                    
                }
                else
                {
                    grid.Background = Colors.Transparent;
                }
            };
         
        Microsoft.Maui.Controls.Grid.SetColumn(label, 0);
        //Microsoft.Maui.Controls.Grid.SetColumn(checkBox, 1);
        Microsoft.Maui.Controls.Grid.SetColumn(button, 2);

        grid.Add(label);
        grid.Add(checkBox);
        grid.Add(button);


        TasksContainer.Add(grid);
    }
    //turn row to green when the checkbox is checked
  
    private void RemoveTaskFromPreferences(string taskToRemove)
    {
        string savedTask = Preferences.Default.Get(TaskListKey, "");
        if (!string.IsNullOrWhiteSpace(savedTask))
        {
            var tasks = savedTask.Split('|').ToList();
            tasks.Remove(taskToRemove);

            string updatedSaved = string.Join("|", tasks);
            Preferences.Default.Set(TaskListKey, updatedSaved);
        }
    }
}
