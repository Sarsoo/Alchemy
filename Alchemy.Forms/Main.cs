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

                // TODO load playlists if available
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

        private void SetMessage(string message)
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

            playlists.Clear();
            Paging<SimplePlaylist> _playlistsPagers = spot.GetUserPlaylists(config.spotifyID);
            while(true) {
                if (_playlistsPagers.HasError())
                {
                    SetMessage($"Playlist Refresh Error: {_playlistsPagers.Error.Status} {_playlistsPagers.Error.Message}");
                }
                else
                {
                    playlists.AddRange(_playlistsPagers.Items);
                    if (!_playlistsPagers.HasNextPage()) break;
                    _playlistsPagers = spot.GetNextPage(_playlistsPagers);
                }
            }

            playlistListBox.Invoke((MethodInvoker)delegate { 
                playlistListBox.DataSource = playlists
                    .OrderBy(playlist => playlist.Name.ToLower())
                    .ToList();
            });

            // TODO dump playlists to binary
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            Stream stream = new FileStream("config.bin", FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, config);
            stream.Close();

            // TODO dump playlists
        }

        private void playlistListBox_DoubleClick(object sender, EventArgs e)
        {
            if(playlistListBox.SelectedItem != null)
            {
                if (playlistListBox.SelectedItem is SimplePlaylist playlist)
                {
                    PlaylistForm form = new PlaylistForm(playlist);
                    form.Show();
                }
                else
                    SetMessage("Couldn't cast selected playlist");
            }
        }

        private void refreshPlaylistsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (config.SpotifyAccess.Length == 0)
            {
                SetMessage("No Spotify Access Token Provided");
                return;
            }

            if (!RefreshPlaylists.IsBusy)
            {
                RefreshPlaylists.RunWorkerAsync();
            }
            else
            {
                SetMessage("Already Refreshing");
            }
        }
    }
}
