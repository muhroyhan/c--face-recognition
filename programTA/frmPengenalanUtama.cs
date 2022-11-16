using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV.Structure;

namespace programTA
{
    public partial class frmPengenalanUtama : Form
    {
        public frmPengenalanUtama()
        {
            InitializeComponent();
        }

        #region atribut
        TampilGambar gambur;
        Rectangle kotakTengah;
        Rectangle kotakBatas;
        Rectangle[] kutak;
        Presensi pres;
        Wajah datawaj;
        Pengenalan kenal;
        int tunggu = 0, timerpoto = 1;
        #endregion

        #region method tambahan
        private void LoadGambar(object sender, EventArgs e)
        {
            try
            {
                gambur.GambAsli = gambur.ProcVideo.QueryFrame().ToImage<Bgr, byte>();
                gambur.GambSem = gambur.GambAsli;
                gambur.GambGray = gambur.GambSem.Convert<Gray, byte>();
                gambur.GambSem.Draw(kotakTengah, new Bgr(0, 200, 0), 2);
                picPengenalan.Image = gambur.GambSem.ToBitmap();
                kutak = gambur.CasClass.DetectMultiScale(gambur.GambGray, 1.1, 10, Size.Empty);
                if (kutak != null)
                {
                    foreach (var face in kutak)
                    {
                        gambur.GambSem.Draw(face, new Bgr(), 2); //the detected face(s) is highlighted here using a box that is drawn around it/them
                        picPengenalan.Image = gambur.GambSem.ToBitmap();
                        gambur.GambKrop = gambur.GambSem; //menampung gambar hasil krop
                        gambur.GambKrop.ROI = kutak[0]; // krop gambar
                        if (kotakTengah.Contains(kutak[0]) && kutak[0].Width > kotakBatas.Width && tunggu == 0)
                        {
                            tmrWaktuFoto.Start();
                        }
                    }
                }
            }
            catch(Exception el)
            {
                //MessageBox.Show(el.Message);
            }
        } //melakukan load gambar

