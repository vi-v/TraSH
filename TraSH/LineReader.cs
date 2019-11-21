namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LineReader
    {
        public EventHandler<string> LineReceived;
        private readonly StringBuilder buffer;
        private readonly Dictionary<ConsoleKey, Action<ConsoleKeyInfo>> consoleKeyMap;

        public LineReader()
        {
            this.buffer = new StringBuilder();
            this.consoleKeyMap = new Dictionary<ConsoleKey, Action<ConsoleKeyInfo>>
            {
                { ConsoleKey.Enter, this.HandleEnter },
                { ConsoleKey.Backspace, this.HandleBackspace }
            };
        }

        public void Start()
        {
            while (true)
            {
                ConsoleKeyInfo c = Console.ReadKey(true);
                if (this.consoleKeyMap.ContainsKey(c.Key))
                {
                    this.consoleKeyMap[c.Key](c);
                }
                else
                {
                    this.HandleCharacter(c);
                }
            }
        }

        private void HandleEnter(ConsoleKeyInfo c)
        {
            Console.WriteLine();
            this.PrintPrompt();

            if (this.buffer.Length > 0)
            {
                this.LineReceived?.Invoke(this, this.buffer.ToString());
                this.buffer.Clear();
            }
        }

        private void HandleBackspace(ConsoleKeyInfo c)
        {
            if (this.buffer.Length > 0)
            {
                this.buffer.Length--;
                Console.Write("\b \b");
            }
        }

        private void HandleCharacter(ConsoleKeyInfo c)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(c.KeyChar);
            Console.ResetColor();
            this.buffer.Append(c.KeyChar);
        }

        private void PrintPrompt()
        {
            Console.Write("TraSH>");
        }
    }
}
