using MediatorState.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediatorState.Models
{
    public class Logger : Colleague
    {
        public void WriteMessage(string message) =>
            Console.WriteLine($"[Лог] {message}");
    }
}
