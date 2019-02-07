using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace hexview
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            filepath.Text = openFileDialog1.FileName;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bitschema.SelectedIndex = 0;
        }

        int[,] filedata = null; 

        private void view_Click(object sender, EventArgs e)
        {
            if (!File.Exists(filepath.Text))
            {
                return;
            }

            int schemaByteSize = (int)Math.Pow(2.0, bitschema.SelectedIndex);

            float k = 0;
            switch (schemaByteSize)
            {
                case 1:                    
                    k = 0x00FFFFFF / 0xFF;
                    break;
                case 2:                    
                    k = 0x00FFFFFF / 0xFFFF;
                    break;
                case 4:                    
                    k = (float)0x00FFFFFF / (float)0xFFFFFFFF;
                    break;
            }

            byte[] buffer = new byte[schemaByteSize];

            FileStream fs = new FileStream(filepath.Text, FileMode.Open);
            filedata = new int[fs.Length / (16 * schemaByteSize), 16];

            while (fs.Position != fs.Length)
            {
                long position = fs.Position;
                fs.Read(buffer, 0, schemaByteSize);
                int value = 0;

                switch (schemaByteSize)
                {
                    case 1:
                        value = buffer[0];                        
                        break;
                    case 2:
                        value = BitConverter.ToInt16(buffer, 0);                        
                        break;
                    case 4:
                        value = BitConverter.ToInt32(buffer, 0);                        
                        break;
                }

                int x = (int)(position % (16 * schemaByteSize));
                int y = (int)(position / (16 * schemaByteSize));

                if (y < filedata.Length / 16)
                {
                    filedata[y, x] = (int)(value * k);
                }
            }            

            fs.Close();
            fs.Dispose();
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
