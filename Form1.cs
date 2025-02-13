using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; //Para utilizar o SQLServer eu preciso da Biblioteca Data.SqlClient importada


namespace ToDo
{
    public partial class Main : Form
    {
    
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=ToDo;Trusted_Connection=True;";  //String de Conexão do meu banco, na tid a gente cria a partir do Conexão.CS
        public Main()
        {
           
            InitializeComponent();  
           
            ConfigureGridView();
           
            CarregarTarefa();
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;  //Força o commit sempre que eu marcar algo no retorno da query para atualizar automaticamente no banco
            
            
        }


        private void ConfigureGridView() //Metodo que cria as tabelas de forma pré-definida e somente as preenche com o conteudo do banco de dados
        {
            dataGridView1.AutoGenerateColumns = false;

            // Coluna ID
            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn(); //Design das colunas no datagridview, todas funcionam como objetos
            colId.Name = "colId";
            colId.HeaderText = "ID";
            colId.DataPropertyName = "ID";
            dataGridView1.Columns.Add(colId);
            colId.Width = 55;

            // Coluna Descrição
            DataGridViewTextBoxColumn colDescricao = new DataGridViewTextBoxColumn();
            colDescricao.Name = "colDescricao";
            colDescricao.HeaderText = "Descrição";
            colDescricao.DataPropertyName = "DESCRICAO";
            dataGridView1.Columns.Add(colDescricao);
            colDescricao.Width = 430;

            // Coluna Status
            DataGridViewCheckBoxColumn colStatus = new DataGridViewCheckBoxColumn();
            colStatus.Name = "colStatus";
            colStatus.HeaderText = "Status";
            colStatus.DataPropertyName = "STATUS_TAREFA";
            dataGridView1.Columns.Add(colStatus);
            colStatus.Width = 55;

            // Coluna Data de Criação
            DataGridViewTextBoxColumn colDataCriacao = new DataGridViewTextBoxColumn();
            colDataCriacao.Name = "colDataCriacao";
            colDataCriacao.HeaderText = "Data de Criação";
            colDataCriacao.DataPropertyName = "DATAINCLUSAO";
            dataGridView1.Columns.Add(colDataCriacao);
            colDataCriacao.Width = 100;

            //Coluna data prevista
            DataGridViewTextBoxColumn colDataPrevisao = new DataGridViewTextBoxColumn();
            colDataPrevisao.Name = "colDataPrevisao";
            colDataPrevisao.HeaderText = "Previsão";
            colDataPrevisao.DataPropertyName = "DATAPREVISTA";
            dataGridView1.Columns.Add(colDataPrevisao);
            colDataPrevisao.Width = 100;
        }

        public void CarregarTarefa()
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) //na prox tentar jogar isso na raiz da form pra não precisar criar um a cada metodo novo
            {
                try
                {
                    
                    conn.Open();
                    string query = "SELECT ID, DESCRICAO,STATUS_TAREFA,DATAINCLUSAO, DATAPREVISTA FROM dbo.TAREFAS";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    //dt.Clear();
                    adapter.Fill(dt);
                    comboBox1.SelectedIndex = 0;
                    dataGridView1.DataSource = dt;
                    
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Sql Error.\n {ex.Message}");
                }
            }
        }
        public void CarregarTarefaConcluida()
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) //na prox tentar jogar isso na raiz da form pra não precisar criar um a cada metodo novo
            {
                try
                {

                    conn.Open();
                    string query = "SELECT ID, DESCRICAO,STATUS_TAREFA,DATAINCLUSAO, DATAPREVISTA FROM dbo.TAREFAS WHERE STATUS_TAREFA = 1";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                   // dt.Clear();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
               
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Sql Error.\n {ex.Message}");
                }
            }
        }

        public void CarregarTarefaPendente()
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) //na prox tentar jogar isso na raiz da form pra não precisar criar um a cada metodo novo
            {
                try
                {

                    conn.Open();
                    string query = "SELECT ID, DESCRICAO,STATUS_TAREFA,DATAINCLUSAO, DATAPREVISTA FROM dbo.TAREFAS WHERE STATUS_TAREFA = 0";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    //dt.Clear();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;
                    
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Sql Error.\n {ex.Message}");
                }
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "colStatus")
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["colId"].Value);
                bool novoStatus = Convert.ToBoolean(dataGridView1.Rows[e.RowIndex].Cells["colStatus"].Value);
                AtualizarStatusBanco(id, novoStatus);
            }
        }

        private void AtualizarStatusBanco(int id, bool novoStatus)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE TAREFAS SET STATUS_TAREFA = @STATUS_TAREFA WHERE ID = @ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@STATUS_TAREFA", novoStatus);
                    cmd.Parameters.AddWithValue("@ID", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["colId"].Value); //Recebe o id da linha que estou selecionando

                AtualizarStatusBanco(id, true);

                CarregarTarefa();

                MessageBox.Show("Tarefa marcada como concluída!");
            }
            else
            {
                MessageBox.Show("Nenhuma tarefa selecionada!");
            }
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            AddTarefa ad = new AddTarefa();
            ad.ShowDialog();

            CarregarTarefa();
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Por favor, selecione uma tarefa para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["colId"].Value);

            DialogResult resultado = MessageBox.Show(
                "Certeza que deseja excluir a linha selecionada?",
                "Atenção",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM TAREFAS WHERE ID = @ID";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ID", id);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Tarefa excluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                CarregarTarefa(); // Recarrega os dados no DataGridView
                            }
                            else
                            {
                                MessageBox.Show("Nenhuma tarefa encontrada com o ID informado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir a tarefa: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

     
        private void btEdit_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma tarefa para editar.");
                return;
            }
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["colId"].Value);
            // Abre o formulário de edição
            TarefaEdit formEdit = new TarefaEdit();
            formEdit.PreencherDados(id);
            formEdit.ShowDialog();
            
            // Após fechar, atualiza o DataGridView
            CarregarTarefa();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    CarregarTarefa();
                    break;
                case 1:
                    CarregarTarefaPendente(); 
                    break;
                case 2:
                    CarregarTarefaConcluida();
                    break;
            }
        }

        private void txtConsulta_Leave(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString)) //na prox tentar jogar isso na raiz da form pra não precisar criar um a cada metodo novo
            {
                try
                {

                    conn.Open();
                    if (txtConsulta.Text == "" || txtConsulta.Text is null)
                    {
                        string query = "SELECT ID, DESCRICAO,STATUS_TAREFA,DATAINCLUSAO, DATAPREVISTA FROM dbo.TAREFAS";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataTable dt = new DataTable();
                        //dt.Clear();
                        adapter.Fill(dt);
                        comboBox1.SelectedIndex = 0;
                        dataGridView1.DataSource = dt;
                    }
                    else
                    {
                        string pesquisa = txtConsulta.Text;
                        string query = $"SELECT ID, DESCRICAO,STATUS_TAREFA,DATAINCLUSAO, DATAPREVISTA FROM dbo.TAREFAS WHERE DESCRICAO LIKE '%{pesquisa}%'";
                        SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                        DataTable dt = new DataTable();
                        //dt.Clear();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Sql Error.\n {ex.Message}");
                }
            }
        }
    }
}
