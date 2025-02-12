using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.ApplicationServices;
using Models;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudyPlanner.Views
{
    enum ViewMode
    {
        Gird, List
    }
    
    public partial class NotesView : UserControl
    {
        private SplitContainer? splitContainer = null;
        private TextBox? titleTextBox = null;
        private TextBox? bodyTextBox = null;
        private Panel? NotesPanel = null;
        private ViewMode selectedViewMode = ViewMode.List;
        private bool filterStar = false;

        private readonly NotesController notesController;

        public NotesView()
        {
            InitializeComponent();
            
            Dock = DockStyle.Fill;

            splitContainer = new()
            {
                Dock = DockStyle.Fill,
            };

            InitilzeRightPanel();
            InitilizeLeftPanel();

            this.notesController = Program.ServiceProvider.GetRequiredService<NotesController>();


            SetNotesPanel(selectedViewMode);
        }

        private void InitilzeRightPanel()
        {

            FlowLayoutPanel stackPanelRight = new()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(52, 52, 52),
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Regular),
                FlowDirection = FlowDirection.LeftToRight, // Ensures vertical stacking
                WrapContents = true,
                AutoScroll = true
            };


            // Create a full-width label
            Label addNoteLabel = new Label()
            {
                Text = "Add new note",
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = false, // Prevents shrinking
                Height = 50, // Set a fixed height
                Width = stackPanelRight.ClientSize.Width, // Set width manually to match panel
                Anchor = AnchorStyles.Left | AnchorStyles.Right, // Allows width resizing
                Margin = new Padding(0, 10, 0, 0) // Adds space from the top
            };


            titleTextBox = new()
            {
                PlaceholderText = "enter title",
                Height = 50, // Set a fixed height
                Width = Convert.ToInt32(Math.Floor(stackPanelRight.ClientSize.Width * 0.8)),
                Margin = new Padding((stackPanelRight.ClientSize.Width - Convert.ToInt32(Math.Floor(stackPanelRight.ClientSize.Width * 0.8))) / 2, 20, 0, 0), // Center horizontally
                BackColor = Color.White,
                Anchor = AnchorStyles.None,
            };



            bodyTextBox = new()
            {
                PlaceholderText = "Enter note",
                Height = 100, // Increased height for multi-line input
                Width = Convert.ToInt32(Math.Floor(stackPanelRight.ClientSize.Width * 0.8)), // 80% of the parent width
                Multiline = true, // Allows multi-line input
                BackColor = Color.White,
                ForeColor = Color.Black,
                Anchor = AnchorStyles.None,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = true,
            };

            Button submitNoteButton = new()
            {
                Text = "Create",
                ForeColor= Color.White,
                Height = 50,
                Width = Convert.ToInt32(Math.Floor(stackPanelRight.ClientSize.Width * 0.5)),
                Cursor = Cursors.Hand,
            };


            submitNoteButton.Click += async (sender , args) => await HandleAddNote();


            stackPanelRight.SizeChanged += (s, e) =>
            {
                addNoteLabel.Width = stackPanelRight.ClientSize.Width;
                titleTextBox.Width = Convert.ToInt32(Math.Floor(stackPanelRight.ClientSize.Width * 0.8));

                int newWidth = Convert.ToInt32(Math.Floor(stackPanelRight.ClientSize.Width * 0.8));

                int newHeight = Convert.ToInt32(Math.Floor(stackPanelRight.ClientSize.Height * 0.6));

                // Update width for titleTextBox and bodyTextBox
                titleTextBox.Width = newWidth;
                bodyTextBox.Width = newWidth;
                bodyTextBox.Height = newHeight;

                // Adjust margins to re-center dynamically
                int marginLeft = (stackPanelRight.ClientSize.Width - newWidth) / 2;
                titleTextBox.Margin = new Padding(marginLeft, 20, 0, 0);
                bodyTextBox.Margin = new Padding(marginLeft, 20, 0, 0);

                submitNoteButton.Width = Math.Max(Convert.ToInt32(Math.Floor(stackPanelRight.ClientSize.Width * 0.5)), 100);
                submitNoteButton.Margin = new Padding((stackPanelRight.ClientSize.Width - submitNoteButton.Width) / 2, 20, 0, 0);
            };


            stackPanelRight.Controls.Add(addNoteLabel);
            stackPanelRight.Controls.Add(titleTextBox);
            stackPanelRight.Controls.Add(bodyTextBox);
            stackPanelRight.Controls.Add(submitNoteButton);

            splitContainer.Panel2.Controls.Add(stackPanelRight);

            this.Controls.Add(splitContainer);
        }
    
        private void InitilizeLeftPanel()
        {
            Panel leftPanel =  new Panel();
            leftPanel.Dock = DockStyle.Fill;

            TableLayoutPanel navBar = new TableLayoutPanel();
            navBar.Dock = DockStyle.Top;
            navBar.Height = 40;
            navBar.AutoSize = true;
            navBar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            navBar.ColumnCount = 7;


            for (int i = 0; i < navBar.ColumnCount; i++)
            {
                navBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / navBar.ColumnCount));
            }
            
            var notesTxt = new Label()
            {
                Text = "Notes",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold)

            };



            navBar.Controls.Add(notesTxt);
            navBar.SetColumnSpan(notesTxt, 3);



            Button listViewBtn = new Button()
            {
                Text = "list view",
                Cursor = Cursors.Hand,
                ForeColor = Color.White,
                BackColor = Color.DimGray,
            };

            Button gridViewButton = new Button()
            {
                Text = "grid view",
                Cursor = Cursors.Hand,
                ForeColor = Color.White,
                BackColor = Color.DimGray,
            };

            Button starredButton = new Button()
            {
                Text = "filter starred",
                Cursor = Cursors.Hand,
                ForeColor = Color.White,
                BackColor = Color.DimGray,
            };

            Button showAllButton = new Button()
            {
                Text = "show all",
                Cursor = Cursors.Hand,
                ForeColor = Color.White,
                BackColor = Color.DimGray,
            };

            listViewBtn.Click += (sender, args) =>
            {
                selectedViewMode = ViewMode.List;
                SetNotesPanel(selectedViewMode);

            };

            gridViewButton.Click += (sender, args) =>
            {
                selectedViewMode = ViewMode.Gird;
                SetNotesPanel(selectedViewMode);

            };

            starredButton.Click += (sender, args) =>
            {
                filterStar = true;
                starredButton.BackColor = Color.Red;
                SetNotesPanel(selectedViewMode);

            };


            showAllButton.Click += (sender, args) =>
            {
                filterStar = false;
                starredButton.BackColor = Color.DimGray;
                SetNotesPanel(selectedViewMode);
            };


            navBar.Controls.Add(listViewBtn);
            navBar.Controls.Add(gridViewButton);
            navBar.Controls.Add(starredButton);
            navBar.Controls.Add(showAllButton);


            NotesPanel = new Panel()
            {
                Dock = DockStyle.Fill,
            };

            leftPanel.Controls.Add(NotesPanel);
            leftPanel.Controls.Add(navBar);

            splitContainer!.Panel1.Controls.Add(leftPanel);
        }

        private  async Task SetNotesPanel(ViewMode mode)
        {
            var response = await notesController.GetNotes();
            if (!response.isSuccessFull)
            {
                ToastNotification.ShowErrorToast(response.Message);
            }
            List<NotesModel> notesModels = response.Data;
            if (filterStar)
            {
                notesModels = notesModels.Where(x => x.Starred).ToList();
            }


            NotesPanel!.Controls.Clear();

            if (mode == ViewMode.List)
            {
                FlowLayoutPanel flowPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    AutoScroll = true,
                    WrapContents = false, // Prevent horizontal wrapping
                    Padding = new Padding(10)
                };

                for (int i = 0; i < notesModels.Count; i++)
                {
                    NotesModel model = notesModels[i];
                    Panel card = CreateCard(model);

                    this.Resize += (s, e) => ResizeCards(flowPanel);

                    flowPanel.Controls.Add(card);
                }
                NotesPanel.Controls.Add(flowPanel);

                ResizeCards(flowPanel);
            }

            if(mode== ViewMode.Gird)
            {
                int colCount = 3;
                int rowCount = (int)Math.Ceiling((double)notesModels!.Count / colCount);

                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.ColumnCount = 3;
                tableLayoutPanel.RowCount = rowCount;
                tableLayoutPanel.AutoScroll = true;

                for (int i = 0; i < tableLayoutPanel.ColumnCount; ++i)
                {
                    // make equal width 
                    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                }

                tableLayoutPanel.Dock = DockStyle.Fill;
                tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

                for (int i = 0; i < notesModels.Count; i++)
                {
                    NotesModel model = notesModels[i];
                    Label l = new();
                    l.Text = model.Title;

                    Panel cell = CreateCard(model);
                    cell.Dock = DockStyle.Fill;
                    

                    tableLayoutPanel.Controls.Add(cell);   
                }

                NotesPanel.Controls.Add(tableLayoutPanel);
            }
        }

        private async Task HandleAddNote()
        {
            if (titleTextBox == null || bodyTextBox == null) return;

            string titleText = titleTextBox.Text;
            string bodyText = bodyTextBox.Text;

            Models.NotesModel notesModel = new()
            {
                Title = titleText,
                Body = bodyText
            };
            var response =  await notesController.AddNote(notesModel);

            if (response.isSuccessFull)
            {
                ToastNotification.ShowSuccessToast(response.Message);
                titleTextBox.Text = String.Empty;
                bodyTextBox.Text = String.Empty;
            }
            else
            {
                ToastNotification.ShowErrorToast(response.Message);
            }

            SetNotesPanel(selectedViewMode);


        }

        private Panel CreateCard(NotesModel noteModel)
        {
            // Create Panel (Card Container)
            Panel cardPanel = new Panel
            {
                Width = 300, // Keep fixed width or adjust dynamically
                AutoSize = true,
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Margin = new Padding(5),
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            // Title Label
            Label lblTitle = new Label
            {
                Text = noteModel.Title,
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true
            };

            // Body Label
            Label lblBody = new Label
            {
                Text = noteModel.Body,
                Font = new Font("Arial", 10, FontStyle.Regular),
                AutoSize = true,
            };


            // Created At Label
            Label lblCreatedAt = new Label
            {
                Text = $"Created At: {noteModel.CreatedAt}",
                Font = new Font("Arial", 9, FontStyle.Italic),
                AutoSize = true,
                ForeColor = Color.DimGray // Give it a softer look
            };

            Label isStarred = new Label
            {
                Text = noteModel.Starred ? "Starred" : "Not Starred",
                Font = new Font("Arial", 9, FontStyle.Italic),
                AutoSize = true,
                ForeColor = Color.DimGray // Give it a softer look
            };



            Button StarNoteBtn = new Button()
            {
                Text = "Add To Starred",
                BackColor = Color.Blue,
                ForeColor = Color.White,
                AutoSize = true,
            };

            StarNoteBtn.Click += async (sender, args) =>
            {
                await notesController.AddToStarred(noteModel.Id);
                SetNotesPanel(selectedViewMode);
                ToastNotification.ShowSuccessToast("added to starred");
            };


            Button RemoveStarNoteBtn = new Button()
            {
                Text = "Remove from Starred",
                BackColor = Color.Red,
                ForeColor = Color.White,
                AutoSize = true,
            };

            RemoveStarNoteBtn.Click += async (sender, args) =>
            {
                await notesController.RemoveFromStarred(noteModel.Id);
                SetNotesPanel(selectedViewMode);
                ToastNotification.ShowSuccessToast("removed from starred");
            };


            Button deleteNoteBtn = new Button()
            {
                Text = "Delete",
                BackColor = Color.Red,
                ForeColor = Color.White
            };

            deleteNoteBtn.Click += async (sender, args) =>
            {
                // delete note from db
                var response = await notesController.Delete(noteModel.Id);
                if (!response.isSuccessFull)
                {
                    ToastNotification.ShowErrorToast(response.Message);
                    return;
                }

                ToastNotification.ShowSuccessToast(response.Message);
                SetNotesPanel(selectedViewMode);
            };

            // Add labels to panel
            cardPanel.Controls.Add(lblTitle);
            cardPanel.Controls.Add(lblBody);
            cardPanel.Controls.Add(lblCreatedAt);
            cardPanel.Controls.Add(isStarred);
            cardPanel.Controls.Add(deleteNoteBtn);


            Button toggleStarBtn = new();
            if (!noteModel.Starred)
            {
                toggleStarBtn = StarNoteBtn;
                cardPanel.Controls.Add(StarNoteBtn);
            }
            else
            {
                toggleStarBtn = RemoveStarNoteBtn;
                cardPanel.Controls.Add(RemoveStarNoteBtn);
            }


            // Positioning (Auto-flow layout)
            lblTitle.Location = new Point(5, 5);
            lblBody.Location = new Point(5, lblTitle.Bottom + 5);
            lblCreatedAt.Location = new Point(5, lblBody.Bottom + 5);
            isStarred.Location = new Point(5, lblCreatedAt.Bottom + 5);
            toggleStarBtn.Location = new Point(5, isStarred.Bottom + 5);   
            deleteNoteBtn.Location = new Point(5, toggleStarBtn.Bottom + 5);
            


            return cardPanel;
        }

        private void ResizeCards(FlowLayoutPanel flowPanel)
        {
            foreach (Control control in flowPanel.Controls)
            {
                if (control is Panel card)
                {
                    var newSize = new System.Drawing.Size(card.Width, card.Height);
                    newSize.Width = flowPanel.ClientSize.Width - 25;
                    card.AutoSize = false;
                    card.Size = newSize;    

                }
            }
        }

    }
}
