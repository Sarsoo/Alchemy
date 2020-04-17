using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses


namespace Sarsoo.Alchemy.Forms
{
    public partial class MainForm : Form
    {
        private Config config = new Config();
        private List<SimplePlaylist> playlists = new List<SimplePlaylist>();

        public MainForm()
        {
            try
            {
                IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                Stream stream = new FileStream("config.bin", FileMode.Open, FileAccess.Read);
                config = (Config)formatter.Deserialize(stream);
                stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to read the config.bin " + ex.ToString());
            }

            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form about = new About();
            about.Show();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aPIKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigForm keys = new ConfigForm(config);
            keys.Show();
        }

        private void playlistRefreshButton_Click(object sender, EventArgs e)
        {
            if(config.SpotifyAccess.Length == 0)
            {
                SetMessage("No Spotify Access Token Provided");
                return;
            }

            if (!RefreshPlaylists.IsBusy)
            {
                RefreshPlaylists.RunWorkerAsync();
            } else
            {
                SetMessage("Already Refreshing");
            }
        }

        private void SetMessage(String message)
        {
            messageLabel.Text = message;
            messageLabel.Visible = true;
        }

        private void RefreshPlaylists_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (config.SpotifyAccess.Length == 0) return;

            var spot = new SpotifyWebAPI
            {
                AccessToken = config.SpotifyAccess,
                TokenType = "Bearer"
            };

            var _playlists = spot.GetUserPlaylists(config.spotifyID);
            if(_playlists.HasError())
            {
                SetMessage($"Playlist Refresh Error: {_playlists.Error.Status} {_playlists.Error.Message}");
            }
            else
            {
                playlistListBox.Items.Clear();
                //_playlists.Items.ForEach((playlist) => { playlistListBox.Items.Add(playlist); });
                playlistListBox.Refresh();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            playlistListBox.DataSource = playlists;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            Stream stream = new FileStream("config.bin", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, config);
            stream.Close();
        }
    }
}
