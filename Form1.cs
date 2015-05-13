﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace midiKeyboarder
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        
        InputManager.VirtualKeyboard keybdx;

        const byte VK_0 = 0x30;  
const byte  VK_1     =    0x31  ; 
const byte  VK_2     =    0x32  ; 
const byte  VK_3     =    0x33  ; 
const byte  VK_4     =    0x34 ;  
const byte  VK_5     =    0x35  ; 
const byte  VK_6     =    0x36 ;  
const byte  VK_7     =    0x37 ;  
const byte  VK_8     =    0x38  ; 
const byte  VK_9      =   0x39 ; 

        Midi.InputDevice keybd;
        string key = "CDEFGABC";
        int octave = 4;
        List<Midi.Pitch> keysDown;

        


        private const int KEYEVENTF_EXTENDEDKEY = 1;
        private const int KEYEVENTF_KEYUP = 2;
        public static void KeyPress(byte vKey)
        {
            KeyDown(vKey);
            System.Threading.Thread.Sleep(100);
            KeyUp(vKey);
        }
        public static void KeyDown( byte vKey)
        {
            keybd_event(vKey, 0,0,  0);
        }

        public static void KeyUp( byte vKey)
        {
            keybd_event(vKey, 0,  KEYEVENTF_KEYUP, 0);
        }
        object locker;
        Queue<Keys> keyQueue;
        bool kill = false;
        void keythread(object o)
        {

            while (!kill)
            {
                Keys k = Keys.None;
                lock (locker)
                {
                    if (keyQueue.Count > 0)
                        k = keyQueue.Dequeue();
                    else
                        k = Keys.None;


                }
                if(k != Keys.None)
                {
                    //InputManager.Keyboard.KeyPress(k, 50);
                    InputManager.Keyboard.KeyDown(k);
                    if (k == Keys.D9 || k == Keys.D0)
                    System.Threading.Thread.Sleep(75);
                    else
                        System.Threading.Thread.Sleep(75);
                    InputManager.Keyboard.KeyUp(k);
                    
                }
               // if (k == Keys.D9 || k == Keys.D0)
                 //   System.Threading.Thread.Sleep(60);
                //else
                //    System.Threading.Thread.Sleep(60); //roughly 35ms pulse.  At 120bpm this is faster than 16th notes  Longer than the keypress interval as well
            }

        }

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            System.Diagnostics.Trace.WriteLine(transpose("C4", 1));

            System.Diagnostics.Trace.WriteLine(transpose("C4", -1));



            locker = new object();
            keybd = Midi.InputDevice.InstalledDevices[0];
            keybd.NoteOn += keybd_NoteOn;
            keybd.NoteOff += keybd_NoteOff;
            keybd.Open();
            keybd.StartReceiving(null);
            keysDown = new List<Midi.Pitch>();
            keybdx = new InputManager.VirtualKeyboard();
            keyQueue = new Queue<Keys>();
            System.Threading.Thread keythreadx = new System.Threading.Thread(keythread);
            keythreadx.Start();

        }

        void queueKeyPress(Keys k, int delay)
        {
            lock(locker)
            {
                keyQueue.Enqueue(k);

            }

        }

        void setOctave(int o, bool flute)
        {
            if (cbFlute.Checked)
            {
                if((o == 4 && octave == 5) || (o == 5 && octave == 4))
                    queueKeyPress(Keys.D9, 30);
                octave = o;
                return;
            }
            if (cbBass.Checked)
            {
                if ((o == 4 && octave == 5))
                    queueKeyPress(Keys.D9, 30);
                if( (o == 5 && octave == 4))
                    queueKeyPress(Keys.D0, 30);
                octave = o;
                return;
            }
            //start in octave 3
            //max octave is 4, min octave is 2
            if (o > 5) o = 5;
            if (o < 3) o = 3;
            System.Diagnostics.Trace.WriteLine("octave change " + o.ToString() + " " + octave.ToString());
            int deltaO = o - octave;
            int i = Math.Sign(deltaO);
            //if(o == 4)
            //{
            //    queueKeyPress(Keys.D9, 30);
            //    queueKeyPress(Keys.D9, 30);
            //    queueKeyPress(Keys.D0, 30);
            //    octave = o;

            //}
            //if (octave != o)
            //{
            //    switch (o)
            //    {
            //        case 3:
            //            queueKeyPress(Keys.D9, 30);
            //            queueKeyPress(Keys.D9, 30);
            //            break;
            //        case 4:
            //            queueKeyPress(Keys.D9, 30);
            //            queueKeyPress(Keys.D9, 30);
            //            queueKeyPress(Keys.D0, 30);
            //            break;
            //        case 5:
            //            queueKeyPress(Keys.D0, 30);
            //            queueKeyPress(Keys.D0, 30);
            //            break;
            //    }
            //}
            while (o != octave)
            {
                if (i < 0)
                    queueKeyPress(Keys.D9, 30);
                else
                    queueKeyPress(Keys.D0, (int)nudDelayTuner.Value);
                octave += i;
                //System.Threading.Thread.Sleep(20);
            }    
           // System.Threading.Thread.Sleep(100);
        }
        void keybd_NoteOff(Midi.NoteOffMessage msg)
        {
            System.Diagnostics.Trace.WriteLine("of" + msg.Pitch.ToString());
        }

        string transpose(string musicKey, int direction)
        {
            //convert the key to a number:
            //"G8" = 8 * "8" + 7
            bool black = musicKey.Contains("Sharp");
             musicKey = musicKey.Replace("Sharp", "");
            string scale = "CDEFGABC";
            int y = int.Parse(""+musicKey[1]);
            int x = scale.IndexOf(musicKey[0]);

            if (cbFlat.Checked && black) //add1 to x
                x = (x + 1);
            if (x == 7)
            {
                x %= 7;
                y++;
            }
            

            int keyID = y * 7 + x;
            keyID += direction;

            y = keyID / 7;
            x = keyID % 7;
            return "" + scale[x] + y.ToString();


        }

        void keyToCode(string musicKey, out Keys code, out int octave)
        {
            //C3
            musicKey = musicKey.Replace("Sharp", "");
            octave = int.Parse("" + musicKey[1]);
            int codekey = key.IndexOf(musicKey[0]);
          
            switch (codekey)
            {
                case 0:
                    code =  Keys.D1;
                    if (octave == 6)
                        code = Keys.D8;
                    break;
                case 1:
                    code =  Keys.D2; break;
                case 2:
                    code =  Keys.D3; break;
                case 3:
                    code =  Keys.D4; break;
                case 4:
                    code =  Keys.D5; break;
                case 5:
                    code =  Keys.D6; break;
                case 6:
                    code =  Keys.D7; break;
                case 7:
                    code =  Keys.D8; break;
                default:
                    code =  0;
                    break;
            }

        }
       // WindowsInput.VirtualKeyCode lastCode = WindowsInput.VirtualKeyCode.NONAME;
        void keybd_NoteOn(Midi.NoteOnMessage msg)
        {
           // if (lastCode != WindowsInput.VirtualKeyCode.NONAME)
              //  WindowsInput.InputSimulator.SimulateKeyUp(lastCode);
            System.Diagnostics.Trace.WriteLine("of" + msg.Pitch.ToString());
            //SendKeys.SendWait("1");
            int o;
            Keys lastCode;
          
          
            if (keysDown.Contains(msg.Pitch))
            {
                //InputManager.Keyboard.KeyUp(lastCode);
                keysDown.Remove(msg.Pitch);
            }
            else
            {
               string pitchIn = msg.Pitch.ToString();
               string pitchout = transpose(pitchIn, -comboBox1.SelectedIndex);
              //  if(comboBox1.SelectedValue == "B" || comboBox1.SelectedValue == "A" || comboBox1.SelectedValue == "G")
               //  pitchout = transpose(pitchIn, comboBox1.SelectedIndex);
                System.Diagnostics.Trace.WriteLine("transpose " + pitchIn + " " + pitchout);
                keyToCode(pitchout, out lastCode, out o);
                setOctave(o, false);
                queueKeyPress(lastCode, (int)nudDelayTuner.Value);
                //System.Threading.Thread.Sleep(20);
               
                keysDown.Add(msg.Pitch);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           // key = textBox1.Text;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nudCapx.Value = Properties.Settings.Default.x;
            nudCapy.Value = Properties.Settings.Default.y;
            comboBox1.SelectedIndex = 0;
           // timer1.Start();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Color three = Color.FromArgb(163, 160, 205);
            Color four = Color.FromArgb(155, 198, 164);
            Color five = Color.FromArgb(213, 144, 146);
            using(Bitmap sc = new Bitmap(pictureBox1.Width, pictureBox1.Height))
            {
                using(Graphics g = Graphics.FromImage(sc))
                {
                    g.CopyFromScreen((int)nudCapx.Value, (int)nudCapy.Value, 0, 0, sc.Size, CopyPixelOperation.SourceCopy);
                }
                Color c = sc.GetPixel(0, 0);
                label1.Text = c.ToString();
                pictureBox1.Image = (Bitmap)sc.Clone();

                if(octave == 3 && c == five)
                {
                    System.Diagnostics.Trace.WriteLine("35");
                    queueKeyPress(Keys.D9, 30);
                    queueKeyPress(Keys.D9, 30);
                }
                if (octave == 3 && c == four)
                {
                    System.Diagnostics.Trace.WriteLine("34");
                    queueKeyPress(Keys.D9, 30);
                    //queueKeyPress(Keys.D9, 30);
                }
                if(c == three && octave == 4)
                {
                    System.Diagnostics.Trace.WriteLine("43");
                    queueKeyPress(Keys.D0, 30);
                }
                if (c == five  && octave == 4)
                {
                    System.Diagnostics.Trace.WriteLine("44");
                    queueKeyPress(Keys.D9, 30);
                }
                if (octave == 5 && c == three)
                {
                    System.Diagnostics.Trace.WriteLine("53");
                    queueKeyPress(Keys.D0, 30);
                    queueKeyPress(Keys.D0, 30);
                }
                if (octave == 5 && c == four)
                {
                    System.Diagnostics.Trace.WriteLine("54");
                    queueKeyPress(Keys.D0, 30);
                    //queueKeyPress(Keys.D9, 30);
                }

            }
        }

        private void nudCapx_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.x = (int)nudCapx.Value;
            Properties.Settings.Default.Save();
        }

        private void nudCapy_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.y = (int)nudCapy.Value;
            Properties.Settings.Default.Save();
        }
    }
}
