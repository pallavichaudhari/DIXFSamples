using System;
using System.Windows.Forms;

namespace RecurringIntegrationApp
{
    public partial class DIXFRecurringJobsProcessor : Form
    {
        private readonly object stateLock = new object();

        private FileProcessor fileProcessor;
           
        public DIXFRecurringJobsProcessor()
        {
            InitializeComponent();

            this.InitializeSettings();
        }

        private void InitializeSettings()
        {
            this.inputLocationTextBox.Text = Settings.InputDir;  

            this.inProcessLocationTextBox.Text = Settings.InProcessDir;

            this.successLocationTextBox.Text = Settings.SuccessDir;

            this.failedLocationTextBox.Text = Settings.ErrorDir;

            this.statusPollingIntervalTextBox.Text = Settings.StatusPollingInterval.ToString();
            this.statusPollingIntervalTextBox.TextChanged += statusPollingIntervalTextBox_TextChanged;

            this.aadTenantTextBox.Text = Settings.AadTenant;
            this.aadTenantTextBox.TextChanged += aadTenantTextBox_TextChanged;

            this.azureAuthEndpointTextBox.Text = Settings.AzureAuthEndpoint;
            this.azureAuthEndpointTextBox.TextChanged += azureAuthEndpointTextBox_TextChanged;

            this.ax7EndpointTextBox.Text = Settings.RainierUri;
            this.ax7EndpointTextBox.TextChanged += ax7EndpointTextBox_TextChanged;

            this.ax7UserNameTextBox.Text = Settings.RainierUserName;
            this.ax7UserNameTextBox.TextChanged += ax7UserNameTextBox_TextChanged;

            this.ax7UserpasswordTextBox.Text = Settings.RainierUserPassword;
            this.ax7UserpasswordTextBox.TextChanged += ax7UserpasswordTextBox_TextChanged;

            this.clientAppIdtextBox.Text = Settings.ClientId;
            this.clientAppIdtextBox.TextChanged += clientAppIdtextBox_TextChanged;

            this.recurringJobQueueIdTextBox.Text = Settings.RecurringJobId.ToString();
            this.recurringJobQueueIdTextBox.TextChanged += RecurringJobQueueIdTextBox_TextChanged;

            this.entityNameTextBox.Text = Settings.EntityName;
            this.entityNameTextBox.TextChanged += entityNameTextBox_TextChanged;

            this.companyTextBox.Text = Settings.Company;
            this.companyTextBox.TextChanged += companyTextBox_TextChanged;

            this.isDataPackageInputCheckBox.Checked = Settings.IsDataPackage;

            this.stopButton.Enabled = false;
            
        }

        private void azureAuthEndpointTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.AzureAuthEndpoint = azureAuthEndpointTextBox.Text;
            
        }

