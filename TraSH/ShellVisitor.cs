namespace TraSH
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TraSH.Model;
    using TraSH.Gen;
    using Antlr4.Runtime.Misc;

    public class ShellVisitor : ShellBaseVisitor<ParserResult>
    {
        public override ParserResult VisitArgs([NotNull] ShellParser.ArgsContext context)
        {
            if (context.args() == null)
            {
                List<string> argList = new List<string> { this.VisitArg(context.arg()).ArgValue };

                ParserResult result = new ParserResult(argList);
                result.IsArgList = true;

                return result;
            }
            else
            {
                ParserResult contextResult = this.VisitArgs(context.args());
                List<string> argList = contextResult.ArgListValue;
                argList.Add(this.VisitArg(context.arg()).ArgValue);

                return contextResult;
            }
        }

        public override ParserResult VisitArg([NotNull] ShellParser.ArgContext context)
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
    }
}
