﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using taste_it.Additionals.NavigationService;
using taste_it.DataService;
using taste_it.Models;

namespace taste_it.ViewModels
{
    public class SignInViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        #region fields

        //private User currentUser;
        private string userName;
        private string userPassword;
        private readonly IUserDataService _userDataService;
        private IFrameNavigationService _navigationService;

        #endregion

        #region properties
        public User CurrentUser {get;set;}
        public ObservableCollection<User> UserCollection { get; set; }
        public ICommand SignInCommand { get; private set; }
        public ICommand NavigateToSignUpCommand { get; private set; }
        public ICommand LoadUsersCommand { get; private set; }
        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                
                Set(ref userName, value);
            }
        }
        public string UserPassword
        {
            get
            {
                return userPassword;
            }

            set
            {

                Set(ref userPassword, value);
            }
        }

        #endregion 

        
        
        public SignInViewModel(IUserDataService userData, IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _userDataService = userData;
            UserCollection = new ObservableCollection<User>();
           // LoadUsers();  //should load users after every change of view!

            SignInCommand = new RelayCommand(SignIn);
            NavigateToSignUpCommand = new RelayCommand(NavigateToSignUp);
            LoadUsersCommand = new RelayCommand(LoadUsers);

        }


        #region methods
        private async void LoadUsers()
        {
            var users = await _userDataService.GetUsersAsync();
            UserCollection.Clear();
            foreach (var item in users)
           {
                UserCollection.Add(item);
            }
            RaisePropertyChanged(() => UserCollection);
        }

        public void SignIn()   // Method execute after button clicked
        {

           CheckCredentials();
            //if (CheckCredentials()) NavigateTo...(); // after logged in display anot her view
        }

        public void NavigateToSignUp()
        {
            //Messenger with collection of users?
            UserName = string.Empty;
            UserPassword = string.Empty;
            HasErrors = false;
            _navigationService.NavigateTo("SignUp"); 
        }
        private bool CheckCredentials()
        {
            HasErrors = !IsVerified();
            if (HasErrors)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("UserName"));
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs("UserPassword"));
            }
            else
            {
                return true;
            }
            return false;
        }

        private bool IsVerified()   
        {
           CurrentUser = UserCollection.FirstOrDefault(u => u.name == UserName);
            
            if (CurrentUser != null)
            {
                return CheckPassword(UserPassword, CurrentUser.password);
            }
            else
            {
                Debug.WriteLine("User does not exist!");
                return false;
            }
        }

        private bool CheckPassword(string inputPassword, string savedPasswordHash)
        {

            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                {
                    //throw new UnauthorizedAccessException();
                    Debug.WriteLine("wrong password");
                    return false;
                }
            Debug.WriteLine("correct password, logged in");
            return true;

        }
        #endregion INotifyDataErrorInfo Members

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || string.IsNullOrWhiteSpace(propertyName) || (!HasErrors))
                return null;
            return new List<string>() { "Invalid credentials" };
        }
        public bool HasErrors { get; set; } = false;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


    }
}

