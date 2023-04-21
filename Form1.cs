using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Wypozyczalnia_kamperow
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Data Source=.;Initial Catalog=WypozyczalniaKamperow;Integrated Security=True;Connect Timeout=30";
        private SqlConnection connection;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Wyświetlenie okna dialogowego z pytaniem o nazwę bazy danych
            string databaseName = "";
            DialogResult result = MessageBox.Show("Proszę podać nazwę bazy danych", "Wybierz bazę danych", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                // Stworzenie własnego okna dialogowego do wprowadzenia nazwy bazy danych
                using (Form inputBox = new Form())
                {
                    inputBox.Size = new Size(400, 150);
                    inputBox.Text = "Wybierz bazę danych";
                    inputBox.StartPosition = FormStartPosition.CenterParent;

                    Label label = new Label();
                    label.Text = "Proszę podać nazwę bazy danych";
                    label.Location = new Point(10, 10);
                    inputBox.Controls.Add(label);

                    TextBox textBox = new TextBox();
                    textBox.Location = new Point(10, 40);
                    textBox.Size = new Size(250, 10);
                    inputBox.Controls.Add(textBox);

                    Button okButton = new Button();
                    okButton.Text = "OK";
                    okButton.DialogResult = DialogResult.OK;
                    okButton.Location = new Point(10, 80);
                    inputBox.Controls.Add(okButton);

                    Button cancelButton = new Button();
                    cancelButton.Text = "Anuluj";
                    cancelButton.DialogResult = DialogResult.Cancel;
                    cancelButton.Location = new Point(90, 80);
                    inputBox.Controls.Add(cancelButton);

                    inputBox.AcceptButton = okButton;
                    inputBox.CancelButton = cancelButton;

                    DialogResult inputResult = inputBox.ShowDialog();
                    if (inputResult == DialogResult.OK)
                    {
                        databaseName = textBox.Text;
                    }
                }
            }

            if (!string.IsNullOrEmpty(databaseName))
            {
                // Utworzenie pełnego connection string z podaną nazwą bazy danych
                string connectionString = $@"Data Source=.;Initial Catalog={databaseName};Integrated Security=True;Connect Timeout=30";

                // Inicjalizacja połączenia z bazą danych
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        MessageBox.Show("Połączenie z bazą danych jest dostępne");

                        // Wczytanie nazw tabel do ComboBoxa
                        DataTable tables = connection.GetSchema("Tables");
                        foreach (DataRow row in tables.Rows)
                        {
                            string tableName = row["TABLE_NAME"].ToString();
                            comboBox1.Items.Add(tableName);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd połączenia z bazą danych: " + ex.Message);
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Sprawdzenie, czy wybrano jakąś tabelę
            if (comboBox1.SelectedItem == null)
                return;

            string selectedTableName = comboBox1.SelectedItem.ToString(); // Pobranie nazwy wybranej tabeli z ComboBoxa

            // Inicjalizacja połączenia z bazą danych
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();

                // Utworzenie zapytania SQL
                string query = $"SELECT * FROM {selectedTableName}";

                // Utworzenie obiektu SqlDataAdapter i DataTable
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();

                    // Wypełnienie DataTable danymi z bazy danych
                    adapter.Fill(dataTable);

                    // Ustawienie DataSource DataGridView na obiekt DataTable
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd odczytu z bazy danych: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void usunButton_Click(object sender, EventArgs e)
        {
            // Sprawdzenie, czy wybrano jakiekolwiek rekordy w DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Inicjalizacja połączenia z bazą danych
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        // Iterowanie przez zaznaczone wiersze
                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            // Pobranie wartości zaznaczonego rekordu w pierwszej kolumnie (lub innej, jeśli nie jest to ID)
                            string selectedValue = row.Cells[0].Value.ToString();

                            // Utworzenie zapytania SQL do usunięcia rekordu na podstawie wartości zaznaczonej kolumny
                            string query = $"DELETE FROM {comboBox1.SelectedItem} WHERE {dataGridView1.Columns[0].HeaderText} = @selectedValue";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@selectedValue", selectedValue);

                            // Wykonanie zapytania
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Rekord o wartości {selectedValue} został usunięty z bazy danych.");
                            }
                            else
                            {
                                MessageBox.Show($"Nie udało się usunąć rekordu o wartości {selectedValue} z bazy danych.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Błąd podczas usuwania rekordu z bazy danych: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie wybrano rekordów do usunięcia.");
            }
        }


        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Sprawdzenie, czy wybrano jakąś tabelę
            if (comboBox1.SelectedItem == null)
                return;

            string selectedTableName = comboBox1.SelectedItem.ToString(); // Pobranie nazwy wybranej tabeli z ComboBoxa

            // Inicjalizacja połączenia z bazą danych
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();

                // Utworzenie zapytania SQL
                string query = $"SELECT * FROM {selectedTableName}";

                // Utworzenie obiektu SqlDataAdapter i DataTable
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();

                    // Wypełnienie DataTable danymi z bazy danych
                    adapter.Fill(dataTable);

                    // Ustawienie DataSource DataGridView na obiekt DataTable
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd odczytu z bazy danych: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // Sprawdzenie, czy wybrano jakąś tabelę
            if (comboBox1.SelectedItem == null)
                return;

            string selectedTableName = comboBox1.SelectedItem.ToString(); // Pobranie nazwy wybranej tabeli z ComboBoxa

            try
            {
                // Pobieranie danych z DataGridView
                DataTable dt = (DataTable)dataGridView1.DataSource;

                // Tworzenie adaptera danych
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {selectedTableName}", connection);

                // Tworzenie komendy SQL do aktualizacji bazy danych
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                // Aktualizacja bazy danych
                adapter.Update(dt);

                MessageBox.Show("Dane zostały zapisane do bazy danych.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas zapisywania danych do bazy danych: " + ex.Message);
            }
        }
    }
}