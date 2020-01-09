// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class Pipe
    {
        private readonly Process inProc;
        private readonly Process outProc;
        private readonly CancellationTokenSource tokenSource;

        private string pipeContent = "";

        public Pipe(Process inProc, Process outProc)
        {
            this.inProc = inProc;
            this.outProc = outProc;
            this.IsClosed = true;
            this.tokenSource = new CancellationTokenSource();
        }

        public Pipe(StreamReader inStreamReader, Process outProc)
        {
            this.InStreamReader = inStreamReader;
            this.outProc = outProc;
            this.IsClosed = true;
            this.tokenSource = new CancellationTokenSource();
        }

        public Pipe(Process inProc, TextWriter outStreamWriter)
        {
            this.inProc = inProc;
            this.OutStreamWriter = outStreamWriter;
            this.IsClosed = true;
            this.tokenSource = new CancellationTokenSource();
        }

        public Pipe(StreamReader inStreamReader, TextWriter outStreamWriter)
        {
            this.OutStreamWriter = outStreamWriter;
            this.InStreamReader = inStreamReader;
            this.IsClosed = true;
            this.tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            if (this.outProc != null)
            {
                this.OutStreamWriter = this.outProc.StandardInput;
            }

            if (this.inProc != null)
            {
                this.InStreamReader = this.inProc.StandardOutput;
            }

            this.IsClosed = false;
            this.PipeTask = Task.Run(() => this.PipeOutput(this.tokenSource.Token));
        }

        public void Close()
        {
            this.IsClosed = true;
            this.tokenSource.Cancel();
        }

        public void Wait()
        {
            if (this.PipeTask == null)
            {
                return;
            }

            this.PipeTask.Wait();
        }

        public StreamReader InStreamReader { get; private set; }

        public TextWriter OutStreamWriter { get; private set; }

        public bool IsClosed { get; private set; }

        private Task PipeTask { get; set; }

        private void PipeOutput(CancellationToken token)
        {
            while (!this.InStreamReader.EndOfStream)
            {
                //this.InProcess.HasExited;
                //token.IsCancellationRequested;

                if (token.IsCancellationRequested)
                {
                    string content = this.InStreamReader.ReadToEnd();
                    this.pipeContent += content;

                    this.OutStreamWriter.Write(content);
                    break;
                }
                else if (!this.InStreamReader.EndOfStream)
                {
                    char content = (char)this.InStreamReader.Read();
                    this.pipeContent += content;

                    this.OutStreamWriter.Write(content);

                    if (content == 0)
                    {
                        break;
                    }
                }
            }

            this.InStreamReader.Close();
            this.OutStreamWriter.Close();
            this.IsClosed = true;
        }
    }
}
