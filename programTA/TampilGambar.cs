using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;


namespace programTA
{
    class TampilGambar
    {
        #region variabel
        private Image<Bgr, byte> gambAsli; // gambar yang ditangkap kamera
        private Image<Gray, byte> gambGray; // gambar gray dari yang ditangkap kamera
        private Image<Bgr, byte> gambKrop; // menampung gambar yang telah dikrop
        private Image<Bgr, byte> gambSem; // menampung gambar asli untuk keperluan penambahan draw
        private VideoCapture test; // class untuk menggunakan kamera
        private CascadeClassifier casClass; // class Viola-jones Algorithm
        #endregion

        #region properties
        public Image<Bgr,byte> GambAsli
        {
            set { gambAsli = value; }
            get { return gambAsli; }
        }

        public Image<Gray, byte> GambGray
        {
            set { gambGray = value; }
            get { return gambGray; }
        }

        public Image<Bgr, byte> GambKrop
        {
            set { gambKrop = value; }
            get { return gambKrop; }
        }

        public Image<Bgr, byte> GambSem
        {
            set { gambSem = value; }
            get { return gambSem; }
        }

        public VideoCapture ProcVideo
        {
            set { test = value; }
            get { return test; }
        }

        public CascadeClassifier CasClass
        {
            set { casClass = value; }
            get { return casClass; }
        }
        #endregion

        #region constructor
        public TampilGambar()
        {
            ProcVideo = new VideoCapture();
            CasClass = new CascadeClassifier("haarcascade_frontalface_alt2.xml");
        }
        #endregion
    }
}
