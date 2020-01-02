// Generated from c:\Users\HomePC\Documents\Projects\TraSH\TraSH\Shell.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class ShellLexer extends Lexer {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		STRING=1, WORD=2, WHITESPACE=3, NEWLINE=4;
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	public static final String[] ruleNames = {
		"STRING", "WORD", "ID", "WHITESPACE", "NEWLINE"
	};

	private static final String[] _LITERAL_NAMES = {
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "STRING", "WORD", "WHITESPACE", "NEWLINE"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}


	public ShellLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "Shell.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\2\6.\b\1\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\3\2\3\2\7\2\20\n\2\f\2\16\2\23\13\2\3\2"+
		"\3\2\3\3\6\3\30\n\3\r\3\16\3\31\3\4\3\4\3\5\6\5\37\n\5\r\5\16\5 \3\5\3"+
		"\5\3\6\5\6&\n\6\3\6\6\6)\n\6\r\6\16\6*\3\6\3\6\3\21\2\7\3\3\5\4\7\2\t"+
		"\5\13\6\3\2\3\4\2\13\13\"\"\2\61\2\3\3\2\2\2\2\5\3\2\2\2\2\t\3\2\2\2\2"+
		"\13\3\2\2\2\3\r\3\2\2\2\5\27\3\2\2\2\7\33\3\2\2\2\t\36\3\2\2\2\13(\3\2"+
		"\2\2\r\21\7$\2\2\16\20\13\2\2\2\17\16\3\2\2\2\20\23\3\2\2\2\21\22\3\2"+
		"\2\2\21\17\3\2\2\2\22\24\3\2\2\2\23\21\3\2\2\2\24\25\7$\2\2\25\4\3\2\2"+
		"\2\26\30\5\7\4\2\27\26\3\2\2\2\30\31\3\2\2\2\31\27\3\2\2\2\31\32\3\2\2"+
		"\2\32\6\3\2\2\2\33\34\n\2\2\2\34\b\3\2\2\2\35\37\t\2\2\2\36\35\3\2\2\2"+
		"\37 \3\2\2\2 \36\3\2\2\2 !\3\2\2\2!\"\3\2\2\2\"#\b\5\2\2#\n\3\2\2\2$&"+
		"\7\17\2\2%$\3\2\2\2%&\3\2\2\2&\'\3\2\2\2\')\7\f\2\2(%\3\2\2\2)*\3\2\2"+
		"\2*(\3\2\2\2*+\3\2\2\2+,\3\2\2\2,-\b\6\2\2-\f\3\2\2\2\b\2\21\31 %*\3\b"+
		"\2\2";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}