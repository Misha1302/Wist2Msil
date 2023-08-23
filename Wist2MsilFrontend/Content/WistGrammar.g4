grammar WistGrammar;

program: line* EOF;
line: endOfLine* (statement | declaration | expression) endOfLine*;

declaration: funcDecl | labelDecl | structDecl;
statement: simpleStatement (';' simpleStatement)*;
simpleStatement: assigment | ret | jmp | ifBlock | newStruct;

ret: 'ret' expression;
jmp: 'jmp' IDENTIFIER;
ifBlock: 'if' expression block elseBlock?;
elseBlock: 'else' block;

assigment: varAssigment | structFieldAssigment;
varAssigment: IDENTIFIER '=' expression;
structFieldAssigment: expression '.' IDENTIFIER '=' expression;

structDecl: 'struct' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' inheritance? block;
inheritance: (':' IDENTIFIER (IDENTIFIER (',' IDENTIFIER)*)?);

funcDecl: 'func' IDENTIFIER '(' (IDENTIFIER (',' IDENTIFIER)*)? ')' block;
labelDecl: IDENTIFIER ':';

block: endOfLine* (('{' line* '}') | (':') line) endOfLine*;

expression
    : constant                                                                          #constantExpression
    | '!' expression                                                                    #notExpression
    | newStruct                                                                         #newStructExpression
    | IDENTIFIER '(' (expression (',' expression)*)? ')'                                #funcCallExpression
    | expression '.' IDENTIFIER '(' (expression (',' expression)*)? ')'                 #structFuncCallExpression
    | expression '.' IDENTIFIER                                                         #structFieldExpression
    | expression REM_OP expression                                                      #remExpression
    | expression MUL_OP expression                                                      #mulExpression
    | expression ADD_OP expression                                                      #addExpression
    | expression CMP_OP expression                                                      #cmpExpression
    | expression BOOL_OP expression                                                     #boolExpression
    | '(' expression ')'                                                                #parenthesizedExpression
    | IDENTIFIER                                                                        #identifierExpression
    ;
    
    
     
newStruct: 'new' IDENTIFIER '(' ')';

constant: NUMBER | STRING | BOOL | NULL;

endOfLine: ( '\r'? '\n' | '\r' | ';' );


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