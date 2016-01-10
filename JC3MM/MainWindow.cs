using System;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JC3MM
{
	public partial class MainWindow : Form
	{
		private class Mod
		{
			public string ModPath { get; }

			public Mod(string path)
			{
				this.ModPath = path;
			}

			public String DisplayName
			{
				get { return Path.GetFileNameWithoutExtension(ModPath); }
			}

			public String FileName
			{
				get { return Path.GetFileName(ModPath); }
			}
		}

		private class ListDialog
		{
			private string caption;
			private string desc;
			private ArrayList strings;

			public ListDialog(string caption, string desc, ArrayList strings)
			{
				this.caption = caption;
				this.desc = desc;
				this.strings = strings;
			}

			public int ShowDialog()
			{
				int W = 300;
				int H = 300;
				Form form = new Form();
				form.Width = W;
				form.Height = H;
				form.Text = this.caption;
				form.FormBorderStyle = FormBorderStyle.FixedSingle;
				form.MaximizeBox = false;
				form.MinimizeBox = false;

				Label caption = new Label() { Top = 10, Left = 10 };
				caption.Text = this.desc;
				caption.AutoSize = true;
				caption.MaximumSize = new System.Drawing.Size(W - 20, 0);
				caption.Size = new System.Drawing.Size(caption.PreferredWidth, caption.PreferredHeight);

				ListBox listBox = new ListBox() { Width = W - 40, Height = H - 100 - caption.Size.Height, Top = 20 + caption.Size.Height, Left = 10 };
				Button btnOk = new Button() { Text = "OK", Left = W - 105, Top = H - 70, DialogResult = DialogResult.OK };
				Button btnCancel = new Button() { Text = "Cancel", Left = 10, Top = H - 70, DialogResult = DialogResult.Cancel };

				foreach (string str in strings)
				{
					listBox.Items.Add(str);
				}
				listBox.SelectedIndex = 0;

				form.Controls.Add(caption);
				form.Controls.Add(listBox);
				form.Controls.Add(btnOk);
				form.Controls.Add(btnCancel);

				DialogResult result = form.ShowDialog();
				if (result == DialogResult.OK)
				{
					return listBox.SelectedIndex;
				}
				else
				{
					return -1;
				}
			}
		}

		private static String dataPath = Environment.ExpandEnvironmentVariables("%APPDATA%\\JC3MM");
		private static String modsPath = Path.Combine(dataPath, "mods");
		private static String jc3Dir = getJC3Dir();
		private static String dropzoneDir = Path.Combine(jc3Dir, "dropzone");
		private static String dropzoneDirBak = Path.Combine(jc3Dir, "dropzone.bak");
		private static ArrayList mods = new ArrayList();

		public MainWindow()
		{
			if (!Directory.Exists(jc3Dir))
			{
				MessageBox.Show("Couldn't find Just Cause 3!");
				Environment.Exit(1);
			}
	
			//Create data directories
			mkdir(dataPath);
			mkdir(modsPath);
			mkdir(dropzoneDir);

			InitializeComponent();

			updateModList();
		}

		//Click Handlers

		private void btnAddMod_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.InitialDirectory = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				"Downloads"
			);
			dialog.RestoreDirectory = true;
			dialog.Filter = "Mod Files (*.zip)|*.zip";
			dialog.Multiselect = true;

			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			foreach (string path in dialog.FileNames)
			{
				addMod(path);
			}
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			removeMod();
		}

		private void btnBuildMods_Click(object sender, EventArgs e)
		{
			buildMods();
		}

		// Functionality

		private void addMod(string path)
		{
			String toPath = Path.Combine(modsPath, Path.GetFileName(path));

			try
			{
				File.Copy(path, toPath);
			}
			catch (IOException ex)
			{
				if (File.Exists(toPath))
				{
					MessageBox.Show("Mod " + Path.GetFileName(path) + " already exists.");
				}
				else
				{
					MessageBox.Show(ex.ToString());
				}
				return;
			}

			updateModList();
		}

		void removeMod()
		{
			if (modList.SelectedIndex == -1)
			{
				return;
			}

			Mod mod = (Mod)mods[modList.SelectedIndex];

			try
			{
				File.Delete(mod.ModPath);
			}
			catch (IOException ex)
			{
				MessageBox.Show(ex.ToString());
			}

			updateModList();
		}

		private void buildMods()
		{
			Console.WriteLine(dropzoneDir);
			Cursor.Current = Cursors.WaitCursor;

			//Clear dropzone
			rmdir(dropzoneDirBak);
			Directory.Move(dropzoneDir, dropzoneDirBak);
			mkdir(dropzoneDir);

			//Prepare regex
			Regex dropRelReg = new Regex(".*/dropzone/");

			//Keep track of what paths are owned by which mods
			Dictionary<String, Mod> modFromPath = new Dictionary<String, Mod>();

			foreach (Mod mod in mods)
			{
				Cursor.Current = Cursors.WaitCursor;

				//Build mod, aborting if it failed
				if (buildMod(mod, modFromPath) == false)
				{
					Cursor.Current = Cursors.Default;
					rmdir(dropzoneDir);
					Directory.Move(dropzoneDirBak, dropzoneDir);
					return;
				}
			}

			Cursor.Current = Cursors.Default;
			MessageBox.Show("Mods built.");
		}

		//Utility

		//Draw the mods to the GUI
		private void drawModList()
		{
			modList.Items.Clear();
			foreach (Mod mod in mods)
			{
				modList.Items.Add(mod.DisplayName);
			}
		}

		//Update the list of mods
		private void updateModList()
		{
			string[] paths = Directory.GetFiles(modsPath);

			mods = new ArrayList();
			foreach (string path in paths)
			{
				mods.Add(new Mod(path));
			}

			drawModList();
		}

		//Build mod to dropzone
		private bool buildMod(Mod mod, Dictionary<string, Mod> modFromPath)
		{
			Console.WriteLine("building mod " + mod.DisplayName);
			Stream readStream;
			try
			{
				readStream = new FileStream(mod.ModPath, FileMode.Open);
			}
			catch (IOException ex)
			{
				MessageBox.Show(ex.Message);
				return false;
			}

			ZipArchive archive = new ZipArchive(readStream, ZipArchiveMode.Read);

			//Get dropzones
			ArrayList dropzones = new ArrayList();
			Regex rx = new Regex("^((.*/)?dropzone)");
			foreach (ZipArchiveEntry ent in archive.Entries)
			{
				Match match = rx.Match(ent.FullName);
				string zone = match.Groups[1].Value;
				if (match.Success && !dropzones.Contains(zone))
				{
					dropzones.Add(zone);
				}
			}

			//Get single dropzone
			string dropzone = "";
			if (dropzones.Count == 0)
			{
				readStream.Close();
				MessageBox.Show("Mod " + mod.DisplayName + " has no dropzone.");
				return false;
			}
			else if (dropzones.Count == 1)
			{
				dropzone = (string)dropzones[0];
			}
			else
			{
				string caption = mod.DisplayName + " has multiple options. Choose one.";
				ListDialog dialog = new ListDialog(mod.DisplayName, caption, dropzones);
				int i = dialog.ShowDialog();
				if (i == -1)
				{
					readStream.Close();
					return false;
				}
				else
				{
					dropzone = (string)dropzones[i];
				}
			}

			foreach (ZipArchiveEntry ent in archive.Entries)
			{
				if (!ent.FullName.StartsWith(dropzone))
				{
					continue;
				}

				string relPath = ent.FullName.Replace(dropzone, "");
				if (relPath[0] == '/')
				{
					relPath = relPath.Substring(1);
				}

				string outPath = Path.Combine(dropzoneDir, relPath);

				//If it's a directory, create it
				if (ent.FullName.EndsWith("/"))
				{
					Console.WriteLine("mkdir " + outPath);
					mkdir(outPath);
				}

				//If not, write the file
				else
				{
					//Check if mod collides with an existing mod
					Mod collidesWith;
					if (modFromPath.TryGetValue(relPath, out collidesWith))
					{
						readStream.Close();
						MessageBox.Show(
							"Mod '" + mod.DisplayName + "'" +
							" collides with '" + collidesWith.DisplayName + "'."
						);
						return false;
					}
					modFromPath.Add(relPath, mod);

					//Write entry to file
					Console.WriteLine("write " + outPath);
					Stream writeStream = File.Create(outPath);
					Stream stream = ent.Open();
					stream.CopyTo(writeStream);
					writeStream.Close();
					stream.Close();
				}
			}

			readStream.Close();
			return true;
		}

		//Delete directory if it exists
		private void rmdir(String path)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

		//Create directory if not exists
		private void mkdir(String path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}


		//Get JC3 install location
		private static String getJC3Dir()
		{
			//First, get config and steam path
			String[] searchPaths = {
				"Program Files\\Steam",
				"Program Files (x86)\\Steam"
			};
			String steamPath = null;
			String confFilePath = null;
			DriveInfo[] drives = DriveInfo.GetDrives();
			foreach (DriveInfo info in drives)
			{
				string letter = info.RootDirectory.FullName;
				foreach (String searchPath in searchPaths)
				{
					String path = Path.Combine(letter, searchPath);
					if (File.Exists(Path.Combine(path, "config\\config.vdf")))
					{
						steamPath = path;
						confFilePath = Path.Combine(path, "config\\config.vdf");
						break;
					}
				}

				if (steamPath != null)
					break;
			}

			if (steamPath == null)
			{
				return askForJC3Dir();
			}

			//First look in steamPath\steamapps, as that's most likely where
			//JC3 is, and saves us config parsing
			if (Directory.Exists(Path.Combine(steamPath, "steamapps\\common\\Just Cause 3")))
			{
				return Path.Combine(steamPath, "steamapps\\common\\Just Cause 3");
			}

			//When that doesn't work, we start parsing the config.
			String steamConf = File.ReadAllText(confFilePath);
			string regexPattern = "\\s*\"BaseInstallFolder_[0-9]+\"\\s*\"(.+)\"";
			MatchCollection matches = Regex.Matches(steamConf, regexPattern, RegexOptions.IgnoreCase);
			foreach (Match match in matches)
			{
				string dir = match.Groups[1].Value.Replace("\\\\", "\\");
				string jc = Path.Combine(dir, "steamapps\\common\\Just Cause 3");
				if (Directory.Exists(jc))
					return jc;
			}

			return askForJC3Dir();
		}

		//Ask where JC3 is located, for when we couldn't find it automatically
		private static String askForJC3Dir()
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Couldn't automatically find the Just Cause 3 directory. Please manually select it.";
			if (dialog.ShowDialog() != DialogResult.OK)
			{
				Environment.Exit(0);
			}

			return dialog.SelectedPath;
		}
	}
}
