using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Login
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {

            if (IsValid())
            {
                //Get Salt and SavedHashPassword from database

                var password=textBoxPassword.Text.Trim();


                using (var sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Narendra\source\repos\Login\Login\Database1.mdf;Integrated Security=True"))
                {
                    string query = "Select * from [Users] Where Username= '" + textBoxUserName.Text.Trim() + "'";
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);

                    DataTable dtbl = new DataTable();
                    //filling it with matching info
                    sda.Fill(dtbl);
                    //now here's what's different: if there is actually a match, we deny the user the registration
                    if (dtbl.Rows.Count ==1)
                    {

                        var storedPassword = dtbl.Rows[0][1].ToString();


                        //validate password
                        if (!ValidatePassword(password, storedPassword))
                        {
                            MessageBox.Show("Please enter correct password");
                        }
                        else
                        {
                            this.Hide();
                            Form1 form=new Form1();
                            form.Show();
                        }

                    }
                }

            }

        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool IsValid()
        {
            if(textBoxUserName.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Username should not be empty");
                return false;
            }

            if (textBoxPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Password should not be empty");
                return false;
            }

            return true;
        }

        private void buttonSignUp_Click(object sender, EventArgs e)
        {
            SignUp signUp = new SignUp();

            this.Hide();

            signUp.Show();
        }

        private bool ValidatePassword(string password, string storedPassword)
        {
            byte[] hashBytesSaved = Convert.FromBase64String(storedPassword);

            byte[] salt = new byte[16];
            
            Array.Copy(hashBytesSaved, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytesSaved[i + 16] != hash[i])
                {
                    return false;
                }
            }



            return true;
        }
    }
}
