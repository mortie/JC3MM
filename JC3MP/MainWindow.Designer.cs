namespace JC3MM
{
	partial class MainWindow
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
			this.btnAddMod = new System.Windows.Forms.Button();
			this.modList = new System.Windows.Forms.ListBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnBuildMods = new System.Windows.Forms.Button();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// btnAddMod
			// 
			this.btnAddMod.Location = new System.Drawing.Point(13, 13);
			this.btnAddMod.Name = "btnAddMod";
			this.btnAddMod.Size = new System.Drawing.Size(75, 23);
			this.btnAddMod.TabIndex = 0;
			this.btnAddMod.Text = "Add Mod";
			this.btnAddMod.UseVisualStyleBackColor = true;
			this.btnAddMod.Click += new System.EventHandler(this.btnAddMod_Click);
			// 
			// modList
			// 
			this.modList.FormattingEnabled = true;
			this.modList.Location = new System.Drawing.Point(13, 42);
			this.modList.Name = "modList";
			this.modList.Size = new System.Drawing.Size(261, 316);
			this.modList.TabIndex = 1;
			// 
			// btnRemove
			// 
			this.btnRemove.Location = new System.Drawing.Point(198, 13);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(75, 23);
			this.btnRemove.TabIndex = 3;
			this.btnRemove.Text = "Remove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnBuildMods
			// 
			this.btnBuildMods.Location = new System.Drawing.Point(198, 365);
			this.btnBuildMods.Name = "btnBuildMods";
			this.btnBuildMods.Size = new System.Drawing.Size(75, 23);
			this.btnBuildMods.TabIndex = 4;
			this.btnBuildMods.Text = "Build Mods";
			this.btnBuildMods.UseVisualStyleBackColor = true;
			this.btnBuildMods.Click += new System.EventHandler(this.btnBuildMods_Click);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(286, 398);
			this.Controls.Add(this.btnBuildMods);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.modList);
			this.Controls.Add(this.btnAddMod);
			this.Name = "MainWindow";
			this.Text = "Just Cause 3 Mod Manager";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnAddMod;
		private System.Windows.Forms.ListBox modList;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnBuildMods;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
	}
}

