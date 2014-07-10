using System;

namespace Email
{
    class EmailDriver
    {
        static void Main(string[] args)
        {
            EmailSensor obj = new EmailSensor(args);
            Console.ReadLine();
        }
    }
}
