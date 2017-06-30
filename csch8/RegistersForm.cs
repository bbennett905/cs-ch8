using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csch8
{
    public partial class RegistersForm : Form
    {
        Label[] regIdLabels;
        Label[] regValLabels;
        Label delayLabel;
        Label soundLabel;
        public RegistersForm()
        {
            InitializeComponent();

            regIdLabels = new Label[16];
            for (int i = 0; i < regIdLabels.Length; i++)
            {
                regIdLabels[i] = new Label()
                {
                    AutoSize = true,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Text = i.ToString("X1")
                };
                tableLayoutPanel1.Controls.Add(regIdLabels[i], 0, i + 3);
            }

            regValLabels = new Label[36];
            for (int i = 0; i < regValLabels.Length; i+=2)
            {
                regValLabels[i] = new Label()
                {
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = "FFFF"
                };
                tableLayoutPanel1.Controls.Add(regValLabels[i]);

                regValLabels[i+1] = new Label()
                {
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = "00"
                };
                tableLayoutPanel1.Controls.Add(regValLabels[i+1]);
            }

            delayLabel = new Label()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "FF"
            };
            tableLayoutPanel2.Controls.Add(delayLabel);
            soundLabel = new Label()
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "FF"
            };
            tableLayoutPanel2.Controls.Add(soundLabel);
        }

        public void Update(byte[] registers, ushort pc, ushort ic, byte delayTimer, byte soundTimer)
        {
            regValLabels[0].Text = pc.ToString("X4");
            regValLabels[1].Text = pc.ToString("D5");
            regValLabels[2].Text = ic.ToString("X4");
            regValLabels[3].Text = ic.ToString("D5");
            for (int i = 4; i < regValLabels.Length; i+=2)
            {
                regValLabels[i].Text = registers[(i - 4) / 2].ToString("X4");
                regValLabels[i + 1].Text = registers[(i - 4) / 2].ToString("D5");
            }
            delayLabel.Text = delayTimer.ToString("X2");
            soundLabel.Text = soundTimer.ToString("X2");
        }
    }
}
