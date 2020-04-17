using System;
using System.Windows.Forms;

namespace Sarsoo.Alchemy.Forms
{
    public partial class ConfigForm : Form
    {
        public Config config { get; set; }

        public ConfigForm(Config config)
        {
            InitializeComponent();
            this.config = config;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            config.SpotifyAccess = spotifyAccessBox.Text.Trim();
            config.SpotifyRefresh = spotifyRefreshBox.Text.Trim();
            config.LastfmApi = lastFMApiBox.Text.Trim();

            config.spotifyID = spotifyUsername.Text.Trim();
            config.lastfmID = lastfmUsername.Text.Trim();
            Close();
        }

        private void KeyForm_Load(object sender, EventArgs e)
        {
            spotifyAccessBox.Text = config.SpotifyAccess ?? "";
            spotifyRefreshBox.Text = config.SpotifyRefresh ?? "";
            lastFMApiBox.Text = config.LastfmApi ?? "";

            spotifyUsername.Text = config.spotifyID ?? "";
            lastfmUsername.Text = config.lastfmID ?? "";
        }
    }

    [Serializable]
    public class Config
    {
        public string SpotifyAccess = "";
        public string SpotifyRefresh = "";
        public string LastfmApi = "";

        public string spotifyID = "";
        public string lastfmID = "";
    }
}
