using System;

namespace Ind_zadan
{
    [Serializable]
    public abstract class Contact // абстрактный клас с общими видами экземпляров класса
    {
        public override int GetHashCode()
        {
            return Name.GetHashCode()/5 +
                   Address.GetHashCode()/5 +
                   PhoneNumber.GetHashCode()/5 +
                   Email.GetHashCode()/5 +
                   Comment.GetHashCode()/5;
        }
        public new abstract string GetType(); //для получения данных в таблицу
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; } 

    }
    [Serializable]
    class Company:Contact
    {
        public override string GetType()
        {
            return "Company";
        }
        public string BusinessType { get; set; }
    }
    [Serializable]
    class Person : Contact
    {
        public override string GetType() //для получения данных в таблицу
        {
            return "Person";
        }
        public string DateOfBirth { get; set; }
    }
}
