using System;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace programTA
{
    public partial class frmTambahKaryawan : Form
    {
        public frmTambahKaryawan()
        {
            InitializeComponent();
        }
        

        private void frmTambahKaryawan_Load(object sender, EventArgs e)
        {
            try
            {
                using (XmlReader r = XmlReader.Create("karyawan.xml"))
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
                                        lblNIK.Text = (int.Parse(r.Value) + 1).ToString();
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                lblNIK.Text = "1001";
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (txtNamaKaryawan.Text != "")
            {
                XmlDocument doku = new XmlDocument();
                doku.Load("karyawan.xml");//membuak dokumn wajah training

                XmlNode node = doku.DocumentElement;
                XmlNode idenkaryawan = doku.CreateElement("identitas_karyawan");
                XmlNode idkar = doku.CreateElement("id_kar");
                idkar.InnerText = lblNIK.Text;
                XmlNode namamilik = doku.CreateElement("nama_karyawan");
                namamilik.InnerText = txtNamaKaryawan.Text;
                node.AppendChild(idenkaryawan);
                idenkaryawan.AppendChild(idkar);
                idenkaryawan.AppendChild(namamilik);
                doku.Save("karyawan.xml");
                MessageBox.Show("Karyawan telah tersimpan.");
            }
            else
            {
                MessageBox.Show("masukkan nama terlebih dahulu.");
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
