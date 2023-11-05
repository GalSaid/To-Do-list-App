using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.IO;

namespace To_Do_List
{
    public partial class TasksForm : Form
    {
        private DataTable table;
        private int openTaskForEditInd; //if we click the button "read" so now we can also edit the task after this
        private List<Task> tasksEntries;

        public TasksForm()
        {
            InitializeComponent();
            openTaskForEditInd = -1; //There is not open task to edit
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            table = new DataTable();
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Message", typeof(string));
            allTasks.DataSource = table; //connect to table
            allTasks.Columns["Message"].Visible = false;
            allTasks.Columns["Title"].Width = 250;
            LoadDataFromJsonFile();
        }

        private void btnNew_Click(object sender, EventArgs e) //clean the info
        {
            ResetInfo();
        }

        private void btnSave_Click(object sender, EventArgs e) //save the infromation of the task
        {
            if (txtTitle.Text.Trim() == "")
                MessageBox.Show("You must enter title!");
            else
            {
                if (openTaskForEditInd != -1) //if there is a open note so it will edit the current note
                {
                    table.Rows[openTaskForEditInd][0] = txtTitle.Text;
                    table.Rows[openTaskForEditInd][1] = txtMessage.Text;

                }
                else //create a new note
                    table.Rows.Add(txtTitle.Text, txtMessage.Text);
                ResetInfo();
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            int rowsCount = allTasks.SelectedRows.Count;
            if (rowsCount > 1)
                MessageBox.Show("You can only choose one task!");
            else if(rowsCount==1)
            {
                int index = allTasks.CurrentCell.RowIndex;
                if (index > -1)
                {
                    openTaskForEditInd = index;
                    txtTitle.Text = table.Rows[index].ItemArray[0].ToString();
                    txtMessage.Text = table.Rows[index].ItemArray[1].ToString();
                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in allTasks.SelectedRows)
            {
                table.Rows[row.Index].Delete();
            }
            ResetInfo();
        }

        private void ResetInfo()
        {
            txtTitle.Clear();
            txtMessage.Clear();
            openTaskForEditInd = -1; //you cant edit a task until you will press read 
        }

        private void allTasks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            openTaskForEditInd = -1;
        }

        private void NoteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDataToJsonFile();
        }
        private void SaveDataToJsonFile()
        {
            tasksEntries = new List<Task>();

            // Retrieve the data from the savedTasksTable and add the task to tasksEntries
            foreach (DataRow row in table.Rows)
            {
                if (!row.IsNull("Title"))
                {
                    Task entry = new Task(row["Title"].ToString(), row["Message"].ToString());
                    tasksEntries.Add(entry);
                }
            }
            // Serialize tasksEntries to JSON
            string json = JsonConvert.SerializeObject(tasksEntries, Formatting.Indented);

            // Write the JSON data to the file
            File.WriteAllText("storage.json", json);
        }

        private void LoadDataFromJsonFile()
        {
            string filePath = "storage.json";

            if (!File.Exists(filePath))
            {
                // File doesn't exist, create a new one with an empty array
                File.WriteAllText(filePath, "[]");
            }


            string json = File.ReadAllText("storage.json");
            tasksEntries = JsonConvert.DeserializeObject<List<Task>>(json);

            // Clear the DataTable
            table.Rows.Clear();

            // Add the data from noteEntries to the DataTable
            foreach (Task entry in tasksEntries)
            {
                table.Rows.Add(entry.Title, entry.Message);
            }
        }

        private void allTasks_SelectionChanged(object sender, EventArgs e)
        {
            ResetInfo(); //if we click on onother task it closes the current
        }
    }
}
