using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace csch8
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            primaryBrush = new SolidBrush(Color.DarkSlateGray);
            secondaryBrush = new SolidBrush(Color.AntiqueWhite);
            graphics = CreateGraphics();
        }

        private Graphics graphics;
        private SolidBrush primaryBrush;
        private SolidBrush secondaryBrush;
        private Emulator emulator;
        private System.Windows.Forms.Timer timer;
        private HistoryForm histForm;

        delegate void SetUshortCallback(ushort us);
        private void SetPCLabel(ushort pc)
        {
            if (pc_label.InvokeRequired)
            {
                SetUshortCallback us = new SetUshortCallback(SetPCLabel);
                pc_label.Invoke(us, pc);
            }
            else
            {
                pc_label.Text = "PC: " + pc.ToString("X4");
            }
        }
        
        private void SetOpcodeLabel(ushort op)
        {
            if (opcode_label.InvokeRequired)
            {
                SetUshortCallback us = new SetUshortCallback(SetOpcodeLabel);
                opcode_label.Invoke(us, op);
            }
            else
            {
                opcode_label.Text = "Op: " + op.ToString("X4");
            }
        }
        /// <summary>
        /// Draws a frame on the screen.
        /// </summary>
        /// <param name="memory">Takes chip8 memory by reference</param>
        public void DrawFrame(byte[] memory)
        {
            SetPCLabel(emulator.ProgramCounter);
            SetOpcodeLabel(emulator.CurrentOpcode);
            if (histForm != null)
            {
                histForm.Update(emulator.History, memory);
            }
            if (emulator.Paused) return;
            //0xF00-0xFFF (3840 - 4095): Display Refresh (1bit/px, 64x32)
            byte[] temp = new byte[256];
            Array.Copy(memory, 3840, temp, 0, 256);
            BitArray ba = new BitArray(temp);

            List<Rectangle> list = new List<Rectangle>(ba.Length);

            for (int i = 0; i < ba.Length; i++)
            {
                if (ba[i])
                {
                    list.Add(new Rectangle(10 * (i % 64), 24 + 10 * (i / 64), 10, 10));
                }
                //graphics.FillRectangle(primaryBrush, r);
            }
            Rectangle bg = new Rectangle(0, 24, 640, 320);

            //TODO "manual" double buffering using bitmap may help with screen tearing
            graphics.FillRectangle(secondaryBrush, bg);
            if (list.Count > 0)
            {
                graphics.FillRectangles(primaryBrush, list.ToArray());
            }
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //TODO check if an emu already exists, if so kill it
                    emulator = new Emulator(fileDialog.FileName, dynamicRecompilerToolStripMenuItem.Checked);
                    timer = new System.Windows.Forms.Timer();
                    timer.Tick += new EventHandler(OnTimerTick);
                   // timer.Elapsed += new ElapsedEventHandler(OnTimerTick);
                    timer.Interval = (1000 / (int)fpsSelector.Value);
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OnTimerTick(object source, EventArgs e)
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

            primaryBrush.Color = cd.Color;
        }

        private void SecondaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            secondaryBrush.Color = cd.Color;
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (emulator == null) return;
            if (emulator.Paused)
            {
                emulator.Paused = false;
                pauseButton.Text = "Pause";
            }
            else
            {
                emulator.Paused = true;
                pauseButton.Text = "Resume";
            }
        }

        private void ViewHistoryButton_Click(object sender, EventArgs e)
        {
            if (histForm != null)
            {
                histForm.Hide();
                histForm = null;
            }
            else
            {
                histForm = new HistoryForm()
                {
                    Location = new System.Drawing.Point(Right, Top - 20)
                };
                histForm.Show(this);
            }
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            if (histForm != null)
            {
                histForm.Location = new System.Drawing.Point(Right, Top - 20);
            }
        }

        private void HistoryToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (histForm != null)
            {
                histForm.Hide();
                histForm = null;
            }
            else
            {
                histForm = new HistoryForm()
                {
                    Location = new System.Drawing.Point(Right, Top - 20)
                };
                histForm.Show(this);
            }
        }

        private void FPSSelector_Changed(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Interval = (1000 / (int)fpsSelector.Value);
            }
        }
    }
}
