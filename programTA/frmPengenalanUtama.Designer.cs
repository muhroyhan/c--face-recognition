namespace programTA
{
    partial class frmPengenalanUtama
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picPengenalan = new System.Windows.Forms.PictureBox();
            this.picDikenali = new System.Windows.Forms.PictureBox();
            this.tmrtunggu = new System.Windows.Forms.Timer(this.components);
            this.tmrWaktuFoto = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblWaktu = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picPengenalan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDikenali)).BeginInit();
            this.SuspendLayout();
            // 
            // picPengenalan
            // 
            this.picPengenalan.Location = new System.Drawing.Point(12, 58);
            this.picPengenalan.Name = "picPengenalan";
            this.picPengenalan.Size = new System.Drawing.Size(270, 200);
            this.picPengenalan.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPengenalan.TabIndex = 0;
            this.picPengenalan.TabStop = false;
            // 
            // picDikenali
            // 
            this.picDikenali.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picDikenali.Location = new System.Drawing.Point(340, 98);
            this.picDikenali.Name = "picDikenali";
            this.picDikenali.Size = new System.Drawing.Size(160, 160);
            this.picDikenali.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picDikenali.TabIndex = 1;
            this.picDikenali.TabStop = false;
            // 
            // tmrtunggu
            // 
            this.tmrtunggu.Interval = 1000;
            this.tmrtunggu.Tick += new System.EventHandler(this.tmrtunggu_Tick);
            // 
            // tmrWaktuFoto
            // 
            this.tmrWaktuFoto.Interval = 1000;
            this.tmrWaktuFoto.Tick += new System.EventHandler(this.tmrWaktuFoto_Tick);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblWaktu
            // 
            this.lblWaktu.AutoSize = true;
            this.lblWaktu.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWaktu.Location = new System.Drawing.Point(94, 12);
            this.lblWaktu.Name = "lblWaktu";
            this.lblWaktu.Size = new System.Drawing.Size(120, 31);
            this.lblWaktu.TabIndex = 3;
            this.lblWaktu.Text = "00:00:00";
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.BackColor = System.Drawing.Color.Transparent;
            this.lblTimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimer.Location = new System.Drawing.Point(133, 222);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(29, 31);
            this.lblTimer.TabIndex = 11;
            this.lblTimer.Text = "2";
            this.lblTimer.Visible = false;
            // 
            // frmPengenalanUtama
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 285);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.lblWaktu);
            this.Controls.Add(this.picDikenali);
            this.Controls.Add(this.picPengenalan);
            this.Name = "frmPengenalanUtama";
            this.Text = "Pencatatan Presensi";
            this.Load += new System.EventHandler(this.frmPengenalanUtama_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picPengenalan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDikenali)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPengenalan;
        private System.Windows.Forms.PictureBox picDikenali;
        private System.Windows.Forms.Timer tmrtunggu;
        private System.Windows.Forms.Timer tmrWaktuFoto;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblWaktu;
        private System.Windows.Forms.Label lblTimer;
    }
}