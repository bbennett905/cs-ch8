using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

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
            this.KeyPreview = true;
            stopwatch = new Stopwatch();
        }

        private Graphics graphics;
        private SolidBrush primaryBrush;
        private SolidBrush secondaryBrush;
        private Emulator emulator;
        private System.Windows.Forms.Timer timer;
        private HistoryForm histForm;
        private RegistersForm regForm;
        private Stopwatch stopwatch;

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
        /// <param name="disp">Takes chip8 display data</param>
        public void DrawFrame(bool[,] disp, byte[] memory)
        {
            SetPCLabel(emulator.ProgramCounter);
            SetOpcodeLabel(emulator.CurrentOpcode);
            if (histForm != null)
            {
                histForm.Update(emulator.History, memory);
            }
            if (regForm != null)
            {
                regForm.Update(emulator.Registers, emulator.ProgramCounter, emulator.AddressRegister, emulator.DelayTimer, emulator.SoundTimer);
            }
            if (emulator.Paused) return;

            List<Rectangle> list = new List<Rectangle>(64 * 32);

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (disp[i, j])
                    {
                        list.Add(new Rectangle(10 * i, 24 + 10 * j, 10, 10));
                    }
                }
            }

            Rectangle bg = new Rectangle(0, 24, 640, 320);

            //TODO "manual" double buffering using bitmap may help with screen tearing
            graphics.FillRectangle(secondaryBrush, bg);
            if (list.Count > 0)
            {
                graphics.FillRectangles(primaryBrush, list.ToArray());
            }
            fpsLabel.Text = "FPS: " + (1000.0 / stopwatch.ElapsedMilliseconds).ToString("F0");
            stopwatch.Restart();
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    emulator = new Emulator(fileDialog.FileName, dynamicRecompilerToolStripMenuItem.Checked);
                    timer = new System.Windows.Forms.Timer();
                    timer.Tick += new EventHandler(OnTimerTick);
                    timer.Interval = (1000 / (int)fpsSelector.Value);
                    timer.Start();
                    stopwatch.Start();
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
                DrawFrame(emulator.Display, emulator.Memory);
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

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            if (histForm != null)
            {
                histForm.Location = new System.Drawing.Point(Right - 7, Top);
            }
            if (regForm != null)
            {
                regForm.Location = new System.Drawing.Point(Right + 160, Top);
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
                    Location = new System.Drawing.Point(Right - 7, Top)
                };
                histForm.Show(this);
            }
            if (regForm != null)
            {
                regForm.Hide();
                regForm = null;
            }
            else
            {
                regForm = new RegistersForm()
                {
                    Location = new System.Drawing.Point(Right + 160, Top)
                };
                regForm.Show(this);
            }
        }

        private void FPSSelector_Changed(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Interval = (1000 / (int)fpsSelector.Value);
                timer.Start();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (emulator == null) return;
            switch(e.KeyCode)
            {
                case Keys.D1:
                    emulator.Keys[0x1] = 1;
                    break;
                case Keys.D2:
                    emulator.Keys[0x2] = 1;
                    break;
                case Keys.D3:
                    emulator.Keys[0x3] = 1;
                    break;
                case Keys.D4:
                    emulator.Keys[0xC] = 1;
                    break;
                case Keys.Q:
                    emulator.Keys[0x4] = 1;
                    break;
                case Keys.W:
                    emulator.Keys[0x5] = 1;
                    break;
                case Keys.E:
                    emulator.Keys[0x6] = 1;
                    break;
                case Keys.R:
                    emulator.Keys[0xD] = 1;
                    break;
                case Keys.A:
                    emulator.Keys[0x7] = 1;
                    break;
                case Keys.S:
                    emulator.Keys[0x8] = 1;
                    break;
                case Keys.D:
                    emulator.Keys[0x9] = 1;
                    break;
                case Keys.F:
                    emulator.Keys[0xE] = 1;
                    break;
                case Keys.Z:
                    emulator.Keys[0xA] = 1;
                    break;
                case Keys.X:
                    emulator.Keys[0x0] = 1;
                    break;
                case Keys.C:
                    emulator.Keys[0xB] = 1;
                    break;
                case Keys.V:
                    emulator.Keys[0xF] = 1;
                    break;
            }
            e.Handled = true;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (emulator == null) return;
            switch (e.KeyCode)
            {
                case Keys.D1:
                    emulator.Keys[0x1] = 0;
                    break;
                case Keys.D2:
                    emulator.Keys[0x2] = 0;
                    break;
                case Keys.D3:
                    emulator.Keys[0x3] = 0;
                    break;
                case Keys.D4:
                    emulator.Keys[0xC] = 0;
                    break;
                case Keys.Q:
                    emulator.Keys[0x4] = 0;
                    break;
                case Keys.W:
                    emulator.Keys[0x5] = 0;
                    break;
                case Keys.E:
                    emulator.Keys[0x6] = 0;
                    break;
                case Keys.R:
                    emulator.Keys[0xD] = 0;
                    break;
                case Keys.A:
                    emulator.Keys[0x7] = 0;
                    break;
                case Keys.S:
                    emulator.Keys[0x8] = 0;
                    break;
                case Keys.D:
                    emulator.Keys[0x9] = 0;
                    break;
                case Keys.F:
                    emulator.Keys[0xE] = 0;
                    break;
                case Keys.Z:
                    emulator.Keys[0xA] = 0;
                    break;
                case Keys.X:
                    emulator.Keys[0x0] = 0;
                    break;
                case Keys.C:
                    emulator.Keys[0xB] = 0;
                    break;
                case Keys.V:
                    emulator.Keys[0xF] = 0;
                    break;
            }
            e.Handled = true;
        }
    }
}
