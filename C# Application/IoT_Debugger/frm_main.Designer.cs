namespace IoT_Debugger
{
    partial class frm_main
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
            this.lst_msg = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lst_msg
            // 
            this.lst_msg.BackColor = System.Drawing.Color.Black;
            this.lst_msg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lst_msg.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lst_msg.ForeColor = System.Drawing.Color.White;
            this.lst_msg.FormattingEnabled = true;
            this.lst_msg.ItemHeight = 20;
            this.lst_msg.Location = new System.Drawing.Point(11, 25);
            this.lst_msg.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lst_msg.Name = "lst_msg";
            this.lst_msg.Size = new System.Drawing.Size(990, 382);
            this.lst_msg.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frm_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1012, 437);
            this.Controls.Add(this.lst_msg);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "frm_main";
            this.Text = "IoT Based Debug Display";
            this.Load += new System.EventHandler(this.frm_main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lst_msg;
        private System.Windows.Forms.Timer timer1;
    }
}

