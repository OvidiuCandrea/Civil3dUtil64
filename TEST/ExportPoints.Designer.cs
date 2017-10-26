namespace Ovidiu. x64.Civil3dUtil
{
    partial class ExportPoints
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblFileSelect = new System.Windows.Forms.Label();
            this.lblSelectAlignment = new System.Windows.Forms.Label();
            this.cmbBoxAlignment = new System.Windows.Forms.ComboBox();
            this.lblPreview = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.txtBoxFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txtBoxFilter = new System.Windows.Forms.TextBox();
            this.lblDescriptionFilter = new System.Windows.Forms.Label();
            this.btnSelAlign = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFileSelect
            // 
            this.lblFileSelect.AutoSize = true;
            this.lblFileSelect.Location = new System.Drawing.Point(41, 77);
            this.lblFileSelect.Name = "lblFileSelect";
            this.lblFileSelect.Size = new System.Drawing.Size(97, 13);
            this.lblFileSelect.TabIndex = 1;
            this.lblFileSelect.Text = "Export point to file: ";
            // 
            // lblSelectAlignment
            // 
            this.lblSelectAlignment.AutoSize = true;
            this.lblSelectAlignment.Location = new System.Drawing.Point(41, 19);
            this.lblSelectAlignment.Name = "lblSelectAlignment";
            this.lblSelectAlignment.Size = new System.Drawing.Size(86, 13);
            this.lblSelectAlignment.TabIndex = 3;
            this.lblSelectAlignment.Text = "Select Alignment";
            // 
            // cmbBoxAlignment
            // 
            this.cmbBoxAlignment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbBoxAlignment.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbBoxAlignment.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbBoxAlignment.FormattingEnabled = true;
            this.cmbBoxAlignment.Location = new System.Drawing.Point(41, 35);
            this.cmbBoxAlignment.Name = "cmbBoxAlignment";
            this.cmbBoxAlignment.Size = new System.Drawing.Size(802, 21);
            this.cmbBoxAlignment.TabIndex = 2;
            this.cmbBoxAlignment.SelectedIndexChanged += new System.EventHandler(this.cmbBoxAlignment_SelectedIndexChanged);
            // 
            // lblPreview
            // 
            this.lblPreview.AutoSize = true;
            this.lblPreview.Location = new System.Drawing.Point(41, 217);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(45, 13);
            this.lblPreview.TabIndex = 4;
            this.lblPreview.Text = "Preview";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(514, 626);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(96, 33);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(731, 626);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 33);
            this.button1.TabIndex = 6;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtBoxFile
            // 
            this.txtBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxFile.Location = new System.Drawing.Point(41, 93);
            this.txtBoxFile.Name = "txtBoxFile";
            this.txtBoxFile.Size = new System.Drawing.Size(802, 20);
            this.txtBoxFile.TabIndex = 7;
            this.txtBoxFile.Text = "D:\\Ovidiu\\temp.csv";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(857, 93);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(29, 19);
            this.btnBrowse.TabIndex = 8;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(43, 233);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(843, 372);
            this.dataGridView1.TabIndex = 9;
            // 
            // txtBoxFilter
            // 
            this.txtBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxFilter.Location = new System.Drawing.Point(40, 158);
            this.txtBoxFilter.Name = "txtBoxFilter";
            this.txtBoxFilter.Size = new System.Drawing.Size(846, 20);
            this.txtBoxFilter.TabIndex = 11;
            this.txtBoxFilter.TextChanged += new System.EventHandler(this.txtBoxFilter_TextChanged);
            this.txtBoxFilter.Validated += new System.EventHandler(this.txtBoxFilter_Validated);
            // 
            // lblDescriptionFilter
            // 
            this.lblDescriptionFilter.AutoSize = true;
            this.lblDescriptionFilter.Location = new System.Drawing.Point(40, 142);
            this.lblDescriptionFilter.Name = "lblDescriptionFilter";
            this.lblDescriptionFilter.Size = new System.Drawing.Size(193, 13);
            this.lblDescriptionFilter.TabIndex = 10;
            this.lblDescriptionFilter.Text = "Filter by description, comma separated: ";
            // 
            // btnSelAlign
            // 
            this.btnSelAlign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelAlign.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelAlign.Location = new System.Drawing.Point(857, 37);
            this.btnSelAlign.Name = "btnSelAlign";
            this.btnSelAlign.Size = new System.Drawing.Size(29, 19);
            this.btnSelAlign.TabIndex = 12;
            this.btnSelAlign.Tag = "Select Alignment";
            this.btnSelAlign.Text = "+";
            this.btnSelAlign.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSelAlign.UseVisualStyleBackColor = true;
            this.btnSelAlign.Click += new System.EventHandler(this.btnSelAlign_Click);
            // 
            // ExportPoints
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 691);
            this.Controls.Add(this.btnSelAlign);
            this.Controls.Add(this.txtBoxFilter);
            this.Controls.Add(this.lblDescriptionFilter);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtBoxFile);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.lblSelectAlignment);
            this.Controls.Add(this.cmbBoxAlignment);
            this.Controls.Add(this.lblFileSelect);
            this.Name = "ExportPoints";
            this.Load += new System.EventHandler(this.ExportPoints_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFileSelect;
        private System.Windows.Forms.Label lblSelectAlignment;
        public System.Windows.Forms.ComboBox cmbBoxAlignment;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtBoxFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox txtBoxFilter;
        private System.Windows.Forms.Label lblDescriptionFilter;
        private System.Windows.Forms.Button btnSelAlign;
    }
}
