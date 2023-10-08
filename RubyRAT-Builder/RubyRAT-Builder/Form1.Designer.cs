namespace RubyRAT_Builder
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
            this.btnGeneratePy = new System.Windows.Forms.Button();
            this.txtTextToken = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtInputToken = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbEP = new System.Windows.Forms.CheckBox();
            this.cbDM = new System.Windows.Forms.CheckBox();
            this.cbSE = new System.Windows.Forms.CheckBox();
            this.cbCTS = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnInstallDef = new System.Windows.Forms.Button();
            this.btnSeeDef = new System.Windows.Forms.Button();
            this.comboBoxVersions = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtGoogleAPI = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.txtCategorie = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGeneratePy
            // 
            this.btnGeneratePy.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGeneratePy.Location = new System.Drawing.Point(12, 300);
            this.btnGeneratePy.Name = "btnGeneratePy";
            this.btnGeneratePy.Size = new System.Drawing.Size(619, 138);
            this.btnGeneratePy.TabIndex = 0;
            this.btnGeneratePy.Text = ":: BUILD ::";
            this.btnGeneratePy.UseVisualStyleBackColor = true;
            this.btnGeneratePy.Click += new System.EventHandler(this.btnGeneratePy_Click);
            // 
            // txtTextToken
            // 
            this.txtTextToken.BackColor = System.Drawing.SystemColors.Control;
            this.txtTextToken.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTextToken.Location = new System.Drawing.Point(17, 21);
            this.txtTextToken.Name = "txtTextToken";
            this.txtTextToken.Size = new System.Drawing.Size(152, 15);
            this.txtTextToken.TabIndex = 1;
            this.txtTextToken.Text = "DISCORD BOT TOKEN:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtCategorie);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.txtGoogleAPI);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.txtInputToken);
            this.groupBox1.Controls.Add(this.txtTextToken);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 119);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CONNECTION";
            // 
            // txtInputToken
            // 
            this.txtInputToken.Location = new System.Drawing.Point(175, 18);
            this.txtInputToken.Name = "txtInputToken";
            this.txtInputToken.Size = new System.Drawing.Size(222, 22);
            this.txtInputToken.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbEP);
            this.groupBox2.Controls.Add(this.cbDM);
            this.groupBox2.Controls.Add(this.cbSE);
            this.groupBox2.Controls.Add(this.cbCTS);
            this.groupBox2.Location = new System.Drawing.Point(421, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 126);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "INSTALL  / PERSISTENCE";
            // 
            // cbEP
            // 
            this.cbEP.AutoSize = true;
            this.cbEP.Location = new System.Drawing.Point(6, 99);
            this.cbEP.Name = "cbEP";
            this.cbEP.Size = new System.Drawing.Size(128, 20);
            this.cbEP.TabIndex = 1;
            this.cbEP.Text = "Encrypt Payload";
            this.cbEP.UseVisualStyleBackColor = true;
            this.cbEP.CheckedChanged += new System.EventHandler(this.cbEP_CheckedChanged);
            // 
            // cbDM
            // 
            this.cbDM.AutoSize = true;
            this.cbDM.Location = new System.Drawing.Point(6, 73);
            this.cbDM.Name = "cbDM";
            this.cbDM.Size = new System.Drawing.Size(108, 20);
            this.cbDM.TabIndex = 7;
            this.cbDM.Text = "Debug Mode";
            this.cbDM.UseVisualStyleBackColor = true;
            // 
            // cbSE
            // 
            this.cbSE.AutoSize = true;
            this.cbSE.Location = new System.Drawing.Point(6, 47);
            this.cbSE.Name = "cbSE";
            this.cbSE.Size = new System.Drawing.Size(113, 20);
            this.cbSE.TabIndex = 6;
            this.cbSE.Text = "Start Elevated";
            this.cbSE.UseVisualStyleBackColor = true;
            // 
            // cbCTS
            // 
            this.cbCTS.AutoSize = true;
            this.cbCTS.Location = new System.Drawing.Point(6, 21);
            this.cbCTS.Name = "cbCTS";
            this.cbCTS.Size = new System.Drawing.Size(126, 20);
            this.cbCTS.TabIndex = 0;
            this.cbCTS.Text = "Copy To Startup";
            this.cbCTS.UseVisualStyleBackColor = true;
            this.cbCTS.CheckedChanged += new System.EventHandler(this.cbCTS_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtOutput);
            this.groupBox3.Location = new System.Drawing.Point(637, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(234, 424);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "OUTPUT";
            // 
            // txtOutput
            // 
            this.txtOutput.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(3, 18);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(228, 403);
            this.txtOutput.TabIndex = 0;
            // 
            // btnInstallDef
            // 
            this.btnInstallDef.Location = new System.Drawing.Point(12, 244);
            this.btnInstallDef.Name = "btnInstallDef";
            this.btnInstallDef.Size = new System.Drawing.Size(75, 50);
            this.btnInstallDef.TabIndex = 6;
            this.btnInstallDef.Text = "INSTALL DEFS";
            this.btnInstallDef.UseVisualStyleBackColor = true;
            this.btnInstallDef.Click += new System.EventHandler(this.btnInstallDef_Click);
            // 
            // btnSeeDef
            // 
            this.btnSeeDef.Location = new System.Drawing.Point(12, 188);
            this.btnSeeDef.Name = "btnSeeDef";
            this.btnSeeDef.Size = new System.Drawing.Size(75, 50);
            this.btnSeeDef.TabIndex = 7;
            this.btnSeeDef.Text = "SEE DEFS";
            this.btnSeeDef.UseVisualStyleBackColor = true;
            this.btnSeeDef.Click += new System.EventHandler(this.btnSeeDef_Click);
            // 
            // comboBoxVersions
            // 
            this.comboBoxVersions.FormattingEnabled = true;
            this.comboBoxVersions.Location = new System.Drawing.Point(510, 270);
            this.comboBoxVersions.Name = "comboBoxVersions";
            this.comboBoxVersions.Size = new System.Drawing.Size(121, 24);
            this.comboBoxVersions.TabIndex = 8;
            this.comboBoxVersions.SelectedIndexChanged += new System.EventHandler(this.comboBoxVersions_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(17, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(152, 15);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "GOOGLE MAPS API:";
            // 
            // txtGoogleAPI
            // 
            this.txtGoogleAPI.Location = new System.Drawing.Point(175, 45);
            this.txtGoogleAPI.Name = "txtGoogleAPI";
            this.txtGoogleAPI.Size = new System.Drawing.Size(222, 22);
            this.txtGoogleAPI.TabIndex = 5;
            this.txtGoogleAPI.Text = "(Only for V1.8 and above)";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.Control;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Location = new System.Drawing.Point(17, 78);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(152, 15);
            this.textBox3.TabIndex = 6;
            this.textBox3.Text = "CATEGORIE NAME:";
            // 
            // txtCategorie
            // 
            this.txtCategorie.Location = new System.Drawing.Point(175, 75);
            this.txtCategorie.Name = "txtCategorie";
            this.txtCategorie.Size = new System.Drawing.Size(222, 22);
            this.txtCategorie.TabIndex = 7;
            this.txtCategorie.Text = "(Only for V1.8 and above)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 450);
            this.Controls.Add(this.comboBoxVersions);
            this.Controls.Add(this.btnSeeDef);
            this.Controls.Add(this.btnInstallDef);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnGeneratePy);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGeneratePy;
        private System.Windows.Forms.TextBox txtTextToken;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtInputToken;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbCTS;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.CheckBox cbSE;
        private System.Windows.Forms.CheckBox cbDM;
        private System.Windows.Forms.Button btnInstallDef;
        private System.Windows.Forms.Button btnSeeDef;
        private System.Windows.Forms.ComboBox comboBoxVersions;
        private System.Windows.Forms.CheckBox cbEP;
        private System.Windows.Forms.TextBox txtCategorie;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox txtGoogleAPI;
        private System.Windows.Forms.TextBox textBox1;
    }
}

