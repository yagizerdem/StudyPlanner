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
        private ContextMenuStrip contextMenu;
        private TreeNode selectedNode;

        private TreeView treeView;

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


            Button expandAll = new Button();
            expandAll.Location = new Point(10, 300);
            expandAll.Width = 180;
            expandAll.Height = 60;
            expandAll.BackColor = Color.DimGray;
            expandAll.ForeColor = Color.White;
            expandAll.Text = "expand all";
            expandAll.Cursor = Cursors.Hand;
            expandAll.Font = new Font("Arial", 14, FontStyle.Bold);


            Button collapseAll = new Button();
            collapseAll.Location = new Point(10, 380);
            collapseAll.Width = 180;
            collapseAll.Height = 60;
            collapseAll.BackColor = Color.DimGray;
            collapseAll.ForeColor = Color.White;
            collapseAll.Text = "collapse all";
            collapseAll.Cursor = Cursors.Hand;
            collapseAll.Font = new Font("Arial", 14, FontStyle.Bold);


            Button expandUnits = new Button();
            expandUnits.Location = new Point(10, 460);
            expandUnits.Width = 180;
            expandUnits.Height = 60;
            expandUnits.BackColor = Color.DimGray;
            expandUnits.ForeColor = Color.White;
            expandUnits.Text = "expand all units";
            expandUnits.Cursor = Cursors.Hand;
            expandUnits.Font = new Font("Arial", 14, FontStyle.Bold);

            Button collapseUnits = new Button();
            collapseUnits.Location = new Point(10, 540);
            collapseUnits.Width = 180;
            collapseUnits.Height = 60;
            collapseUnits.BackColor = Color.DimGray;
            collapseUnits.ForeColor = Color.White;
            collapseUnits.Text = "collapse units";
            collapseUnits.Cursor = Cursors.Hand;
            collapseUnits.Font = new Font("Arial", 14, FontStyle.Bold);


            rightPanel.Controls.Add(SubjectTextBox);
            rightPanel.Controls.Add(UnitTextBox);
            rightPanel.Controls.Add(ResourceTextBox);
            rightPanel.Controls.Add(submitButton);
            rightPanel.Controls.Add(expandAll);
            rightPanel.Controls.Add(collapseAll);
            rightPanel.Controls.Add(expandUnits);
            rightPanel.Controls.Add(collapseUnits);

            submitButton.Click += async (sender, args) =>
            {
                string subject = SubjectTextBox.Text;
                string unit = UnitTextBox.Text;
                string resource = ResourceTextBox.Text;

                if (string.IsNullOrEmpty(subject))
                {
                    ToastNotification.ShowErrorToast("subject should not be emtyp");
                    return;
                }
                if (string.IsNullOrEmpty(unit))
                {
                    ToastNotification.ShowErrorToast("unit should not be emtyp");
                    return;
                }

                if (string.IsNullOrEmpty(resource))
                {
                    ToastNotification.ShowErrorToast("resource should not be emtyp");
                    return;
                }

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
                    this.Refresh();
                }
                else
                {
                    ToastNotification.ShowErrorToast(response.Message);
                }

                
            };

            expandAll.Click += (sender, args) =>
            {
                treeView.ExpandAll();
            };
            collapseAll.Click += (sender, args) =>
            {
                treeView.CollapseAll();
            };
            expandUnits.Click += (sender, args) =>
            {
                foreach (TreeNode node in treeView.Nodes) 
                {
                    node.Expand(); 
                }
            };

            collapseUnits.Click += (sender, args) =>
            {
                foreach (TreeNode node in treeView.Nodes) 
                {
                    node.Collapse(); 
                }
            };

            Controls.Add(rightPanel);
        }
        
        public async void InitilizeMainPanel()
        {
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;



            this.treeView = new TreeView();
            this.treeView.Dock = DockStyle.Fill;


            // Initialize ContextMenuStrip
            contextMenu = new ContextMenuStrip();
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("🗑 Delete");
            deleteItem.Click += (sender, args)=>DeleteNode_Click(treeView); // Attach event to delete node
            contextMenu.Items.Add(deleteItem);

            // Attach event to show context menu on right-click
            treeView.MouseUp += (sender,args) => TreeView_MouseUp(sender,args,treeView);


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
                rootNode.Tag = s.Id;


                s.Unit.ForEach(u =>
                {
                    TreeNode midNode = new();
                    midNode.Text = u.Name;
                    midNode.Tag = u.Id;

                    u.Resources.ForEach(r =>
                    {
                        TreeNode newNode = new();
                        newNode.Text = r.Name;
                        newNode.Tag = r.Id; 

                        midNode.Nodes.Add(newNode);
                    });
                    rootNode.Nodes.Add(midNode);    
                });
                treeView.Nodes.Add(rootNode);   
            });


            mainPanel.Controls.Add(treeView);


            this.Controls.Add(mainPanel);
            
        }

        private async void DeleteNode_Click(TreeView treeView)
        {
            if (selectedNode != null)
            {
                int NodeId = Convert.ToInt32(selectedNode.Tag);

                if (selectedNode.Parent == null)
                {
                    // subject node
                    await this.studyResourceController.RemoveSubject(NodeId);

                }else if (selectedNode.Parent.Parent == null)
                {
                    // unit node
                    await this.studyResourceController.RemoveUnit(NodeId);
                }
                else
                {
                    // resource node
                    await this.studyResourceController.RemoveResource(NodeId);
                }

                this.Refresh();
            }
        }

        private void TreeView_MouseUp(object sender, MouseEventArgs e, TreeView treeView )
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode clickedNode = treeView.GetNodeAt(e.X, e.Y);
                if (clickedNode != null)
                {
                    treeView.SelectedNode = clickedNode;
                    selectedNode = clickedNode;
                    contextMenu.Show(treeView, e.Location);
                }
            }
        }

        public void Refresh()
        {
            this.Controls.Clear();

            InitilizeRightPanel();
            InitilizeMainPanel();
        }
    }

}
