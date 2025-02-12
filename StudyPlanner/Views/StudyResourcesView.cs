using Microsoft.Extensions.DependencyInjection;
using Models;
using Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudyPlanner.Views
{
    public partial class StudyResourcesView : UserControl
    {
        Panel rightPanel;
        Panel mainPanel;

        private readonly  StudyResourcesController studyResourceController;
        public StudyResourcesView()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            studyResourceController = Program.ServiceProvider.GetRequiredService<StudyResourcesController>();

            InitilizeRightPanel();
            InitilizeMainPanel();
        }
    
        public void InitilizeRightPanel()
        {
            rightPanel = new Panel()
            {
                Dock = DockStyle.Right,
                BackColor = Color.DimGray,
                Width = 200
            };

            TextBox SubjectTextBox = new TextBox();
            SubjectTextBox.Location = new Point(10, 20);
            SubjectTextBox.Width = 180;
            SubjectTextBox.Height = 40;
            SubjectTextBox.PlaceholderText = "subject";
            SubjectTextBox.Multiline = true;

            TextBox UnitTextBox = new TextBox();
            UnitTextBox.Location = new Point(10, 80);
            UnitTextBox.Width = 180;
            UnitTextBox.Height = 40;
            UnitTextBox.PlaceholderText = "unit";
            UnitTextBox.Multiline = true;

            TextBox ResourceTextBox = new TextBox();
            ResourceTextBox.Location = new Point(10, 140);
            ResourceTextBox.Width = 180;
            ResourceTextBox.Height= 40;
            ResourceTextBox.PlaceholderText = "resource";
            ResourceTextBox.Multiline = true;

            Button submitButton = new Button();
            submitButton.Location = new Point(10, 200);
            submitButton.Width = 180;
            submitButton.Height = 60;
            submitButton.BackColor = Color.Blue;
            submitButton.ForeColor = Color.White;
            submitButton.Text = "submit";
            submitButton.Cursor = Cursors.Hand;
            submitButton.Font = new Font("Arial", 14, FontStyle.Bold);

            rightPanel.Controls.Add(SubjectTextBox);
            rightPanel.Controls.Add(UnitTextBox);
            rightPanel.Controls.Add(ResourceTextBox);
            rightPanel.Controls.Add(submitButton);


            submitButton.Click += async (sender, args) =>
            {
                string subject = SubjectTextBox.Text;
                string unit = UnitTextBox.Text;
                string resource = ResourceTextBox.Text;

                ResourcesModel resourcesModel = new ResourcesModel();
                resourcesModel.Name = resource;


                UnitModel unitModel = new UnitModel();
                unitModel.Name = unit;


                SubjectModel subjectModel = new SubjectModel();
                subjectModel.Name = subject;

                unitModel.Resources.Add(resourcesModel);
                subjectModel.Unit.Add(unitModel);


                var response = await studyResourceController.AddSubject(subjectModel);

                if (response.isSuccessFull)
                {
                    ToastNotification.ShowSuccessToast(response.Message);
                }
                else
                {
                    ToastNotification.ShowErrorToast(response.Message);
                }

            };

            Controls.Add(rightPanel);
        }
        
        public async void InitilizeMainPanel()
        {
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;

            TreeView treeView = new TreeView();
            treeView.Dock = DockStyle.Fill;


            var response = await this.studyResourceController.GetSubjects();
            if (!response.isSuccessFull)
            {
                ToastNotification.ShowErrorToast($"{response.Message}");    
                return;
            }

            List<SubjectModel> subjectsFromDb = response.Data;

            subjectsFromDb.ForEach(s =>
            {
                TreeNode rootNode = new();
                rootNode.Text = s.Name;

                s.Unit.ForEach(u =>
                {
                    TreeNode midNode = new();
                    midNode.Text = u.Name;

                    u.Resources.ForEach(r =>
                    {
                        TreeNode newNode = new();
                        newNode.Text = r.Name;
                        midNode.Nodes.Add(newNode);
                    });
                    rootNode.Nodes.Add(midNode);    
                });
                treeView.Nodes.Add(rootNode);   
            });


            mainPanel.Controls.Add(treeView);


            this.Controls.Add(mainPanel);
            
        }
    }

}
