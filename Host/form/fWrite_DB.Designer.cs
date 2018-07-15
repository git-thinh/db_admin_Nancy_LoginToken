namespace host
{
    partial class fWrite_DB
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
            this.b_update_db = new System.Windows.Forms.Button();
            this.browser_files_csv = new System.Windows.Forms.Button();
            this.path_file_textBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_result = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // b_update_db
            // 
            this.b_update_db.Location = new System.Drawing.Point(606, 20);
            this.b_update_db.Name = "b_update_db";
            this.b_update_db.Size = new System.Drawing.Size(75, 23);
            this.b_update_db.TabIndex = 0;
            this.b_update_db.Text = "Update DB";
            this.b_update_db.UseVisualStyleBackColor = true;
            this.b_update_db.Click += new System.EventHandler(this.b_update_db_Click);
            // 
            // browser_files_csv
            // 
            this.browser_files_csv.Location = new System.Drawing.Point(499, 20);
            this.browser_files_csv.Name = "browser_files_csv";
            this.browser_files_csv.Size = new System.Drawing.Size(98, 23);
            this.browser_files_csv.TabIndex = 1;
            this.browser_files_csv.Text = "Browser";
            this.browser_files_csv.UseVisualStyleBackColor = true;
            this.browser_files_csv.Click += new System.EventHandler(this.browser_files_csv_Click);
            // 
            // path_file_textBox
            // 
            this.path_file_textBox.Location = new System.Drawing.Point(15, 22);
            this.path_file_textBox.Name = "path_file_textBox";
            this.path_file_textBox.Size = new System.Drawing.Size(463, 20);
            this.path_file_textBox.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.b_update_db);
            this.panel1.Controls.Add(this.browser_files_csv);
            this.panel1.Controls.Add(this.path_file_textBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(694, 60);
            this.panel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Đường dẫn file *.mmf";
            // 
            // lb_result
            // 
            this.lb_result.AutoSize = true;
            this.lb_result.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_result.Location = new System.Drawing.Point(26, 75);
            this.lb_result.Name = "lb_result";
            this.lb_result.Size = new System.Drawing.Size(0, 25);
            this.lb_result.TabIndex = 5;
            // 
            // fWrite_DB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 147);
            this.Controls.Add(this.lb_result);
            this.Controls.Add(this.panel1);
            this.Name = "fWrite_DB";
            this.Text = "Convert file excels (*.csv) to file MMF ";
            this.Load += new System.EventHandler(this.fPLC_export_mmf_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button b_update_db;
        private System.Windows.Forms.Button browser_files_csv;
        private System.Windows.Forms.TextBox path_file_textBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_result;
    }
}