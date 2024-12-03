using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceProcess;
using MimeKit;

namespace WindowsService3
{
    public partial class ServiceDbUpdateChecker : ServiceBase
    {
        const string email_login = "goldensunrisee@gmail.com";
        //const string email_password = "oril aeud fgwf zpht";
        //const string email_password = "gkda kqbc dfpn ydjs";
        //const string email_password = "sdoq vnlx soui bqdh";
        const string email_password = "srtm fejm jrux yjqu";
        static string _connectionString = "Server=localhost\\MSSQLSERVER01;Database=asp_lr12;Trusted_Connection=True;Encrypt=false";

        public static void sendMessage(string msg)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Sender: ", email_login));
            email.To.Add(new MailboxAddress("Receiver", email_login));

            email.Subject = "An update in database";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"<b>{msg}</b>"
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 465, true);

                // Note: only needed if the SMTP server requires authentication
                smtp.Authenticate(email_login, email_password);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }

        // Метод для обробки змін
        public static void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            string msg = $"Notification info: {e.Info}, Source: {e.Source}, Type: {e.Type}";
            Console.WriteLine(msg);
            sendMessage(msg);

            if (e.Info == SqlNotificationInfo.Insert || e.Info == SqlNotificationInfo.Update || e.Info == SqlNotificationInfo.Delete)
            {
                // Повторна реєстрація
                RegisterSqlDependency();
            }
        }

        public static void RegisterSqlDependency()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_connectionString))
                {
                    if (cn.State == ConnectionState.Closed)
                        cn.Open();

                    SqlCommand cmd = new SqlCommand(
                        "SELECT Id, Name, Address, year_of_establishment, Industry FROM asp_lr12.dbo.company",
                        cn
                    );

                    cmd.Notification = null;

                    SqlDependency sqlDependency = new SqlDependency(cmd);
                    sqlDependency.OnChange += new OnChangeEventHandler(OnDependencyChange);

                    // Активація повідомлення
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // обробка даних після виконання запиту
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public ServiceDbUpdateChecker()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Ініціалізація SqlDependency
            SqlDependency.Start(_connectionString);
            // Реєстрація SqlDependency
            RegisterSqlDependency();
        }

        protected override void OnStop()
        {
            // Зупинка SqlDependency
            SqlDependency.Stop(_connectionString);
        }
    }
}