using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ind_zadan
{
    public partial class EditContact : Form
    {
        private const string ComboBoxDataFile = "comboBoxData.dat";
        private List<string> comboBoxData = new List<string>();
        Contact contact;

        public EditContact(Contact contact)
        {
            InitializeComponent();
            this.contact = contact;

            textBox1.Text = contact.Name;
            textBox2.Text = contact.Address;
            textBox3.Text = contact.PhoneNumber;
            textBox4.Text = contact.Email;
            textBox5.Text = contact.Comment;
            if (contact.GetType() == "Company")
                textBox6.Text = (contact as Company).BusinessType;
            else
                textBox6.Text = (contact as Person).DateOfBirth;

            using (FileStream stream = new FileStream(ComboBoxDataFile, FileMode.Open))
                comboBoxData = (List<string>)new BinaryFormatter().Deserialize(stream);

            if (contact.GetType() == "Company")
                label6.Text = "Тип бізнесу";
            else
                label6.Text = "Дата народження";
        }

        bool name = false;
        bool adres = false;
        bool telefon_number = false;
        bool email = false;
        bool text_box_5_swap = false;

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
                    MessageBox.Show("Текст не соответствует шаблону 'Город/Текст далее'.");
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
            else if (text3.Length < 13)
            {
                MessageBox.Show("Мінімальная длина номера - 12 чисел після '+'.");
            }
            else if (text3.Length == 13)
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
            if (contact.GetType() != "Company")
            {
                string text5 = textBox6.Text;

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

            if (name && adres && telefon_number && email && text_box_5_swap)
            {
                contact.Name = textBox1.Text;
                contact.Address = textBox2.Text;
                contact.PhoneNumber = textBox3.Text;
                contact.Email = textBox4.Text;
                contact.Comment = textBox5.Text;
                if (contact.GetType() == "Company")
                    (contact as Company).BusinessType = textBox6.Text;
                else
                    (contact as Person).DateOfBirth = textBox6.Text;

                if(!comboBoxData.Contains(textBox5.Text)) comboBoxData.Add(textBox5.Text);

                using (FileStream stream = new FileStream(ComboBoxDataFile, FileMode.Create))
                    new BinaryFormatter().Serialize(stream, comboBoxData);

                DialogResult = DialogResult.OK;
            }
        }
    }
}
