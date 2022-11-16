using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
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
using Emgu.CV;
using Emgu.CV.Structure;

namespace programTA
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        #region Atribut
        int x = 0;
        Presensi paras;
        Karyawan karya;
        Wajah tampWajah;
        string status;
        bool tercetak = false;
        ManipulasiArray manipul;
        Image<Gray, byte>[] wajahtrain;
        TampilGambar gambur;
        Rectangle kotakTengah = new Rectangle(180, 90, 300, 300);
        Rectangle kotakBatas = new Rectangle(120, 30, 240, 240);
        Rectangle[] kutak;
        Pengenalan kenal;
        int tunggu = 0, timerpoto = 1;
        #endregion
                
        #region Method Presensi
        private void LoadPresensi()
        {
            dgvPresensiHarian.Rows.Clear();
            dgvTotalPerOrang.Rows.Clear();
            lblWaktu.Text = DateTime.Now.ToString("HH:mm:ss");
            tampWajah = new Wajah();
            paras = new Presensi();
            karya = new Karyawan();

            if (paras.Dapres != null)
            {
                //pengaturan si datetimepicker
                dtpTanggalAwal.Value = DateTime.Parse(paras.TanggalPresensi()[1][0]);
                dtpTanggalAwal.MinDate = DateTime.Parse(paras.TanggalPresensi()[1][0]);
                dtpTanggalAkhir.MinDate = dtpTanggalAwal.Value.AddDays(+1);
                dtpTanggalAwal.MaxDate = dtpTanggalAkhir.Value;

                TampilanTotalWaktu(dtpTanggalAwal.Value, dtpTanggalAkhir.Value);
                TampilanPresensiHarian();
            }
            //pengaturan scroll
            dgvTotalWaktu.MouseWheel += new MouseEventHandler(dgvTotalWaktu_MouseWheel);
            dgvPresensiHarian.Focus();
        }

        private void TampilanPresensiHarian()
        {
            string[][] acak = paras.AmbilPresensiWajah(paras.Dapres);

            string[] a = acak[0]; //idpresensi
            string[] b = acak[1]; //wajahid dalam presensi
            string[] c = acak[2]; //status kedatangan peserta presens
            string[] d = acak[3]; //waktu kedatangan peserta presensi

            string[][] ambilwaj = karya.AmbilDataKaryawan();
            string[] g = ambilwaj[0]; //wajahid pada database wajah
            string[] f = ambilwaj[1]; //nama pada wajah id yang sesuai

            int m = 0;
            for (int k = 0; k < a.Length; k++)
            {
                if (a[k] == paras.Dapres) //jika id presensi pada presensi wajah sama dengan id yg dipassing
                {
                    for (int o = 0; o < b.Length; o++)
                    {
                        for (int n = 0; n < g.Length; n++)
                        {
                            if (b[o] == g[n])//jika orang e[n] terlibat dalam presensi
                            {
                                m++;
                                dgvPresensiHarian.Rows.Add();
                                dgvPresensiHarian.Rows[o].Cells[0].Value = m;
                                dgvPresensiHarian.Rows[o].Cells[1].Value = g[n];
                                dgvPresensiHarian.Rows[o].Cells[2].Value = f[n];
                                dgvPresensiHarian.Rows[o].Cells[3].Value = c[o];
                                dgvPresensiHarian.Rows[o].Cells[4].Value = d[o];
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void TampilanTotalWaktu(DateTime tglAwal, DateTime tglAkhir)
        {
            dgvTotalWaktu.Rows.Clear();
            dgvTotalPerOrang.Rows.Clear();
            dgvTotalWaktu.Columns.Clear();
            string[][] tanggal = paras.TanggalPresensi();

            #region Cetak Kolom
            dgvTotalWaktu.Columns.Add("Column0", "No");
            dgvTotalWaktu.Columns["Column0".ToString()].Width = 30;
            dgvTotalWaktu.Columns.Add("Column1", "ID");
            dgvTotalWaktu.Columns["Column1".ToString()].Width = 50;
            dgvTotalWaktu.Columns.Add("Column2", "Nama");
            dgvTotalWaktu.Columns["Column2".ToString()].Width = 100;
            for (int i = 0; i < tanggal[0].Length; i++)
            {
                if (DateTime.Parse(tanggal[1][i]) >= tglAwal.Date && tglAkhir.Date >= DateTime.Parse(tanggal[1][i]))
                {
                    dgvTotalWaktu.Columns.Add("Column" + (i + 3).ToString(), tanggal[1][i]);
                    dgvTotalWaktu.Columns["Column" + (i + 3).ToString()].Width = 70;
                }
            }
            #endregion

            #region tampung isi tabel
            string[][][] bla = paras.SemuaPresensiWajah();
            string[][] b = bla[0]; //tampung semua id karyawan pada semua presensi
            string[][] c = bla[1]; //tampung semua waktu datang
            string[][] d = bla[2]; //tampung semua waktu pulang

            string[][] ambilwaj = karya.AmbilDataKaryawan();
            string[] g = ambilwaj[0]; //wajahid pada database wajah
            string[] f = ambilwaj[1]; //nama pada wajah id yang sesuai
            #endregion

            int num = 0;
            int totcet = 0;
            TimeSpan tot = new TimeSpan();
            string[] semorg = tampWajah.AmbilIDWajah(); //menampung setiap orang yang sudah terdaftar wajahny

            #region menampilkan nilai pada tabel
            for (int i = 0; i < semorg.Length; i++) // mengulang sejumlah smw id yang telah terdaftar d presesi
            {
                for (int j = 0; j < g.Length; j++) // mengulang sebanyak id pada tabel karyawan
                {
                    if (i == 2)
                    {

                    }
                    if (semorg[i] == g[j]) //untuk 1 baris
                    {
                        num++;
                        dgvTotalPerOrang.Rows.Add();
                        dgvTotalWaktu.Rows.Add();
                        dgvTotalWaktu.Rows[i].Cells[0].Value = num;
                        dgvTotalWaktu.Rows[i].Cells[1].Value = g[j];
                        dgvTotalWaktu.Rows[i].Cells[2].Value = f[j];
                        for (int k = 0; k < tanggal[0].Length; k++) // untuk setiap presensi pada database
                        { // dilakukan per kolom
                            if (b.Length != 0)
                            {
                                if (DateTime.Parse(tanggal[1][k]) >= tglAwal.Date && tglAkhir.Date >= DateTime.Parse(tanggal[1][k]))
                                {
                                    int cek = 0;
                                    for (int m = 0; m < b[k].Length; m++) //untuk setiap id karyawan pada presensi k
                                    {
                                        if (b[k][m] == g[j] && c[k][m] != "-" && d[k][m] != "-") //jika orang tsb memiliki waktu datang dan waktu plg
                                        {
                                            TimeSpan duration = DateTime.Parse(d[k][m]).Subtract(DateTime.Parse(c[k][m]));
                                            tot += duration;
                                            dgvTotalWaktu.Rows[i].Cells[totcet + 3].Value = Math.Round(double.Parse(duration.TotalHours.ToString()), 0) + " Jam"; //cetak tabel dgv
                                            totcet++;
                                            break;
                                        }
                                        else if (b[k][m] == g[j] && (c[k][m] == "-" || d[k][m] == "-"))
                                        {
                                            dgvTotalWaktu.Rows[i].Cells[totcet + 3].Value = "-"; //cetak tabel dgv
                                            totcet++;
                                            break;
                                        }
                                        else if (b[k][m] != g[j])
                                        {
                                            cek++;
                                        }
                                        if (cek == b[k].Length)
                                        {
                                            dgvTotalWaktu.Rows[i].Cells[totcet + 3].Value = "-"; //cetak tabel dgv
                                            totcet++;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        totcet = 0;
                        dgvTotalPerOrang.Rows[i].Cells[0].Value = Math.Round(double.Parse(tot.TotalHours.ToString()),0);
                        tot = new TimeSpan();
                        break;
                    }
                }
            }
            #endregion

            //for (int i = semorg.Length; i < 50; i++)
            //{
            //    dgvTotalPerOrang.Rows.Add();
            //    dgvTotalWaktu.Rows.Add();
            //    dgvTotalWaktu.Rows[i].Cells[2].Value = "cell coba";
            //    dgvTotalPerOrang.Rows[i].Cells[0].Value = "totcoba";
            //}
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void XLSXTotalWaktu()
        {
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //cetak header
            for (int i = 1; i <= dgvTotalWaktu.Columns.Count; i++)
            {
                if (i == 1)
                {
                    xlWorkSheet.Columns[i].ColumnWidth = 3; //set lebar kolom
                }
                else if (i == 2)
                {
                    xlWorkSheet.Columns[i].ColumnWidth = 7;
                }
                else if (i == 3)
                {
                    xlWorkSheet.Columns[i].ColumnWidth = 20;
                }
                else if (i > 3)
                {
                    xlWorkSheet.Columns[i].ColumnWidth = 10;
                }
                xlWorkSheet.Cells[1, i] = dgvTotalWaktu.Columns[i - 1].HeaderText;
            }
            xlWorkSheet.Cells[1, dgvTotalWaktu.Columns.Count + 1] = dgvTotalPerOrang.Columns[0].HeaderText;
            xlWorkSheet.Columns[dgvTotalWaktu.Columns.Count + 1].ColumnWidth = 11;

            //cetak data
            for (int i = 0; i < dgvTotalWaktu.Rows.Count - 1; i++)
            {
                for (int j = 0; j < dgvTotalWaktu.Columns.Count; j++)
                {
                    if (dgvTotalWaktu.Rows[i].Cells[j].Value != null)
                    {
                        xlWorkSheet.Cells[i + 2, j + 1] = dgvTotalWaktu.Rows[i].Cells[j].Value.ToString();
                    }
                    else
                    {
                        xlWorkSheet.Cells[i + 2, j + 1] = "-";
                    }
                }
                if (dgvTotalWaktu.Rows[i].Cells[0].Value != null)
                {
                    xlWorkSheet.Cells[i + 2, dgvTotalWaktu.Columns.Count + 1] = dgvTotalPerOrang.Rows[i].Cells[0].Value.ToString();
                }
                else
                {
                    xlWorkSheet.Cells[i + 2, dgvTotalWaktu.Columns.Count + 1] = "00:00:00";
                }
            }

            xlWorkBook.SaveAs("TW_" + DateTime.Parse(dgvTotalWaktu.Columns[3].HeaderText)
                .ToString("D", CultureInfo.CreateSpecificCulture("id-ID")) + "_"
                + DateTime.Parse(dgvTotalWaktu.Columns[dgvTotalWaktu.Columns.Count - 1].HeaderText)
                .ToString("D", CultureInfo.CreateSpecificCulture("id-ID")) + ".xls"
                , Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue
                , misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,
                misValue, misValue, misValue, misValue, misValue); //simpan data di dokumen
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

        }
        #endregion

        #region Method Tambah Data Training
        private void LoadGambar(object sender, EventArgs e)
        {
            try
            {
                gambur.GambAsli = gambur.ProcVideo.QueryFrame().ToImage<Bgr, byte>();
                gambur.GambSem = gambur.GambAsli;
                gambur.GambGray = gambur.GambSem.Convert<Gray, byte>();
                gambur.GambSem.Draw(kotakTengah, new Bgr(0, 200, 0), 2);
                picCam.Image = gambur.GambSem.ToBitmap();
                Rectangle[] kutak = gambur.CasClass.DetectMultiScale(gambur.GambGray, 1.1, 10, Size.Empty);
                if (kutak != null)
                {
                    foreach (var face in kutak)
                    {
                        gambur.GambSem.Draw(face, new Bgr(), 2); //the detected face(s) is highlighted here using a box that is drawn around it/them
                        picCam.Image = gambur.GambSem.ToBitmap();
                        gambur.GambKrop = gambur.GambAsli; //menampung gambar hasil krop
                        gambur.GambKrop.ROI = kutak[0]; // krop gambar
                        picCapture.Image = gambur.GambKrop.ToBitmap();
                        if (kotakTengah.Contains(kutak[0]) && kutak[0].Width > kotakBatas.Width)
                        {
                            btnCapture.Enabled = true;
                            lblingatingat.Visible = false;
                        }
                        else if (kotakTengah.Contains(kutak[0]) && kutak[0].Width < kotakBatas.Width)
                        {
                            btnCapture.Enabled = false;
                            lblingatingat.Visible = true;
                        }
                        else if (!kotakTengah.Contains(kutak[0]))
                        {
                            btnCapture.Enabled = false;
                            lblingatingat.Visible = false;
                            picCapture.Image = null;
                        }
                    }
                }
            }
            catch
            {
                //Application.Idle -= LoadGambar;
                //Notifikasi.Show("Gambar yang tertangkap terlalu gelap.", "Peringatan", 1000);
            }
        }//melakukan load gambar

        private void LoadDataTrain()
        {
            karya = new Karyawan();
            manipul = new ManipulasiArray();
            wajahtrain = new Image<Gray, byte>[10];
            tampWajah = new Wajah();
            cmbIDKaryawan.Enabled = false;
            btnSimpan.Enabled = false;
            btnUndo.Enabled = false;
            IsiComboBox();
            cmbIDKaryawan.SelectedIndex = 0;
            btnCapture.Enabled = false;
        }

        private void IsiComboBox()
        {
            cmbIDKaryawan.Items.Clear();
            cmbIDKaryawan.Items.Add("id karyawan");
            string[] idan = karya.AmbilDataKaryawan()[0];
            string[] idwj = tampWajah.AmbilIDWajah();
            for (int i = 0; i < idan.Length; i++)
            {
                bool s = false;
                for (int j = 0; j < idwj.Length; j++)
                {
                    if (idan[i] == idwj[j])
                    {
                        s = true;
                    }
                }
                if (s == false)
                {
                    cmbIDKaryawan.Items.Add(idan[i]);
                }
            }
        }
        
        #endregion

        #region Method Uji Pengenalan
        private void LoadGambarUji(object sender, EventArgs e)
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
            catch
            {
                //Application.Idle -= LoadGambarUji;
                //Notifikasi.Show("Gambar yang tertangkap terlalu gelap.", "Peringatan", 1000);
            }
        }

        private void LoadPengujian()
        {
            karya = new Karyawan();
            paras = new Presensi();
            tampWajah = new Wajah();
            kenal = new Pengenalan();
        }

        private void TampilEuc()
        {
            dgvPengenalan.Rows.Clear();
            string[] datwaj = tampWajah.AmbilIDWajah();
            string[][] datakar = karya.AmbilDataKaryawan();
            int k = 0, urut = 1;
            for (int i = 0; i < kenal.NilaiEuc.Length / 10; i++)
            {
                double nilaiKecil = kenal.NilaiEuc[k];
                dgvPengenalan.Rows.Add();
                dgvPengenalan.Rows[i].Cells[0].Value = urut;
                urut++;
                for (int m = 0; m < datakar[0].Length; m++)
                {
                    if (datwaj[i] == datakar[0][m])
                    {
                        dgvPengenalan.Rows[i].Cells[1].Value = datakar[0][m];
                        dgvPengenalan.Rows[i].Cells[2].Value = datakar[1][m];
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    if (nilaiKecil > kenal.NilaiEuc[j + k]) { nilaiKecil = kenal.NilaiEuc[j + k]; }
                }
                dgvPengenalan.Rows[i].Cells[3].Value = nilaiKecil;
                k += 10;
            }
        } //menampilkan nilai euc terkecil
        
        private void TampilRatEuc()
        {
            dgvPengenalan.Rows.Clear();
            string[] datwaj = tampWajah.AmbilIDWajah();
            string[][] datakar = karya.AmbilDataKaryawan();
            int k = 0, urut = 1;
            for (int i = 0; i < kenal.RataEuclid().Length; i++)
            {
                double nilaiKecil = kenal.RataEuclid()[k];
                dgvPengenalan.Rows.Add();
                dgvPengenalan.Rows[i].Cells[0].Value = urut;
                urut++;
                for (int m = 0; m < datakar[0].Length; m++)
                {
                    if (datwaj[i] == datakar[0][m])
                    {
                        dgvPengenalan.Rows[i].Cells[1].Value = datakar[0][m];
                        dgvPengenalan.Rows[i].Cells[2].Value = datakar[1][m];
                    }
                }
                dgvPengenalan.Rows[i].Cells[3].Value = kenal.RataEuclid()[i];
                k++;
            }
        }
        #endregion

        private void Form2_Load(object sender, EventArgs e)
        {
            //tampWajah = new Wajah();
            //tampWajah.UpdateEigenface();
            //MessageBox.Show("udah");
            timer1.Start();
            gambur = new TampilGambar();
            try
            {
                XmlReader ra = XmlReader.Create("wajah_training.xml");
                while (ra.Read())
                {
                    if (ra.IsStartElement() && ra.Name == "wajahpemilik")
                    {
                        x++;
                    }
                }
                ra.Close(); //mengecek apakah tabel data training tidak kosong
            }
            catch
            {
                XmlWriter writer = XmlWriter.Create("wajah_training.xml");
                writer.WriteStartDocument();
                writer.WriteStartElement("wajah");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            if (x != 0)
            {
                tbcMenuAdmin.SelectedIndex = 0;
                LoadPresensi();
            }
            else
            {
                tbcMenuAdmin.SelectedIndex = 1;
                Application.Idle -= LoadGambarUji;
                Application.Idle += LoadGambar;
            }
        }

        private void tbcMenuAdmin_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlReader ra = XmlReader.Create("wajah_training.xml");
            while (ra.Read())
            {
                if (ra.IsStartElement() && ra.Name == "wajahpemilik")
                {
                    x++;
                }
            }
            if (tbcMenuAdmin.SelectedIndex == 0)
            {
                if (x != 0)
                {
                    LoadPresensi();
                    Application.Idle -= LoadGambar;
                    Application.Idle -= LoadGambarUji;
                }
                else
                {
                    MessageBox.Show("Belum ada wajah pada database, mohon melakukan penambahan data training");
                    tbcMenuAdmin.SelectedIndex = 1;
                }
            }
            else if (tbcMenuAdmin.SelectedIndex == 1)
            {
                Application.Idle -= LoadGambarUji;
                Application.Idle += LoadGambar;
                LoadDataTrain();
            }
            else if (tbcMenuAdmin.SelectedIndex == 2)
            {
                if (x != 0)
                {
                    dgvPengenalan.Rows.Clear();
                    Application.Idle -= LoadGambar;
                    Application.Idle += LoadGambarUji;
                    LoadPengujian();
                }
                else
                {
                    MessageBox.Show("Belum ada wajah pada database, mohon melakukan penambahan data training");
                    tbcMenuAdmin.SelectedIndex = 1;
                }
            }
        }

        #region Komponen Presensi
        private void btnKembali1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            tmrCekTutup.Stop();
            tmrCetak.Stop();
            tmrtunggu.Stop();
            tmrWaktuFoto.Stop();
            this.DialogResult = DialogResult.OK;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblWaktu.Text = DateTime.Now.ToString("HH:mm:ss");
            lblTanggal.Text = DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("id-ID"));
        }

        private void tmrCekTutup_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.TimeOfDay.Hours >= new TimeSpan(17, 0, 0).Hours)
            {
                tmrCekTutup.Stop();
                tercetak = true;
            }
        }

        private void btnTutupPresensi_Click(object sender, EventArgs e)
        {
            paras.PresensiAktif();
            XDocument elek = XDocument.Load("presensi.xml");
            elek.XPathSelectElement("//presensi/presensi_harian/id_presensi[text()='" + paras.Dapres + "']/../status_presensi").Value = "tutup";
            elek.Save("presensi.xml"); // merubah status buka menjadi tutup
            paras.BuatPDF(paras.Dapres);
            Notifikasi.Show("file PDF telah dibuat", "Notifikasi", 1500);
        }

        private void dtpTanggalAwal_ValueChanged(object sender, EventArgs e)
        {
            TampilanTotalWaktu(dtpTanggalAwal.Value.Date, dtpTanggalAkhir.Value.Date);
            dtpTanggalAkhir.MinDate = dtpTanggalAwal.Value;
            dtpTanggalAwal.MaxDate = dtpTanggalAkhir.Value;
        }

        private void dtpTanggalAkhir_ValueChanged(object sender, EventArgs e)
        {
            TampilanTotalWaktu(dtpTanggalAwal.Value, dtpTanggalAkhir.Value);
            dtpTanggalAkhir.MinDate = dtpTanggalAwal.Value.AddDays(+1);
            dtpTanggalAwal.MaxDate = dtpTanggalAkhir.Value;
        }

        private void tmrCetak_Tick(object sender, EventArgs e)
        {
            if (tercetak == true)
            {
                tmrCetak.Stop();
                paras.PresensiAktif();
                MessageBox.Show("Presensi telah ditutup dan pdf telah dibuat.");
                tercetak = false;
            }
        }

        void dgvTotalWaktu_MouseWheel(object sender, MouseEventArgs e)
        {
            int currentIndex = this.dgvTotalWaktu.FirstDisplayedScrollingRowIndex;
            int scrollLines = SystemInformation.MouseWheelScrollLines;

            if (e.Delta > 0)
            {
                this.dgvTotalWaktu.FirstDisplayedScrollingRowIndex
                    = Math.Max(0, currentIndex - scrollLines);
            }
            else if (e.Delta < 0)
            {
                this.dgvTotalWaktu.FirstDisplayedScrollingRowIndex
                    = currentIndex + scrollLines;
            }
            dgvTotalPerOrang.FirstDisplayedScrollingRowIndex = dgvTotalWaktu.FirstDisplayedScrollingRowIndex;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                dgvPresensiHarian.Focus();
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                dgvTotalWaktu.Focus();
            }
        }

        private void dgvTotalPerOrang_Click(object sender, EventArgs e)
        {
            dgvTotalWaktu.Focus();
        }

        private void dtpTanggalAwal_CloseUp(object sender, EventArgs e)
        {
            dgvTotalWaktu.Focus();
        }

        private void dtpTanggalAkhir_CloseUp(object sender, EventArgs e)
        {
            dgvTotalWaktu.Focus();
        }

        private void dgvTotalPerOrang_Scroll(object sender, ScrollEventArgs e)
        {
            dgvTotalWaktu.FirstDisplayedScrollingRowIndex = dgvTotalPerOrang.FirstDisplayedScrollingRowIndex;
        }

        private void btnBuatXLSX_Click(object sender, EventArgs e)
        {
            XLSXTotalWaktu();
            Notifikasi.Show("file Excel Telah tersimpan pada folder dokumen", "Peringatan", 2000);
        }
        #endregion

        #region komponen Tambah Data Training
        private void btnKembali2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            tmrCekTutup.Stop();
            tmrCetak.Stop();
            tmrtunggu.Stop();
            tmrWaktuFoto.Stop();
            Application.Idle -= LoadGambar;
            this.DialogResult = DialogResult.OK;
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            btnUndo.Enabled = true;
            if (picWajTrain1.Image == null)
            {
                picWajTrain1.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[0] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain2.Image == null)
            {
                picWajTrain2.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[1] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain3.Image == null)
            {
                picWajTrain3.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[2] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain4.Image == null)
            {
                picWajTrain4.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[3] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain5.Image == null)
            {
                picWajTrain5.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[4] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain6.Image == null)
            {
                picWajTrain6.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[5] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain7.Image == null)
            {
                picWajTrain7.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[6] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain8.Image == null)
            {
                picWajTrain8.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[7] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain9.Image == null)
            {
                picWajTrain9.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[8] = new Image<Gray, byte>(bip);
            }
            else if (picWajTrain10.Image == null)
            {
                picWajTrain10.Image = picCapture.Image;
                Bitmap bip = manipul.ResizeImage(picCapture.Image, 100, 100);
                wajahtrain[9] = new Image<Gray, byte>(bip);
                cmbIDKaryawan.Enabled = true;
                btnCapture.Enabled = false;
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            cmbIDKaryawan.Enabled = false;
            btnSimpan.Enabled = false;
            btnCapture.Enabled = true;
            if (picWajTrain10.Image != null)
            {
                picWajTrain10.Image = null;
                wajahtrain[9] = null;
            }
            else if (picWajTrain9.Image != null)
            {
                picWajTrain9.Image = null;
                wajahtrain[8] = null;
            }
            else if (picWajTrain8.Image != null)
            {
                picWajTrain8.Image = null;
                wajahtrain[7] = null;
            }
            else if (picWajTrain7.Image != null)
            {
                picWajTrain7.Image = null;
                wajahtrain[6] = null;
            }
            else if (picWajTrain6.Image != null)
            {
                picWajTrain6.Image = null;
                wajahtrain[5] = null;
            }
            else if (picWajTrain5.Image != null)
            {
                picWajTrain5.Image = null;
                wajahtrain[4] = null;
            }
            else if (picWajTrain4.Image != null)
            {
                picWajTrain4.Image = null;
                wajahtrain[3] = null;
            }
            else if (picWajTrain3.Image != null)
            {
                picWajTrain3.Image = null;
                wajahtrain[2] = null;
            }
            else if (picWajTrain2.Image != null)
            {
                picWajTrain2.Image = null;
                wajahtrain[1] = null;
            }
            else if (picWajTrain1.Image != null)
            {
                picWajTrain1.Image = null;
                wajahtrain[0] = null;
                btnUndo.Enabled = false;
            }
        }

        private void cmbIDKaryawan_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbIDKaryawan.SelectedIndex != 0)
            {
                btnSimpan.Enabled = true;
            }
            else
            {
                btnSimpan.Enabled = false;
            }
        }

        private void btnBuatDatabase_Click(object sender, EventArgs e)
        {
            XmlWriter writer = XmlWriter.Create("wajah_training.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("wajah");
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            MessageBox.Show("Database Telah Terbuat!");
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            tampWajah.TambahDatabase(wajahtrain, cmbIDKaryawan.SelectedItem.ToString()); //penambahan data ke database
            tampWajah.UpdateEigenface();

            #region Cleaning
            cmbIDKaryawan.Enabled = false;
            btnSimpan.Enabled = false;
            btnCapture.Enabled = true;
            wajahtrain = new Image<Gray, byte>[10];
            picWajTrain1.Image = null;
            picWajTrain2.Image = null;
            picWajTrain3.Image = null;
            picWajTrain4.Image = null;
            picWajTrain5.Image = null;
            picWajTrain6.Image = null;
            picWajTrain7.Image = null;
            picWajTrain8.Image = null;
            picWajTrain9.Image = null;
            picWajTrain10.Image = null;
            IsiComboBox();
            cmbIDKaryawan.SelectedIndex = 0;
            #endregion

            MessageBox.Show("Wajah Telah Ditambahkan!");
        }

        private void btnTambahKaryawan_Click(object sender, EventArgs e)
        {
            frmTambahKaryawan firm = new frmTambahKaryawan();
            firm.ShowDialog();
            IsiComboBox();
        }

        private void btnListWajah_Click(object sender, EventArgs e)
        {
            frmListTerdaftar est = new frmListTerdaftar();
            est.ShowDialog();
        }
        #endregion

        #region komponen Uji Pengenalan
        private void btnKembali3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            tmrCekTutup.Stop();
            tmrCetak.Stop();
            tmrtunggu.Stop();
            tmrWaktuFoto.Stop();
            tunggu = 2;
            timerpoto = 1;
            Application.Idle -= LoadGambarUji;
            this.DialogResult = DialogResult.OK;
        }
        
        private void tmrWaktuFoto_Tick(object sender, EventArgs e)
        {
            lblTimer.Visible = true;
            try
            {
                if (timerpoto != 0 && kotakTengah.Contains(kutak[0]))
                {
                    lblTimer.Text = timerpoto.ToString();
                    timerpoto--;
                }
                else if (timerpoto == 0 && kotakTengah.Contains(kutak[0]))
                {
                    lblTimer.Text = timerpoto.ToString();
                    tmrWaktuFoto.Stop();
                    timerpoto = 1;
                    tunggu = 2;
                    paras = new Presensi();
                    byte[] gambut;
                    int nowajah = 0;
                    Stopwatch wa = new Stopwatch();
                    wa.Start();
                    int idkenal = kenal.Kenali(gambur.GambKrop.ToBitmap(),out gambut, out nowajah); //wajahid yang terlibat pada presensi
                    wa.Stop();
                    if (nowajah != 0)
                    {
                        picDikenali.Image = (Bitmap)((new ImageConverter()).ConvertFrom(gambut));
                        TampilEuc();
                        //TampilRatEuc();
                        Notifikasi.Show("Waktu Dikenali selama " + wa.ElapsedMilliseconds.ToString() + " ms, dikenali sebagai " + idkenal, "Berhasil", 2000);
                    }
                    else // jika tidak memenuhi threshold
                    {
                        Notifikasi.Show("Wajah Tidak dikenali, Mohon Coba Lagi!", "Peringatan", 1500);
                    }
                    tmrtunggu.Start();
                }
                else if (!kotakTengah.Contains(kutak[0]))
                {
                    lblTimer.Visible = false;
                    tmrWaktuFoto.Stop();
                    timerpoto = 1;
                }
            }
            catch
            {
                lblTimer.Visible = false;
                tmrWaktuFoto.Stop();
                timerpoto = 1;
            }
        }

        private void tmrtunggu_Tick(object sender, EventArgs e)
        {
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
        #endregion
    }
}
