%{
#include "exp.tab.h"
#include "stdio.h"
#include "string.h"
typedef union {
    char* value;
    void* ptr;
} YYLVAL;
char* CopyStr(const char*);
#define YYSTYPE YYLVAL

// #define TOKENOUT

int prev_line = 1;
int prev_column = 0;
int line = 1;
int column = 0;


void Advance(const char* s)
{
    int lx = strlen(s);
    prev_column = column;
    prev_line = line;
    for(int i=0; i<lx; i++)
    {
        if(s[i] == '\n')
        {
            column = 0;
            line += 1;
        }
        else
        {
            column += 1;
        }
        
    }
    
    // printf("move to %d %d\n", line, column);
}

%}

%%

=> {
    #ifdef TOKENOUT
    printf("get return : %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return RETURN;
}

::: {
    #ifdef TOKENOUT
    printf("get loop : %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return LOOP;
}

[#][^\n]*\n {
    #ifdef TOKENOUT
    printf("get comment : %s\n", yytext);
    #endif
    Advance(yytext);
}

[0-9]+ {
    #ifdef TOKENOUT
    printf("get int : %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return INTEGER;
}

[0-9]\.[0-9]* {
    #ifdef TOKENOUT
    printf("get float : %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return FLOAT;
}

[a-zA-Z_][a-zA-Z0-9_]* {
    #ifdef TOKENOUT
    printf("get identifier : %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return IDENTIFIER;
}

'.' {
    #ifdef TOKENOUT
    printf("get char : %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext + 1);
    yylval.value[1] = '\0';
    Advance(yytext);
    return CHAR;
}

(&&)|(\|\|) {
    #ifdef TOKENOUT
    printf("get logic operator %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return OP_LOGIC;
}

[!] {
    #ifdef TOKENOUT
    printf("get not operator %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return OP_NOT;
}

[+\-*/%]?[=] {
    #ifdef TOKENOUT
    printf("get assign operator %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return OP_ASSIGN;
}

[+\-] {
    #ifdef TOKENOUT
    printf("get linear operator %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return LIN_OP;
}

[><=!][=] {
    #ifdef TOKENOUT
    printf("get compare operator %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return CMP_OP;
}

[><] {
    #ifdef TOKENOUT
    printf("get compare operator %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return CMP_OP;
}

[*/%] {
    #ifdef TOKENOUT
    printf("get mult operator %s\n", yytext);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return MUL_OP;
}

[ \t\r\n]+ {
    #ifdef TOKENOUT
    // printf("get stand alone space.\n");
    #endif
    Advance(yytext);
}

. {
    #ifdef TOKENOUT
    printf("get others : %c\n", yytext[0]);
    #endif
    yylval.value = CopyStr(yytext);
    Advance(yytext);
    return yytext[0];
}
%%

int yywrap()
{
    
    return 1;
}

char* CopyStr(const char* from) {
    char* res = (char*) malloc(strlen(from) + 1);
    strcpy(res, from);
    return res;
}
