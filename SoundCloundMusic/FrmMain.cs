using Bunifu.UI.WinForms;
using NAudio.Wave;
using NAudio.WaveFormRenderer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundCloundMusic
{
    public partial class FrmMain : Form
    {
        private IWavePlayer wavePlayer = new WaveOutEvent();
        private AudioFileReader audioFileReader;
        private FileInfo file;
        
        public FrmMain()
        {
            InitializeComponent();

            grid.Columns[0].DefaultCellStyle.NullValue = null;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string item in files)
            {
                FileInfo fi = new FileInfo(item);
                TagLib.File f = TagLib.File.Create(fi.FullName);
                var r = grid.Rows.Add(new object[]
                {
                    null,
                    fi.Name,                   
                    f.Tag.JoinedGenres,
                    f.Tag.JoinedAlbumArtists,
                    Math.Round( f.Properties.Duration.TotalMinutes,2)+" Phút"
                });

                grid.Rows[r].Tag = fi;
            }
        }

        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void grid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            file = ((FileInfo)grid.Rows[e.RowIndex].Tag);
            Play();
        }

        private void Play()
        {
            Application.UseWaitCursor = true;
            Application.DoEvents();
            if (file == null) return;
            audioFileReader = new AudioFileReader(file.FullName);
            if (wavePlayer.PlaybackState != PlaybackState.Stopped)
            {
                wavePlayer.Stop();
            }
            wavePlayer.Init(audioFileReader);
            GenerateWV();
            wavePlayer.Play();
            btnPlayPause.Image = iconPause.Image;
            Application.UseWaitCursor = false;
        }

        private void GenerateWV()
        {
            var myRendererSettings = new StandardWaveFormRendererSettings();
            myRendererSettings.Width = pnlWaveForm.Width;
            myRendererSettings.TopHeight = pnlWaveForm.Height / 2;
            myRendererSettings.BottomHeight = pnlWaveForm.Height / 2;
            myRendererSettings.TopPeakPen = new Pen(Color.FromArgb(255, 109, 0));
            myRendererSettings.BottomPeakPen = new Pen(Color.FromArgb(255, 221, 186));
            myRendererSettings.BackgroundColor = this.BackColor;

            var myRendererSettings2 = new StandardWaveFormRendererSettings();
            myRendererSettings2.Width = pnlWaveForm.Width;
            myRendererSettings2.TopHeight = pnlWaveForm.Height / 2;
            myRendererSettings2.BottomHeight = pnlWaveForm.Height / 2;
            myRendererSettings2.TopPeakPen = new Pen(Color.FromArgb(255, 109, 0));
            myRendererSettings2.BottomPeakPen = new Pen(Color.FromArgb(255, 221, 186));
            myRendererSettings2.BackgroundColor = this.BackColor;

            var renderer = new WaveFormRenderer();
            var audioFilePath = file.FullName;
            /*pnlWaveForm.BackgroundImage = renderer.Render(audioFilePath, new AveragePeakProvider(3), myRendererSettings);
            picWv.BackgroundImage = renderer.Render(audioFilePath, new AveragePeakProvider(3), myRendererSettings2);*/
        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            foreach (var item in openFileDialog1.FileNames)
            {
                FileInfo fi = new FileInfo(item);
                TagLib.File f = TagLib.File.Create(fi.FullName);
                var r = grid.Rows.Add(new object[]
                {
                    null,
                    fi.Name,
                    f.Tag.JoinedGenres,
                    f.Tag.JoinedAlbumArtists,
                    Math.Round( f.Properties.Duration.TotalMinutes,2)+" Phút"
                });

                grid.Rows[r].Tag = fi;
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            try
            {
                if (wavePlayer.PlaybackState == PlaybackState.Playing)
                {
                    wavePlayer.Pause();
                    btnPlayPause.Image = iconPlay.Image;
                }
                else
                {
                    wavePlayer.Play();
                    btnPlayPause.Image = iconPause.Image;
                }
            }
            catch (Exception)
            {
                goto a;
            a:
                if (grid.RowCount == 0)
                {
                    openFileDialog1.ShowDialog();
                    if (grid.RowCount > 0) goto a;
                }
                else
                {
                    file = ((FileInfo)grid.CurrentRow.Tag);
                    Play();
                }
            }
        }

        private void pnlWaveForm_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                picWv.Width = e.X;
                var max = audioFileReader.Length;

                //max = pnlw
                //x
                var val = (e.X * max) / pnlWaveForm.Width;
                audioFileReader.Position = val;
            }
            catch (Exception)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (wavePlayer.PlaybackState == PlaybackState.Playing)
            {
                lblSong.Text = file.Name;
                picPlay.Enabled = true;
                picPlay2.Enabled = true;
                SetSlider();

                if (!lblArtist.Text.Contains("🎵"))
                {
                    lblArtist.Text = "Đang phát nhạc 🎵 🎵 🎵";
                }
                else if (lblArtist.Text.Contains("🎵 🎵 🎵"))
                {
                    lblArtist.Text = "Đang phát nhạc 🎵 🎵";
                }
                else if (lblArtist.Text.Contains("🎵 🎵"))
                {
                    lblArtist.Text = "Đang phát nhạc 🎵";
                }
                else
                {
                    lblArtist.Text = "Đang phát nhạc";
                }
            }
            else
            {
                if (wavePlayer.PlaybackState == PlaybackState.Stopped)
                {
                    picWv.Width = 0;
                }
                lblSong.Text = "Soundcloud Music";
                picPlay.Enabled = false;
                picPlay2.Enabled = false;
                lblArtist.Text = wavePlayer.PlaybackState.ToString();
            }
        }

        private void SetSlider()
        {
            double max = audioFileReader.Length;
            double cur = audioFileReader.Position;

            var val = (cur * pnlWaveForm.Width) / max;
            picWv.Width = int.Parse(Math.Truncate(val).ToString());
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtSearch.Text.Trim().Length == 0 || e.KeyCode == Keys.Enter)
            {
                foreach (DataGridViewRow item in grid.Rows)
                {
                    item.Visible = item.Cells[1].Value.ToString().ToLower().Contains(txtSearch.Text.Trim().ToLower());
                }
            }
        }

        private void btnPlaylist_Click(object sender, EventArgs e)
        {
            label3.Visible = !label3.Visible;
            panel2.Visible = !panel2.Visible;
            grid.Visible = !grid.Visible;
        }

        private void grid_VisibleChanged(object sender, EventArgs e)
        {
            this.Height = grid.Visible ? 695 : 340;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                if (grid.CurrentRow.Index == 0)
                {
                    grid.CurrentCell = grid.Rows[grid.Rows.GetLastRow(DataGridViewElementStates.Visible)].Cells[1];
                }
                else
                {
                    grid.CurrentCell = grid.Rows[grid.CurrentRow.Index - 1].Cells[1];
                }
                file = ((FileInfo)grid.CurrentRow.Tag);
                Play();
            }
            catch (Exception)
            {

            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (grid.CurrentRow.Index == grid.Rows.GetLastRow(DataGridViewElementStates.None))
                {
                    grid.CurrentCell = grid.Rows[0].Cells[1];
                }
                else
                {
                    grid.CurrentCell = grid.Rows[grid.CurrentRow.Index + 1].Cells[1];
                }
                file = ((FileInfo)grid.CurrentRow.Tag);
                Play();
            }
            catch (Exception)
            {

            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                wavePlayer.Stop();
            }
            catch (Exception)
            {

            }
        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            try
            {
                audioFileReader.Position = 0;
            }
            catch (Exception)
            {

            }
        }

        private void bunifuHSlider1_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
        {
            audioFileReader.Volume = wavePlayer.Volume = (vol.Value - 1) / 100f;
        }

        private void vol_Click(object sender, EventArgs e)
        {
            audioFileReader.Volume = wavePlayer.Volume = (vol.Value - 1) / 100f;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            bunifuFormDock1.WindowState = BunifuFormDock.FormWindowStates.Minimized;
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuButton21_Click(object sender, EventArgs e)
        {
            try
            {
                if (wavePlayer.PlaybackState == PlaybackState.Playing)
                {
                    //wavePlayer.Pause();
                    //btnPlayPause.Image = iconPlay.Image;
                }
                else
                {
                    wavePlayer.Play();
                    btnPlayPause.Image = iconPause.Image;
                }
            }
            catch (Exception)
            {
                goto a;
            a:
                if (grid.RowCount == 0)
                {
                    openFileDialog1.ShowDialog();
                    if (grid.RowCount > 0) goto a;
                }
                else
                {
                    file = ((FileInfo)grid.CurrentRow.Tag);
                    Play();
                }
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            //bunifuFormDock1.WindowState = BunifuFormDock.FormWindowStates.Maximized;
            if (bunifuFormDock1.WindowState == BunifuFormDock.FormWindowStates.Normal)
            {
                bunifuFormDock1.WindowState = BunifuFormDock.FormWindowStates.Maximized;
            }
            else
            {
                bunifuFormDock1.WindowState = BunifuFormDock.FormWindowStates.Normal;
            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