        private void PencatatanPresensi()
        {
            lblTimer.Text = timerpoto.ToString();
            tmrWaktuFoto.Stop();
            timerpoto = 1;
            tunggu = 2;
            pres = new Presensi();
            byte[] gambut; int nowajah = 0;
            int idkenal = kenal.Kenali(gambur.GambKrop.ToBitmap(), out gambut, out nowajah); //wajahid yang terlibat pada presensi
            if (pres.Dapres != null) // jika tidak ada presensi aktif
            {
                if (nowajah != 0) // jika ada wajah yang mirip
                {
                    XDocument elek = XDocument.Load("presensi_wajah.xml");
                    if (elek.XPathSelectElement("//presensi_wajah/presensi_tercatat/id_presensi[text()='" + pres.Dapres + "']/../wajah_terlibat/id_kar[text()='" + idkenal + "']/../waktu_datang").Value == "-")
                    {
                        picDikenali.Image = (Bitmap)((new ImageConverter()).ConvertFrom(gambut));
                        elek.XPathSelectElement("//presensi_wajah/presensi_tercatat/id_presensi[text()='" + pres.Dapres + "']/../wajah_terlibat/id_kar[text()='" + idkenal + "']/../waktu_datang").Value = DateTime.Now.ToString("T", CultureInfo.CreateSpecificCulture("es-ES"));
                        elek.Save("presensi_wajah.xml");
                        Notifikasi.Show("Waktu kedatangan telah tercatat", "Berhasil", 1500);
                    }
                    else if (elek.XPathSelectElement("//presensi_wajah/presensi_tercatat/id_presensi[text()='" + pres.Dapres + "']/../wajah_terlibat/id_kar[text()='" + idkenal + "']/../waktu_datang").Value != "-" &&
                        elek.XPathSelectElement("//presensi_wajah/presensi_tercatat/id_presensi[text()='" + pres.Dapres + "']/../wajah_terlibat/id_kar[text()='" + idkenal + "']/../waktu_pulang").Value == "-")
                    {
                        picDikenali.Image = (Bitmap)((new ImageConverter()).ConvertFrom(gambut));
                        elek.XPathSelectElement("//presensi_wajah/presensi_tercatat/id_presensi[text()='" + pres.Dapres + "']/../wajah_terlibat/id_kar[text()='" + idkenal + "']/../waktu_pulang").Value = DateTime.Now.ToString("T", CultureInfo.CreateSpecificCulture("es-ES"));
                        elek.Save("presensi_wajah.xml");
                        Notifikasi.Show("Waktu pulang telah tercatat.", "Berhasil", 1500);
                    }
                    else if (elek.XPathSelectElement("//presensi_wajah/presensi_tercatat/id_presensi[text()='" + pres.Dapres + "']/../wajah_terlibat/id_kar[text()='" + idkenal + "']/../waktu_datang").Value != "-" &&
                        elek.XPathSelectElement("//presensi_wajah/presensi_tercatat/id_presensi[text()='" + pres.Dapres + "']/../wajah_terlibat/id_kar[text()='" + idkenal + "']/../waktu_pulang").Value != "-")
                    {
                        picDikenali.Image = (Bitmap)((new ImageConverter()).ConvertFrom(gambut));
                        elek.XPathSelectElement("//presensi_wajah/presensi_tercatat/id_presensi[text()='" + pres.Dapres + "']/../wajah_terlibat/id_kar[text()='" + idkenal + "']/../waktu_pulang").Value = DateTime.Now.ToString("T", CultureInfo.CreateSpecificCulture("es-ES"));
                        elek.Save("presensi_wajah.xml");
                        Notifikasi.Show("Waktu Pulang Telah Diperbaharui.", "Peringatan", 1500);
                    }
                }
                else
                {
                    Notifikasi.Show("Wajah tidak dikenali, Mohon Coba Lagi", "Peringatan", 1500);
                }
            }
        }
        #endregion

        private void frmPengenalanUtama_Load(object sender, EventArgs e)
        {
            timer1.Start();
            gambur = new TampilGambar();
            if (gambur.ProcVideo.IsOpened == false)
            {
                MessageBox.Show("Kamera tidak terdeteksi.");
                this.Close();
            }
            pres = new Presensi();
            datawaj = new Wajah();
            kenal = new Pengenalan();
            kotakTengah = new Rectangle(180, 90, 300, 300);
            kotakBatas = new Rectangle(120, 30, 240, 240);
            Application.Idle += LoadGambar;
        }

        private void tmrWaktuFoto_Tick(object sender, EventArgs e)
        {
            lblTimer.Visible = true;
            if (timerpoto != 0 && kotakTengah.Contains(kutak[0]))
            {
                lblTimer.Text = timerpoto.ToString();
                timerpoto--;
            }
            else if (timerpoto == 0 && kotakTengah.Contains(kutak[0]))
            {
                PencatatanPresensi();
                tmrtunggu.Start();
            }
            else if (!kotakTengah.Contains(kutak[0]))
            {
                lblTimer.Visible = false;
                tmrWaktuFoto.Stop();
                timerpoto = 1;
            }
        }

        private void tmrtunggu_Tick(object sender, EventArgs e)
        {
            lblTimer.Visible = true;
            if (tunggu != 0)
            {
                lblTimer.Text = tunggu.ToString();
                tunggu--;
            }
            else if (tunggu == 0)
            {
                lblTimer.Text = tunggu.ToString();
                lblTimer.Visible = false;
                tmrtunggu.Stop();
            }
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            Application.Idle -= LoadGambar;
            this.Hide();
            Form2 form = new Form2();
            form.ShowDialog();
            this.Show();
            Application.Idle += LoadGambar;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblWaktu.Text = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
