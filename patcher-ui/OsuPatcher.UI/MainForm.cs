using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsuPatcher.UI
{
    internal sealed class MainForm : Form
    {
        private static readonly Color ErrorColor  = Color.FromArgb(0xF2, 0x64, 0x5A);
        private static readonly Color OkColor      = Color.FromArgb(0x4C, 0xA0, 0x18);
        private static readonly Color NeutralColor = Color.FromArgb(0x60, 0x60, 0x60);

        private readonly Config _config = Config.Load();
        private bool _busy;

        private PictureBox _logo;
        private Label _statusText;
        private LinkLabel _locateLink;
        private Button _injectButton;

        public MainForm()
        {
            BuildUi();
            RefreshStatus();
        }

        private void BuildUi()
        {
            Text            = "osu!somtum patcher";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            ClientSize      = new Size(440, 440);

            try { Icon = new Icon(GetType().Assembly.GetManifestResourceStream("OsuPatcher.UI.icon.ico")); }
            catch { /* icon optional */ }

            // Logo
            _logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(28, 20),
                Size     = new Size(ClientSize.Width - 56, 180),
            };
            try
            {
                using (var stream = GetType().Assembly.GetManifestResourceStream("OsuPatcher.UI.Assets.logo.png"))
                    if (stream != null) _logo.Image = Image.FromStream(stream);
            }
            catch { /* logo optional */ }
            Controls.Add(_logo);

            // Status text
            _statusText = new Label
            {
                Text      = "",
                ForeColor = ErrorColor,
                Font      = new Font(Font.FontFamily, 10f),
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(20, 240),
                Size      = new Size(ClientSize.Width - 40, 60),
            };
            Controls.Add(_statusText);

            // Locate link
            _locateLink = new LinkLabel
            {
                Text      = "Locate osu!.exe",
                TextAlign = ContentAlignment.MiddleCenter,
                Location  = new Point(20, 302),
                Size      = new Size(ClientSize.Width - 40, 20),
                Visible   = false,
            };
            _locateLink.LinkClicked += (_, __) => OnLocate();
            Controls.Add(_locateLink);

            // Inject button
            _injectButton = new Button
            {
                Text     = "Inject",
                Size     = new Size(ClientSize.Width - 120, 50),
                Location = new Point(60, ClientSize.Height - 50 - 28),
                Font     = new Font(Font.FontFamily, 14f),
            };
            _injectButton.Click += OnInject;
            Controls.Add(_injectButton);
        }

        private void RefreshStatus()
        {
            if (_busy) return;
            var osu = InjectorCore.FindRunningOsu();
            _locateLink.Visible = false;

            switch (osu.State)
            {
                case InjectorCore.OsuState.RunningValid:
                    SetStatus("osu! is running — ready to inject", OkColor);
                    break;
                case InjectorCore.OsuState.RunningInvalid:
                    SetStatus($"osu! is running but not on -devserver {InjectorCore.DevServer}.\nClose it first.", ErrorColor);
                    break;
                case InjectorCore.OsuState.NotRunning when KnownOsuPath() != null:
                    SetStatus("osu! is not running — Inject will launch it", NeutralColor);
                    break;
                default:
                    SetStatus("osu! could not be found\nStart the game or locate it", ErrorColor);
                    _locateLink.Visible = true;
                    break;
            }
        }

        private string KnownOsuPath() =>
            !string.IsNullOrEmpty(_config.OsuPath) && File.Exists(_config.OsuPath) ? _config.OsuPath : null;

        private async void OnInject(object sender, EventArgs e)
        {
            if (_busy) return;
            SetBusy(true);

            // Hide the window immediately after the user clicks Inject
            Hide();

            try
            {
                await Task.Run(() => RunInjectFlow(CancellationToken.None));
                Close();
            }
            catch (OperationCanceledException)
            {
                Show();
                SetBusy(false);
                RefreshStatus();
            }
            catch (Exception ex)
            {
                Show();
                SetBusy(false);
                SetStatus(ex.Message, ErrorColor);
            }
        }

        private void RunInjectFlow(CancellationToken token)
        {
            var osu = InjectorCore.FindRunningOsu();

            if (osu.State == InjectorCore.OsuState.RunningInvalid)
                throw new Exception($"osu! is running but not on -devserver {InjectorCore.DevServer}. Close it first.");

            if (osu.State == InjectorCore.OsuState.RunningValid)
            {
                if (!string.IsNullOrEmpty(osu.ExecutablePath))
                {
                    _config.OsuPath = osu.ExecutablePath;
                    _config.Save();
                }
                InjectorCore.Inject(osu.Pid);
                return;
            }

            // Not running — need a path
            var path = KnownOsuPath() ?? AskForOsuPathOnUiThread();
            if (path == null) throw new OperationCanceledException();

            _config.OsuPath = path;
            _config.Save();

            var proc = InjectorCore.LaunchOsu(path);
            var pid  = InjectorCore.WaitForInjectable(proc, token, TimeSpan.FromSeconds(60));
            InjectorCore.Inject(pid);
        }

        private string AskForOsuPathOnUiThread()
        {
            return (string)Invoke(new Func<string>(() =>
            {
                using (var dialog = new OpenFileDialog
                {
                    Title    = "Locate osu!.exe",
                    Filter   = "osu! executable (osu!.exe)|osu!.exe|Executables (*.exe)|*.exe",
                    FileName = "osu!.exe",
                })
                {
                    return dialog.ShowDialog(this) == DialogResult.OK ? dialog.FileName : null;
                }
            }));
        }

        private void OnLocate()
        {
            var path = AskForOsuPathOnUiThread();
            if (path == null) return;
            _config.OsuPath = path;
            _config.Save();
            RefreshStatus();
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            _injectButton.Enabled = !busy;
        }

        private void SetStatus(string message, Color color)
        {
            _statusText.Text      = message;
            _statusText.ForeColor = color;
        }
    }
}
