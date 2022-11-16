using System;
using System.Xml;
using System.Drawing;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Accord.Imaging.Converters;
using Accord.Math;
using Accord.Math.Decompositions;
using Accord.Statistics;
using Accord.Imaging.Filters;
using Spire.Xls;

namespace programTA
{
    class Wajah
    {
        private ManipulasiArray manipul;
        private double[,] nilaieigen;
        private double[][] trixNorm;
        private double[] rataeig;
        private byte[,] arrgumar;

        #region Properties
        public double[,] NilaiEigen
        {
            set { nilaieigen = value; }
            get { return nilaieigen; }
        }

        public double[][] TrixNorm
        {
            set { trixNorm = value; }
            get { return trixNorm; }
        }

        public double[] RateEig
        {
            set { rataeig = value; }
            get { return rataeig; }
        }
        #endregion

        public Wajah()
        {
            manipul = new ManipulasiArray();
            try
            {
                XmlReader ra = XmlReader.Create("eigenface.xml");
                AmbilEigen();
            }
            catch { }
        }

        #region method
        public string[] AmbilIDWajah()
        {
            List<string> idkar = new List<string>();
            try
            {
                using (XmlReader r = XmlReader.Create("wajah_training.xml"))
                {
                    while (r.Read())
                    {
                        if (r.IsStartElement())
                        {
                            switch (r.Name)
                            {
                                case "id_kar":
                                    if (r.Read())
                                    {
                                        idkar.Add(r.Value);
                                    }
                                    break;
                            }
                        }
                    }
                }
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
            return idkar.ToArray();
        }

        public byte[,][] ambilGambData()
        {
            int a = 0;
            List<byte[]> wajah = new List<byte[]>();
            List<int> wajudi = new List<int>();
            XmlReader r = XmlReader.Create("wajah_training.xml");
            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "id_kar":
                            if (r.Read())
                            {
                                a++;
                            }
                            break;
                        case "gambar":
                            if (r.Read())
                            {
                                wajah.Add(Convert.FromBase64String(r.Value));
                            }
                            break;
                    }
                }
            }
            r.Close();
            int b = 0;
            byte[][] gambgamb = wajah.ToArray();
            byte[,][] allGamb = new byte[a, wajah.ToArray().Length / a][];
            for (int i = 0; i < allGamb.GetLength(0); i++)
            {
                for (int j = 0; j < allGamb.GetLength(1); j++)
                {
                    allGamb[i, j] = gambgamb[b];
                    b++;
                }
            }
            return allGamb;
        } //mengambil gambar wajah pada database

        private void AmbilEigen()
        {
            List<double> rato = new List<double>();
            int bareig = 0, koleig = 0; int barnor = 0, kolnor = 0;

            //ambil nilai rata2 dan nilai dimensi array eigenface dan matrix normalisasi
            XmlReader r = XmlReader.Create("eigenface.xml");
            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "barisEigen":
                            if (r.Read()) { bareig = int.Parse(r.Value); }
                            break;
                        case "kolomEigen":
                            if (r.Read()) { koleig = int.Parse(r.Value); }
                            break;
                        case "indeksMean":
                            if (r.Read())
                            {
                                rato.Add(double.Parse(r.Value));
                            }
                            break;
                        case "barisNormal":
                            if (r.Read()) { barnor = int.Parse(r.Value); }
                            break;
                        case "kolomNormal":
                            if (r.Read()) { kolnor = int.Parse(r.Value); }
                            break;

                    }
                }
            }
            r.Close();
            nilaieigen = new double[bareig, koleig];
            double[,] suba = new double[barnor, kolnor];
            int m = 0, q = 0, n = 0, s = 0;
            XmlReader re = XmlReader.Create("eigenface.xml");
            while (re.Read())
            {
                if (re.IsStartElement())
                {
                    switch (re.Name)
                    {
                        case "indeksEigen":
                            if (re.Read())
                            {
                                nilaieigen[n, s] = double.Parse(re.Value);
                                s++;
                                if (s == koleig) { s = 0; n++; }
                            }
                            break;
                        case "indeksNorm":
                            if (re.Read())
                            {
                                suba[m, q] = double.Parse(re.Value);
                                q++;
                                if (q == kolnor) { q = 0; m++; }
                            }
                            break;
                    }
                }
            }
            re.Close();
            trixNorm = manipul.ToJaggedArray(suba);
            rataeig = rato.ToArray();
        } //mengambil nilai eigen pada database

        public bool CekIDWajah(string idkar)
        {
            bool yak = false;
            XmlReader r = XmlReader.Create("wajah_training.xml");
            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "id_kar":
                            if (r.Read())
                            {
                                if (idkar == r.Value)
                                {
                                    yak = true;
                                }
                            }
                            break;
                    }
                }
            }
            r.Close();
            return yak;
        }

        public void TambahDatabase(Image<Gray, byte>[] wajahtrain, string idWajah)
        {

            byte[][,,] test = new byte[wajahtrain.Length][,,];
            ImageConverter imgConv = new ImageConverter();// untuk convert gambar ke byte[]
            XmlDocument doku = new XmlDocument();
            doku.Load("wajah_training.xml");//membuak dokumn wajah training

            XmlNode node = doku.DocumentElement;
            XmlNode nodepemilik = doku.CreateElement("wajahpemilik");
            XmlNode wajahid = doku.CreateElement("id_kar");
            wajahid.InnerText = idWajah;
            node.AppendChild(nodepemilik);
            nodepemilik.AppendChild(wajahid);
            for (int i = 0; i < wajahtrain.Length; i++)
            {
                test[i] = wajahtrain[i].Data;
                XmlNode gambar = doku.CreateElement("gambar");
                gambar.InnerText = Convert.ToBase64String((byte[])imgConv.ConvertTo(wajahtrain[i].ToBitmap(), typeof(byte[])));
                nodepemilik.AppendChild(gambar);
            }
            doku.Save("wajah_training.xml");
            
        } // penambahan database wajah

        public double[,] trainPCA(byte[,][] arrGambar, out double[] rata, out double[][] substrak)
        {
            double[][] allpic = new double[arrGambar.GetLength(0) * arrGambar.GetLength(1)][];
            int gamball = 0;
            //penginputan gambar
            for (int i = 0; i < arrGambar.GetLength(0); i++)//memilih gambar pada baris
            {
                for (int j = 0; j < arrGambar.GetLength(1); j++)//pilih gambar pada kolom
                {
                    Bitmap bmp = (Bitmap)((new ImageConverter()).ConvertFrom(arrGambar[i, j])); //ambil gambar baris i kolom j
                    HistogramEqualization filt = new HistogramEqualization();
                    bmp = filt.Apply(bmp);

                    ImageToMatrix immat = new ImageToMatrix(); byte[][] matrixWajah;
                    int bla = 0;
                    immat.Convert(bmp, out matrixWajah);
                    double[] dobyte = new double[matrixWajah.GetLength(0) * matrixWajah[0].Length]; //penyimpan array byte sementara 
                    for (int k = 0; k < matrixWajah.GetLength(0); k++) //mengubah tipe data dalam jagged array byte menjadi tipe double
                    {
                        for (int m = 0; m < matrixWajah[i].Length; m++)
                        {
                            dobyte[bla] = matrixWajah[k][m];
                            bla++;
                        }
                    }
                    allpic[gamball] = dobyte; //masukkan pada array besar yang berisi nilai byte semua gambar, 1 gambar diwakili 1 baris..
                    gamball++;
                }
            }
            double[][] transpose = manipul.TransposeRowsAndColumns(allpic); //transpose matriks kumpulan gambar database (baris<->Colom)
            int rat = 0;
            //menghitung rata2
            rata = new double[allpic[0].Length];
            for (int i = 0; i < transpose.GetLength(0); i++)
            {
                for (int j = 0; j < transpose[i].Length; j++)
                {
                    rata[rat] += transpose[i][j];
                }
                rata[rat] = rata[rat] / transpose[i].Length;
                rat++;
            }

            //melakukan normalisasi dengan substraksi
            substrak = new double[transpose.GetLength(0)][];
            for (int i = 0; i < transpose.GetLength(0); i++)
            {
                substrak[i] = new double[transpose[i].Length];
                for (int j = 0; j < transpose[i].Length; j++)
                {
                    substrak[i][j] = transpose[i][j] - rata[i];
                }
            }

            double[][] kovarian = substrak.Covariance(); //menghitung kovarian
            EigenvalueDecomposition eig = new EigenvalueDecomposition(manipul.ConvJaggto2D(kovarian)); //generate nilai eigen
            double[,] matriksId = eig.DiagonalMatrix; //mencari matriks identitas
            double[,] eigvek = eig.Eigenvectors; //pengambilan nilai eigenvektor

            double jumval = 0;
            for (int i = 0; i < matriksId.GetLength(0); i++)
            {
                if (matriksId[i, i] < 1) //melakukan pengecekan nilai eigenvalue yang dibawah 1
                {
                    matriksId[i, i] = 0;
                }
                jumval += matriksId[i, i];
            }
            //mereduksi nilai value dibawah 90%
            double pers = 0;
            List<int> kolok = new List<int>();
            for (int i = matriksId.GetLength(0)-1; i >= 0; i--)
            {
                if (pers / jumval * 100 > 90)
                {
                    break;
                }
                else
                {
                    kolok.Add(i);
                    pers += matriksId[i, i];
                }
            }

            double[,] idela_eigvek = new double[eigvek.GetLength(0), kolok.ToArray().Length];
            List<double>[] ideal = new List<double>[eigvek.GetLength(0)];
            double[][] test = new double[eigvek.GetLength(0)][];
            int tambah = 0;
            for (int i = 0; i < eigvek.GetLength(0); i++)
            {
                for (int j = kolok.ToArray().Length - 1; j >= 0; j--)
                {
                    idela_eigvek[i, tambah] = eigvek[i, kolok.ToArray()[j]];
                    tambah++;
                }
                tambah = 0;
            }
            double[,] elia = idela_eigvek;
            
            double[,] eigface = new double[transpose.GetLength(0), elia.GetLength(1)];
            //melakukan penghitungan eigenface
            for (int i = 0; i < transpose.GetLength(0); i++)
            {
                for (int j = 0; j < elia.GetLength(1); j++)
                {
                    for (int k = 0; k < transpose[i].Length; k++)
                    {
                        eigface[i, j] += transpose[i][k] * elia[k,j];
                    }
                }
            }
            return eigface;
        } //penghitngan nilai eigen database wajah

        public void UpdateEigenface()
        {
            ambilGambData();

            #region simpan eigenface
            double[] rataeig; // nilai rata2 tiap wajah
            double[][] subsub; //nilai hasil normalisasi
            double[,] eigface = trainPCA(ambilGambData(), out rataeig, out subsub);
            XmlWriter write = XmlWriter.Create("eigenface.xml");
            write.WriteStartDocument();
            write.WriteStartElement("eigenface");
            write.WriteElementString("barisEigen", eigface.GetLength(0).ToString());
            write.WriteElementString("kolomEigen", eigface.GetLength(1).ToString());
            write.WriteStartElement("nilaiEigen");
            for (int i = 0; i < eigface.GetLength(0); i++)
            {
                for (int j = 0; j < eigface.GetLength(1); j++)
                {
                    write.WriteElementString("indeksEigen", eigface[i, j].ToString());
                }
            }
            write.WriteEndElement();
            write.WriteStartElement("meanEigen");
            for (int i = 0; i < rataeig.GetLength(0); i++)
            {
                write.WriteElementString("indeksMean", rataeig[i].ToString());
            }
            write.WriteEndElement();
            write.WriteElementString("barisNormal", subsub.GetLength(0).ToString());
            write.WriteElementString("kolomNormal", subsub[0].Length.ToString());
            write.WriteStartElement("normalisasi");
            for (int i = 0; i < subsub.GetLength(0); i++)
            {
                for (int j = 0; j < subsub[0].Length; j++)
                {
                    write.WriteElementString("indeksNorm", subsub[i][j].ToString());
                }
            }
            write.WriteEndElement();
            write.WriteEndElement();
            write.WriteEndDocument();
            write.Close();
            #endregion
        } //membuat dan menambahkan eigenface
        #endregion
    }
}
