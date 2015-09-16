namespace Dos.Common
{
    using System;

    public class EmailAccount
    {
        private string account;
        private string displayName;
        private bool enableSsl;
        private string host;
        private bool isUnnormal;
        private string password;
        private int port;

        public EmailAccount()
        {
            this.port = 0x19;
        }

        public EmailAccount(string account, string password, string host)
        {
            this.port = 0x19;
            this.host = host;
            this.password = password;
            this.account = account;
        }

        public EmailAccount(string account, string displayName, string password, string host)
        {
            this.port = 0x19;
            this.host = host;
            this.password = password;
            this.account = account;
            this.displayName = displayName;
        }

        public EmailAccount(string account, string displayName, string password, string host, int port, bool enableSsl, bool isUnnormal)
        {
            this.port = 0x19;
            this.host = host;
            this.password = password;
            this.account = account;
            this.displayName = displayName;
            this.port = port;
            this.enableSsl = enableSsl;
            this.isUnnormal = isUnnormal;
        }

        public string Account
        {
            get
            {
                return this.account;
            }
            set
            {
                this.account = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        public bool EnableSsl
        {
            get
            {
                return this.enableSsl;
            }
            set
            {
                this.enableSsl = value;
            }
        }

        public string Host
        {
            get
            {
                return this.host;
            }
            set
            {
                this.host = value;
            }
        }

        public bool IsUnnormal
        {
            get
            {
                return this.isUnnormal;
            }
            set
            {
                this.isUnnormal = value;
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port = value;
            }
        }
    }
}

