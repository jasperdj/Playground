using dsm.dataModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dsm
{
    /// <summary>
    /// Takes care of dragging of controls and adding all the correct events
    /// </summary>
    class DragDropManager
    {
        Control activeControl;
        Point previousLocation;
        List<Element> elements = new List<Element>();
        int lastId = 0;

        #region Drag Controls
        /// <summary>
        /// Called for making a panel suitable for drag events
        /// </summary>
        /// <param name="panel"></param>
        public void makePanelDraggable(Panel panel)
        {
            panel.DragDrop += playGround_DragDrop;
            panel.DragEnter += playGround_DragEnter;
            panel.DragOver += playGround_DragOver;
        }

        private void playGround_DragDrop(object sender, DragEventArgs e)
        {
            activeControl.Location = (sender as Panel).PointToClient(Cursor.Position);
            (sender as Panel).Controls.Add(activeControl);
            activeControl.BringToFront();
            Console.WriteLine(activeControl.Text);
            activeControl = null;
        }

        private void playGround_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void playGround_DragOver(object sender, DragEventArgs e)
        {
            //Creates a preview of what your actually dragging
            //panel.Location = playGround.PointToClient(new Point(e.X, e.Y));
            activeControl.BringToFront();
        }
        #endregion Drag Controls

        #region Move Controls
        /// <summary>
        /// Adds all events that makes the control draggable after creation
        /// </summary>
        /// <param name="control"></param>
        public void makeControlMove(Control control)
        {
            control.MouseDown += new MouseEventHandler(control_MouseDown);
            control.MouseUp += new MouseEventHandler(control_MouseUp);
            control.MouseMove += new MouseEventHandler(control_MouseMove);
        }

        private void control_MouseDown(object sender, MouseEventArgs e)
        {
            activeControl = sender as Control;
            activeControl.BringToFront();
            previousLocation = e.Location;
            Cursor.Current = Cursors.SizeAll;
        }

        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (activeControl == null || activeControl != sender) {
                return;
            }
            var location = activeControl.Location;

            activeControl.BringToFront();
            location.Offset(e.Location.X - previousLocation.X, e.Location.Y - previousLocation.Y);
            activeControl.Location = location;
        }

        private void control_MouseUp(object sender, MouseEventArgs e)
        {
            activeControl.BringToFront();
            activeControl = null;
            Cursor.Current = Cursors.Default;
        }
        #endregion Move Controls

        #region Start Drag Events
        public void comboBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (sender as ComboBox).DroppedDown == false) {
                
                //setup Element
                DataTemplate dataTemplate = new DataTemplate();
                dataTemplate.idProperty = lastId;
                
                Output output = new Output();
                DataField dataField = new DataField();

                dataTemplate.addTemplateElement(output);
                dataTemplate.addTemplateElement(dataField);
                
                lastId++;

                //Setup control
                Panel elementContainer = new Panel();
                Label elementTitle = new Label();
                Label elementValues = new Label();



                if ((sender as ComboBox).SelectedItem != null)
                {
                    string value = (sender as ComboBox).Text;
                    int split = value.IndexOf(':') + 2;
                    elementTitle.Text = value.Substring(split, value.Length - split);
                }


                Label label = new Label();
                if ((sender as ComboBox).SelectedItem != null) {
                    string value = (sender as ComboBox).SelectedItem.ToString();
                    int split = value.IndexOf(':') + 2;
                    label.Text = value.Substring(split, value.Length - split);
                }
                else {
                    string value = (sender as ComboBox).Text;
                    int split = value.IndexOf(':') + 2;
                    label.Text = value.Substring(split, value.Length - split);
                }
                label.AutoSize = true;
                makeControlMove(label);
                setActiveControl(label);
                (sender as ComboBox).DoDragDrop(label, DragDropEffects.All);
            }
        }
        #endregion Start Drag Events

        public void setActiveControl(Control control)
        {
            activeControl = control;
        }
    }
}
