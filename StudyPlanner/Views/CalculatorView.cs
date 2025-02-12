using StringMath;
using System.Runtime.InteropServices;

namespace StudyPlanner.Views
{
    public partial class CalculatorView : UserControl
    {
        private TextBox results;
        private Dictionary<string, string> IndexCommandMap = new();
        private TableLayoutPanel tablePanel;

        private void LoadCalculatorView()
        {
            tablePanel = new TableLayoutPanel();
            results = new TextBox();
            results.Text = "0.0";
            results.Font = new Font("Arial", 14, FontStyle.Bold);
            results.TextAlign = HorizontalAlignment.Right; // Aligns text to the right
            results.Dock = DockStyle.Top;
            results.ReadOnly = true;
            results.GotFocus += (s1, e1) => { HideCaret(results.Handle); };


            this.Resize += (s, e) =>
            {
                results.Width = (int)(this.Width * 0.8);
            };


            tablePanel.Dock = DockStyle.Fill;
            tablePanel.RowCount = 5;
            tablePanel.ColumnCount = 4;
            tablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single; //
            tablePanel.AutoSize= true;

            for (int col = 0; col < tablePanel.ColumnCount; col++)
            {
                tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / tablePanel.ColumnCount));
            }

            for (int row = 0; row < tablePanel.RowCount; row++)
            {
                tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / tablePanel.RowCount));
            }

            for (int row = 0; row < tablePanel.RowCount; row++)
            {
                for (int col = 0; col < tablePanel.ColumnCount; col++)
                {
                    string command = IndexCommandMap[$"{row},{col}"];
                    Label cell = new Label
                    {      
                        Text = command,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.LightGray,
                        Cursor = Cursors.Hand
                    }; 

                    cell.MouseEnter += (sender, args) =>
                    {
                        cell.BackColor = Color.FromArgb(178, 190, 181);
                    };

                    cell.MouseLeave += (sender, args) =>
                    {
                        cell.BackColor = Color.LightGray;
                    };

                    cell.Click += (sender, args) =>
                    {
                        string command = ((Control)sender!).Text;
                        ExecuteCommand(command);
                    };
                    if(col == 0 && row == tablePanel.RowCount - 1)
                    {
                        tablePanel.Controls.Add(cell, col, row);
                        tablePanel.SetColumnSpan(cell ,2);
                    }
                    if(!(col == 1 && row == tablePanel.RowCount - 1))
                    {
                        tablePanel.Controls.Add(cell, col, row);
                    }

                }
            }



            // main panel
            this.Dock = DockStyle.Fill;
            Controls.Add(tablePanel);
            Controls.Add(results);  
        }

        public CalculatorView()
        {
            IndexCommandMap.Add("0,0", "AC");
            IndexCommandMap.Add("0,1", "+/-");
            IndexCommandMap.Add("0,2", "%");
            IndexCommandMap.Add("0,3", "/");

            IndexCommandMap.Add("1,0", "7");
            IndexCommandMap.Add("1,1", "8");
            IndexCommandMap.Add("1,2", "9");
            IndexCommandMap.Add("1,3", "x");

            IndexCommandMap.Add("2,0", "4");
            IndexCommandMap.Add("2,1", "5");
            IndexCommandMap.Add("2,2", "6");
            IndexCommandMap.Add("2,3", "-");

            IndexCommandMap.Add("3,0", "1");
            IndexCommandMap.Add("3,1", "2");
            IndexCommandMap.Add("3,2", "3");
            IndexCommandMap.Add("3,3", "+");

            IndexCommandMap.Add("4,0", "0");
            IndexCommandMap.Add("4,1", "0");
            IndexCommandMap.Add("4,2", ".");
            IndexCommandMap.Add("4,3", "=");


            InitializeComponent();
            this.Load += (sender, args) => LoadCalculatorView();

            this.Resize += (sender, args) =>
            {
                // Ensure sender is a Control and tablePanel/results are not null
                if (sender is Control parentControl && tablePanel != null && results != null)
                {
                    if (parentControl.Width < 350)
                    {
                        tablePanel.Hide();
                        results.Hide();
                    }
                    else
                    {
                        tablePanel.Show();
                        results.Show();
                    }
                }
            };

        }

        public void ExecuteCommand(string command)
        {
            string[] operations = ["/", "x", "-", "+", "%"];

            if (CheckNumeric(command))
            {
                if(CheckNumeric(results.Text) && Convert.ToDouble(results.Text) == 0)
                {
                    results.Text = "";
                }
                results.Text += command;
            }
            else if(command == ".")
            {
                results.Text += ".";
            }
            else if(command == "AC")
            {
                results.Text = "";
            }
            else if (operations.Contains(command))
            {
                results.Text += $" {command} ";
            }
            else if (command == "+/-")
            {
                results.Text = $"-({results.Text})";
            }
            else if(command == "=")
            {
       
                try
                {
                    double calculation = results.Text.Replace("x", "*").Eval();
                    this.results.Text = calculation.ToString().Replace(',', '.');
                }catch(Exception ex)
                {
                    results.Text = "";
                    ToastNotification.ShowErrorToast("Invalid operation ...", 3000);
                }

            }
        }
        private bool CheckNumeric(string input)
        {
            return double.TryParse(input, out _);
        }

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

    }
}
