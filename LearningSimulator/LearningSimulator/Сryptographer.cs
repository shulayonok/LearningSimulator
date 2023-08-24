using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LearningSimulator.Models;

namespace LearningSimulator
{
    public class Сryptographer
    {
        public string ToHash(string password)
        {
             
            SHA256 encoder = SHA256.Create();
            byte[] bytePassword = Encoding.UTF8.GetBytes(password);
            byte[] hashValue = encoder.ComputeHash(bytePassword);
            string hashString = string.Empty;
            foreach (byte x in hashValue)
            {
                hashString += string.Format("{0:x2}", x);
            }
            return hashString;
        }

        public async Task<bool> IsAuthorised(string username, string data, bool isPassword = true)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(username)) return false;

            // ищем юзера по его username (идентификация)
            User user = await App.Database.FindUserAsync(username);
            if (user == null) return false;

            if (isPassword)
            {
                // сопоставляем хэш с хэшем в БД (аутентификация)
                string hashString = ToHash(data);
                if (hashString == user.Password)
                    return true;
            }
            else
            {
                // проверка контрольного вопроса
                if (data == user.Answer)
                    return true;
            }

            return false;
        }

        public async Task<bool> IsRegistered(string username)
        {
            // проверяем, нет ли такого юзера в БД
            User user = await App.Database.FindUserAsync(username);
            if (user != null) return true;
            return false;
        }
    }
}
