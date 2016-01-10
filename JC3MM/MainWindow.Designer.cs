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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
			this.btnAddMod = new System.Windows.Forms.Button();
			this.modList = new System.Windows.Forms.ListBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnBuildMods = new System.Windows.Forms.Button();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// btnAddMod
			// 
			resources.ApplyResources(this.btnAddMod, "btnAddMod");
			this.btnAddMod.Name = "btnAddMod";
			this.btnAddMod.UseVisualStyleBackColor = true;
			this.btnAddMod.Click += new System.EventHandler(this.btnAddMod_Click);
			// 
			// modList
			// 
			this.modList.FormattingEnabled = true;
			resources.ApplyResources(this.modList, "modList");
			this.modList.Name = "modList";
			// 
			// btnRemove
			// 
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnBuildMods
			// 
			resources.ApplyResources(this.btnBuildMods, "btnBuildMods");
			this.btnBuildMods.Name = "btnBuildMods";
			this.btnBuildMods.UseVisualStyleBackColor = true;
			this.btnBuildMods.Click += new System.EventHandler(this.btnBuildMods_Click);
			// 
			// MainWindow
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnBuildMods);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.modList);
			this.Controls.Add(this.btnAddMod);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainWindow";
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

