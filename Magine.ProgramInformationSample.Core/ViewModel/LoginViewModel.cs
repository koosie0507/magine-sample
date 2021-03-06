﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Magine.ProgramInformationSample.Core.ViewModel
{
    public class LoginViewModel
    {
        private readonly LoginErrorInfo errorInfo = new LoginErrorInfo();

        private readonly IMagineApi magineApi;

        private readonly IRouter router;

        public LoginViewModel(IMagineApi magineApi, IRouter router)
        {
            this.magineApi = magineApi;
            this.router = router;
        }

        public INotifyDataErrorInfo ErrorInfo
        {
            get
            {
                return errorInfo;
            }
        }

        private bool ValidateCredentials(NetworkCredential credentials)
        {
            errorInfo.LoginError = null;
            if (string.IsNullOrEmpty(credentials.UserName))
            {
                errorInfo.LoginError = "User name is mandatory";
                return false;
            }
            if (string.IsNullOrEmpty(credentials.Password))
            {
                errorInfo.LoginError = "Password is mandatory";
                return false;
            }
            return true;
        }

        public async Task Login(NetworkCredential credentials)
        {
            if (!ValidateCredentials(credentials)) return;

            try
            {
                await magineApi.Login(credentials.UserName, credentials.Password);
                router.GoTo<ProgramInformationViewModel>();
            }
            catch (Exception)
            {
                errorInfo.LoginError = "The user name/password you entered is invalid";
            }
        }

        private class LoginErrorInfo : INotifyDataErrorInfo
        {
            private string loginError;

            public string LoginError
            {
                set
                {
                    if (Equals(loginError, value)) return;
                    loginError = value;
                    HasErrors = !string.IsNullOrWhiteSpace(value);
                    OnErrorsChanged();
                }
            }

            public IEnumerable GetErrors(string propertyName)
            {
                if (!HasErrors)
                {
                    yield break;
                }
                yield return loginError;
            }

            public bool HasErrors { get; private set; }

            public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

            private void OnErrorsChanged([CallerMemberName] string propertyName = null)
            {
                EventHandler<DataErrorsChangedEventArgs> handler = ErrorsChanged;
                if (handler == null)
                {
                    return;
                }
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}