
namespace SortView
{
    partial class Form1
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbAlgorithm = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLen = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbPresort = new System.Windows.Forms.ComboBox();
            this.btnSort = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.sbSpeed = new System.Windows.Forms.HScrollBar();
            this.label5 = new System.Windows.Forms.Label();
            this.chkRange = new System.Windows.Forms.CheckBox();
            this.txtRange = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(248, -3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 529);
            this.panel1.TabIndex = 40;
            // 
            // cmbAlgorithm
            // 
            this.cmbAlgorithm.FormattingEnabled = true;
            this.cmbAlgorithm.Location = new System.Drawing.Point(86, 216);
            this.cmbAlgorithm.Name = "cmbAlgorithm";
            this.cmbAlgorithm.Size = new System.Drawing.Size(149, 21);
            this.cmbAlgorithm.TabIndex = 41;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Алгоритм:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 43;
            this.label2.Text = "Размер массива:";
            // 
            // txtLen
            // 
            this.txtLen.Location = new System.Drawing.Point(114, 17);
            this.txtLen.Name = "txtLen";
            this.txtLen.Size = new System.Drawing.Size(121, 20);
            this.txtLen.TabIndex = 44;
            this.txtLen.TextChanged += new System.EventHandler(this.txtLen_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 13);
            this.label3.TabIndex = 46;
            this.label3.Text = "Начальное заполнение:";
            // 
            // cmbPresort
            // 
            this.cmbPresort.FormattingEnabled = true;
            this.cmbPresort.Location = new System.Drawing.Point(114, 131);
            this.cmbPresort.Name = "cmbPresort";
            this.cmbPresort.Size = new System.Drawing.Size(121, 21);
            this.cmbPresort.TabIndex = 45;
            // 
            // btnSort
            // 
            this.btnSort.Location = new System.Drawing.Point(179, 254);
            this.btnSort.Name = "btnSort";
            this.btnSort.Size = new System.Drawing.Size(56, 24);
            this.btnSort.TabIndex = 49;
            this.btnSort.Text = "Sort";
            this.btnSort.UseVisualStyleBackColor = true;
            this.btnSort.Click += new System.EventHandler(this.btnSort_Click);
            // 
            // lblResult
            // 
            this.lblResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblResult.Location = new System.Drawing.Point(14, 385);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(216, 119);
            this.lblResult.TabIndex = 50;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(179, 168);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(56, 24);
            this.btnCreate.TabIndex = 51;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(179, 334);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(56, 24);
            this.btnStop.TabIndex = 52;
            this.btnStop.Text = "Cancel";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // sbSpeed
            // 
            this.sbSpeed.Location = new System.Drawing.Point(86, 297);
            this.sbSpeed.Name = "sbSpeed";
            this.sbSpeed.Size = new System.Drawing.Size(149, 14);
            this.sbSpeed.TabIndex = 54;
            this.sbSpeed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sbSpeed_Scroll);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 297);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 55;
            this.label5.Text = "Скорость:";
            // 
            // chkRange
            // 
            this.chkRange.AutoSize = true;
            this.chkRange.Location = new System.Drawing.Point(17, 55);
            this.chkRange.Name = "chkRange";
            this.chkRange.Size = new System.Drawing.Size(215, 17);
            this.chkRange.TabIndex = 56;
            this.chkRange.Text = "Диапазон значений по размеру, или:";
            this.chkRange.UseVisualStyleBackColor = true;
            this.chkRange.CheckedChanged += new System.EventHandler(this.chkRange_CheckedChanged);
            // 
            // txtRange
            // 
            this.txtRange.Location = new System.Drawing.Point(114, 78);
            this.txtRange.Name = "txtRange";
            this.txtRange.Size = new System.Drawing.Size(121, 20);
            this.txtRange.TabIndex = 57;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 525);
            this.Controls.Add(this.txtRange);
            this.Controls.Add(this.chkRange);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sbSpeed);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnSort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbPresort);
            this.Controls.Add(this.txtLen);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbAlgorithm);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Сортировки";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox cmbAlgorithm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbPresort;
        private System.Windows.Forms.Button btnSort;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.HScrollBar sbSpeed;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkRange;
        private System.Windows.Forms.TextBox txtRange;
    }
}

