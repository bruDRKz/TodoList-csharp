using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToDo
{
    public partial class TarefaEdit : Form
    { 
        Main m = new Main();

        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=ToDo;Trusted_Connection=True;";
        public TarefaEdit()
        {
            InitializeComponent();
        }

        private void TarefaEdit_Load(object sender, EventArgs e)
        {
            

        }
        private void btSave_Click(object sender, EventArgs e)
        {
            SalvaTarefa();
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       

        public void SalvaTarefa() //Parei o raciocinio aqui
        {

            
          
            string descricao = textBox1.Text.Trim(); // Remove espaços extras
            DateTime dataInclusao = DateTime.Now; // Data atual
            int id = Convert.ToInt32(labelId.Text);
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
                    UPDATE TAREFAS 
                    SET DESCRICAO = @DESCRICAO,
                    STATUS_TAREFA = @STATUS_TAREFA,
                    DATAPREVISTA = @DATAPREVISTA 
                    WHERE ID = @ID";
                   

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@DESCRICAO", descricao);
                        cmd.Parameters.AddWithValue("@STATUS_TAREFA", status);
                        cmd.Parameters.AddWithValue("@DATAINCLUSAO", dataInclusao);
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

        public void PreencherDados(int id)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                
                try
                {
                    conn.Open();
                    string query = "SELECT DESCRICAO, STATUS_TAREFA, DATAPREVISTA FROM TAREFAS WHERE ID = @ID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.ExecuteNonQuery();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                bool status = Convert.ToBoolean(reader["STATUS_TAREFA"]);
                                labelStatus.Text = status ? "Concluído" : "Pendente"; // Apenas exibe o status
                                textBox1.Text = reader["DESCRICAO"].ToString();
                                dateTimePicker1.Value = Convert.ToDateTime(reader["DATAPREVISTA"]);
                                labelId.Text = id.ToString(); // Salva o ID oculto
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar os dados: {ex.Message}");
                }
            }
        }

    }
}
