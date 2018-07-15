namespace host
{
    partial class fPLC_export_mmf
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
            this.b_export_to_file_mmf = new System.Windows.Forms.Button();
            this.browser_files_csv = new System.Windows.Forms.Button();
            this.path_file_textBox = new System.Windows.Forms.TextBox();
            this.grid_file = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grid_file)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // b_export_to_file_mmf
            // 
            this.b_export_to_file_mmf.Location = new System.Drawing.Point(606, 20);
            this.b_export_to_file_mmf.Name = "b_export_to_file_mmf";
            this.b_export_to_file_mmf.Size = new System.Drawing.Size(75, 23);
            this.b_export_to_file_mmf.TabIndex = 0;
            this.b_export_to_file_mmf.Text = "Export MMF";
            this.b_export_to_file_mmf.UseVisualStyleBackColor = true;
            this.b_export_to_file_mmf.Click += new System.EventHandler(this.b_export_to_file_mmf_Click);
            // 
            // browser_files_csv
            // 
            this.browser_files_csv.Location = new System.Drawing.Point(499, 20);
            this.browser_files_csv.Name = "browser_files_csv";
            this.browser_files_csv.Size = new System.Drawing.Size(98, 23);
            this.browser_files_csv.TabIndex = 1;
            this.browser_files_csv.Text = "Browser excel";
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
            // grid_file
            // 
            this.grid_file.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid_file.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_file.Location = new System.Drawing.Point(0, 60);
            this.grid_file.Name = "grid_file";
            this.grid_file.Size = new System.Drawing.Size(694, 503);
            this.grid_file.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.b_export_to_file_mmf);
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
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Đường dẫn file *.csv";
            // 
            // fPLC_export_mmf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 563);
            this.Controls.Add(this.grid_file);
            this.Controls.Add(this.panel1);
            this.Name = "fPLC_export_mmf";
            this.Text = "Convert file excels (*.csv) to file MMF ";
            this.Load += new System.EventHandler(this.fPLC_export_mmf_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grid_file)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button b_export_to_file_mmf;
        private System.Windows.Forms.Button browser_files_csv;
        private System.Windows.Forms.TextBox path_file_textBox;
        private System.Windows.Forms.DataGridView grid_file;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}