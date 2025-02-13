using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace ToDo
{
    public partial class AddTarefa : Form
    {
        Main m = new Main();

        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=ToDo;Trusted_Connection=True;";
        private void AddTarefa_Load(object sender, EventArgs e)
        {

        }
        public AddTarefa()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void btCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btSave_Click(object sender, EventArgs e)
        {

            if (textBox1.Text == "")
            {
                MessageBox.Show("A descrição não pode ser nula!");
            }

            else
            {
                SalvaTarefa();
            }
        }

        public void SalvaTarefa()
        {
            string descricao = textBox1.Text.Trim(); // Remove espaços extras
            DateTime dataInclusao = DateTime.Now; // Data atual
            int id = GerarNovoID(); // Método para gerar o próximo ID
            bool status = false; // Status inicial
            DateTime dataPrevista;

            // Converte o texto do DateTimePicker para DateTime
            if (!DateTime.TryParse(dateTimePicker1.Text, out dataPrevista))
            {
                MessageBox.Show("Data prevista inválida. Por favor, selecione uma data válida.");
                return;
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                INSERT INTO TAREFAS (ID, DESCRICAO, STATUS_TAREFA, DATAINCLUSAO, DATAPREVISTA) 
                VALUES (@ID, @DESCRICAO, @STATUS_TAREFA, @DATAINCLUSAO, @DATAPREVISTA)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@DESCRICAO", descricao);
                        cmd.Parameters.AddWithValue("@STATUS_TAREFA", status);
                        cmd.Parameters.AddWithValue("@DATAINCLUSAO", dataInclusao); // Corrigido o nome do parâmetro
                        cmd.Parameters.AddWithValue("@DATAPREVISTA", dataPrevista);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Tarefa salva com sucesso!");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro SQL:\n" + ex.Message);
                }
            }
        }


        private int GerarNovoID()
        {
            int novoId = 1;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MAX(ID) FROM TAREFAS";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();

                        if (result != DBNull.Value)
                        {
                            novoId = Convert.ToInt32(result) + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao gerar ID" + ex.Message);
                }

            }

            return novoId;  
        }
    }


}
