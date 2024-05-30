using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Ind_zadan
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            //filter method
            LoadCheckedListBoxData();
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            this.Load += Form1_Load;

            textBox1.TextChanged += textBox1_TextChanged;
            
            dataGridView1.ColumnCount = 6;
            dataGridView1.RowCount = 1;

            string[] names = { "Имя", "Адрес", "Номер телефона", "Почта", "Коментарий", "Дата рождения/тип бизнесса" };

            for (int i = 0; i < names.Length; ++i)
            {
                dataGridView1.Columns[i].HeaderCell.Value = names[i];
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            // Загрузка данных при инициализации формы
            LoadContactsFromBinary("contacts.bin");
            
            new Thread(() => {
                Thread.Sleep(100);
                Invoke((MethodInvoker)(() => {
                    dataGridView1.AutoResizeColumns();
                    dataGridView1.AutoResizeRows();
                }));
            }).Start();

            UpdateDataGridViewFilterAndSort();
        }
        List<Contact> contacts = new List<Contact>();
        List<Contact> fCnt = new List<Contact>();

        void FillDGV(List<Contact> list)
        {
            dataGridView1.RowCount = list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = list[i].Name;
                dataGridView1.Rows[i].Cells[1].Value = list[i].Address;
                dataGridView1.Rows[i].Cells[2].Value = list[i].PhoneNumber;
                dataGridView1.Rows[i].Cells[3].Value = list[i].Email;
                dataGridView1.Rows[i].Cells[4].Value = list[i].Comment;
                if (list[i].GetType() == "Company")
                    dataGridView1.Rows[i].Cells[5].Value = (list[i] as Company).BusinessType;
                else
                    dataGridView1.Rows[i].Cells[5].Value = (list[i] as Person).DateOfBirth;
            }
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRows();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);

            if (form2.ShowDialog() == DialogResult.OK)
            {

                contacts.Add(form2.newContact);

                FillDGV(contacts);
            }
        }
        
        private void UpdateDataGridViewFilterAndSort()
        {

            // Создаем копию исходного списка контактов
            var filteredContacts = contacts.ToList();

            // Применяем фильтрацию по CheckedItems в CheckedListBox1
            var checkedItems = checkedListBox1.CheckedItems.Cast<string>().ToList();
            if (checkedItems.Any())
                filteredContacts = filteredContacts.Where(contact => checkedItems.Contains(contact.Comment)).ToList();

            // Применяем фильтрацию по CheckedItems в CheckedListBox2
            var checkedItems2 = checkedListBox2.CheckedItems.Cast<string>().ToList();
            if (checkedItems2.Any())
            {
                filteredContacts = filteredContacts.Where(contact =>
                {
                    // Получаем адрес контакта
                    string address = contact.Address;

                    // Получаем первое слово до символа "/"
                    string firstWord = address.Split('/')[0].Trim();

                    return checkedItems2.Contains(firstWord);
                }).ToList();
            }

            // Применяем фильтры в зависимости от состояния чекбоксов CheckBox3 и CheckBox4
            if (checkBox3.Checked && !checkBox4.Checked)
                filteredContacts = filteredContacts.Where(contact => contact is Person).ToList();
            else if (checkBox4.Checked && !checkBox3.Checked)
                filteredContacts = filteredContacts.Where(contact => contact is Company).ToList();


            // Применяем сортировку по алфавиту, если активен чекбокс checkBox1
            if (checkBox1.Checked)
                filteredContacts = filteredContacts.OrderBy(contact => contact.Name).ToList();

            // Применяем сортировку в обратном алфавитном порядке, если активен чекбокс checkBox2
            else if (checkBox2.Checked)
                filteredContacts = filteredContacts.OrderByDescending(contact => contact.Name).ToList();

            // Проверяем, остались ли элементы в filteredContacts после фильтрации
            if (filteredContacts.Any())
            {
                // Обновляем источник данных DataGridView
                FillDGV(filteredContacts);
            }

            fCnt = filteredContacts.ToArray().ToList();

            // После выполнения всех действий снимаем флаг
                
        }
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            UpdateDataGridViewFilterAndSort();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewFilterAndSort();
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewFilterAndSort();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) checkBox2.Checked = false;
            UpdateDataGridViewFilterAndSort();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked) checkBox1.Checked = false;
            UpdateDataGridViewFilterAndSort();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) checkBox4.Checked = false;
            UpdateDataGridViewFilterAndSort();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked) checkBox3.Checked = false;
            UpdateDataGridViewFilterAndSort();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            LoadCheckedListBoxData();
        }
        //Поиск
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            fCnt = contacts.Where(contact => contact.Name.ToLower().Contains(textBox1.Text.ToLower())).ToList();
            FillDGV(fCnt);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли выбранный ряд
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Получаем выбранный ряд
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Получаем связанный объект контакта
                Contact selectedContact = selectedRow.DataBoundItem as Contact;

                // Удаляем контакт из списка
                contacts.Remove(selectedContact);

                // Обновляем привязанный источник данных в DataGridView
                FillDGV(contacts);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Очистка элементов checkedListBox2 перед заполнением
            checkedListBox2.Items.Clear();

            // Используем HashSet для хранения уникальных значений
            HashSet<string> uniqueItems = new HashSet<string>();

            // Перебор строк в колонке Address и добавление части до символа "/" в HashSet
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string address = row.Cells[1].Value.ToString();
                string[] parts = address.Split('/');
                if (parts.Length > 0)
                {
                    string firstPart = parts[0].Trim(); // Получаем первую часть строки и убираем лишние пробелы
                    if (!string.IsNullOrEmpty(firstPart))
                    {
                        uniqueItems.Add(firstPart);
                    }
                }
            }

            // Добавляем уникальные элементы из HashSet в checkedListBox2
            foreach (string item in uniqueItems)
            {
                checkedListBox2.Items.Add(item, true); // Установка всех элементов с галочкой
            }
        }

        #region FileIO

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                // Вызываем метод для бинарной сериализации данных
                SerializeContactsBinary("contacts.bin");
                MessageBox.Show("Данні успішно збережені.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні данных: {ex.Message}");
            }
        }

        private void SerializeContactsBinary(string filename)
        {
            // Создаем объект BinaryFormatter для сериализации
            BinaryFormatter formatter = new BinaryFormatter();

            // Открываем файл для записи
            using (FileStream stream = new FileStream(filename, FileMode.Create))
            {
                // Сериализуем список контактов в бинарный формат и записываем его в файл
                formatter.Serialize(stream, contacts);
            }
        }
        private void LoadContactsFromBinary(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    // Создаем объект BinaryFormatter для десериализации
                    BinaryFormatter formatter = new BinaryFormatter();

                    // Открываем файл для чтения
                    using (FileStream stream = new FileStream(filename, FileMode.Open))
                    {
                        // Десериализуем список контактов из бинарного формата
                        contacts = (List<Contact>)formatter.Deserialize(stream);
                    }

                    // Обновляем источник данных DataGridView
                    FillDGV(contacts);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при завантаженні данних: {ex.Message}");
                }
            }

        }

        private void LoadCheckedListBoxData()
        {
            const string comboBoxDataFile = "comboBoxData.dat";

            if (File.Exists(comboBoxDataFile))
            {

                try
                {
                    using (FileStream stream = new FileStream(comboBoxDataFile, FileMode.Open))
                    {
                        // Десериализуем список строк из бинарного формата
                        List<string> items = (List<string>)new BinaryFormatter().Deserialize(stream);

                        checkedListBox1.Items.Clear();

                        foreach (var item in items)
                            checkedListBox1.Items.Add(item, true); // Установка всех элементов с галочкой
                    }
                    UpdateDataGridViewFilterAndSort();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Файл не найден.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filename = "contacts.bin";
            try
            {
                if (File.Exists(filename))
                {
                    // Выводим диалоговое окно с запросом подтверждения
                    DialogResult result = MessageBox.Show("Вы точно хотите удалить все данные из адресной книги?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Очистка DataGridView
                        dataGridView1.Rows.Clear();

                        // Удаление файла с данными
                        File.Delete(filename);
                        MessageBox.Show("Файл успешно очищен.");
                    }
                }
                else
                {
                    MessageBox.Show("Файл не существует.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при очистке файла: {ex.Message}");
            }
        }

        #endregion

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.RowIndex < 0) return;
            _ = new EditContact(fCnt[e.RowIndex]).ShowDialog();
            LoadCheckedListBoxData();
        }
    }
}
