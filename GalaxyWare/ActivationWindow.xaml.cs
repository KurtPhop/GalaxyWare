using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace GalaxyWare
{
    public partial class ActivationWindow : Window
    {
        public ActivationWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
            }
        }

        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Hide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsWrapper.InitSettings();
            try
            {
                SettingsWrapper.Check();
            }
            catch
            {
                SettingsWrapper.Erase();
                MessageBox.Show("Файл конфигурации был поврежден. Необходимо пройти продцедуру активации", "GalaxyWare", MessageBoxButton.OK);
                return;
            }
            new Thread(delegate ()
            {
                try
                {
                    string value = SettingsWrapper.GetValue(SettingsAttribute.Key);
                    using (Stream responseStream = WebRequest.Create("https://adminxyz.ru/api/galaxyware.php?action=check_dateKey&dateKey=" + value).GetResponse().GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            string text = streamReader.ReadToEnd();
                            if (text != "dateKeyNotFound" && text != "dateKeyExpired")
                            {
                                string[] array = text.Split(new char[]
                                {
                                    '-'
                                });
                                DateTime now = DateTime.Now;
                                DateTime d = new DateTime(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]));
                                TimeSpan timeSpan = d - now;
                                if (timeSpan.Days >= 0)
                                {
                                    base.Dispatcher.BeginInvoke(new Action(delegate ()
                                    {
                                        UpdaterWindow updaterWindow = new UpdaterWindow();
                                        updaterWindow.SetLicenseDays(timeSpan.Days + 1);
                                        updaterWindow.Show();
                                        this.Close();
                                    }), Array.Empty<object>());
                                }
                                else
                                {
                                    SettingsWrapper.InitSettings();
                                    base.Dispatcher.BeginInvoke(new Action(delegate ()
                                    {
                                        this.StatusText.Text = "АВТОРИЗАЦИЯ В КЛИЕНТЕ";
                                        this.ButtonActivate.IsEnabled = true;
                                    }), Array.Empty<object>());
                                }
                            }
                            else
                            {
                                base.Dispatcher.BeginInvoke(new Action(delegate ()
                                {
                                    this.StatusText.Text = "АВТОРИЗАЦИЯ В КЛИЕНТЕ";
                                    this.ButtonActivate.IsEnabled = true;
                                }), Array.Empty<object>());
                            }
                        }
                    }
                }
                catch
                {
                    //string value2 = SettingsWrapper.GetValue(SettingsAttribute.Expires);
                    string value2 = "";
                    if (value2 != "")
                    {
                        DateTime now2 = DateTime.Now;
                        DateTime d2 = Convert.ToDateTime(value2);
                        TimeSpan timeSpan = d2 - now2;
                        if (timeSpan.Days >= 0)
                        {
                            base.Dispatcher.BeginInvoke(new Action(delegate ()
                            {
                                UpdaterWindow updaterWindow = new UpdaterWindow();
                                updaterWindow.SetLicenseDays(timeSpan.Days + 1);
                                updaterWindow.Show();
                                this.Close();
                            }), Array.Empty<object>());
                        }
                        else
                        {
                            SettingsWrapper.InitSettings();
                            base.Dispatcher.BeginInvoke(new Action(delegate ()
                            {
                                this.StatusText.Text = "АВТОРИЗАЦИЯ В КЛИЕНТЕ";
                                this.ButtonActivate.IsEnabled = true;
                            }), Array.Empty<object>());
                        }
                    }
                    else
                    {
                        SettingsWrapper.InitSettings();
                        base.Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            this.StatusText.Text = "АВТОРИЗАЦИЯ В КЛИЕНТЕ";
                            this.ButtonActivate.IsEnabled = true;
                        }), Array.Empty<object>());
                    }
                }
            }).Start();
            new Thread(delegate ()
            {
                try
                {
                    WebRequest.Create("https://adminxyz.ru/action.php?type=add-install").GetResponse();
                }
                catch
                {
                }
            }).Start();
        }

        private void TextBoxKey_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.TextBoxKey.Text == "ВВЕДИТЕ КОД ДОСТУПА")
            {
                this.TextBoxKey.Text = "";
            }
        }

        private void TextBoxKey_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.TextBoxKey.Text == "")
            {
                this.TextBoxKey.Text = "ВВЕДИТЕ КОД ДОСТУПА";
            }
        }

        private void ButtonActivate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            using (Stream responseStream = WebRequest.Create("http://adminxyz.ru/api/galaxyware.php?action=activate_key&key=" + this.TextBoxKey.Text).GetResponse().GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    string text = streamReader.ReadToEnd();
                    //crack
                    text = "2099-12-30";
                    //>/crack
                    if (text == "keyNotFound")
                    {
                        MessageBox.Show("Указан неверный ключ", "GalaxyWare", MessageBoxButton.OK);
                    }
                    else if (text == "keyAlreadyActivated")
                    {
                        MessageBox.Show("Данный ключ уже активирован", "GalaxyWare", MessageBoxButton.OK);
                    }
                    else if (text == "keyExpired")
                    {
                        MessageBox.Show("Срок действия данного ключа истек", "GalaxyWare", MessageBoxButton.OK);
                    }
                    else
                    {
                        string[] array = text.Split(new char[]
                        {
                            '-'
                        });
                        DateTime now = DateTime.Now;
                        TimeSpan timeSpan = new DateTime(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2])) - now;
                        MessageBox.Show("Успешная активация ключа!", "GalaxyWare", MessageBoxButton.OK);
                        SettingsWrapper.SetValue(SettingsAttribute.Key, this.TextBoxKey.Text);
                        SettingsWrapper.SetValue(SettingsAttribute.Expires, text);
                        UpdaterWindow updaterWindow = new UpdaterWindow();
                        updaterWindow.SetLicenseDays(timeSpan.Days + 1);
                        updaterWindow.Show();
                        base.Close();
                    }
                }
            }
        }

        private const bool https = true;

        private const string host = "adminxyz.ru";

        private const string handler = "galaxyware.php";
    }
}
