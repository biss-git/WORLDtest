using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WORLD.NET;

namespace WORLDtest
{
    public partial class Form1 : Form
    {

        // 読み込んだ音声のパラメータ
        WorldParameters wp;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ファイル読込を押したときの処理
        /// </summary>
        private void button_input_Click(object sender, EventArgs e)
        {
            // 音声ファイル選択用のダイアログを表示
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "音声(.wav)|*.wav";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                // 音声分析
                wp = WorldAnalysis.Analyse(ofd.FileName);
                // ピッチをグラフに表示
                chart1.Series[0].Points.Clear();
                foreach(var f in wp.f0)
                {
                    chart1.Series[0].Points.Add(f);
                }
                numericUpDown1.Maximum = wp.f0_length - 1;
            }
        }

        /// <summary>
        /// フレーム数が選択されたときの処理
        /// </summary>
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (wp == null) // 音声がない
            {
                return; // 何もしない
            }
            // スペクトル包絡を表示
            chart2.Series[0].Points.Clear();
            int frameNumber = (int)numericUpDown1.Value;
            float[] spectrogram = wp.spectrogram[frameNumber];
            for (int i = 1; i < spectrogram.Length; i++)
            {
                chart2.Series[0].Points.AddXY(Math.Log(i), spectrogram[i]);
            }
        }

        /// <summary>
        /// 「キー＋１」ボタンが押されたときの処理
        /// </summary>
        private void button_keyUp_Click(object sender, EventArgs e)
        {
            if (wp == null) // 音声がない
            {
                return; // 何もしない
            }
            // ピッチを上げる
            for (int i = 0; i < wp.f0_length; i++)
            {
                wp.f0[i] *= (float)Math.Pow(2, 1.0 / 12);
            }
            // ピッチをグラフに表示
            chart1.Series[0].Points.Clear();
            foreach (var f in wp.f0)
            {
                chart1.Series[0].Points.Add(f);
            }
        }

        /// <summary>
        /// 「キーー１」ボタンが押されたときの処理
        /// </summary>
        private void button_keyDown_Click(object sender, EventArgs e)
        {
            if (wp == null) // 音声がない
            {
                return; // 何もしない
            }
            // ピッチを下げる
            for (int i = 0; i < wp.f0_length; i++)
            {
                wp.f0[i] /= (float)Math.Pow(2, 1.0 / 12);
            }
            // ピッチをグラフに表示
            chart1.Series[0].Points.Clear();
            foreach (var f in wp.f0)
            {
                chart1.Series[0].Points.Add(f);
            }
        }

        /// <summary>
        /// 音声の保存ボタンが押されたときの処理
        /// </summary>
        private void button_save_Click(object sender, EventArgs e)
        {
            if (wp == null) // 音声がない
            {
                return; // 何もしない
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "音声(.wav)|*.wav";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                // 保存する
                Synthesizer synthesizer = new Synthesizer(wp);
                synthesizer.SaveWav(sfd.FileName);
            }
        }


    }
}
