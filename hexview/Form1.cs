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
        const int pointsize = 16;

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

        uint[,] filedata = null;

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
                    k = (float)0x00FFFFFF / (float)0x000000FF;
                    break;
                case 2:
                    k = 0x00FFFFFF / 0x0000FFFF;
                    break;
                case 4:
                    k = (float)0x00FFFFFF / (float)0xFFFFFFFF;
                    break;
            }

            byte[] buffer = new byte[schemaByteSize];

            FileStream fs = new FileStream(filepath.Text, FileMode.Open);
            filedata = new uint[fs.Length / (16 * schemaByteSize), 16];

            while (fs.Position != fs.Length)
            {
                long position = fs.Position;
                fs.Read(buffer, 0, schemaByteSize);
                uint value = 0;

                switch (schemaByteSize)
                {
                    case 1:
                        value = buffer[0];
                        break;
                    case 2:
                        value = BitConverter.ToUInt16(buffer, 0);
                        break;
                    case 4:
                        value = BitConverter.ToUInt32(buffer, 0);
                        break;
                }

                int x = (int)((position / schemaByteSize) % 16);
                int y = (int)((position / schemaByteSize) / 16);

                if (y < filedata.Length / 16)
                {
                    filedata[y, x] = ~(uint)(value * k);
                }
            }

            fs.Close();
            fs.Dispose();

            int yLen = filedata.Length / 16;
            Bitmap bitmap = new Bitmap(16 * pointsize, yLen * pointsize);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                for (int x = 0; x < 16; x++)
                    for (int y = 0; y < yLen; y++)
                    {
                        Rectangle ee = new Rectangle(x * pointsize, y * pointsize, pointsize, pointsize);
                        using (SolidBrush b = new SolidBrush(Color.FromArgb((int)filedata[y, x])))
                        {
                            graphics.FillRectangle(b, ee);
                        }
                    }
            }

            pictureBox.Image = bitmap;
        }

        private void save_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                pictureBox.Image.Save(saveFileDialog1.FileName);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}
