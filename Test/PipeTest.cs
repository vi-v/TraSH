// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH.Test
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TraSH;

    [TestClass]
    public class PipeTest
    {
        [TestMethod]
        public void SinglePipeTest()
        {
            StreamReader inStream = StreamFromText("Some pipeline input text");
            StringWriter outStream = OutputStream(out StringBuilder sb);
            Pipe pipe = new Pipe(inStream, outStream);

            pipe.Start();
            pipe.Close();
            pipe.Wait();

            sb.ToString().Should().Be("Some pipeline input text");
            pipe.IsClosed.Should().BeTrue();
        }

        [TestMethod]
        public void MultiplePipesTest()
        {
            StreamReader inStream1 = StreamFromText("Some pipeline input text");
            StringWriter outStream1 = OutputStream(out StringBuilder sb1);
            Pipe pipe1 = new Pipe(inStream1, outStream1);
            pipe1.Start();
            pipe1.Close();
            pipe1.Wait();

            StreamReader inStream2 = StreamFromText(sb1.ToString().ToUpper());
            StringWriter outStream2 = OutputStream(out StringBuilder sb2);
            Pipe pipe2 = new Pipe(inStream2, outStream2);
            pipe2.Start();
            pipe2.Close();
            pipe2.Wait();

            StreamReader inStream3 = StreamFromText(string.Join("", sb2.ToString().Split(" ")));
            StringWriter outStream3 = OutputStream(out StringBuilder sb3);
            Pipe pipe3 = new Pipe(inStream3, outStream3);
            pipe3.Start();
            pipe3.Close();
            pipe3.Wait();

            sb3.ToString().Should().Be("SOMEPIPELINEINPUTTEXT");
            pipe1.IsClosed.Should().BeTrue();
            pipe2.IsClosed.Should().BeTrue();
            pipe3.IsClosed.Should().BeTrue();
        }

        [TestMethod]
        public void EmptyPipeTest()
        {
            StreamReader inStream = StreamFromText("");
            StringWriter outStream = OutputStream(out StringBuilder sb);
            Pipe pipe = new Pipe(inStream, outStream);

            pipe.Start();
            pipe.Close();
            pipe.Wait();

            sb.ToString().Should().Be("");
            pipe.IsClosed.Should().BeTrue();
        }

        private static StreamReader StreamFromText(string s)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            Stream stream = new MemoryStream(byteArray);
            StreamReader reader = new StreamReader(stream);

            return reader;
        }

        private static StringWriter OutputStream(out StringBuilder sb)
        {
            sb = new StringBuilder();
            StringWriter outStream = new StringWriter(sb);
            return outStream;
        }
    }
}
