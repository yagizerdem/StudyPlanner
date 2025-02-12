using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudyPlanner.Views
{

    enum TimerModes
    {
        Pomodoro,
        Chronomter,
        CountDown
    }

    public partial class TimerView : UserControl
    {
        private GradientPanel leftPanel;
        private Button toggleButton;
        private bool isCollapsed = false;
        private Panel timerPanel;
        private TableLayoutPanel bottomPanel;
        private Label timerLabel;
        private TimeModel timeModel;

        private System.Windows.Forms.Timer timer;
        private TimerModes? timerMode = null;
        private int? selectedInterval = null; // as seconds

        private Button? LastSelectedOptionButton = null;

        public TimerView()
        {
            timeModel = new TimeModel() { Minute = 0  , Second = 0, Hour = 0 };
            timer = new();
            timer.Interval = 10; // 1 second 
            timer.Tick += (sernder, args) =>
            {
                if (timerMode == TimerModes.Chronomter)
                {
                  timeModel.IncreaseOneSecond();
                }
                else if (timerMode == TimerModes.CountDown)
                {
                    timeModel.DecreaseOneSecond();

                    if(timeModel.Second == 0 && timeModel.Minute == 0 &&  timeModel.Hour == 0)
                    {
                        timer.Stop();
                    }
                }
                else if(timerMode == TimerModes.Pomodoro)
                {
                    if(selectedInterval == null)
                    {
                        ToastNotification.ShowErrorToast("timer interval not selected");
                        timerMode = null;
                        timer.Stop();
                        return;
                    }

                    timeModel.DecreaseOneSecond();

                    if (timeModel.Second == 0 && timeModel.Minute == 0 && timeModel.Hour == 0)
                    {
                        // refresh time and play alarm sound
                        int h = (int)selectedInterval / 3600;      // 1 hour = 3600 seconds
                        int m= ((int)selectedInterval % 3600) / 60; // Remaining seconds to minutes
                        int s = (int)selectedInterval % 60;      // Remaining secondss

                        this.timeModel.Hour = h;
                        this.timeModel.Minute = m;
                        this.timeModel.Second = s;


                    }
                }

            };

            this.Dock = DockStyle.Fill;
            InitializeComponent();
            InitializeRightPanel();
            InitilizeTimerPanel();
            InitilzeBottomPanel();
        }

        private void InitializeRightPanel()
        {
            // Left Panel
            leftPanel = new GradientPanel
            {
                Color1 = Color.Black,
                Color2 = Color.DimGray,
                Size = new Size(200, this.Height),
                Dock = DockStyle.Right,
            };
            this.Controls.Add(leftPanel);

            // Toggle Button
            toggleButton = new Button
            {
                Text = "☰",
                Size = new Size(40, 40),
                Location = new Point(10, 10),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };
            toggleButton.FlatAppearance.BorderSize = 0;
            toggleButton.Click += ToggleButton_Click;
            leftPanel.Controls.Add(toggleButton);

            // Add Sidebar Buttons

            AddSidebarLabel("operations", 60);
            AddSidebarButton("Start", 110).Click += ExecuteOperations;
            AddSidebarButton("Reset", 160).Click += ExecuteOperations;
            AddSidebarButton("Pause", 210).Click += ExecuteOperations;

            AddSidebarLabel("options", 300);
            AddSidebarButton("Pomodoro", 350).Click+= ExecuteOptions;
            AddSidebarButton("Chronometer", 400).Click += ExecuteOptions;
            AddSidebarButton("Count Down", 450).Click += ExecuteOptions;


        }
        private Button AddSidebarButton(string text, int yPos)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(180, 40),
                Location = new Point(10, yPos),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = Color.Blue; // Blue Border
            btn.FlatAppearance.BorderSize = 3; // Thicker border
            btn.ForeColor = Color.White;
            btn.Font = new Font("Arial", 10, FontStyle.Bold);
            btn.Padding = new Padding(2); // Simulate glow effect
            btn.FlatAppearance.MouseOverBackColor = Color.LightBlue; // Soft glow on hover

            leftPanel.Controls.Add(btn);
            return btn;

        }

        private void AddSidebarLabel(string text, int yPos)
        {
            Label lbl = new Label
            {
                Text = text,
                Size = new Size(180, 40),
                Location = new Point(10, yPos),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            leftPanel.Controls.Add(lbl);
        }

        private void InitilzeBottomPanel()
        {
            Panel wrapper = new();
            wrapper.Dock = DockStyle.Bottom;

            timerPanel.Controls.Add(wrapper);


            bottomPanel = new TableLayoutPanel();
            bottomPanel.BackColor = Color.DimGray;
            bottomPanel.Width = wrapper.Width - 172;
            bottomPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            wrapper.Controls.Add(bottomPanel);
            wrapper.Resize += (sender, args) =>
            {
                bottomPanel.Width = wrapper.Width - 17;

            };


            string[] minutes = ["1.00", "5.00", "10.00", "15.00", "20.00", "25.00", "30.00" , "1.00.00"];

            bottomPanel.RowCount = 2;
            bottomPanel.ColumnCount = 4;

            bottomPanel.CellBorderStyle =
    TableLayoutPanelCellBorderStyle.Outset;

            Enumerable.Range(0, bottomPanel.ColumnCount).ToList().ForEach(x => {
                bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / bottomPanel.ColumnCount));
            });

            Enumerable.Range(0, bottomPanel.RowCount).ToList().ForEach(x =>
            {
                bottomPanel.RowStyles.Add(new ColumnStyle(SizeType.Percent, 100 / bottomPanel.RowCount));
            });


            int index = 0;
            minutes.ToList().ForEach(m  =>
            {
                int row = (int)Math.Floor(Convert.ToDecimal(index / 3));
                int col = index % 3;
                Button b = new()
                {
                    Cursor = Cursors.Hand,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 10),
                    Anchor = AnchorStyles.None, 
                    Text = m,
                    Location = new Point(row * 40, col * 40)
                };
                b.Click += TimerButtonClicked;
                index++;
                bottomPanel.Controls.Add(b);
            });
                


        }

        private void ToggleButton_Click(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                leftPanel.Width = 200;
                toggleButton.Text = "☰";
            }
            else
            {
                leftPanel.Width = 50;
                toggleButton.Text = "▶";
            }
            isCollapsed = !isCollapsed;
        }


        private void InitilizeTimerPanel()
        {
            timerPanel = new Panel();
            timerPanel.Dock = DockStyle.Fill;

            timerLabel = new Label
            {
                ForeColor = Color.Black,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 45, FontStyle.Bold), 
            };

            timerLabel.DataBindings.Add("Text", timeModel, "TimeDisplay");

            timerPanel.Controls.Add(timerLabel);
            timerPanel.SizeChanged += (s, e) => CenterLabel(); 

        Controls.Add(timerPanel);
            timerPanel.Controls.Add(timerLabel);
        }
        private void CenterLabel()
        {
            timerLabel.Location = new Point(
                ((timerPanel.Width - timerLabel.Width) / 2) - 90,
                ((timerPanel.Height - timerLabel.Height) / 2) - 90
            );
        }

        private void TimerButtonClicked(object sender, EventArgs args)
        {
            if(sender is Button)
            {
                Button button = sender as Button;
                string[] all = button!.Text.Split('.');
                
                int minutes = 0, hours = 0;

                // add minutes
                if(all.Length == 2)
                {
                    minutes = int.Parse(all.FirstOrDefault() ?? "0");
                }
                // add hour
                else
                {
                    hours = int.Parse(all.FirstOrDefault() ?? "0");
                }
                timeModel.Minute = minutes;
                timeModel.Hour = hours;

                this.selectedInterval = minutes * 60 + hours * 60 * 60;
                
            }
        }


        private void ExecuteOperations(object sender, EventArgs args)
        {
            if(sender is Button)
            {
                Button btn = sender as Button;
                switch (btn.Text)
                {
                    case "Start":
                        if(timerMode is TimerModes)
                        {
                            timer.Start();
                        }
                        else
                        {
                            ToastNotification.ShowErrorToast("select option to start");
                        }
                        break;
                    case "Reset":
                        timeModel.Hour = 0;
                        timeModel.Minute = 0;
                        timeModel.Second = 0;
                        break;
                    case "Pause":
                        timer.Stop();
                        break;
                }
            }
        }

        private void ExecuteOptions(object sender, EventArgs args)
        {
            if (sender is Button)
            {
                if(LastSelectedOptionButton != null)
                {
                    LastSelectedOptionButton.BackColor = Color.LightGray;
                    LastSelectedOptionButton.FlatStyle = FlatStyle.Flat;
                }

                Button btn = sender as Button;
                LastSelectedOptionButton = btn;

                switch (btn.Text)
                {
                    case "Pomodoro":
                        timerMode = TimerModes.Pomodoro;
                        break;
                    case "Chronometer":
                        timerMode = TimerModes.Chronomter;
                        break;
                    case "Count Down":
                        timerMode = TimerModes.CountDown;
                        break;
                }



                // Apply blue glow effect to clicked button
                btn.BackColor = Color.FromArgb(50, 50, 255); // Light Blue Background
                btn.FlatAppearance.BorderColor = Color.Blue; // Blue Border
                btn.FlatAppearance.BorderSize = 3; // Thicker border
                btn.ForeColor = Color.White;
                btn.Font = new Font("Arial", 10, FontStyle.Bold);
                btn.Padding = new Padding(2); // Simulate glow effect
                btn.FlatAppearance.MouseOverBackColor = Color.LightBlue; // Soft glow on hover

                // Store the current button as last selected
                LastSelectedOptionButton = btn;

            }
        }
    }


    public class TimeModel : INotifyPropertyChanged
    {
        private int hour;
        private int minute;
        private int second;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Hour
        {
            get { return hour; }
            set
            {
                hour = value;
                OnPropertyChanged(nameof(Hour));
                OnPropertyChanged(nameof(TimeDisplay)); // Notify UI that TimeDisplay changed
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Minute
        {
            get { return minute; }
            set
            {
                minute = value;
                OnPropertyChanged(nameof(Minute));
                OnPropertyChanged(nameof(TimeDisplay)); // Notify UI that TimeDisplay changed
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Second
        {
            get { return second; }
            set
            {
                second = value;
                OnPropertyChanged(nameof(Second));
                OnPropertyChanged(nameof(TimeDisplay));
            }
        }

        public string TimeDisplay => $"{Hour:D2}:{Minute:D2}:{Second:D2}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void IncreaseOneSecond()
        {
            int s = this.second;
            int m = this.minute;
            int h = this.Hour;

            s = s + 1;
            if(s >= 60)
            {
                s = 0;
                m++;
            }

            if(m >= 60)
            {
                m = 0;
                h++;
            }
            this.Second = s;
            this.Minute = m;
            this.Hour = h;   
        }

        public void DecreaseOneSecond()
        {
            int s = this.Second;
            int m = this.Minute;
            int h = this.Hour;

            if (h == 0 && m == 0 && s == 0)
                return; 
            
            s--;

            if (s < 0)
            {
                s = 59;
                m--;
            }

            if (m < 0)
            {
                m = 59;
                h--;
            }

            this.Second = s;
            this.Minute = m;
            this.Hour = h;
        }


    }


    public class GradientPanel : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Color1 { get; set; } = Color.Blue;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Color2 { get; set; } = Color.LightBlue;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LinearGradientMode GradientMode { get; set; } = LinearGradientMode.ForwardDiagonal;

        private Bitmap gradientBuffer; // Store gradient to avoid unnecessary repaints

        public GradientPanel()
        {
            this.DoubleBuffered = true; // Reduce flickering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw, // Redraw on resize
                          true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (gradientBuffer == null || gradientBuffer.Size != this.Size)
            {
                // Create gradient bitmap buffer
                gradientBuffer?.Dispose();
                gradientBuffer = new Bitmap(this.Width, this.Height);
                using (Graphics g = Graphics.FromImage(gradientBuffer))
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color1, Color2, GradientMode))
                    {
                        g.FillRectangle(brush, this.ClientRectangle);
                    }
                }
            }

            // Draw the cached gradient image
            e.Graphics.DrawImage(gradientBuffer, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Prevent default flickering
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            gradientBuffer?.Dispose();
            gradientBuffer = null; // Force repaint to generate new gradient
            this.Invalidate(); // Trigger redraw
        }
    }


}

