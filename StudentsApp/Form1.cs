using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace StudentsApp
{
    public partial class Form1 : Form
    {
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtAge;
        private Button btnAdd;
        private Button btnLoad;
        private DataGridView dgvStudents;

        private string connectionString = "Host=46.191.235.28;Port=5432;Database=JiraCopy_Bakieva;Username=postgres;Password=Asdf=1234Asdf=1234";

        public Form1()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Список студентов";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

      
            txtFirstName = new TextBox
            {
                Name = "txtFirstName",
                Location = new System.Drawing.Point(30, 30),
                Size = new System.Drawing.Size(200, 23),
                PlaceholderText = "Имя"
            };

           
            txtLastName = new TextBox
            {
                Name = "txtLastName",
                Location = new System.Drawing.Point(30, 60),
                Size = new System.Drawing.Size(200, 23),
                PlaceholderText = "Фамилия"
            };

            
            txtAge = new TextBox
            {
                Name = "txtAge",
                Location = new System.Drawing.Point(30, 90),
                Size = new System.Drawing.Size(200, 23),
                PlaceholderText = "Возраст"
            };

            btnAdd = new Button
            {
                Name = "btnAdd",
                Text = "Добавить",
                Location = new System.Drawing.Point(250, 30),
                Size = new System.Drawing.Size(100, 30)
            };
            btnAdd.Click += BtnAdd_Click;

            btnLoad = new Button
            {
                Name = "btnLoad",
                Text = "Загрузить",
                Location = new System.Drawing.Point(250, 70),
                Size = new System.Drawing.Size(100, 30)
            };
            btnLoad.Click += BtnLoad_Click;

            dgvStudents = new DataGridView
            {
                Name = "dgvStudents",
                Location = new System.Drawing.Point(30, 140),
                Size = new System.Drawing.Size(730, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            this.Controls.Add(txtFirstName);
            this.Controls.Add(txtLastName);
            this.Controls.Add(txtAge);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnLoad);
            this.Controls.Add(dgvStudents);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAge.Text, out int age))
            {
                MessageBox.Show("Возраст должен быть числом", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Заполните имя и фамилию", "ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO \"Students\" (\"FirstName\", \"LastName\", \"Age\") VALUES (@firstName, @lastName, @age)";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", firstName);
                        cmd.Parameters.AddWithValue("@lastName", lastName);
                        cmd.Parameters.AddWithValue("@age", age);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Студент добавлен", "молодчи", MessageBoxButtons.OK, MessageBoxIcon.Information);


                txtFirstName.Clear();
                txtLastName.Clear();
                txtAge.Clear();

                BtnLoad_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении {ex.Message}", "косяк", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT \"Id\", \"FirstName\" AS \"Имя\", \"LastName\" AS \"Фамилия\", \"Age\" AS \"Возраст\" FROM \"Students\" ORDER BY \"Id\"";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dgvStudents.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке {ex.Message}", "косяк тут", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}