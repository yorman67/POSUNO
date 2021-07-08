﻿using POSUNO.Components;
using POSUNO.Helpers;
using POSUNO.Models;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace POSUNO.Pages
{

    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = await ValidForm();
            if (!isValid)
            {
                return;
            }

            Loader loader = new Loader("Por favor espere...");
            loader.Show();

            Response response = await ApiService.LoginAsync(new LoginRequest
            {
                Email = EmailTextBox.Text,
                Password = PasswordPasswordBox.Password
            });

            loader.Close();

            MessageDialog messageDialog;
            if (!response.IsSuccess)
            {
                messageDialog = new MessageDialog(response.Message, "Error");
                await messageDialog.ShowAsync();
                return;
            }

            User user = (User)response.Result;
            if (user == null)
            {
                messageDialog = new MessageDialog("Usuario o clave incorrecta", "Error");
                await messageDialog.ShowAsync();
                return;
            }

            Frame.Navigate(typeof(MainPage), user);


        }

        private async Task<bool> ValidForm()
        {
            MessageDialog messageDialog;

            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                messageDialog = new MessageDialog("Debes ingresar tu correo", "ERROR");
                await messageDialog.ShowAsync();
                return false;
            }

            if (!RegexUtilities.IsValidEmail(EmailTextBox.Text))
            {
                messageDialog = new MessageDialog("Debes ingresar un correo valido", "ERROR");
                await messageDialog.ShowAsync();
                return false;
            }


            if (PasswordPasswordBox.Password.Length < 6)
            {
                messageDialog = new MessageDialog("Debes ingresar una clave de almenos 6 caracteres", "ERROR");
                await messageDialog.ShowAsync();
                return false;
            }

            return true;
        }
    }
}
