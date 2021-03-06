﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KontakteApp
{
    public partial class FormKontakte : Form
    {
        private string _connectionString;
        private string _caption;
        private SqlDataAdapter dataAdapter;

        public string ConnectionString { get => _connectionString; set => _connectionString = value; }
        public string Caption { get => _caption; set => _caption = value; }

        public FormKontakte()
        {
            InitializeComponent();
        }

        private void FormKontakte_Load(object sender, EventArgs e)
        {
            Config config = Config.GetInstance("Config.xml");

            ConnectionString = config.ConnectionString;
            Caption = config.Caption;

            try
            {
                SQLHelper helper = new SQLHelper(ConnectionString);
                bool connectionOK = helper.IsConnected;
            }
            catch
            {
                MessageBox.Show("Die Datenbank 'Kontakte' wurde nicht gefunden!\n" +
                    "Bitte die Datei 'KontakteApp.exe.config.deploy' anpassen.\n" +
                    "Das Programm wird jetzt beendet.");
                this.Close();
            }

            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            SqlCommand sqlCommand = new SqlCommand("select * from dbo.Personen", sqlConnection);
            dataAdapter = new SqlDataAdapter(sqlCommand);

            kontakteDataSet = new DataSet("Kontakte");
            kontakteDataSet.Tables.Add("Personen");
            dataAdapter.Fill(kontakteDataSet.Tables["Personen"]);

            kontakteBindingNavigator.BindingSource = kontakteBindingSource;
            kontakteBindingSource.DataMember = "Personen";
            BindingControls(kontakteDataSet.Tables["Personen"]);

        }

        private void FormKontake_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void showGridFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form = new FormGrid();
            form.ShowDialog();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            kontakteBindingSource.MovePrevious();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            kontakteBindingSource.MoveNext();
        }

        private void kontakteBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.kontakteBindingSource.EndEdit();

            SqlCommandBuilder builder = new SqlCommandBuilder(dataAdapter);
            dataAdapter.UpdateCommand = builder.GetUpdateCommand();
            dataAdapter.Update(kontakteDataSet, "Kontakte");

        }

        private void BindingControls(DataTable dt)
        {
            kontakteBindingSource.DataSource = dt;
            kontakt_IDTextBox.DataBindings.Add(new Binding("Text", kontakteBindingSource, "Kontakt_ID", true));
            vornameTextBox.DataBindings.Add(new Binding("Text", kontakteBindingSource, "Vorname", true));
            nachnameTextBox.DataBindings.Add(new Binding("Text", kontakteBindingSource, "Nachname", true));
            firmaTextBox.DataBindings.Add(new Binding("Text", kontakteBindingSource, "Firma", true));
            telefonTextBox.DataBindings.Add(new Binding("Text", kontakteBindingSource, "Telefon", true));
            emailTextBox.DataBindings.Add(new Binding("Text", kontakteBindingSource, "Email", true));
            anrufDateTimePicker.DataBindings.Add(new Binding("Value", kontakteBindingSource, "Anruf", true));
        }

    }
}
