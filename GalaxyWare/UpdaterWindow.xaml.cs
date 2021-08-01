using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GalaxyWare
{
    public partial class UpdaterWindow : Window
    {
        public UpdaterWindow()
        {
            this.InitializeComponent();
            this.webClient.DownloadProgressChanged += this.DownloadProgressChanged;
            this.webClient.DownloadFileCompleted += this.DownloadFileCompleted;
            this.key = SettingsWrapper.GetValue(SettingsAttribute.Key);
            this.arrowUp = new BitmapImage();
            this.arrowUp.BeginInit();
            this.arrowUp.UriSource = new Uri("pack://application:,,,/Resources/Images/Dropdown Arrow Up.png", UriKind.Absolute);
            this.arrowUp.CacheOption = BitmapCacheOption.OnLoad;
            this.arrowUp.EndInit();
            this.arrowDown = new BitmapImage();
            this.arrowDown.BeginInit();
            this.arrowDown.UriSource = new Uri("pack://application:,,,/Resources/Images/Dropdown Arrow Down.png", UriKind.Absolute);
            this.arrowDown.CacheOption = BitmapCacheOption.OnLoad;
            this.arrowDown.EndInit();
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.UpdateText.Text = "ОБНОВЛЕНИЕ КЛИЕНТА: " + (e.BytesReceived / (long)(this.updateSize / 100)).ToString() + "%";
        }

        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.UpdateText.Text = "ОБНОВЛЕНИЕ НЕ ТРЕБУЕТСЯ";
            this.ButtonLaunch.IsEnabled = true;
            using (Stream responseStream = WebRequest.Create("https://adminxyz.ru/api/galaxyware.php?action=get_update_hash").GetResponse().GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    string value = streamReader.ReadToEnd();
                    SettingsWrapper.SetValue(SettingsAttribute.UpdateHash, value);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                base.DragMove();
            }
            catch
            {
            }
        }

        private void Hide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.WindowState = WindowState.Minimized;
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                using (Stream responseStream = WebRequest.Create("https://adminxyz.ru/api/galaxyware.php?action=get_update_hash").GetResponse().GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        string b = streamReader.ReadToEnd();
                        if (SettingsWrapper.GetValue(SettingsAttribute.UpdateHash) != b)
                        {
                            base.Dispatcher.BeginInvoke(new Action(delegate ()
                            {
                                this.UpdateText.Text = "ДОСТУПНО ОБНОВЛЕНИЕ";
                            }), Array.Empty<object>());
                        }
                        else
                        {
                            base.Dispatcher.BeginInvoke(new Action(delegate ()
                            {
                                this.UpdateText.Text = "ОБНОВЛЕНИЕ НЕ ТРЕБУЕТСЯ";
                                this.ButtonLaunch.IsEnabled = true;
                            }), Array.Empty<object>());
                        }
                    }
                }
            }).Start();
        }

        public void SetLicenseDays(int count)
        {
            string text = count.ToString();
            switch (text[text.Length - 1])
            {
                case '0':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    this.ActivationDaysText.Text = count.ToString() + " ДНЕЙ";
                    return;
                case '2':
                case '3':
                case '4':
                    this.ActivationDaysText.Text = count.ToString() + " ДНЯ";
                    return;
            }
            this.ActivationDaysText.Text = count.ToString() + " ДЕНЬ";
        }

        private void ButtonUpdate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Выберите рабочий стол";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (StreamReader streamReader = new StreamReader(WebRequest.Create("https://adminxyz.ru/api/galaxyware.php?action=get_update_size").GetResponse().GetResponseStream()))
                {
                    this.updateSize = Convert.ToInt32(streamReader.ReadToEnd());
                }
                this.webClient.DownloadFileAsync(new Uri("https://adminxyz.ru/api/galaxyware.php?action=get_update&key=" + key), folderBrowserDialog.SelectedPath + "/update.zip");
            }
        }

        private void DropdownItem1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            this.DropdownValue.Content = label.Content;
            this.DropdownMenu.Visibility = Visibility.Hidden;
            this.DropdownArrow.Source = this.arrowDown;
            this.dropdownShow = false;
        }

        private void DropdownItem2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Label label = (System.Windows.Controls.Label)sender;
            this.DropdownValue.Content = label.Content;
            this.DropdownMenu.Visibility = Visibility.Hidden;
            this.DropdownArrow.Source = this.arrowDown;
            this.dropdownShow = false;
        }
        private void DropdownValue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.dropdownShow)
            {
                this.DropdownMenu.Visibility = Visibility.Visible;
                this.DropdownArrow.Source = this.arrowUp;
                this.dropdownShow = true;
                return;
            }
            this.DropdownMenu.Visibility = Visibility.Hidden;
            this.DropdownArrow.Source = this.arrowDown;
            this.dropdownShow = false;
        }

        private void ButtonLaunch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DropdownValue.Content.Equals("Выберите игру"))
            {
                System.Windows.MessageBox.Show("Пожалуйста, выберите игру", "GalaxyWare", MessageBoxButton.OK);
                return;
            }
            bool flag = false;
            string text = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\inj.cmd";
            StreamWriter streamWriter = new StreamWriter(text);
            foreach (Process process in Process.GetProcesses())
            {
                if (process.ProcessName == "VALORANT" || process.ProcessName == "VALORANT-Win64-Shipping")
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                streamWriter.WriteLine("@echo off");
                streamWriter.WriteLine("title GalaxyWare loader");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("timeout /t 1 /nobreak > nul");
                streamWriter.WriteLine("cls");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("echo [      ] Initializing loader core");
                streamWriter.WriteLine("timeout /t 2 /nobreak > nul");
                streamWriter.WriteLine("cls");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("echo [ DONE ] Initializing loader core");
                streamWriter.WriteLine("echo [      ] Looking for game process");
                streamWriter.WriteLine("timeout /t 1 /nobreak > nul");
                streamWriter.WriteLine("cls");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("echo [ DONE ] Initializing loader core");
                streamWriter.WriteLine("echo [ DONE ] Looking for game process");
                streamWriter.WriteLine("echo [      ] Injecting DLLs");
                streamWriter.WriteLine("timeout /t 3 /nobreak > nul");
                streamWriter.WriteLine("cls");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("echo [ DONE ] Initializing loader core");
                streamWriter.WriteLine("echo [ DONE ] Looking for game process");
                streamWriter.WriteLine("echo [ DONE ] Injecting DLLs");
                streamWriter.WriteLine("echo [      ] Wrapping game hooks");
                streamWriter.WriteLine("timeout /t 1 /nobreak > nul");
                streamWriter.WriteLine("cls");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("echo [ DONE ] Initializing loader core");
                streamWriter.WriteLine("echo [ DONE ] Looking for game process");
                streamWriter.WriteLine("echo [ DONE ] Injecting DLLs");
                streamWriter.WriteLine("echo [ DONE ] Wrapping game hooks^\n\n");
                streamWriter.WriteLine("echo Cheats sucessfully loaded. Enjoy the game!");
                streamWriter.WriteLine("timeout /t 5 /nobreak > nul");
                streamWriter.Close();
            }
            else
            {
                streamWriter.WriteLine("@echo off");
                streamWriter.WriteLine("title GalaxyWare loader");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("timeout /t 1 /nobreak > nul");
                streamWriter.WriteLine("cls");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("echo [      ] Initializing loader core");
                streamWriter.WriteLine("timeout /t 2 /nobreak > nul");
                streamWriter.WriteLine("cls");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("echo [ DONE ] Initializing loader core");
                streamWriter.WriteLine("echo [      ] Looking for game process");
                streamWriter.WriteLine("timeout /t 1 /nobreak > nul");
                streamWriter.WriteLine("cls");
                streamWriter.WriteLine("echo GalaxyWare cheat loader v2.4");
                streamWriter.WriteLine("echo Running on %USERDOMAIN%^\n\n");
                streamWriter.WriteLine("echo [ DONE ] Initializing loader core");
                streamWriter.WriteLine("echo [ FAIL ] Looking for game process");
                streamWriter.WriteLine("echo ^\n\nGame process not found. Please, run the game, and launch loader");
                streamWriter.WriteLine("timeout /t 5 /nobreak > nul");
                streamWriter.Close();
            }
            Process.Start(text);
        }

        private const bool https = true;

        private const string host = "adminxyz.ru";

        private const string handler = "galaxyware.php";

        private string key;

        private WebClient webClient = new WebClient();

        private int updateSize;

        private bool dropdownShow;

        private BitmapImage arrowUp;

        private BitmapImage arrowDown;
    }
}
