grammar WistGrammar;

program: line* EOF;
line: endOfLine* (statement | declaration | expression) endOfLine*;

declaration: funcDecl | labelDecl | structDecl;
statement: simpleStatement (';' simpleStatement)*;
simpleStatement: assigment | ret | jmp | ifBlock | newStruct | include | break | continue | loops;

ret: 'ret' expression;
jmp: 'jmp' IDENTIFIER;
ifBlock: 'if' expression block elseBlock?;
elseBlock: 'else' block;

include: 'include' STRING; 

assigment: varAssigment | structFieldAssigment;
varAssigment: IDENTIFIER '=' expression;
structFieldAssigment: expression '.' IDENTIFIER '=' expression;

loops: whileLoop | forLoop | repeatLoop;

break: 'break' endOfLine;
continue: 'continue' endOfLine;

whileLoop: 'while' expression block;
forLoop: 'for' assigment? endOfLine expression? endOfLine assigment? block;
repeatLoop: 'repeat' '('? (expression ',')? (expression ',')? expression ')'? 'with' IDENTIFIER block;

structDecl: 'struct' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' inheritance? block;
inheritance: (':' IDENTIFIER (IDENTIFIER (',' IDENTIFIER)*)?);

funcDecl: 'func' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' block;
labelDecl: IDENTIFIER ':';

block: endOfLine* (('{' endOfLine* line* '}') | (':') (statement | expression)) endOfLine*;

expression
    : constant                                                                          #constantExpression
    | '!' expression                                                                    #notExpression
    | newStruct                                                                         #newStructExpression
    | IDENTIFIER '(' (expression (',' expression)*)? ')'                                #funcCallExpression
    | expression '.' IDENTIFIER '(' (expression (',' expression)*)? ')'                 #structFuncCallExpression
    | expression '.' IDENTIFIER                                                         #structFieldExpression
    | '[' (expression (',' expression)*)? ']'                                           #listExpression
    | expression POW_OP expression                                                      #powExpression
    | expression REM_OP expression                                                      #remExpression
    | expression MUL_OP expression                                                      #mulExpression
    | expression ADD_OP expression                                                      #addExpression
    | expression CMP_OP expression                                                      #cmpExpression
    | expression BOOL_OP expression                                                     #boolExpression
    | '(' expression ')'                                                                #parenthesizedExpression
    | IDENTIFIER                                                                        #identifierExpression
    ;
    
    
     
newStruct: 'new' IDENTIFIER '(' (expression (',' expression)*)? ')';

constant: NUMBER | STRING | BOOL | NULL;

endOfLine: ( '\r'? '\n' | '\r' | ';' );


NUMBER: [-]? [0-9] [0-9_]* ('.' [0-9_]*)? ('e' ('+' | '-')? [0-9_]*)?;
STRING: ('"' ~'"'* '"') | ('\'' ~'\''* '\'');
BOOL: 'true' | 'false' | 'yes' | 'no';
NULL: 'null' | 'none';

BOOL_OP: 'and' | 'or' | 'xor';
POW_OP: '^';
MUL_OP: '*' | '/';
ADD_OP: '+' | '-';
REM_OP: '%';
CMP_OP: '==' | '!=' | '>' | '<' | '<=' | '>=';

IDENTIFIER: [a-zA-Z_][a-zA-Z0-9_]*;
WHITE_SPACE: [ \t\r\n]+ -> skip;
SINGLE_LINE_COMMENT: '//' ~[\r\n]* -> skip;