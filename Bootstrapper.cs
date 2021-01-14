using sxlib;
using sxlib.Specialized;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Synapse_X
{
    public partial class Bootstrapper : Form
    {
        public Bootstrapper()
        {
            InitializeComponent();
            Functions.Lib = SxLib.InitializeWinForms(this, Directory.GetCurrentDirectory());
            Functions.Lib.Load();
            Functions.Lib.LoadEvent += LibraryLoadEvent;
        }

        private void CenterTitleText()
        {
            Point CenterSize = new Point();
            CenterSize.X = (this.Size.Width / 2) - (label1.Size.Width / 2);
            CenterSize.Y = label1.Location.Y;
            label1.Location = CenterSize;
        }

        private void LibraryLoadEvent(SxLibBase.SynLoadEvents Event, object whatever)
        {
            switch (Event)
            {
                case SxLibBase.SynLoadEvents.CHECKING_WL:
                    label1.Text = "Checking whitelist...";
                    CenterTitleText();
                    progressBar1.Value = 16;
                    break;
                case SxLibBase.SynLoadEvents.CHANGING_WL:
                    label1.Text = "Changing whitelist...";
                    CenterTitleText();
                    progressBar1.Value = 32;
                    break;
                case SxLibBase.SynLoadEvents.DOWNLOADING_DATA:
                    label1.Text = "Downloading data..";
                    CenterTitleText();
                    progressBar1.Value = 53;
                    break;
                case SxLibBase.SynLoadEvents.CHECKING_DATA:
                    label1.Text = "Checking data...";
                    CenterTitleText();
                    progressBar1.Value = 64;
                    break;
                case SxLibBase.SynLoadEvents.DOWNLOADING_DLLS:
                    label1.Text = "Downloading DLLs...";
                    CenterTitleText();
                    progressBar1.Value = 86;
                    break;
                case SxLibBase.SynLoadEvents.READY:
                    label1.Text = "Ready to launch!";
                    CenterTitleText();
                    progressBar1.Value = 100;
                    Main main = new Main();
                    main.Show();
                    this.Hide();
                    break;
            }
        }

        private void Bootstrapper_Load(object sender, EventArgs e)
        {
            this.Text = Functions.RandomString(Functions.Rnd.Next(10, 32));
            Point CenterSize = new Point();
            CenterSize.X = (this.Size.Width / 2) - (title.Size.Width / 2);
            CenterSize.Y = title.Location.Y;
            title.Location = CenterSize;
        }
    }
}
