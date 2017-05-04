using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace csch8
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private Color primaryColor;
        private Color secondaryColor;
        private Emulator emulator;
        private System.Timers.Timer timer;

        /// <summary>
        /// Draws a frame on the screen.
        /// </summary>
        /// <param name="memory">Takes chip8 memory by referencee</param>
        public void DrawFrame(byte[] memory)
        {
            //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(fileDialog.FileName, "PLACEHOLDER");
                try
                {
                    //TODO check if an emu already exists, if so kill it
                    emulator = new Emulator(fileDialog.FileName, dynamicRecompilerToolStripMenuItem.Checked);
                    timer = new System.Timers.Timer();
                    timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
                    timer.Interval = (1000 / 60);
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OnTimerTick(object source, ElapsedEventArgs e)
        {
            try
            {
                emulator.RunCycle();
                DrawFrame(emulator.memory);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            timer.Start();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to quit?", "Quit?", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void InterpreterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dynamicRecompilerToolStripMenuItem.Enabled && interpreterToolStripMenuItem.Enabled)
            {
                if (interpreterToolStripMenuItem.Checked)
                {
                    emulator.DynamicRecompiler = true;
                    dynamicRecompilerToolStripMenuItem.Checked = true;
                    interpreterToolStripMenuItem.Checked = false;
                }
                else
                {
                    emulator.DynamicRecompiler = false;
                    dynamicRecompilerToolStripMenuItem.Checked = false;
                    interpreterToolStripMenuItem.Checked = true;
                }
            }
        }

        private void DynamicRecompilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dynamicRecompilerToolStripMenuItem.Enabled && interpreterToolStripMenuItem.Enabled)
            {
                if (dynamicRecompilerToolStripMenuItem.Checked)
                {
                    emulator.DynamicRecompiler = true;
                    dynamicRecompilerToolStripMenuItem.Checked = false;
                    interpreterToolStripMenuItem.Checked = true;
                }
                else
                {
                    emulator.DynamicRecompiler = false;
                    dynamicRecompilerToolStripMenuItem.Checked = true;
                    interpreterToolStripMenuItem.Checked = false;
                }
            }
        }

        private void PrimaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            primaryColor = cd.Color;

            Rectangle rect = new Rectangle(0, 24, 10, 10);
            SolidBrush brush = new SolidBrush(cd.Color);
            Graphics graphics;
            graphics = CreateGraphics();
            graphics.FillRectangle(brush, rect);
        }

        private void SecondaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            secondaryColor = cd.Color;

            Rectangle rect = new Rectangle(10, 24, 10, 10);
            SolidBrush brush = new SolidBrush(cd.Color);
            Graphics graphics;
            graphics = CreateGraphics();
            graphics.FillRectangle(brush, rect);
        }
    }
}
