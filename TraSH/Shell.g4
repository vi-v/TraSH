grammar Shell;

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

STRING			: ('"' .*? '"' | '\'' .*? '\'') ;
WORD            : ID+ ;
fragment ID     : ~(' '|'\t') ;
WHITESPACE		: (' '|'\t')+ -> skip ;                                                                        
NEWLINE			: ('\r'?'\n')+ -> skip ;