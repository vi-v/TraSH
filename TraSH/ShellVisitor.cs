// MIT License - Copyright (c) Vishnu Vijayan
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TraSH.Model;
    using TraSH.Gen;
    using Antlr4.Runtime.Misc;
    using System.Linq;
    using static TraSH.Gen.ShellParser;
    using System.Reflection;

    public class ShellVisitor : ShellBaseVisitor<ParserResult>
    {
        public override ParserResult VisitShellCommand([NotNull] ShellCommandContext context)
        {
            List<SimpleCommand> pipeList = this.VisitPipeList(context.pipeList()).PipeListValue;

            ShellCommand shellCommand = new ShellCommand();
            shellCommand.CommandList = pipeList;

            ParserResult result = new ParserResult(shellCommand);
            result.IsShellCommand = true;

            return result;
        }

        public override ParserResult VisitPipeList([NotNull] PipeListContext context)
        {
            return this.BuildList<PipeListContext, SimpleCommandContext, SimpleCommand>(
                context.pipeList,
                context.simpleCommand,
                this.VisitPipeList,
                this.VisitSimpleCommand,
                "SimpleCommandValue",
                "PipeListValue",
                "IsPipeList");
        }

        public override ParserResult VisitSimpleCommand([NotNull] SimpleCommandContext context)
        {
            string command = this.VisitCmd(context.cmd()).CmdValue;
            IEnumerable<string> arguments = Enumerable.Empty<string>();

            if (context.args() != null)
            {
                arguments = this.VisitArgs(context.args()).ArgListValue;
            }

            SimpleCommand simpleCommand = new SimpleCommand(command, arguments);
            ParserResult result = new ParserResult(simpleCommand);
            result.IsSimpleCommand = true;

            return result;
        }

        public override ParserResult VisitArgs([NotNull] ArgsContext context)
        {
            return this.BuildList<ArgsContext, ArgContext, string>(
                context.args,
                context.arg,
                this.VisitArgs,
                this.VisitArg,
                "ArgValue",
                "ArgListValue",
                "IsArgList");
        }

        public override ParserResult VisitArg([NotNull] ArgContext context)
        {
            if (context.STRING() != null)
            {
                string parsedString = context.STRING().GetText();
                parsedString = parsedString.Substring(1, parsedString.Length - 2);

                ParserResult result = new ParserResult(parsedString);
                result.IsArg = true;

                return result;
            }
            else
            {
                ParserResult result = new ParserResult(context.WORD().GetText());
                result.IsArg = true;

                return result;
            }
        }

        public override ParserResult VisitCmd([NotNull] CmdContext context)
        {
            string cmdArg = this.VisitArg(context.arg()).ArgValue;

            ParserResult result = new ParserResult(cmdArg);
            result.IsCmd = true;

            return result;
        }

        private ParserResult BuildList<TListContext, TElContext, TElement>(
            Func<TListContext> listContext,
            Func<TElContext> elementContext,
            Func<TListContext, ParserResult> visitList,
            Func<TElContext, ParserResult> visitListElement,
            string elementName,
            string elementListName,
            string typePresentName)
        {
            ParserResult elResult = visitListElement(elementContext());
            TElement elValue = (TElement)elResult
                .GetType()
                .GetProperty(elementName)
                .GetValue(elResult);

            if (listContext() == null)
            {
                List<TElement> list = new List<TElement> { elValue };

                ParserResult result = new ParserResult(list);

                Type type = result.GetType();
                PropertyInfo prop = type.GetProperty(typePresentName);
                prop.SetValue(result, true);

                return result;
            }
            else
            {
                ParserResult contextResult = visitList(listContext());
                List<TElement> list = (List<TElement>)contextResult
                    .GetType()
                    .GetProperty(elementListName)
                    .GetValue(contextResult);

                list.Add(elValue);

                return contextResult;
            }
        }
    }
}
