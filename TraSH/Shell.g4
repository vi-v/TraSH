grammar Shell;

shellCommand
	: pipeList
	| pipeList redirectionList
	;

redirectionList
	: redirection
	| redirectionList redirection
	;

redirection
	: GREAT arg
	| LESS arg
	;

pipeList
	: simpleCommand
	| pipeList PIPE simpleCommand
	;

simpleCommand
	: cmd
	| cmd args
	;

cmd
	: arg
	;

args
	: arg
	| args arg
	;

arg
	: WORD
	| STRING
	;

LESS			: '<' ;
LESSLESS		: '<<' ;
GREAT           : '>' ;
GREATGREAT      : '>>' ;
PIPE            : '|' ;
STRING			: ('"' .*? '"' | '\'' .*? '\'') ;
WORD            : ID+ ;
fragment ID     : ~(' '|'\t') ;
WHITESPACE		: (' '|'\t')+ -> skip ;                                                                        
NEWLINE			: ('\r'?'\n')+ -> skip ;