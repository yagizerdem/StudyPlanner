using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

public class ToastNotification : Form
{
    private Label messageLabel;
    private System.Windows.Forms.Timer fadeTimer;
    private int displayTime = 3000; // Time before disappearing (in milliseconds)
    private float opacityStep = 0.1f;

    private static int offsetY = 0;

    public ToastNotification(string message, int duration = 3000)
    {
        this.displayTime = duration;

        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.Manual;
        this.BackColor = Color.Black;
        this.Opacity = 0; 
        this.ShowInTaskbar = false;
        this.Size = new Size(300, 60);
        this.TopMost = true;

        this.Region = new Region(CreateRoundRect(0, 0, this.Width, this.Height, 15));

        messageLabel = new Label
        {
            Text = message,
            ForeColor = Color.White,
            Font = new Font("Arial", 12, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        this.Controls.Add(messageLabel);

        int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
        int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
        this.Location = new Point(screenWidth - this.Width - 20, screenHeight - this.Height - 50);

        ShowToast();
    }

    private async void ShowToast()
    {
        for (float i = 0; i <= 1; i += opacityStep)
        {
            this.Opacity = i;
            await Task.Delay(30);
        }

        await Task.Delay(displayTime);

        for (float i = 1; i >= 0; i -= opacityStep)
        {
            this.Opacity = i;
            await Task.Delay(30);
        }

        this.Close();
    }

    private GraphicsPath CreateRoundRect(int x, int y, int width, int height, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        path.AddArc(x, y, radius, radius, 180, 90);
        path.AddArc(x + width - radius, y, radius, radius, 270, 90);
        path.AddArc(x + width - radius, y + height - radius, radius, radius, 0, 90);
        path.AddArc(x, y + height - radius, radius, radius, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void ShowErrorToast(string message, int duration = 3000)
    {
        ToastNotification toast = new ToastNotification(message, duration);
        toast.BackColor = Color.Red;

        toast.Top -= offsetY;
        offsetY += toast.Height + 10;

        toast.Show();

        Task.Delay(duration + 500).ContinueWith(_ => { offsetY -= toast.Height + 10; });
    }
    public static void ShowSuccessToast(string message, int duration = 3000)
    {
        ToastNotification toast = new ToastNotification(message, duration);
        toast.BackColor = Color.Green;

        toast.Top -= offsetY; 
        offsetY += toast.Height + 10; 

        toast.Show();

        Task.Delay(duration + 500).ContinueWith(_ => { offsetY -= toast.Height + 10; });
    }
}
