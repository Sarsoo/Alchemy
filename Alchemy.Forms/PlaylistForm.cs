using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sarsoo.Alchemy.Forms
{
    public partial class PlaylistForm : Form
    {
        private readonly SimplePlaylist playlist;
        public PlaylistForm(SimplePlaylist _playlist)
        {
            InitializeComponent();
            playlist = _playlist;
        }

        private void Playlist_Load(object sender, EventArgs e)
        {
            Text = playlist.Name;
        }
    }
}
