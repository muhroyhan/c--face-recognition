using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace programTA
{
    class Presensi
    {
        private string dapres;
        private bool preskos;
        private Karyawan karya;

        #region properties
        public string Dapres
        {
            set { dapres = value; }
            get { return dapres; }
        }
        #endregion

        #region constructor
        public Presensi()
        {
            karya = new Karyawan();
            try
            {
                XmlReader ra = XmlReader.Create("presensi.xml");
                ra.Close();
                dapres = PresensiAktif();
                if (dapres == null && preskos == true)
                {
                    GeneratePresensiHariIni();
                    dapres = PresensiAktif();
                    GeneratePesertaPresensi();
                }
            } catch  {
                int x = 0;
                XmlReader ra = XmlReader.Create("wajah_training.xml");
                while (ra.Read())
                {
                    if (ra.IsStartElement() && ra.Name == "wajahpemilik")
                    {
                        x++;
                    }
                }
                ra.Close();
                if (x != 0) { BuatDataPresensi(); }
            }
        }
        #endregion

        #region method
        public void BuatDataPresensi()
        {
            XmlWriter writer = XmlWriter.Create("presensi.xml");
            writer.WriteStartDocument();
            writer.WriteStartElement("presensi");
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            XmlWriter write = XmlWriter.Create("presensi_wajah.xml");
            write.WriteStartDocument();
            write.WriteStartElement("presensi_wajah");
            write.WriteEndElement();
            write.WriteEndDocument();
            write.Close();
        } // pembuatan file database presensi

        public string[][] TanggalPresensi()
        {
            List<string> idpreses = new List<string>();
            List<string> tanggal = new List<string>();
            XmlReader r = XmlReader.Create("presensi.xml");
            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "id_presensi":
                            if (r.Read())
                            {
                                idpreses.Add(r.Value);
                            }
                            break;
                        case "tanggal_presensi":
                            if (r.Read())
                            {
                                tanggal.Add(r.Value);
                            }
                            break;
                    }
                }
            }
            return new string[][] { idpreses.ToArray(), tanggal.ToArray() };
        }

        public string[][][] SemuaPresensiWajah()
        {
            List<string> wajid = new List<string>();//menampung semua id org telibat dalam 1 id presensi
            List<string[]> wajid1 = new List<string[]>(); //menampung semua id orang terlibat dalam semua id presensi
            List<string> presid = new List<string>();
            List<string> stsdtg = new List<string>();//menampung semua status org telibat dalam 1 id presensi
            List<string[]> stsdtg1 = new List<string[]>(); //menampung semua status orang terlibat dalam semua id presensi
            List<string> wktwtg = new List<string>(); //menampung semua waktu org telibat dalam 1 id presensi
            List<string[]> wktwtg1 = new List<string[]>();//menampung semua waktu orang terlibat dalam semua id presensi
            int b = 0;

            #region Menampung data dari database presensi wajah
            XmlReader re = XmlReader.Create("presensi_wajah.xml");
            while (re.Read())
            {
                if (re.IsStartElement())
                {
                    switch (re.Name)
                    {
                        case "id_presensi":
                            if (re.Read())
                            {
                                presid.Add(re.Value);
                            }
                            break;
                        case "wajah_terlibat":
                            while (re.Read())
                            {
                                if (re.IsStartElement())
                                {
                                    switch (re.Name)
                                    {
                                        case "id_kar":
                                            if (re.Read())
                                            {
                                                b++;
                                                wajid.Add(re.Value);
                                            }
                                            break;
                                        case "waktu_datang":
                                            if (re.Read())
                                            {
                                                b++;
                                                stsdtg.Add(re.Value);
                                            }
                                            break;
                                        case "waktu_pulang":
                                            if (re.Read())
                                            {
                                                b++;
                                                wktwtg.Add(re.Value);
                                            }
                                            break;
                                    }
                                }
                                if (b == 3)
                                {
                                    b = 0; break;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (re.Name)
                    {
                        case "presensi_tercatat":
                            if (wajid.Count != 0 && stsdtg.Count != 0 && wktwtg.Count != 0)
                            {
                                wajid1.Add(wajid.ToArray());
                                stsdtg1.Add(stsdtg.ToArray());
                                wktwtg1.Add(wktwtg.ToArray());
                                wajid = new List<string>();
                                stsdtg = new List<string>();
                                wktwtg = new List<string>();
                            }
                            break;
                    }
                }
            }
            re.Close();
            #endregion

            //memilih urutan orang yang masuk ke dalam database presensi_wajah sebagai data baru
            List<string[]> idwaj = new List<string[]>();
            List<string[]> stswaj = new List<string[]>();
            List<string[]> wtkwaj = new List<string[]>();
            List<string> idwaj1 = new List<string>();
            List<string> stswaj1 = new List<string>();
            List<string> wtkwaj1 = new List<string>();
            int a = 0;
            if (wajid1.Count != 0)
            {
                for (int i = 0; i < presid.ToArray().Length; i++)
                {
                    for (int j = 0; j < wajid1.ToArray()[i].Length; j++)
                    {
                        idwaj1.Add(wajid1.ToArray()[i][j]); //pengambilan id pada 1 presensi
                    }
                    idwaj.Add(idwaj1.ToArray()); // penginputan idwajah untuk satu per satu presensi
                    idwaj1 = new List<string>(); //reset list id karyawan pada presensi sebelumnya
                    for (int j = 0; j < stsdtg1.ToArray()[i].Length; j++)
                    {
                        stswaj1.Add(stsdtg1.ToArray()[i][j]);
                    }
                    stswaj.Add(stswaj1.ToArray());
                    stswaj1 = new List<string>();
                    for (int j = 0; j < wktwtg1.ToArray()[i].Length; j++)
                    {
                        wtkwaj1.Add(wktwtg1.ToArray()[i][j]);
                    }
                    wtkwaj.Add(wtkwaj1.ToArray());
                    wtkwaj1 = new List<string>();
                }
            }
            return new string[][][] { idwaj.ToArray(), stswaj.ToArray(), wtkwaj.ToArray() };
        } //mengambil semua data presensi wajah dari semua id presensi

        public string[][] AmbilPresensiWajah(string idpres)
        {
            List<string> wajid = new List<string>();//menampung semua id org telibat dalam 1 id presensi
            List<string[]> wajid1 = new List<string[]>(); //menampung semua id orang terlibat dalam semua id presensi
            List<string> presid = new List<string>();
            List<string> stsdtg = new List<string>();//menampung semua status org telibat dalam 1 id presensi
            List<string[]> stsdtg1 = new List<string[]>(); //menampung semua status orang terlibat dalam semua id presensi
            List<string> wktwtg = new List<string>(); //menampung semua waktu org telibat dalam 1 id presensi
            List<string[]> wktwtg1 = new List<string[]>();//menampung semua waktu orang terlibat dalam semua id presensi
            int b = 0;

            #region Menampung data dari database presensi wajah
            XmlReader re = XmlReader.Create("presensi_wajah.xml");
            while (re.Read())
            {
                if (re.IsStartElement())
                {
                    switch (re.Name)
                    {
                        case "id_presensi":
                            if (re.Read())
                            {
                                presid.Add(re.Value);
                            }
                            break;
                        case "wajah_terlibat":
                            while (re.Read())
                            {
                                if (re.IsStartElement())
                                {
                                    switch (re.Name)
                                    {
                                        case "id_kar":
                                            if (re.Read())
                                            {
                                                b++;
                                                wajid.Add(re.Value);
                                            }
                                            break;
                                        case "waktu_datang":
                                            if (re.Read())
                                            {
                                                b++;
                                                stsdtg.Add(re.Value);
                                            }
                                            break;
                                        case "waktu_pulang":
                                            if (re.Read())
                                            {
                                                b++;
                                                wktwtg.Add(re.Value);
                                            }
                                            break;
                                    }
                                }
                                if (b == 3)
                                {
                                    b = 0; break;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    switch (re.Name)
                    {
                        case "presensi_tercatat":
                            if (wajid.Count != 0 && stsdtg.Count != 0 && wktwtg.Count != 0)
                            {
                                wajid1.Add(wajid.ToArray());
                                stsdtg1.Add(stsdtg.ToArray());
                                wktwtg1.Add(wktwtg.ToArray());
                                wajid = new List<string>();
                                stsdtg = new List<string>();
                                wktwtg = new List<string>();
                            }
                            break;
                    }
                }
            }
            re.Close();
            #endregion

            //memilih urutan orang yang masuk ke dalam database presensi_wajah sebagai data baru
            List<string> idwaj = new List<string>();
            List<string> stswaj = new List<string>();
            List<string> wtkwaj = new List<string>();
            int a = 0;
            if (wajid1.Count != 0)
            {
                for (int i = 0; i < presid.ToArray().Length; i++)
                {
                    if (presid.ToArray()[i] == idpres)
                    {
                        for (int j = 0; j < wajid1.ToArray()[i].Length; j++)
                        {
                            idwaj.Add(wajid1.ToArray()[i][j]);
                        }
                        for (int j = 0; j < stsdtg1.ToArray()[i].Length; j++)
                        {
                            stswaj.Add(stsdtg1.ToArray()[i][j]);
                        }
                        for (int j = 0; j < wktwtg1.ToArray()[i].Length; j++)
                        {
                            wtkwaj.Add(wktwtg1.ToArray()[i][j]);
                        }
                        break;
                    }
                }
            }
            return new string[][] { presid.ToArray(), idwaj.ToArray(), stswaj.ToArray(), wtkwaj.ToArray() };
        } //ambil id presensi, id karyawan, waktu datang, dan waktu pulang dr id presensi yg terbuka

        private int AutoIncrePresensi()
        {
            int jumPresData = 0; ;
            try
            {
                using (XmlReader re = XmlReader.Create("presensi.xml"))
                {
                    while (re.Read())
                    {
                        if (re.IsStartElement())
                        {
                            switch (re.Name)
                            {
                                case "id_presensi":
                                    if (re.Read())
                                    {
                                        jumPresData = int.Parse(re.Value);
                                        jumPresData++;
                                    }
                                    break;

                            }
                        }
                    }
                    re.Close();
                }
            }
            catch
            {
                jumPresData = 0;
            }
            return jumPresData;
        } // auto increment database presensi

        public string PresensiAktif()
        {
            int jumpres = 0; preskos = false;
            List<string> idpres = new List<string>();
            List<string> tglpres = new List<string>();
            List<string> stspres = new List<string>();

            #region baca tabel presensi
            XmlReader rea = XmlReader.Create("presensi.xml");
            while (rea.Read())
            {
                if (rea.IsStartElement())
                {
                    switch (rea.Name)
                    {
                        case "tanggal_presensi":
                            if (rea.Read())
                            {
                                tglpres.Add(rea.Value);
                            }
                            break;
                        case "id_presensi":
                            if (rea.Read())
                            {
                                idpres.Add(rea.Value); //mengoleksi idpresensi yang ada
                            }
                            break;
                        case "status_presensi":
                            if (rea.Read())
                            {
                                stspres.Add(rea.Value);
                            }
                            break;
                    }
                }
            }
            rea.Close();
            #endregion

            string[] pres = idpres.ToArray();
            string[] wkt = tglpres.ToArray();
            string[] stats = stspres.ToArray();

            #region Pengecekan presensi aktif dan pembuatan pdf
            for (int k = 0; k < pres.Length; k++)
            {
                if ((wkt[k] != DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("en-NZ")) &&
                    stats[k] == "buka") && DateTime.Now.Hour >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0).Hour) // jika tanggal presensi bukan hari ini dan statusnya terbuka dan waktu hari ini belum jm 6 pagi
                {
                    int jarak = (DateTime.Now - Convert.ToDateTime(wkt[k])).Days;
                    XDocument elek = XDocument.Load("presensi.xml");
                    elek.XPathSelectElement("//presensi/presensi_harian/id_presensi[text()='" + pres[k] + "']/../status_presensi").Value = "tutup";
                    elek.Save("presensi.xml"); // merubah status buka menjadi tutup
                    BuatPDF(pres[k], jarak);
                    Notifikasi.Show("File PDF presensi tanggal " + wkt[k] + " telah dibuat.","Pemberitahuan", 1500);
                    PresensiAktif();
                }
                else if (wkt[k] != DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("en-NZ")) &&
                    stats[k] == "tutup") //menghtung jumlah presensi yang ada di tabel presensi untuk triger buat presensi baru
                {
                    jumpres++;
                    if (jumpres == pres.Length)
                    {
                        preskos = true;
                    }
                }
                else if (wkt[k] == DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("en-NZ")) &&
                   stats[k] == "buka") //jika presensi hari ini terbuka
                {
                    dapres = pres[k];
                }
                else if (wkt[k] != DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("en-NZ")) &&
                  stats[k] == "buka" && DateTime.Now.Hour < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0).Hour) //jika presensi hari ini terbuka
                {
                    dapres = pres[k];
                }
            }
            if (pres.Length == 0) // jika tabel database masih kosong
            {
                preskos = true;
            }
            #endregion

            return dapres;
        } //melakukan cek presnsi yang terbuka

        public void GeneratePresensiHariIni()
        {
            XmlDocument doku = new XmlDocument();
            doku.Load("presensi.xml");//membuak dokumn wajah training

            XmlNode node = doku.DocumentElement;
            XmlNode nodepresensi = doku.CreateElement("presensi_harian");
            XmlNode idpresensi = doku.CreateElement("id_presensi");
            idpresensi.InnerText = (AutoIncrePresensi()).ToString();
            XmlNode tglpresensi = doku.CreateElement("tanggal_presensi");
            tglpresensi.InnerText = DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("en-NZ"));
            XmlNode stspresensi = doku.CreateElement("status_presensi");
            stspresensi.InnerText = "buka";
            node.AppendChild(nodepresensi);
            nodepresensi.AppendChild(tglpresensi);
            nodepresensi.AppendChild(idpresensi);
            nodepresensi.AppendChild(stspresensi);

            doku.Save("presensi.xml");
        } // pembuatan database presensi untuk hariini 

        public void GeneratePesertaPresensi()
        {
            XmlDocument doku = new XmlDocument();
            doku.Load("presensi_wajah.xml");//membuka database peserta presensi

            XmlNode node = doku.DocumentElement;
            XmlNode presCatat = doku.CreateElement("presensi_tercatat");

            XmlNode presensid = doku.CreateElement("id_presensi");
            presensid.InnerText = dapres;
            presCatat.AppendChild(presensid);

            try
            {
                XmlReader r = XmlReader.Create("wajah_training.xml");
                while (r.Read())
                {
                    if (r.IsStartElement())
                    {
                        //ambil data dari database wajah
                        switch (r.Name)
                        {
                            case "id_kar":
                                if (r.Read())
                                {
                                    XmlNode orglibat = doku.CreateElement("wajah_terlibat");
                                    XmlNode wajahid = doku.CreateElement("id_kar");
                                    wajahid.InnerText = r.Value;
                                    orglibat.AppendChild(wajahid);
                                    XmlNode wtkdtg = doku.CreateElement("waktu_datang");
                                    wtkdtg.InnerText = "-";
                                    XmlNode wktplg = doku.CreateElement("waktu_pulang");
                                    wktplg.InnerText = "-";
                                    orglibat.AppendChild(wtkdtg);
                                    orglibat.AppendChild(wktplg);
                                    presCatat.AppendChild(orglibat);
                                }
                                break;
                        }
                    }
                }
                r.Close();
            }
            catch
            {

            }
            node.AppendChild(presCatat);
            doku.Save("presensi_wajah.xml");

        } // pembuatan database peserta presensi
        
        public void BuatPDF(string idpres)
        {
            XmlReader r = XmlReader.Create("presensi.xml");
            FileStream fs = new FileStream("absen_" + DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("id-ID")) + ".pdf",
                FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writ = PdfWriter.GetInstance(doc, fs);
            iTextSharp.text.Rectangle rec2 = new iTextSharp.text.Rectangle(PageSize.A4); // menentukan ukuran pdf
            PdfPTable tabil = new PdfPTable(5); //  buat table (pdfptable(jumlah kolom))
            Chunk glue = new Chunk(new VerticalPositionMark()); // buat 2 paragraf dalam 1 line
            Paragraph p = new Paragraph();
            List<string> tg = new List<string>();
            List<string> idpreses = new List<string>();
            doc.Open();
            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {

                        case "tanggal_presensi":
                            if (r.Read())
                            {
                                tg.Add(r.Value);
                            }
                            break;
                        case "id_presensi":
                            if (r.Read())
                            {
                                idpreses.Add(r.Value);
                            }
                            break;
                    }

                }
            }
            r.Close();

            for (int i = 0; i < idpreses.ToArray().Length; i++)
            {
                if (idpreses.ToArray()[i] == idpres)
                {
                    p = new Paragraph("ID PRESENSI : " + idpres);
                    p.Add(new Chunk(glue));
                    p.Add("Tanggal Presensi = " + DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("id-ID")));
                    doc.Add(p);
                    tabil.SpacingBefore = 20;
                    tabil.AddCell("Nomor");
                    tabil.AddCell("ID Karyawan");
                    tabil.AddCell("Nama Karyawan");
                    tabil.AddCell("Waktu Datang");
                    tabil.AddCell("Waktu Pulang");
                }
            }

            #region Tampilkan Peserta Presensi
            string[][] acak = AmbilPresensiWajah(idpres);

            string[] a = acak[0]; //idpresensi
            string[] b = acak[1]; //wajahid dalam presensi
            string[] c = acak[2]; //waktu datang karyawan
            string[] d = acak[3]; //waktu pulang karyawan

            string[][] ambilwaj = karya.AmbilDataKaryawan();
            string[] e = ambilwaj[0]; //wajahid pada database wajah
            string[] f = ambilwaj[1]; //nama pada wajah id yang sesuai

            int m = 0;
            for (int k = 0; k < a.Length; k++)
            {
                if (a[k] == idpres) //jika id presensi pada presensi wajah sama dengan id yg dipassing
                {
                    for (int o = 0; o < b.Length; o++)
                    {
                        for (int n = 0; n < e.Length; n++)
                        {
                            if (b[o] == e[n])//jika orang e[n] terlibat dalam presensi
                            {
                                m++;
                                tabil.AddCell(m.ToString());
                                tabil.AddCell(e[n].ToString());
                                tabil.AddCell(f[n].ToString());
                                tabil.AddCell(c[o].ToString());
                                tabil.AddCell(d[o].ToString());
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            doc.Add(tabil);
            doc.Close();
        } // untuk hari ini

        public void BuatPDF(string idpres, int jartgl)
        {
            XmlReader r = XmlReader.Create("presensi.xml");
            FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\absen_" + DateTime.Now.AddDays(-jartgl).ToString("D", CultureInfo.CreateSpecificCulture("id-ID")) + ".pdf",
                FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writ = PdfWriter.GetInstance(doc, fs);
            iTextSharp.text.Rectangle rec2 = new iTextSharp.text.Rectangle(PageSize.A4); // menentukan ukuran pdf
            PdfPTable tabil = new PdfPTable(5); //  buat table (pdfptable(jumlah kolom))
            Chunk glue = new Chunk(new VerticalPositionMark()); // buat 2 paragraf dalam 1 line
            Paragraph p = new Paragraph();
            List<string> tg = new List<string>();
            List<string> idpreses = new List<string>();
            doc.Open();
            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {

                        case "tanggal_presensi":
                            if (r.Read())
                            {
                                tg.Add(r.Value);
                            }
                            break;
                        case "id_presensi":
                            if (r.Read())
                            {
                                idpreses.Add(r.Value);
                            }
                            break;
                    }

                }
            }
            r.Close();

            for (int i = 0; i < idpreses.ToArray().Length; i++)
            {
                if (idpreses.ToArray()[i] == idpres)
                {
                    p = new Paragraph("ID PRESENSI : " + idpres);
                    p.Add(new Chunk(glue));
                    p.Add("Tanggal Presensi = " + DateTime.Now.AddDays(-jartgl).ToString("D", CultureInfo.CreateSpecificCulture("id-ID")));
                    doc.Add(p);
                    tabil.SpacingBefore = 20;
                    tabil.AddCell("Nomor");
                    tabil.AddCell("ID Karyawan");
                    tabil.AddCell("Nama Karyawan");
                    tabil.AddCell("Waktu Datang");
                    tabil.AddCell("Waktu Pulang");
                }
            }

            #region Tampilkan Peserta Presensi
            string[][] acak = AmbilPresensiWajah(idpres);

            string[] a = acak[0]; //idpresensi
            string[] b = acak[1]; //wajahid dalam presensi
            string[] c = acak[2]; //waktu datang karyawan
            string[] d = acak[3]; //waktu pulang karyawan

            string[][] ambilwaj = karya.AmbilDataKaryawan();
            string[] e = ambilwaj[0]; //wajahid pada database wajah
            string[] f = ambilwaj[1]; //nama pada wajah id yang sesuai

            int m = 0;
            for (int k = 0; k < a.Length; k++)
            {
                if (a[k] == idpres) //jika id presensi pada presensi wajah sama dengan id yg dipassing
                {
                    for (int o = 0; o < b.Length; o++)
                    {
                        for (int n = 0; n < e.Length; n++)
                        {
                            if (b[o] == e[n])//jika orang e[n] terlibat dalam presensi
                            {
                                m++;
                                tabil.AddCell(m.ToString());
                                tabil.AddCell(e[n].ToString());
                                tabil.AddCell(f[n].ToString());
                                tabil.AddCell(c[o].ToString());
                                tabil.AddCell(d[o].ToString());
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            doc.Add(tabil);
            doc.Close();
        } //untuk hari2 sebelum hari ini
        #endregion
    }
}
