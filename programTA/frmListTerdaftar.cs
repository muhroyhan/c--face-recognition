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
    public partial class frmListTerdaftar : Form
    {
        public frmListTerdaftar()
        {
            InitializeComponent();
        }

        Wajah wiju;
        Karyawan karya;

        private void TampilList()
        {
            string[] idwaj = wiju.AmbilIDWajah();
            string[][] karia = karya.AmbilDataKaryawan();

            for(int i = 0; i < idwaj.Length; i++)
            {
                for(int j = 0; j < karia[0].Length; j++)
                {
                    if (idwaj[i] == karia[0][j])
                    {
                        dgvListKar.Rows.Add();
                        dgvListKar.Rows[i].Cells[0].Value = i + 1;
                        dgvListKar.Rows[i].Cells[1].Value = karia[0][j];
                        dgvListKar.Rows[i].Cells[2].Value = karia[1][j];
                    }
                }
            }
        }

        private void frmListTerdaftar_Load(object sender, EventArgs e)
        {
            wiju = new Wajah();
            karya = new Karyawan();
            TampilList();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
