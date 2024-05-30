using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Ind_zadan
{
    public partial class Form2 : Form
    {
        
        private const string ComboBoxDataFile = "comboBoxData.dat";
        private List<string> comboBoxData = new List<string>();
        private Form1 form1;
        public Form2(Form1 form1) 
        {
            InitializeComponent();
            this.form1 = form1;
            LoadComboBoxData();
        }
        bool name = false;
        bool adres = false;
        bool telefon_number = false;
        bool email = false;
        bool text_box_5_swap = false;

        public Contact newContact;


        private void label3_Click(object sender, EventArgs e)
        {

        }
        #region  True/False
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label5.Text = "Тип Бизнесу";
            }
            else
            {
                label5.Text = "День Народження";
            }
            if (checkBox1.Checked)
            {
                label7.Text = "Створити контакт компании";
            }
            else
            {
                label7.Text = "Створити контакт людини";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text1 = textBox1.Text;

            if (!string.IsNullOrWhiteSpace(text1))
            {
                name = true;
            }
            else
            {
                MessageBox.Show("Строка 'Имя' порожня.");
            }

            //Проверка адреса
            string text2 = textBox2.Text;

            if (!string.IsNullOrWhiteSpace(text2))
            {
                // Паттерн для проверки "Город/текст далее"
                string pattern = @"[А-Я].+\s*\/[А-Я].\s*.+";

                if (Regex.IsMatch(text2, pattern))
                {
                    // Текст соответствует паттерну
                    adres = true;
                }
                else
                {
                    MessageBox.Show("Текст не соответствует шаблону 'Город/текст далее'.");
                }
            }
            else
            {
                MessageBox.Show("Строка Имя пуста.");
            }

            //Проверка Номера Телефона
            string text3 = textBox3.Text;

              string numberpattern = @"^\+\d*$";

              if (!Regex.IsMatch(text3, numberpattern))
              {
                  MessageBox.Show("Введіть номер в форматі: +числа (наприклад, +123456789).");
              }
                if (text3.Length > 13)
                  {
                      MessageBox.Show("Максимальна дожина номера - 12 чисел після '+'.");
                  }
                  else if(text3.Length < 13)
                  {
                      MessageBox.Show("Мінімальная длина номера - 12 чисел після '+'.");
                  }else if(text3.Length == 13)
                  {
                     telefon_number = true;
                  }
             //Проверка Електронной почти
             string text4 = textBox4.Text;

             string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

             bool isValidEmail = Regex.IsMatch(text4, emailPattern);

             if (isValidEmail)
             {
                 email = true;
             }
             else
             {
                 MessageBox.Show("Текст не відповідає паттерну(example@example.com) електронної пошти.");
             }

             //Проверка Дня Рождения
             if (!checkBox1.Checked)
             {
                 string text5 = textBox5.Text;

                 string datePattern = @"^(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.\d{4}$";

                 bool isValidDate = Regex.IsMatch(text5, datePattern);

                 if (isValidDate)
                 {
                     text_box_5_swap = true;
                 }
                 else
                 {
                     MessageBox.Show("Текст не відповідає паттерну дати нарождення (DD.MM.YYYY).");
                 }
             }
             else
             {
                 text_box_5_swap = true;
             }

            #endregion
            //Проверка на правильное введение всех строк
            if (name == true && adres == true && telefon_number == true && email == true && text_box_5_swap == true )
            {
                if (checkBox1.Checked == true)
                {
                    Company newContact1 = new Company();
                    { 
                        newContact1.Name = textBox1.Text;
                        newContact1.Address = textBox2.Text;
                        newContact1.PhoneNumber = textBox3.Text;
                        newContact1.Email = textBox4.Text;
                        newContact1.BusinessType = textBox5.Text;
                        newContact1.Comment = comboBox1.Text;
                    }
                    newContact = newContact1;

                }
                else
                {
                    Person newContact1 = new Person();
                    {
                        newContact1.Name = textBox1.Text;
                        newContact1.Address = textBox2.Text;
                        newContact1.PhoneNumber = textBox3.Text;
                        newContact1.Email = textBox4.Text;
                        newContact1.DateOfBirth = textBox5.Text;
                        newContact1.Comment = comboBox1.Text;
                    }
                    newContact = newContact1;

                }
                    MessageBox.Show("Контакт записан");
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Не вдалось записать контакт");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        #region ComboBox
        private void button2_Click(object sender, EventArgs e)
        {
            //добавление
            string newItem = textBox6.Text;

            if (!string.IsNullOrEmpty(newItem))
            {
                comboBox1.Items.Add(newItem);
                comboBoxData.Add(newItem);
                textBox6.Text = string.Empty;

                SaveComboBoxData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите значение для добавления.");
            }
        }
        private void LoadComboBoxData()
        {
            try
            {
                if (File.Exists(ComboBoxDataFile))
                {
                    using (FileStream stream = new FileStream(ComboBoxDataFile, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        comboBoxData = (List<string>)formatter.Deserialize(stream);
                    }

                    comboBox1.Items.AddRange(comboBoxData.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных ComboBox: {ex.Message}");
            }
        }
        private void SaveComboBoxData()
        {
            try
            {
                if (comboBoxData != null)
                {
                    using (FileStream stream = new FileStream(ComboBoxDataFile, FileMode.Create))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, comboBoxData);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных в ComboBox: {ex.Message}");
            }
        }   
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Сохранение данных ComboBox при закрытии формы
            SaveComboBoxData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear(); 
            comboBoxData.Clear(); 
            SaveComboBoxData(); 
        }

        //Передача в первую
        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        #endregion

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
