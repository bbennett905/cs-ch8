using System.Collections.Generic;
using System.Windows.Forms;

namespace csch8
{
    public partial class HistoryForm : Form
    {
        public HistoryForm()
        {
            InitializeComponent();
            labels = new System.Windows.Forms.Label[40];
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = new System.Windows.Forms.Label()
                {
                    AutoSize = true,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Text = "FFFF"
                };
                tableLayoutPanel1.Controls.Add(labels[i]);
            }
        }

        System.Windows.Forms.Label[] labels;

        public void Update(Queue<ushort> history, byte[] memory)
        {
            int i = 0;
            foreach (ushort pc_val in history)
            {
                labels[i].Text = pc_val.ToString("X4");
                labels[i + 1].Text = memory[pc_val].ToString("X4");
                i += 2;
            }
        }
    }
}
