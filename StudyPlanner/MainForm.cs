using System.Drawing.Drawing2D;
using System.ComponentModel;
using StudyPlanner.Views;
using System.Drawing.Design;
using StudyPlanner;
using DataContext;


namespace StudyPlanner
{
    public partial class MainForm : Form
    {
        private GradientPanel LeftPanel;
        private Panel MainPanel;


        public void LoadMainFrame()
        {

            LeftPanel = new GradientPanel
            {
                Color1 = Color.Black,
                Color2 = Color.DimGray,
                GradientMode = LinearGradientMode.ForwardDiagonal,
                Dock = DockStyle.Fill
            };

            MainPanel = new Panel();
            MainPanel.BackColor = Color.FromArgb(211, 211, 211);


            this.Width = 800;
            this.Height = 600;

            LeftPanel.Width = 200;
            LeftPanel.Dock = DockStyle.Left;
            LeftPanel.AutoSize = false;


            MainPanel.Dock = DockStyle.Fill;
            MainPanel.AutoSize = true;



            Button HomeButton = new MenuButtons(30, "Home");
            HomeButton.Click += (sender, args) => SwitchPanel(new HomeView());
            HomeButton.Left = (LeftPanel.Width - HomeButton.Width) / 2;

            
            Button TimerButton = new MenuButtons(120, "Timer");
            TimerButton.Click += (sender, args) => SwitchPanel(new TimerView());
            TimerButton.Left = (LeftPanel.Width - TimerButton.Width) / 2;

            Button CalculatorButton = new MenuButtons(210, "Calculator");
            CalculatorButton.Click += (sender, args) => SwitchPanel(new CalculatorView());
            CalculatorButton.Left = (LeftPanel.Width - CalculatorButton.Width) / 2;


            Button NoteViewButton = new MenuButtons(300, "Notes");
            NoteViewButton.Click += (sender, args) => SwitchPanel(new NotesView());
            NoteViewButton.Left = (LeftPanel.Width - NoteViewButton.Width) / 2;

            Button StudyResourcesViewButton = new MenuButtons(390, "Study Resources");
            StudyResourcesViewButton.Click += (sender, args) => SwitchPanel(new StudyResourcesView());
            StudyResourcesViewButton.Left = (LeftPanel.Width - StudyResourcesViewButton.Width) / 2;


            LeftPanel.Controls.Add(HomeButton);
            LeftPanel.Controls.Add(TimerButton);
            LeftPanel.Controls.Add(CalculatorButton);
            LeftPanel.Controls.Add(NoteViewButton);
            LeftPanel.Controls.Add(StudyResourcesViewButton);

            SwitchPanel(new StudyResourcesView());


            this.Controls.Add(MainPanel);
            this.Controls.Add(LeftPanel);
        }
            

        public MainForm()
        {
            InitializeComponent();
            this.Load += (sender, args) => LoadMainFrame();

        }



        private void SwitchPanel(UserControl activeControl)
        {
            MainPanel.Controls.Clear();
            MainPanel.Controls.Add(activeControl);
        }

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


public class MenuButtons : Button
{
    public MenuButtons(int top, string text = "")
    {
        this.Top = top;
        this.Text = text;
        this.Width = 120; // Increase width for better appearance
        this.Height = 50;


        MouseEnter += (s, e) => this.Cursor = Cursors.Hand;
        MouseLeave += (s, e) => this.Cursor = Cursors.Default;

        // Set basic styling
        this.BackColor = Color.FromArgb(0, 150, 255);  // Background color
        this.ForeColor = Color.White; // Text color
        this.Font = new Font("Arial", 12, FontStyle.Bold);
        this.FlatStyle = FlatStyle.Flat; // Remove default button styles
        this.FlatAppearance.BorderSize = 0; // Remove border
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create rounded rectangle
        int cornerRadius = 20;
        GraphicsPath path = new GraphicsPath();
        path.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
        path.AddArc(Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
        path.AddArc(Width - cornerRadius, Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
        path.AddArc(0, Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
        path.CloseFigure();

        // Fill button with blue color
        e.Graphics.FillPath(new SolidBrush(this.BackColor), path);

        // Draw text in center
        StringFormat sf = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), ClientRectangle, sf);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Region = new Region(CreateRoundRectPath(0, 0, Width, Height, 20));
    }

    private GraphicsPath CreateRoundRectPath(int x, int y, int width, int height, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        path.AddArc(x, y, radius, radius, 180, 90);
        path.AddArc(x + width - radius, y, radius, radius, 270, 90);
        path.AddArc(x + width - radius, y + height - radius, radius, radius, 0, 90);
        path.AddArc(x, y + height - radius, radius, radius, 90, 90);
        path.CloseFigure();
        return path;
    }
}
