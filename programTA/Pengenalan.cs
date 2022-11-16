using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Accord.Imaging.Converters;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using Accord.Imaging.Filters;
using Emgu.CV;
using Emgu.CV.Structure;

namespace programTA
{
    class Pengenalan
    {
        private Karyawan karya;
        private Wajah ambilWajah;
        private ManipulasiArray manipul;
        private double[] nilaiEuc;
        
        public double[] NilaiEuc
        {
            set { nilaiEuc = value; }
            get { return nilaiEuc; }
        }

        #region Constructor
        public Pengenalan()
        {
            karya = new Karyawan();
            manipul = new ManipulasiArray();
            ambilWajah = new Wajah();
        }
        #endregion

        #region method
        public int Kenali(Bitmap gumb, out byte[] tampWajah, out int simpulwajah)
        {
            Bitmap bip = manipul.ResizeImage(gumb, 100, 100);
            ImageConverter imgCon = new ImageConverter();
            byte[] gambi = (byte[])imgCon.ConvertTo(bip, typeof(byte[]));
            simpulwajah = ujiPCA(ambilWajah.NilaiEigen, gambi, ambilWajah.RateEig, ambilWajah.TrixNorm);

            //pake setiap wajah
            int asal = 0;
            tampWajah = new byte[10]; //inisialisasi
            int idwajpilih = 0;

            for (int i = 0; i < ambilWajah.ambilGambData().GetLength(0); i++)
            {
                for (int j = 0; j < ambilWajah.ambilGambData().GetLength(1); j++)
                {
                    asal++;
                    if (asal == simpulwajah + 1)
                    {
                        idwajpilih = int.Parse(ambilWajah.AmbilIDWajah()[i]); //id yang terpilih
                        tampWajah = ambilWajah.ambilGambData()[i, j];// gambar yang dipilih
                        break;
                    }
                }
                if (asal == simpulwajah + 1)
                {
                    break;
                }
            }
            return idwajpilih;
        }

        public int Kenali(Bitmap gumb, out int simpulwajah)
        {
            Bitmap bip = manipul.ResizeImage(gumb, 100, 100);
            ImageConverter imgCon = new ImageConverter();
            byte[] gambi = (byte[])imgCon.ConvertTo(bip, typeof(byte[]));
            simpulwajah = ujiPCA(ambilWajah.NilaiEigen, gambi, ambilWajah.RateEig, ambilWajah.TrixNorm);

            //pake rata2 wajah per orang
            double[] ratwaj = RataEuclid();
            int idwajpilih = 0;
            double pendek = 0;
            for (int i = 0; i < ratwaj.Length; i++)
            {
                if (i == 0)
                {
                    pendek = ratwaj[i];
                    idwajpilih = int.Parse(ambilWajah.AmbilIDWajah()[i]); //id yang terpilih
                }
                if (pendek > ratwaj[i])
                {
                    pendek = ratwaj[i];
                    idwajpilih = int.Parse(ambilWajah.AmbilIDWajah()[i]); //id yang terpilih
                }
            }
            return idwajpilih;
        }
        
        private int ujiPCA(double[,] eigface, byte[] uji, double[] rata, double[][] subeig)
        {
            eigface = manipul.TransposeRowsAndColumn(eigface);// melakukan transpose array eigenface untuk dikalikan dengan nilai wajah yang dituju

            //projeksi gambar matriks untuk semua gambar di array eigface
            double[,] projekimg = new double[eigface.GetLength(0), subeig[0].Length];
            for (int i = 0; i < eigface.GetLength(0); i++)
            {
                for (int j = 0; j < subeig[0].Length; j++)
                {
                    for (int k = 0; k < eigface.GetLength(1); k++)
                    {
                        projekimg[i, j] += eigface[i, k] * subeig[k][j];
                    }
                }
            }

            Bitmap bmp = (Bitmap)((new ImageConverter()).ConvertFrom(uji));
            bmp = new Image<Gray, byte>(bmp).ToBitmap();
            HistogramEqualization filt = new HistogramEqualization();
            bmp = filt.Apply(bmp);
            
            ImageToMatrix immat = new ImageToMatrix(); byte[][] matrixWajah;
            immat.Convert(bmp, out matrixWajah);
            double[] dobyte = new double[matrixWajah.GetLength(0) * matrixWajah[0].Length]; //penyimpan array byte sementara
            int bla = 0;
            // merubah tipe data byte ke double tanpa merubah nilai dan dimasukkan ke array double
            for (int k = 0; k < matrixWajah.GetLength(0); k++)
            {
                for (int m = 0; m < matrixWajah[0].Length; m++)
                {
                    dobyte[bla] = matrixWajah[k][m];
                    bla++;
                }
            }

            //melakukan normalisasi dengan substraksi untuk array uji dengan menggunakan rata2 dari train PCA
            double[,] dodo = manipul.Make2DArray(dobyte, dobyte.Length, 1);
            double[][] substrak = new double[dodo.GetLength(0)][];
            for (int i = 0; i < dodo.GetLength(0); i++)
            {
                substrak[i] = new double[dodo.GetLength(1)];
                for (int j = 0; j < dodo.GetLength(1); j++)
                {
                    substrak[i][j] = dodo[i, j] - rata[i];
                }
            }

            //melakukan projeksi dengan perkalian wajah uji dengan semua gambar eigenface
            double[,] projeksi = new double[eigface.GetLength(0), substrak[0].Length];
            for (int i = 0; i < eigface.GetLength(0); i++)
            {
                for (int j = 0; j < substrak[0].Length; j++)
                {
                    for (int k = 0; k < eigface.GetLength(1); k++)
                    {
                        projeksi[i, j] += eigface[i, k] * substrak[k][j];
                    }
                }
            }

            //penghitungan euclidean distance
            double jarakpendek = 0; int wajahmirip = 0;
            double[] eucdis = new double[projekimg.GetLength(1)];
            nilaiEuc = new double[projekimg.GetLength(1)];
            double threshold = 36000000;
            for (int i = 0; i < projekimg.GetLength(1); i++)
            {
                double[] arrtemp = new double[projekimg.GetLength(0)]; //menampung hasil projeksi tiap gambar
                for (int j = 0; j < projekimg.GetLength(0); j++)
                {
                    arrtemp[j] = projekimg[j, i];
                }
                eucdis[i] = Distance.Euclidean(manipul.Conv1dArray(projeksi), arrtemp);
                nilaiEuc[i] = eucdis[i]; //nilai ditampung buat ditampilkan
                //melakukan perbandingan wajah yang mirip
                if (i == 0)
                {
                    jarakpendek = eucdis[i];
                    wajahmirip = i;
                }
                else
                {
                    if (jarakpendek > eucdis[i])
                    {
                        jarakpendek = eucdis[i];
                        wajahmirip = i;
                    }
                }
            }
            if (jarakpendek > threshold)
            {
                wajahmirip = 0;
            }
            return wajahmirip;
        } //mencari kesimpulan kemiripan wajah
        
        public double[] RataEuclid()
        {
            double[] rataeuc = new double[nilaiEuc.Length / 10];
            int bebe = 0;
            for (int i = 0; i < nilaiEuc.Length / 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    rataeuc[i] += nilaiEuc[bebe];
                    bebe++;
                }
                rataeuc[i] = rataeuc[i] / nilaiEuc.Length / 10;
            }
            return rataeuc;
        }
        #endregion
    }
}
