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
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void buttonSignUp_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
               
                // check if username is available
                using (var sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Narendra\source\repos\Login\Login\Database1.mdf;Integrated Security=True"))
                {
                    string query = "Select * from [Users] Where Username= '" + textBoxUserName.Text.Trim() + "'";
                    
                    SqlDataAdapter sda = new SqlDataAdapter(query, sqlCon);

                    DataTable dtbl = new DataTable();
                   
                    sda.Fill(dtbl);
                   
                    if (dtbl.Rows.Count > 0)
                    {
                        MessageBox.Show("Username is taken!");
                    }
                  
                    else
                    {
                        try
                        {
                            var password = textBoxPassword.Text.Trim();

                            byte[] salt;

                            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

                            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

                            byte[] hash = pbkdf2.GetBytes(20);
                            byte[] hashBytes = new byte[36];

                            Array.Copy(salt, 0, hashBytes, 0, 16);
                            Array.Copy(hash, 0, hashBytes, 16, 20);

                            string savedPasswordHash = Convert.ToBase64String(hashBytes);

                            string commString = "INSERT INTO [Users](Username,FullName, Password) VALUES (@val1, @val2,@val3)";


                            using (SqlCommand comm = new SqlCommand())
                            {
                                comm.Connection = sqlCon;
                                comm.CommandText = commString;

                                comm.Parameters.AddWithValue("@val1", textBoxUserName.Text);
                                comm.Parameters.AddWithValue("@val2", textBoxFullName.Text);
                                comm.Parameters.AddWithValue("@val3", savedPasswordHash);
                                sqlCon.Open();
                                //execute the query we just wrote
                                comm.ExecuteNonQuery();
                                sqlCon.Close();
                            }


                            MessageBox.Show("Successfully signed up");

                            LoginForm loginForm = new LoginForm();

                            this.Hide();
                            loginForm.Show();

                        }
                        catch { MessageBox.Show("A registration error has occured. Please contact the developer :("); }

                    }

                }


               
            }
        }

        private bool IsValid()
        {
            if (textBoxUserName.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Username should not be empty");
                return false;
            }

            if (textBoxPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Password should not be empty");
                return false;
            }

            if (textBoxConfirmPassword.Text.Trim() == string.Empty)
            {
                MessageBox.Show(" Confirm Password should not be empty");
                return false;
            }

            if (textBoxPassword.Text.Trim() != textBoxConfirmPassword.Text.Trim())
            {
                MessageBox.Show("Password and Confirm Password should match");
                return false;
            }



            return true;

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();

            this.Hide();
            loginForm.Show();
        }




    }
}
