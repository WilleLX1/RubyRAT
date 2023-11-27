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
            this.txtPreStart = new System.Windows.Forms.TextBox();
            this.txtPreTask = new System.Windows.Forms.TextBox();
            this.txtPreReg = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.txtCategorie = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.txtGoogleAPI = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
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
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.txtPrePublic = new System.Windows.Forms.TextBox();
            this.btnAddVersion = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGeneratePy
            // 
            this.btnGeneratePy.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGeneratePy.Location = new System.Drawing.Point(9, 244);
            this.btnGeneratePy.Margin = new System.Windows.Forms.Padding(2);
            this.btnGeneratePy.Name = "btnGeneratePy";
            this.btnGeneratePy.Size = new System.Drawing.Size(464, 112);
            this.btnGeneratePy.TabIndex = 0;
            this.btnGeneratePy.Text = ":: BUILD ::";
            this.btnGeneratePy.UseVisualStyleBackColor = true;
            this.btnGeneratePy.Click += new System.EventHandler(this.btnGeneratePy_Click);
            // 
            // txtTextToken
            // 
            this.txtTextToken.BackColor = System.Drawing.SystemColors.Control;
            this.txtTextToken.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTextToken.Location = new System.Drawing.Point(13, 17);
            this.txtTextToken.Margin = new System.Windows.Forms.Padding(2);
            this.txtTextToken.Name = "txtTextToken";
            this.txtTextToken.Size = new System.Drawing.Size(114, 13);
            this.txtTextToken.TabIndex = 1;
            this.txtTextToken.Text = "DISCORD BOT TOKEN:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPrePublic);
            this.groupBox1.Controls.Add(this.textBox6);
            this.groupBox1.Controls.Add(this.txtPreStart);
            this.groupBox1.Controls.Add(this.txtPreTask);
            this.groupBox1.Controls.Add(this.txtPreReg);
            this.groupBox1.Controls.Add(this.textBox5);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.txtCategorie);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.txtGoogleAPI);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.txtInputToken);
            this.groupBox1.Controls.Add(this.txtTextToken);
            this.groupBox1.Location = new System.Drawing.Point(9, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(302, 184);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CONNECTION";
            // 
            // txtPreStart
            // 
            this.txtPreStart.Location = new System.Drawing.Point(130, 137);
            this.txtPreStart.Margin = new System.Windows.Forms.Padding(2);
            this.txtPreStart.Name = "txtPreStart";
            this.txtPreStart.Size = new System.Drawing.Size(168, 20);
            this.txtPreStart.TabIndex = 13;
            this.txtPreStart.Text = "(Only for V1.9.5 and above)";
            // 
            // txtPreTask
            // 
            this.txtPreTask.Location = new System.Drawing.Point(131, 113);
            this.txtPreTask.Margin = new System.Windows.Forms.Padding(2);
            this.txtPreTask.Name = "txtPreTask";
            this.txtPreTask.Size = new System.Drawing.Size(168, 20);
            this.txtPreTask.TabIndex = 12;
            this.txtPreTask.Text = "(Only for V1.9.5 and above)";
            // 
            // txtPreReg
            // 
            this.txtPreReg.Location = new System.Drawing.Point(131, 89);
            this.txtPreReg.Margin = new System.Windows.Forms.Padding(2);
            this.txtPreReg.Name = "txtPreReg";
            this.txtPreReg.Size = new System.Drawing.Size(168, 20);
            this.txtPreReg.TabIndex = 11;
            this.txtPreReg.Text = "(Only for V1.9.5 and above)";
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.SystemColors.Control;
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Location = new System.Drawing.Point(13, 140);
            this.textBox5.Margin = new System.Windows.Forms.Padding(2);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(114, 13);
            this.textBox5.TabIndex = 10;
            this.textBox5.Text = "STARTNAME";
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.Control;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Location = new System.Drawing.Point(13, 116);
            this.textBox4.Margin = new System.Windows.Forms.Padding(2);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(114, 13);
            this.textBox4.TabIndex = 9;
            this.textBox4.Text = "TASKNAME";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.Control;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(13, 92);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(114, 13);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = "REGNAME";
            // 
            // txtCategorie
            // 
            this.txtCategorie.Location = new System.Drawing.Point(131, 61);
            this.txtCategorie.Margin = new System.Windows.Forms.Padding(2);
            this.txtCategorie.Name = "txtCategorie";
            this.txtCategorie.Size = new System.Drawing.Size(168, 20);
            this.txtCategorie.TabIndex = 7;
            this.txtCategorie.Text = "(Only for V1.8 and above)";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.Control;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Location = new System.Drawing.Point(13, 63);
            this.textBox3.Margin = new System.Windows.Forms.Padding(2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(114, 13);
            this.textBox3.TabIndex = 6;
            this.textBox3.Text = "CATEGORIE NAME:";
            // 
            // txtGoogleAPI
            // 
            this.txtGoogleAPI.Location = new System.Drawing.Point(131, 37);
            this.txtGoogleAPI.Margin = new System.Windows.Forms.Padding(2);
            this.txtGoogleAPI.Name = "txtGoogleAPI";
            this.txtGoogleAPI.Size = new System.Drawing.Size(168, 20);
            this.txtGoogleAPI.TabIndex = 5;
            this.txtGoogleAPI.Text = "(Only for V1.8 and above)";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(13, 39);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(114, 13);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "GOOGLE MAPS API:";
            // 
            // txtInputToken
            // 
            this.txtInputToken.Location = new System.Drawing.Point(131, 15);
            this.txtInputToken.Margin = new System.Windows.Forms.Padding(2);
            this.txtInputToken.Name = "txtInputToken";
            this.txtInputToken.Size = new System.Drawing.Size(168, 20);
            this.txtInputToken.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbEP);
            this.groupBox2.Controls.Add(this.cbDM);
            this.groupBox2.Controls.Add(this.cbSE);
            this.groupBox2.Controls.Add(this.cbCTS);
            this.groupBox2.Location = new System.Drawing.Point(316, 10);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(158, 102);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "INSTALL  / PERSISTENCE";
            // 
            // cbEP
            // 
            this.cbEP.AutoSize = true;
            this.cbEP.Location = new System.Drawing.Point(4, 80);
            this.cbEP.Margin = new System.Windows.Forms.Padding(2);
            this.cbEP.Name = "cbEP";
            this.cbEP.Size = new System.Drawing.Size(103, 17);
            this.cbEP.TabIndex = 1;
            this.cbEP.Text = "Encrypt Payload";
            this.cbEP.UseVisualStyleBackColor = true;
            this.cbEP.CheckedChanged += new System.EventHandler(this.cbEP_CheckedChanged);
            // 
            // cbDM
            // 
            this.cbDM.AutoSize = true;
            this.cbDM.Location = new System.Drawing.Point(4, 59);
            this.cbDM.Margin = new System.Windows.Forms.Padding(2);
            this.cbDM.Name = "cbDM";
            this.cbDM.Size = new System.Drawing.Size(88, 17);
            this.cbDM.TabIndex = 7;
            this.cbDM.Text = "Debug Mode";
            this.cbDM.UseVisualStyleBackColor = true;
            // 
            // cbSE
            // 
            this.cbSE.AutoSize = true;
            this.cbSE.Location = new System.Drawing.Point(4, 38);
            this.cbSE.Margin = new System.Windows.Forms.Padding(2);
            this.cbSE.Name = "cbSE";
            this.cbSE.Size = new System.Drawing.Size(93, 17);
            this.cbSE.TabIndex = 6;
            this.cbSE.Text = "Start Elevated";
            this.cbSE.UseVisualStyleBackColor = true;
            // 
            // cbCTS
            // 
            this.cbCTS.AutoSize = true;
            this.cbCTS.Location = new System.Drawing.Point(4, 17);
            this.cbCTS.Margin = new System.Windows.Forms.Padding(2);
            this.cbCTS.Name = "cbCTS";
            this.cbCTS.Size = new System.Drawing.Size(103, 17);
            this.cbCTS.TabIndex = 0;
            this.cbCTS.Text = "Copy To Startup";
            this.cbCTS.UseVisualStyleBackColor = true;
            this.cbCTS.CheckedChanged += new System.EventHandler(this.cbCTS_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtOutput);
            this.groupBox3.Location = new System.Drawing.Point(478, 11);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(176, 344);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "OUTPUT";
            // 
            // txtOutput
            // 
            this.txtOutput.BackColor = System.Drawing.SystemColors.Window;
            this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(2, 15);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(2);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(172, 327);
            this.txtOutput.TabIndex = 0;
            // 
            // btnInstallDef
            // 
            this.btnInstallDef.Location = new System.Drawing.Point(9, 198);
            this.btnInstallDef.Margin = new System.Windows.Forms.Padding(2);
            this.btnInstallDef.Name = "btnInstallDef";
            this.btnInstallDef.Size = new System.Drawing.Size(56, 41);
            this.btnInstallDef.TabIndex = 6;
            this.btnInstallDef.Text = "INSTALL DEFS";
            this.btnInstallDef.UseVisualStyleBackColor = true;
            this.btnInstallDef.Click += new System.EventHandler(this.btnInstallDef_Click);
            // 
            // btnSeeDef
            // 
            this.btnSeeDef.Location = new System.Drawing.Point(69, 198);
            this.btnSeeDef.Margin = new System.Windows.Forms.Padding(2);
            this.btnSeeDef.Name = "btnSeeDef";
            this.btnSeeDef.Size = new System.Drawing.Size(56, 41);
            this.btnSeeDef.TabIndex = 7;
            this.btnSeeDef.Text = "SEE DEFS";
            this.btnSeeDef.UseVisualStyleBackColor = true;
            this.btnSeeDef.Click += new System.EventHandler(this.btnSeeDef_Click);
            // 
            // comboBoxVersions
            // 
            this.comboBoxVersions.FormattingEnabled = true;
            this.comboBoxVersions.Location = new System.Drawing.Point(382, 219);
            this.comboBoxVersions.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxVersions.Name = "comboBoxVersions";
            this.comboBoxVersions.Size = new System.Drawing.Size(92, 21);
            this.comboBoxVersions.TabIndex = 8;
            this.comboBoxVersions.SelectedIndexChanged += new System.EventHandler(this.comboBoxVersions_SelectedIndexChanged);
            // 
            // textBox6
            // 
            this.textBox6.BackColor = System.Drawing.SystemColors.Control;
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox6.Location = new System.Drawing.Point(13, 164);
            this.textBox6.Margin = new System.Windows.Forms.Padding(2);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(114, 13);
            this.textBox6.TabIndex = 14;
            this.textBox6.Text = "PUBLIC NAME";
            // 
            // txtPrePublic
            // 
            this.txtPrePublic.Location = new System.Drawing.Point(130, 161);
            this.txtPrePublic.Margin = new System.Windows.Forms.Padding(2);
            this.txtPrePublic.Name = "txtPrePublic";
            this.txtPrePublic.Size = new System.Drawing.Size(168, 20);
            this.txtPrePublic.TabIndex = 15;
            this.txtPrePublic.Text = "(Only for V1.9.5 and above)";
            // 
            // btnAddVersion
            // 
            this.btnAddVersion.Location = new System.Drawing.Point(382, 117);
            this.btnAddVersion.Name = "btnAddVersion";
            this.btnAddVersion.Size = new System.Drawing.Size(91, 23);
            this.btnAddVersion.TabIndex = 16;
            this.btnAddVersion.Text = "Add Version";
            this.btnAddVersion.UseVisualStyleBackColor = true;
            this.btnAddVersion.Click += new System.EventHandler(this.btnAddVersion_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 366);
            this.Controls.Add(this.btnAddVersion);
            this.Controls.Add(this.comboBoxVersions);
            this.Controls.Add(this.btnSeeDef);
            this.Controls.Add(this.btnInstallDef);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnGeneratePy);
            this.Margin = new System.Windows.Forms.Padding(2);
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
        private System.Windows.Forms.TextBox txtPreTask;
        private System.Windows.Forms.TextBox txtPreReg;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox txtPreStart;
        private System.Windows.Forms.TextBox txtPrePublic;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button btnAddVersion;
    }
}

