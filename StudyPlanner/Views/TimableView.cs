using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudyPlanner.Views
{
    public partial class TimableView : UserControl , INotifyPropertyChanged
    {
        private bool addTimableMode = false;
        private Panel MainPanel;

        private readonly TimableEventController timableEventController;

        private DateTime _targetDate;
        private BindingSource bindingSource;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime targetDate
        {
            get => _targetDate;
            set
            {
                if (_targetDate != value)
                {
                    _targetDate = value;
                    OnPropertyChanged(nameof(targetDate)); // Notify the UI of the change
                }
            }
        }

        public TimableView()
        {

            bindingSource = new BindingSource
            {
                DataSource = this
            };

            timableEventController = Program.ServiceProvider.GetRequiredService<TimableEventController>();

            InitializeComponent();
            this.Dock = DockStyle.Fill;

            targetDate = DateTime.Now;

            Button goPrevMontBtn = new Button();
            goPrevMontBtn.Text = "prev";
            goPrevMontBtn.Dock = DockStyle.Left;
            goPrevMontBtn.BackColor = Color.FromArgb(54, 69, 79);
            goPrevMontBtn.Cursor = Cursors.Hand;
            goPrevMontBtn.ForeColor = Color.White;

            goPrevMontBtn.Click += (sender, args) =>
            {
                if (MainPanel == null) return;
                MainPanel.Controls.Clear();

                targetDate = targetDate.AddMonths(-1);
                InintilzeCalender();
            };


            Button goNextMonth = new Button();
            goNextMonth.Text = "next";
            goNextMonth.Dock = DockStyle.Right;
            goNextMonth.BackColor = Color.FromArgb(54, 69, 79);
            goNextMonth.Cursor = Cursors.Hand;
            goNextMonth.ForeColor = Color.White;


            goNextMonth.Click += (sender, args) =>
            {
                if (MainPanel == null) return;
                MainPanel.Controls.Clear();

                targetDate = targetDate.AddMonths(1);
                InintilzeCalender();

            };


            Panel header = new Panel();
            header.Dock = DockStyle.Top;
            header.Height = 70;
            header.Padding = new Padding(10);

            Button addTimableButton = new Button();
            addTimableButton.Text = "add timable";
            addTimableButton.TextAlign = ContentAlignment.MiddleCenter;
            addTimableButton.Dock = DockStyle.Right;
            addTimableButton.Width = 150;
            addTimableButton.Cursor = Cursors.Hand;
            addTimableButton.BackColor = Color.FromArgb(24, 154, 211);
            addTimableButton.ForeColor = Color.White;
            addTimableButton.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            var displayCurrentDate = new Label
            {
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold), // Stylish font
                ForeColor = Color.White, // Text color
                BackColor = Color.DodgerBlue, // Background color
                BorderStyle = BorderStyle.FixedSingle, // Add a border
                Padding = new Padding(10), // Add padding
                TextAlign = ContentAlignment.MiddleCenter // Center-align text
            };
            displayCurrentDate.DataBindings.Add("Text", bindingSource, nameof(targetDate), true, 
                DataSourceUpdateMode.OnPropertyChanged, "", "yyyy-MM");


            MainPanel = new Panel();
            MainPanel.Dock = DockStyle.Fill;

            addTimableButton.Click += (sender, args) =>
            {
                this.addTimableMode = !this.addTimableMode;

                MainPanel.Controls.Clear();

                if (addTimableMode)
                {
                    InitilzeAddTimable();
                    addTimableButton.Text = "go back";
                }
                else
                {
                    addTimableButton.Text = "add timable";
                    InintilzeCalender();
                }
            };

            header.Controls.Add(addTimableButton);
            header.Controls.Add(displayCurrentDate);    

            this.Controls.Add(MainPanel);
            this.Controls.Add(header);
            this.Controls.Add(goPrevMontBtn);
            this.Controls.Add(goNextMonth);

            InintilzeCalender();
        }
        private async void InintilzeCalender()
        {
            CustomTableLayoutPanel calendarPanel = new CustomTableLayoutPanel();
            calendarPanel.RowCount = 5;
            calendarPanel.ColumnCount = 7;
            calendarPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;
            calendarPanel.Dock = DockStyle.Fill;


            Enumerable.Range(0, calendarPanel.RowCount).ToList().ForEach(x =>
            {
                calendarPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / calendarPanel.RowCount));
            });
            Enumerable.Range(0, calendarPanel.ColumnCount).ToList().ForEach(x =>
            {
                calendarPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / calendarPanel.ColumnCount));
            });



            int month = targetDate.Month;
            int year = targetDate.Year;

            DateTime firstDayOfMonth = new DateTime(year, month, 1);

            DayOfWeek dayOfWeek = firstDayOfMonth.DayOfWeek;

            // index of first day
            int dayIndex = ((int)dayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            // total number of days in month
            int daysInMonth = DateTime.DaysInMonth(year, month);



            // fethc events
            var response = await this.timableEventController.GetByYearAndMonth(year, month);

            if (!response.isSuccessFull)
            {
                ToastNotification.ShowErrorToast(response.Message);
                return;
            }

            List<TimableEventModel> evnetsFromDb = response.Data!;

            for (int i = 0; i < daysInMonth; i++)
            {
                List<TimableEventModel> todaysEvents = evnetsFromDb.Where(x => x.EventTime.Day == firstDayOfMonth.Day + i).ToList();


                FlowLayoutPanel stackPanel = new FlowLayoutPanel();
                stackPanel.AutoScroll = true;
                stackPanel.Dock = DockStyle.Fill;

                Label dayNumber = new Label();
                dayNumber.Text = (i + 1).ToString();

                stackPanel.Controls.Add(dayNumber);


                // add evetns
                if (todaysEvents.Count > 0)
                {
                    todaysEvents.ForEach(e =>
                    {
                        FlowLayoutPanel stackPanelhorizontal = new FlowLayoutPanel
                        {
                            FlowDirection = FlowDirection.LeftToRight,
                            AutoSize = true,
                            Dock = DockStyle.Top,
                        };

                        Label l = new Label();
                        l.Text = e.Name;
                        l.ForeColor = Color.Black;

                        Button deleteTimableEventBtn = new Button();
                        deleteTimableEventBtn.Text = "delete";
                        deleteTimableEventBtn.BackColor = Color.Red;
                        deleteTimableEventBtn.ForeColor = Color.White;

                        stackPanelhorizontal.Controls.Add(l);
                        stackPanelhorizontal.Controls.Add(deleteTimableEventBtn);


                        stackPanel.Controls.Add(stackPanelhorizontal);


                        deleteTimableEventBtn.Click += async (sender, args) =>
                        {
                            var response = await this.timableEventController.RemoveTimable(e.Id);

                            if (response.isSuccessFull)
                            {
                                ToastNotification.ShowSuccessToast(response.Message);
                                this.MainPanel.Controls.Clear();
                                InintilzeCalender();
                            }
                            else
                            {
                                ToastNotification.ShowSuccessToast($"{response.Message}");
                            }
                        };
                    });
                }


                int rowIndex = (int)Math.Floor((dayIndex + i) / 7.0);
                int colIndex = (int)Math.Floor((dayIndex + i) % 7.0);

                calendarPanel.Controls.Add(stackPanel, colIndex, rowIndex);

            }


            MainPanel.Controls.Add(calendarPanel);
        }

        private void InitilzeAddTimable()
        {

            FlowLayoutPanel stackPanel = new FlowLayoutPanel();
            stackPanel.Dock = DockStyle.Fill;

            TextBox textBox = new TextBox();
            textBox.PlaceholderText = "enter event name";
            textBox.Width = stackPanel.Width - 20;


            // Create a DateTimePicker control
            DateTimePicker timePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short, // Short date format (e.g., 10/13/2023)
                Size = new Size(150, 20)
            };

            stackPanel.Resize += (sender, args) =>
            {
                textBox.Width = stackPanel.Width - 20;
            };

            Button submitButton = new Button();
            submitButton.Text = "add new timable";

            submitButton.Click += async (sender, args) =>
            {
                string eventName = textBox.Text;
                DateTime eventTime = timePicker.Value.Date;

                TimableEventModel timeablEvent = new TimableEventModel();
                timeablEvent.Name = eventName;
                timeablEvent.EventTime = eventTime;

                var response = await timableEventController.AddTimable(timeablEvent);

                if (response.isSuccessFull)
                {
                    ToastNotification.ShowSuccessToast(response.Message);
                }
                else
                {
                    ToastNotification.ShowErrorToast(response.Message);
                }

            };



            stackPanel.Controls.Add(textBox);
            stackPanel.Controls.Add(timePicker);
            stackPanel.Controls.Add(submitButton);

            this.MainPanel.Controls.Add(stackPanel);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


    public class CustomTableLayoutPanel : TableLayoutPanel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color CellBorderColor { get; set; } = Color.Black;

        public CustomTableLayoutPanel()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Pen pen = new Pen(CellBorderColor, 1))
            {
                int y = 0;
                for (int row = 0; row < this.RowCount; row++)
                {
                    y += this.GetRowHeights()[row];
                    e.Graphics.DrawLine(pen, 0, y, this.Width, y);
                }

                int x = 0;
                for (int col = 0; col < this.ColumnCount; col++)
                {
                    x += this.GetColumnWidths()[col];
                    e.Graphics.DrawLine(pen, x, 0, x, this.Height);
                }
            }


            // Draw a border around the control
            using (Pen pen = new Pen(CellBorderColor, 2)) // Adjust the thickness as needed
            {
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }
        }
    }

}
