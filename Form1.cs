using System;
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

System.Net.Sockets.TcpListener server = new System.Net.Sockets.TcpListener(1337);
System.Net.Sockets.TcpClient xclient;
        Midi.InputDevice keybd;
        string key = "CDEFGABC";
        int octave = 4;
        List<Midi.Pitch> keysDown;

        Midi.OutputDevice od;
        Midi.Clock clock;

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
        Queue<midikey> keyQueue;
        bool kill = false;
        void keythread(object o)
        {

            while (!kill)
            {
                midikey k = midikey.None;
                lock (locker)
                {
                    if (keyQueue.Count > 0)
                        k = keyQueue.Dequeue();
                    else
                        k = midikey.None;


                }
                if (k.k != Keys.None)
                {
                    //add a pre-delay if octave changing since the last key
                    System.Diagnostics.Trace.WriteLine("doctave " + k.deltaOctave.ToString());
                    if(k.deltaOctave != 0)
                         System.Threading.Thread.Sleep(75);
                    while(k.deltaOctave < 0)
                    {
                        InputManager.Keyboard.KeyPress(Keys.D9, 25);
                         System.Threading.Thread.Sleep(80);
                        k.deltaOctave ++;
                    }
                    while(k.deltaOctave > 0)
                    {
                        InputManager.Keyboard.KeyPress(Keys.D0, 25);
                         System.Threading.Thread.Sleep(80);
                        k.deltaOctave --;
                    }
                    //InputManager.Keyboard.KeyPress(k, 50);
                   
                   
                       
                    InputManager.Keyboard.KeyPress(k.k, 25);
                    
                }
               // if (k == Keys.D9 || k == Keys.D0)
                 //   System.Threading.Thread.Sleep(60);
                //else
                //    System.Threading.Thread.Sleep(60); //roughly 35ms pulse.  At 120bpm this is faster than 16th notes  Longer than the keypress interval as well
              
            }

        }
        void onClientConnect(IAsyncResult res)
        {
            var listener = (System.Net.Sockets.TcpListener)res.AsyncState;
            var client = (System.Net.Sockets.TcpClient)listener.EndAcceptTcpClient(res);
            listener.BeginAcceptTcpClient(onClientConnect, listener);
            //G1
            while(client.Connected)
            {
            string smsg;
            byte [] msg = new byte [256];
            int len = client.GetStream().Read(msg,0,256);
            if (msg[0] == 0) continue;
            smsg = ASCIIEncoding.ASCII.GetString(msg);
            Keys lastCode;
            int o;
            System.Diagnostics.Trace.WriteLine("recv from server " + smsg);
                if(smsg != string.Empty)
                {
                keyToCode(smsg, out lastCode, out o);
                int doctave = setOctave(o, false);
                
                queueKeyPress(lastCode, doctave);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Midi.Pitch outpitch;
            System.Diagnostics.Trace.WriteLine(transpose("C4", 1, out outpitch));

            System.Diagnostics.Trace.WriteLine(transpose("C4", -1, out outpitch));

            clock = new Midi.Clock(999);

            foreach (Midi.InputDevice dev in Midi.InputDevice.InstalledDevices)
                cbInputDevice.Items.Add(dev.Name);

            foreach (Midi.OutputDevice dev in Midi.OutputDevice.InstalledDevices)
                cbOutputDevice.Items.Add(dev.Name);

            if (cbInputDevice.Items.Count != 0)
                cbInputDevice.SelectedIndex = 0;
            if (cbOutputDevice.Items.Count != 0)
                cbOutputDevice.SelectedIndex = 0;

            if (cbInputDevice.Items.Count == 0)
                label1.Text = "No midi input device found! app will not work!";

            locker = new object();
            server.Start();
            server.BeginAcceptTcpClient(onClientConnect, server);
           
            
        }
        public struct midikey
        {
            public int deltaOctave;
            public Keys k;
            public static midikey None
            { 
                get
                {
                    midikey ret = new midikey();
                    ret.deltaOctave=0;
                    ret.k = Keys.None;
                    return ret;
                }
            }

        }
        void startOutputDevice(int xod)
        {
            od = Midi.OutputDevice.InstalledDevices[xod];
            od.Open();
            od.SendControlChange(Midi.Channel.Channel1, Midi.Control.Volume, 127); //max the volume
        }
        void startInputDevice(int id)
        {
            keybd = Midi.InputDevice.InstalledDevices[id];
            keybd.NoteOn += keybd_NoteOn;
            keybd.NoteOff += keybd_NoteOff;
            keybd.ControlChange += keybd_ControlChange;
            keybd.Open();

            keybd.StartReceiving(clock);
            keysDown = new List<Midi.Pitch>();
            keybdx = new InputManager.VirtualKeyboard();
            keyQueue = new Queue<midikey>();
            System.Threading.Thread keythreadx = new System.Threading.Thread(keythread);
            keythreadx.Start();
        }

        void keybd_ControlChange(Midi.ControlChangeMessage msg)
        {
            if (msg.Control == Midi.Control.Volume && cbConnectOutput.Checked)
                od.SendControlChange(Midi.Channel.Channel1, Midi.Control.Volume, msg.Value);

        }
        void stopInputDevice()
        {
            if (keybd == null) //ping
                return;
            keybd.StopReceiving();
            keybd.Close();
            keybd.RemoveAllEventHandlers();

        }
        void stopOutputDevice()
        {
            if (od == null)
                return;
            od.SilenceAllNotes();
            od.Close();
            
        }
        void queueKeyPress(Keys xmidikey,  int deltaOctave)
        {
            midikey k = new midikey();
            k.k = xmidikey;
            k.deltaOctave = deltaOctave;
            lock(locker)
            {
                keyQueue.Enqueue(k);

            }

        }

        int setOctave(int o, bool flute)
        {

            if (cbFlute.Checked)
            {
                if((o == 4 && octave == 5) || (o == 5 && octave == 4))
                    queueKeyPress(Keys.D9, 30);
                octave = o;
                return 0;
            }
            if (cbBass.Checked)
            {
                int doctave = 0;
                if ((o == 1 && octave == 2))
                    doctave = -1;
                if ((o == 2 && octave == 1))
                    doctave = 1;
                octave = o;
                return doctave;
            }
            //start in octave 3
            //max octave is 4, min octave is 2
            if (o > 5) o = 5;
            if (o < 3) o = 3;
            System.Diagnostics.Trace.WriteLine("octave change " + o.ToString() + " " + octave.ToString());
            int deltaO = o - octave;
            octave += deltaO;
            return deltaO;
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
            System.Diagnostics.Trace.WriteLine("off" + msg.Pitch.ToString());
            if (keysDown.Contains(msg.Pitch))
            {
                //InputManager.Keyboard.KeyUp(lastCode);
                string pitchIn = msg.Pitch.ToString();
                Midi.Pitch outPitch;
                string pitchout = transpose(pitchIn, -comboBox1.SelectedIndex, out outPitch);
                keysDown.Remove(msg.Pitch);
                od.SendNoteOff(Midi.Channel.Channel1, outPitch, 127);
            }
        }

        string transpose(string musicKey, int direction,out Midi.Pitch outpitch)
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
            switch((string)comboBox1.SelectedValue)
            {
                case "G":
                    direction = 3;
                    break;
                case "A": direction = 2;
                    break;

                case "B":
                    direction =1;
                    break;
                    
                    

            }

            keyID += direction;
            
            ;

            y = keyID / 7;
            x = keyID % 7;
           string preout = "" + scale[x] +  ((black)?("Sharp"):("")) +y.ToString();

           string cnote = "" + preout[0];
          // bool sharp = musicKey.Contains("Sharp");
           
          if (black)
               cnote += "#";
            Midi.Note outnote = new Midi.Note(cnote);
            outpitch = outnote.PitchInOctave(y);
           return preout;

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
            System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
            sw1.Start();
           // if (lastCode != WindowsInput.VirtualKeyCode.NONAME)
              //  WindowsInput.InputSimulator.SimulateKeyUp(lastCode);
            System.Diagnostics.Trace.WriteLine("of" + msg.Pitch.ToString());
            //SendKeys.SendWait("1");
            int o;
            Keys lastCode;
          
          
            if (keysDown.Contains(msg.Pitch))
            {
                //InputManager.Keyboard.KeyUp(lastCode);
                string pitchIn = msg.Pitch.ToString();
                Midi.Pitch outPitch;
                string pitchout = transpose(pitchIn, -comboBox1.SelectedIndex, out outPitch);
                keysDown.Remove(msg.Pitch);
                if(cbConnectOutput.Checked)
                od.SendNoteOff(Midi.Channel.Channel1, outPitch, 127);
            }
            else
            {

                Midi.Pitch outPitch;
               string pitchIn = msg.Pitch.ToString();
               string pitchout = transpose(pitchIn, -comboBox1.SelectedIndex, out outPitch);
              //  if(comboBox1.SelectedValue == "B" || comboBox1.SelectedValue == "A" || comboBox1.SelectedValue == "G")
               //  pitchout = transpose(pitchIn, comboBox1.SelectedIndex);
                System.Diagnostics.Trace.WriteLine("transpose " + pitchIn + " " + pitchout);
                if(cbConnectOutput.Checked)
                    od.SendNoteOn(Midi.Channel.Channel1, outPitch, 127/*msg.Velocity*/);
               
                

                keyToCode(pitchout, out lastCode, out o);

                if (xclient != null && xclient.Connected && o < 3 && !cbBass.Checked) //bass only.. send to server
                {
                    System.Diagnostics.Trace.WriteLine("send to server! " + pitchout);
                    byte[] cmsg = ASCIIEncoding.ASCII.GetBytes(pitchout);
                    xclient.GetStream().Write(cmsg, 0, cmsg.Length);
                }
                else
                {
                    int doctave = setOctave(o, false);

                    queueKeyPress(lastCode, doctave);
                    //System.Threading.Thread.Sleep(20);
                }
                keysDown.Add(msg.Pitch);
            }
            sw1.Stop();
            System.Diagnostics.Trace.WriteLine("timed " + sw1.Elapsed.TotalSeconds.ToString());
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

        private void cbConnect_CheckedChanged(object sender, EventArgs e)
        {
            if (cbConnect.Checked)
            {
                startInputDevice(cbInputDevice.SelectedIndex);


            }
            else
                stopInputDevice();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbConnectOutput.Checked)
                startOutputDevice(cbOutputDevice.SelectedIndex);
            else
                stopOutputDevice();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                xclient = new System.Net.Sockets.TcpClient(textBox1.Text, 1337);
            else
                if (xclient != null)
                    xclient.Close();
            
        }
    }
}
