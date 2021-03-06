﻿using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using UniCadeAndroid.Backend;
using UniCadeAndroid.Constants;
using UniCadeAndroid.Security;

namespace UniCadeAndroid.Activities
{
    [Activity(Label = "UniCade Mobile Settings", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SettingsActivity : Activity
    {
        #region Private Instance Variables

        private Button _loadDatabaseButton;

        private Button _loadBackupButton;

        private Button _saveDatabaseButton;

        private Button _backupDatabaseButton;

        private CheckBox _showSplashScreenCheckbox;

        private CheckBox _passwordProtectSettingsCheckBox;

        private CheckBox _enableFingerprintProtectionCheckbox;

        private CheckBox _displayModernEsrbIconsCheckBox;

        private Button _deleteAllLocalImagesButton;

        private Button _unicadeCloudButton;

        private Button _webScraperSettingsButton;

        private Button _enterLicenseButton;

        private Button _applyButton;

        private Button _closeSettingsButton;

        private TextView _licenseStatusTextView;

        private string _licenseName;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set the view
            SetContentView(Resource.Layout.SettingsView);

            FindElementsById();

            LinkClickHandlers();

            PopulateSettings();
        }

		private void FindElementsById()
		{
            _loadDatabaseButton = FindViewById<Button>(Resource.Id.LoadDatabaseButton);
            _loadBackupButton = FindViewById<Button>(Resource.Id.LoadBackupButton);
            _saveDatabaseButton = FindViewById<Button>(Resource.Id.SaveDatabaseButton);
            _backupDatabaseButton = FindViewById<Button>(Resource.Id.BackupDatabaseButton);
            _showSplashScreenCheckbox = FindViewById<CheckBox>(Resource.Id.ShowSplashScreenCheckbox);
            _passwordProtectSettingsCheckBox = FindViewById<CheckBox>(Resource.Id.PasswordProtectSettingsCheckbox);
            _enableFingerprintProtectionCheckbox = FindViewById<CheckBox>(Resource.Id.EnableFingerprintSecurityCheckbox);
            _displayModernEsrbIconsCheckBox = FindViewById<CheckBox>(Resource.Id.DisplayModernESRBIconsCheckbox);
            _deleteAllLocalImagesButton = FindViewById<Button>(Resource.Id.DeleteAllLocalImagesButton);
            _unicadeCloudButton = FindViewById<Button>(Resource.Id.UniCadeCloudButton);
            _webScraperSettingsButton = FindViewById<Button>(Resource.Id.WebScraperSettingsButton);
            _enterLicenseButton = FindViewById<Button>(Resource.Id.EnterLicenseKeyButton);
            _applyButton = FindViewById<Button>(Resource.Id.ApplyButton);
            _closeSettingsButton = FindViewById<Button>(Resource.Id.CloseButton);
		    _licenseStatusTextView = FindViewById<TextView>(Resource.Id.LicenseStatusTextView);
        }

        private void PopulateSettings(){
            _showSplashScreenCheckbox.Checked = Preferences.ShowSplashScreen;
            _passwordProtectSettingsCheckBox.Checked = (Preferences.PasswordProtection != null);
             _enableFingerprintProtectionCheckbox.Checked = Preferences.FingerprintProtectionEnabled;
            _displayModernEsrbIconsCheckBox.Checked = Preferences.UseModernEsrbLogos;
        }

		protected void ShowInputDialog(string title, Action<string> handlerFunction)
		{

            EditText editText = new EditText(this);
			AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
			dialogBuilder.SetTitle(title);
			dialogBuilder.SetPositiveButton("Enter", (senderAlert, args) =>
			{
                handlerFunction(editText.Text);
			});

			dialogBuilder.SetNegativeButton("Cancel", (senderAlert, args) =>
			{
                
			});
			dialogBuilder.SetView(editText); 
			dialogBuilder.Show();
		}

        private void HandleLicenseName(string text){
            _licenseName = text;
            ShowInputDialog("Please Enter License Key", HandleLicenseKey);
        }

		private void HandleLicenseKey(string text)
		{
            if(CryptoEngine.ValidateLicense(_licenseName, text)){
                Toast.MakeText(this, "License is valid", ToastLength.Short).Show();

            }
            else{
                Toast.MakeText(this, "License is invalid", ToastLength.Short).Show();
            }
		    UpdateLicenseStatusText();
		}

        public void UpdateLicenseStatusText()
        {
            if (Preferences.IsLicenseValid)
            {
                _licenseStatusTextView.Text = "UniCade License Status: Valid";
            }
            else
            {
                _licenseStatusTextView.Text = "UniCade License Status: Invalid";
            }
        }

		private void HandleSetPassword(string text)
		{
            if (Utilties.CheckForInvalidChars(text))
            {
                Toast.MakeText(this, "Password contains invalid chars", ToastLength.Short).Show();
                _passwordProtectSettingsCheckBox.Checked = false;
                return;
            }
            if(text.Length < ConstValues.MinUserPasswordLength){
                Toast.MakeText(this, $"Password must be at least {ConstValues.MinUserPasswordLength} chars", ToastLength.Short).Show();
                _passwordProtectSettingsCheckBox.Checked = false;
				return;
            }
            if (text.Length > ConstValues.MaxUserPasswordLength)
			{
				Toast.MakeText(this, $"Password must be less than {ConstValues.MinUserPasswordLength} chars", ToastLength.Short).Show();
                _passwordProtectSettingsCheckBox.Checked = false;
				return;
			}
            _passwordProtectSettingsCheckBox.Checked = true;
            Preferences.PasswordProtection = text;
		}

		private void LinkClickHandlers()
		{
		    _loadDatabaseButton.Click += (sender, e) =>
		    {
		        FileOps.LoadDatabase();
		    };

		    _loadBackupButton.Click += (sender, e) =>
		    {
		        FileOps.LoadDatabase(ConstPaths.DatabaseFileBackupPath);
            };

		    _saveDatabaseButton.Click += (sender, e) =>
		    {
		        FileOps.SaveDatabase();
		    };

		    _backupDatabaseButton.Click += (sender, e) =>
		    {
		        FileOps.SaveDatabase(ConstPaths.DatabaseFileBackupPath);
		    };

		    _deleteAllLocalImagesButton.Click += (sender, e) =>
		    {
                FileOps.DeleteAllLocalMedia();
		    };

		    _unicadeCloudButton.Click += (sender, e) =>
		    {
		        var intent = new Intent(this, typeof(LoginActivity));
		        StartActivity(intent);
		    };

            _webScraperSettingsButton.Click += (sender, e) =>
			{
                var intent = new Intent(this, typeof(ScraperSettingsActivity));
				StartActivity(intent);
			};

            _passwordProtectSettingsCheckBox.Click += (sender, e) =>
			{
                if(_passwordProtectSettingsCheckBox.Checked){
                   ShowInputDialog("Please enter a new password", HandleSetPassword);
                }
                else{
					Preferences.PasswordProtection = null;
					Toast.MakeText(this, "Password cleared", ToastLength.Short).Show();
                }
			};

            _enableFingerprintProtectionCheckbox.Click += (sender, e) =>
            {
                if (_enableFingerprintProtectionCheckbox.Checked)
                {
                    Preferences.PasswordProtection = null;
                    Preferences.FingerprintProtectionEnabled = true;
                    Toast.MakeText(this, "Fingerprint Authentication Enabled", ToastLength.Short).Show();
                }
                else{
                    Preferences.FingerprintProtectionEnabled = false;
                    Toast.MakeText(this, "Fingerprint Authentication Disabled", ToastLength.Short).Show();
                }
            };

		    _enterLicenseButton.Click += (sender, e) =>
		    {
                ShowInputDialog("Enter License Key", HandleLicenseName);
		    };

            _closeSettingsButton.Click += (sender, e) =>
		    {
		        Finish();
		    };

		    _applyButton.Click += (sender, e) =>
		    {
                Preferences.UseModernEsrbLogos = _displayModernEsrbIconsCheckBox.Checked;
		    };
        }

	}
}
