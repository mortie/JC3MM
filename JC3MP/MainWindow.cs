using System;
using System.IO;
using System.Windows.Forms;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace JC3MP
{
	public partial class MainWindow : Form
	{
		private static String dataPath = Environment.ExpandEnvironmentVariables("%APPDATA%\\JC3MM");
		private static String modsPath = Path.Combine(dataPath, "mods");
		private static String jc3Dir = getJC3Dir();
		private static String dropzone = Path.Combine(jc3Dir, "dropzone");

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

			InitializeComponent();

			drawModList();
		}

		//Click Handlers

		private void btnAddMod_Click(object sender, EventArgs e)
		{
			addMod();
		}

		private void btnBuildMods_Click(object sender, EventArgs e)
		{
			buildMods();
		}

		// Functionality

		private void addMod()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.InitialDirectory = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
				"Downloads"
			);
			dialog.RestoreDirectory = true;
			dialog.Filter = "Mod Files (*.zip)|*.zip";

			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			String toPath = Path.Combine(modsPath, Path.GetFileName(dialog.FileName));

			try
			{
				File.Copy(dialog.FileName, toPath);
			}
			catch (IOException ex)
			{
				if (File.Exists(toPath))
				{
					MessageBox.Show("Mod " + Path.GetFileName(dialog.FileName) + " already exists.");
				}
				else
				{
					MessageBox.Show(ex.ToString());
				}
				return;
			}

			drawModList();
		}

		private void buildMods()
		{
			Cursor.Current = Cursors.WaitCursor;

			//Clear dropzone
			deldir(dropzone);
			mkdir(dropzone);

			string[] paths = Directory.GetFiles(modsPath);

			foreach (string path in paths)
			{
				Stream readStream = new FileStream(path, FileMode.Open);
				ZipArchive archive = new ZipArchive(readStream, ZipArchiveMode.Read);

				foreach (ZipArchiveEntry ent in archive.Entries)
				{
					Stream stream = ent.Open();
					Console.WriteLine(ent.FullName);
					stream.Close();
				}

				readStream.Close();
				Application.DoEvents();
			}

			Cursor.Current = Cursors.Default;
			MessageBox.Show("Mods built.");
		}

		private void drawModList()
		{
			string[] paths = Directory.GetFiles(modsPath);

			modList.Items.Clear();
			foreach (string str in paths)
			{
				String name = Path.GetFileNameWithoutExtension(str);
				modList.Items.Add(name);
			}
		}

		//Utility

		//Create directory if not exists
		private void mkdir(String path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		//Delete directory if it exists
		private void deldir(String path)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
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
