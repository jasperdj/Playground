using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Svg;
using System.Drawing;
using dsm.dataModel;

namespace dsm
{
    public partial class PlayGround : Form
    {
        Data data;
        static DragDropManager dragDropManager;
        static ToolManager toolManager;
        FolderManager folderManager;
                               
        public PlayGround()
        {
            InitializeComponent();
            
            data = new Data();
            dragDropManager = new DragDropManager();
            folderManager = new FolderManager();
            toolManager = new ToolManager(panelPlayground, dragDropManager);

            //      Init Form      //
            toolManager.movePanel(panelPlayground);
            dragDropManager.makePanelDraggable(panelPlayground);
            menuBar.Width = this.Width - 18;
            this.Size = new System.Drawing.Size((SystemInformation.VirtualScreen.Width / 100) * 70, (SystemInformation.VirtualScreen.Height / 100) * 70);

            //      Init Tools      //
            handTool.Click += toolManager.handTool_click;
            lineTool.Click += toolManager.lineTool_Click;
            squareTool.Click += toolManager.squareTool_Click;
            circleTool.Click += toolManager.circleTool_Click;
            textTool.MouseDown += toolManager.textTool_MouseDown;
            menuBar.ItemClicked += toolManager.menuBar_ItemClicked;
            editModeButton.DropDownItemClicked += toolManager.editModeButton_DropDownItemClicked;

            populateShapes();

            PlayGround2 payground = new PlayGround2();
            payground.Show();
            
        }

        #region Database Events
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateTimer.Enabled = false;
            bool connectionSucceeded = data.databaseConnection();
            drawDataCombo(string.Format("SELECT ST.*, FT.start, FT.eind, FT.Extruder FROM [dbo].[SecondTable] ST,[dbo].[Table] FT"));
            data.getConnection().Close();

            setConnectionLabel(connectionSucceeded);
            lastUpdateLabel.Text = "Updated on " + DateTime.Now.ToString("hh:mm:ss");
            updateTimeTextBox_TextChanged(updateTimeTextBox, e);
            updateTimer.Enabled = true;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            updateTimer_Tick(updateTimer, e);
        }

        private void updateTimeTextBox_TextChanged(object sender, EventArgs e)
        {
            int value;
            bool isNum = int.TryParse(updateTimeTextBox.Text, out value);
            if (isNum) {
                updateTimer.Interval = value * 60000;
                updateTimer.Stop();
                updateTimer.Start();
            }
        }

        private void drawDataCombo(string sql)
        {
            List<DataRow> rows = data.executeQuery(sql);
            DataTable dataTable = data.getDatatable();
            ComboBox[] boxes = new ComboBox[rows.Count];
            try {
                for (int i = 0; i < rows.Count; i++) {
                    ComboBox comboBox = new ComboBox();
                    comboBox.Location = new System.Drawing.Point(5, i * 20);
                    comboBox.Size = new System.Drawing.Size(200, 25);
                    comboBox.Text = "TAG: " + rows[i].ItemArray[0].ToString();
                    comboBox.MouseDown += dragDropManager.comboBox_MouseDown;

                    for (int j = 0; j < rows[i].ItemArray.Length; j++) {
                        comboBox.Items.Add(dataTable.Columns[j].ColumnName.ToUpper() + ": " + rows[i].ItemArray[j]);
                    }

                    boxes[i] = comboBox;
                }
            }
            catch (Exception e) {
                Console.WriteLine("Error at creating comboBoxes | " + e.Message);
            }
            try {
                List<Control> oldControls = new List<Control>();
                foreach (Control c in panelData.Controls) {
                    oldControls.Add(c);
                }
                panelData.Controls.AddRange(boxes);
                foreach (Control c in oldControls) {
                    panelData.Controls.Remove(c);
                }
            }
            catch (Exception e) {
                Console.WriteLine("Error at removing comboBoxes | " + e.Message);
            }
        }

        private void setConnectionLabel(bool value)
        {
            if (value) {
                connectionLabel.Text = "Connection: Sucessfull";
                connectionLabel.ForeColor = System.Drawing.Color.Green;
                lastUpdateLabel.Text = "Updated on " + DateTime.Now.ToString("hh:mm:ss");
            }
            else {
                connectionLabel.Text = "Connection: Failed";
                connectionLabel.ForeColor = System.Drawing.Color.Red;
            }
        }
        #endregion Database Events

        #region Form Events
        private void PlayGround_SizeChanged(object sender, EventArgs e)
        {
            menuBar.Width = this.Width - 18;
        }
        #endregion Form Events

        #region Shape Events
        private void populateShapes()
        {
            int width = 10;
            int height = 10;
            int line = 0;
            foreach (string photoPath in folderManager.getImages()) {
                PictureBox picBox = new PictureBox();
                picBox.Load(photoPath);
                picBox.Location = new Point(width, height);
                picBox.SizeMode = PictureBoxSizeMode.StretchImage;
                picBox.Size = new System.Drawing.Size(75, 75);
                picBox.MouseDown += toolManager.shape_MouseDown;
                
                Label label = new Label();
                label.Text = getName(photoPath);
                label.Size = new System.Drawing.Size(95, 20);
                label.Location = new Point(width - 10, height + 75);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.BackColor = Color.Yellow;
                
                while (label.Width < System.Windows.Forms.TextRenderer.MeasureText(label.Text,
                    new Font(label.Font.FontFamily, label.Font.Size, label.Font.Style)).Width) {
                    label.Font = new Font(label.Font.FontFamily, label.Font.Size - 0.5f, label.Font.Style);
                }

                panelShapes.Controls.Add(label);
                panelShapes.Controls.Add(picBox);

                if (line == 0) {
                    width += 100;
                    line++;
                }
                else {
                    line = 0;
                    height += 110;
                    width -= 100;
                }
            } 
        }

        private string getName(string path)
        {
            return path.Substring(path.LastIndexOf(@"\") + 1, path.Length - path.LastIndexOf(@"\") - 5);
        }

        private Font getCorrectFontSize(Label label)
        {
            Font font = null;
            
            return font;
        }
        #endregion Shape Events
    }
}