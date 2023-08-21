grammar WistGrammar;

program: line* EOF;
line: endOfLine* (statement | declaration | funcCall) endOfLine*;

declaration: funcDecl | labelDecl;
statement: simpleStatement (';' simpleStatement)*;
simpleStatement: assigment | ret | jmp | ifBlock;

ret: 'ret' expression;
jmp: 'jmp' IDENTIFIER;
ifBlock: 'if' expression block elseBlock?;
elseBlock: 'else' block;

assigment: varAssigment;
varAssigment: IDENTIFIER '=' expression;

funcDecl: 'func' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' block;
labelDecl: IDENTIFIER ':';

block: endOfLine* (('{' line* '}') | (':') line) endOfLine*;

expression
    : constant                              #constantExpression
    | IDENTIFIER                            #identifierExpression
    | '(' expression ')'                    #parenthesizedExpression
    | '!' expression                        #notExpression
    | funcCall                              #funcCallExpressio
    | expression REM_OP expression          #remExpression
    | expression MUL_OP expression          #mulExpression
    | expression ADD_OP expression          #addExpression
    | expression CMP_OP expression          #cmpExpression
    | expression BOOL_OP expression         #boolExpression
    ;

funcCall: IDENTIFIER '(' (expression (',' expression)*)? ')';

constant: NUMBER | STRING | BOOL | NULL;

endOfLine: ( '\r'? '\n' | '\r' | '\f' | ';' );


NUMBER: [-]? [0-9] [0-9_]* ('.' [0-9_]*)? ('e' ('+' | '-')? [0-9_]*)?;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
BOOL: 'true' | 'false' | 'yes' | 'no';
NULL: 'null' | 'none';

BOOL_OP: 'and' | 'or' | 'xor';
MUL_OP: '*' | '/';
ADD_OP: '+' | '-';
REM_OP: '%';
CMP_OP: '==' | '!=' | '>' | '<' | '<=' | '>=';

IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
WHITE_SPACE: [ \t] -> skip;
SINGLE_LINE_COMMENT: '//' ~[\r\n]* -> skip;