        private void ax7EndpointTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.RainierUri = ax7EndpointTextBox.Text;
            
        }

        private void ax7UserNameTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.RainierUserName = ax7UserNameTextBox.Text;
            
        }

        private void ax7UserpasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.RainierUserPassword = ax7UserpasswordTextBox.Text;
            
        }

        private void clientAppIdtextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.ClientId = clientAppIdtextBox.Text;
            
        }

        private void entityNameTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.EntityName = entityNameTextBox.Text;
            
        }

        private void companyTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.Company = companyTextBox.Text;
            
        }

        private void aadTenantTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.AadTenant = aadTenantTextBox.Text;
            
        }

        private void statusPollingIntervalTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.StatusPollingInterval = Convert.ToInt32(this.statusPollingIntervalTextBox.Text);
            
        }

        private void RecurringJobQueueIdTextBox_TextChanged(object sender, EventArgs e)
        {
            Guid _guid = Guid.Empty;
            if(Guid.TryParse(this.recurringJobQueueIdTextBox.Text, out _guid))
            {
                Settings.RecurringJobId = _guid;
                
            }
        }

        private void clearLogButton_Click(object sender, EventArgs e)
        {            
            logTextBox.Clear();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            this.fileProcessor = new FileProcessor(this);
            this.fileProcessor.Start();
            this.startButton.Enabled = false;
            this.stopButton.Enabled = true;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            this.fileProcessor.Stop();
            this.stopButton.Enabled = false;
            this.startButton.Enabled = true;
            
        }

        public void logText(string message)
        {
            if (this.InvokeRequired)
            {
                this.logTextBox.Invoke(new Action<string>(logText), new object[] { message });
                return;
            }
            this.logTextBox.Text += "\r\n" + message;            
        }

        public void updateStats(StatType statType, int count)
        {
            int tmpCount = 0;

            switch (statType)
            {
                case StatType.Input:
                    if (this.InvokeRequired)
                    {
                        this.submittedJobsLabel.Invoke(new Action<StatType, int>(updateStats), new object[] { statType, count });
                    }                    
                    lock (stateLock)
                    {
                        tmpCount = count;
                    }
                    this.submittedJobsLabel.Text = tmpCount.ToString();
                    break;

                case StatType.Inprocess:
                    if (this.InvokeRequired)
                    {
                        this.inProcessJobsLabel.Invoke(new Action<StatType, int>(updateStats), new object[] { statType, count });
                    }
                    lock (stateLock)
                    {
                        tmpCount = count;
                    }
                    this.inProcessJobsLabel.Text = tmpCount.ToString();
                    break;

                case StatType.Success:
                    if (this.InvokeRequired)
                    {
                        this.successJobsLabel.Invoke(new Action<StatType, int>(updateStats), new object[] { statType, count });
                    }
                    lock (stateLock)
                    {
                        tmpCount = count;
                    }
                    this.successJobsLabel.Text = tmpCount.ToString();
                    break;

                case StatType.Failure:
                    if (this.InvokeRequired)
                    {
                        this.failedJobsLabel.Invoke(new Action<StatType, int>(updateStats), new object[] { statType, count });
                    }
                    lock (stateLock)
                    {
                        tmpCount = count;
                    }
                    this.failedJobsLabel.Text = count.ToString();
                    break;
            }
        }

        private void DIXFRecurringJobsProcessor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

              
        private void successLocationBtn_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                successLocationTextBox.Text = folderBrowserDialog.SelectedPath;
                Settings.SuccessDir = successLocationTextBox.Text;
                
            }
        }

        private void inProcessLocationBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                inProcessLocationTextBox.Text = folderBrowserDialog.SelectedPath;
                Settings.InProcessDir = inProcessLocationTextBox.Text;
                
            }
        }

        private void failedLocationBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                failedLocationTextBox.Text = folderBrowserDialog.SelectedPath;
                Settings.ErrorDir = failedLocationTextBox.Text;
                
            }
        }

        private void isDataPackageInputCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.IsDataPackage = isDataPackageInputCheckBox.Checked;
        }

        private void inputLocBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                inputLocationTextBox.Text = folderBrowserDialog.SelectedPath;
                Settings.InputDir = this.inputLocationTextBox.Text;                
            }
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            SettingManager.WriteSetting("Input Directory", this.inputLocationTextBox.Text);
            SettingManager.WriteSetting("InProcess Directory", inProcessLocationTextBox.Text);
            SettingManager.WriteSetting("Error Directory", failedLocationTextBox.Text);
            SettingManager.WriteSetting("Success Directory", successLocationTextBox.Text);
            SettingManager.WriteSetting("Status Polling Interval", statusPollingIntervalTextBox.Text);

            SettingManager.WriteSetting("Aad Tenant", aadTenantTextBox.Text);
            SettingManager.WriteSetting("Azure Auth Endpoint", azureAuthEndpointTextBox.Text);
            SettingManager.WriteSetting("Azure Client Id", clientAppIdtextBox.Text);
            SettingManager.WriteSetting("Rainier Uri", ax7EndpointTextBox.Text);
            SettingManager.WriteSetting("User", ax7UserNameTextBox.Text);
            SettingManager.WriteSetting("Password", ax7UserpasswordTextBox.Text);

            SettingManager.WriteSetting("Recurring Job Id", this.recurringJobQueueIdTextBox.Text);
            SettingManager.WriteSetting("Entity Name", entityNameTextBox.Text);
            SettingManager.WriteSetting("Is Data Package", Convert.ToString(isDataPackageInputCheckBox.Checked));
            SettingManager.WriteSetting("Company", companyTextBox.Text);
            
            
            
        }
    }

    public enum StatType
    {
        Input,

        Inprocess,

        Success,

        Failure
    }

}
