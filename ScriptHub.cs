using sxlib.Specialized;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse_X
{
    public partial class ScriptHub : Form
    {
        SxLibBase.SynHubEntry CurrentEntry;

        public ScriptHub()
        {
            InitializeComponent();
            Functions.Lib.ScriptHubEvent += LibraryScriptHubEvent;
        }

        List<SxLibBase.SynHubEntry> _entries;

        private void LibraryScriptHubEvent(List<SxLibBase.SynHubEntry> Entries)
        {
            foreach (SxLibBase.SynHubEntry entry in Entries)
            {
                HubList.Items.Add(entry.Name);
            }
            _entries = Entries;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ScriptHub_Load(object sender, EventArgs e)
        {
            Point CenterSize = new Point();
            CenterSize.X = (this.Size.Width / 2) - (title.Size.Width / 2);
            CenterSize.Y = title.Location.Y;
            title.Location = CenterSize;
            Functions.Lib.ScriptHub();
        }

        private void HubList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HubList.SelectedIndex == -1)
            {
                return;
            }
            var CurrentIndex = HubList.SelectedIndex;
            CurrentEntry = _entries[CurrentIndex];
            textBox1.Text = CurrentEntry.Description;
            pictureBox2.Load(CurrentEntry.Picture);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (SxLibBase.SynHubEntry entry in _entries)
            {
                if (CurrentEntry.Name == entry.Name && HubList.SelectedIndex != -1)
                {
                    entry.Execute();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        Point lastPoint1;
        Point lastPoint2;
        Point lastPoint3;
        Point lastPoint4;

        private void ScriptHub_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint1 = new Point(e.X, e.Y);
        }

        private void ScriptHub_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint1.X;
                this.Top += e.Y - lastPoint1.Y;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint2 = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint2.X;
                this.Top += e.Y - lastPoint2.Y;
            }
        }

        private void title_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint3 = new Point(e.X, e.Y);
        }

        private void title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint3.X;
                this.Top += e.Y - lastPoint3.Y;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint4 = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint4.X;
                this.Top += e.Y - lastPoint4.Y;
            }
        }
    }
}
