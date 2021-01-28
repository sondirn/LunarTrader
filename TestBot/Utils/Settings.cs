using System;
using System.Text;

namespace TestBot.Utils
{
    public class Settings
    {
        public string KeyId;
        public string SecretKey;

        public void Log()
        {
            Console.WriteLine("Key Id: {0} \n Secret Key: {1}", KeyId, SecretKey);
        }
    }
}