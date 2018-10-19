%error-verbose
%{

#include "stdio.h"
#include "stdlib.h"
#include "string.h"

extern FILE *yyin;
extern char *yytext;

extern int column;
extern int line;
extern int prev_column;
extern int prev_line;


char* CopyStr(const char*);
typedef struct Content Content;
typedef enum CtType
{
    CtError,
    CtRoot,
    CtStatementGroup,
    CtStatementEmpty,
    CtStatementExp,
    CtStatementRetExp,
    CtStatementRetEmpty,
    CtStatementLoop,
    CtExpFuncExec,
    CtExpArray,
    CtExpIndex,
    CtExpAssign,
    CtExpNegative,
    CtExpNot,
    CtExpLogic,
    CtExpMul,
    CtExpAdd,
    CtExpCmp,
    CtExpIf,
    CtParamsActual,
    CtParamsFormal,
    CtFuncDef,
    CtLiteralInt,
    CtLiteralFloat,
    CtLiteralChar,
    CtIdentifier
} CtType;
const char* GetDisc(CtType type)
{
    switch(type)
    {
        case CtRoot: return "Root";
        case CtStatementGroup: return "StatementGroup";
        case CtStatementEmpty: return "StatementEmpty";
        case CtStatementExp: return "StatementExp";
        case CtStatementRetExp: return "StatementRetExp";
        case CtStatementRetEmpty: return "StatementRetEmpty";
        case CtStatementLoop: return "StatementLoop";
        case CtExpFuncExec: return "ExpFuncExec";
        case CtExpArray: return "ExpArray";
        case CtExpIndex: return "ExpIndex";
        case CtExpAssign: return "ExpAssign";
        case CtExpNegative: return "ExpNegative";
        case CtExpNot: return "ExpNot";
        case CtExpLogic: return "ExpLogic";
        case CtExpMul: return "ExpMul";
        case CtExpAdd: return "ExpAdd";
        case CtExpCmp: return "ExpCmp";
        case CtExpIf: return "ExpIf";
        case CtParamsActual: return "ParamsActual";
        case CtParamsFormal: return "ParamsFormal";
        case CtFuncDef: return "FuncDef";
        case CtIdentifier: return "Identifier";
        case CtLiteralInt: return "Literal.Int";
        case CtLiteralFloat: return "Literal.Float";
        case CtLiteralChar: return "Literal.Char";
        default: return "Error!";
    }
}
struct Content {
    CtType type;
    char* value;        // the description of the content it self.
    int line;           // line number of this content.
    int column;         // column number of this content.
    Content* next;      // nodes on the same level.
    Content* sub;       // nodes on the level below.
};
Content* NewContent(CtType type)
{
    Content* c = (Content*)malloc(sizeof(Content));
    memset(c, 0, sizeof(Content));
    c->type = type;
    c->line = prev_line;
    c->column = prev_column;
    return c;
}
char* StrConcat(const char* a, const char* b)
{
    char* t = (char*)malloc(strlen(a) + strlen(b) + 1);
    strcpy(t, a);
    strcpy(t + strlen(a), b);
    // printf("merge [%s] [%s] -[%s]\n", a, b, t);
    return t;
}
Content* Ct(void* i) { return (Content*) i; }
#define ForSubs(i,x) for(Content* i=(x->sub); i; i=i->next)

static Content* root = NULL;

%}


///////////////////////////////////////////////////////////////////////////////


%union {
    char*  value;
    void*  ptr;
};
%token <value>
    INTEGER FLOAT CHAR RETURN LOOP
    LIN_OP MUL_OP CMP_OP OP_ASSIGN OP_NOT OP_LOGIC
    IDENTIFIER
%type  <ptr>
    statement_group statement exp
    actual_params formal_params
    function_def
    literal

%left ';'
%left LOOP
%left ')'
%right OP_ASSIGN
%right '?' ':'
%left OP_LOGIC
%left CMP_OP
%left LIN_OP
%left MUL_OP
%left '.'
%right UMINUS
%right OP_NOT
%left '('
%left '[' ']'

%%
input: statement_group {
        root = $1;
    }

statement_group : statement {
        $$ = NewContent(CtStatementGroup);
        Ct($$)->sub = $1;
    }

statement : {
        $$ = NULL;
    }
    | exp ';' statement {
        $$ = NewContent(CtStatementExp);
        Ct($$)->sub = $1;   // $exp
        Ct($$)->next = $3;  // $statement...
    }
    
    | RETURN exp ';' statement {
        $$ = NewContent(CtStatementRetExp);
        Ct($$)->sub = $2;   // $exp
        Ct($$)->next = $4;  // $statement...
    }
    
    | RETURN '.' ';' statement {
        $$ = NewContent(CtStatementRetEmpty);
        Ct($$)->next = $4; // $statement...
    }
    
    | exp LOOP exp ';' statement {
        $$ = NewContent(CtStatementLoop);
        Ct($$)->sub = $1;   // $condition
        Ct($1)->next = $3;  // $exec
        Ct($$)->next = $5;  // $statements...
    }
    
    | error ';' { $$ = NewContent(CtError); }

exp : 
      IDENTIFIER {
        $$ = NewContent(CtIdentifier);
        Ct($$)->value = $1;
    }
    
    | function_def {
        $$ = $1;
    }
    
    | exp '(' actual_params ')' {
        // this takes expression,
        // because return value of a function execution may be a function.
        $$ = NewContent(CtExpFuncExec);
        Ct($$)->sub = $1;       // $function
        Ct($1)->next = $3;      // $actual_params
    }
    
    | '{' exp '}' '[' exp ']' {
        $$ = NewContent(CtExpArray);
        Ct($$)->sub = $2;       // init value
        Ct($2)->next = $5;      // size
    }
    
    | exp '.' exp {
        $$ = NewContent(CtExpIndex);
        Ct($$)->sub = $1;       // array
        Ct($1)->next = $3;      // index
    }
    
    | literal {
        $$ = $1;
    }
    
    | exp OP_ASSIGN exp {
        $$ = NewContent(CtExpAssign);
        Ct($$)->sub = $1;       // $dst
        Ct($1)->next = $3;      // $src
        Ct($$)->value = $2;     // $type
    }
    
    | OP_NOT exp {
        $$ = NewContent(CtExpNot);
        Ct($$)->sub = $2;       // $exp
    }
    
    | LIN_OP exp %prec UMINUS {
        $$ = NewContent(CtExpNegative);
        Ct($$)->sub = $2;       // $exp
    }
    
    | exp OP_LOGIC exp {
        $$ = NewContent(CtExpLogic);
        Ct($$)->sub = $1;
        Ct($$)->sub->next = $3;
        Ct($$)->value = $2;
    }
    
    | exp MUL_OP exp {
        $$ = NewContent(CtExpMul);
        Ct($$)->sub = $1;       // &op
        Ct($1)->next = $3;      // $left
        Ct($$)->value = $2;     // $right
    }
    
    | exp LIN_OP exp {
        $$ = NewContent(CtExpAdd);
        Ct($$)->sub = $1;       // &op
        Ct($1)->next = $3;      // $left
        Ct($$)->value = $2;     // $right
    }
    
    | exp CMP_OP exp {
        $$ = NewContent(CtExpCmp);
        Ct($$)->sub = $1;           // &op
        Ct($$)->sub->next = $3;     // $left
        Ct($$)->value = $2;         // $right
    }
    
    | exp '?' exp ':' exp {
        $$ = NewContent(CtExpIf);
        Ct($$)->sub = $1;                   // $condition
        Ct($$)->sub->next = $3;             // $then
        Ct($$)->sub->next->next = $5;       // $else
    }
    
    | '(' exp ')' {
        $$ = $2;
    }
    
actual_params : {
        $$ = NULL;
    }
    | exp {
        $$ = $1;
    }
    | exp ',' actual_params {
        $$ = $1;
        Ct($$)->next = $3;  // $params...
    }

formal_params : {
        $$ = NULL;
    }
    
    | IDENTIFIER {
        $$ = NewContent(CtIdentifier);
        Ct($$)->value = $1;
    }
    
    | IDENTIFIER ',' formal_params {
        $$ = NewContent(CtIdentifier);
        Ct($$)->next = $3;                      // $params...
        Ct($$)->value = $1;
    }
    
function_def :
    '[' formal_params ']' '{' statement_group '}' {
        $$ = NewContent(CtFuncDef);
        if($5) Ct($$)->sub = $5;   // $statement
        else Ct($$)->sub = NewContent(CtStatementEmpty);
        Ct($$)->sub->next = $2;    // $params
    }
    
    | '{' statement_group '}' {
        $$ = NewContent(CtFuncDef);
        if($2) Ct($$)->sub = $2;   // $statement
        else Ct($$)->sub = NewContent(CtStatementEmpty);
    }
    
    | error '}' { $$ = NewContent(CtError); }

literal :
    INTEGER {
        $$ = NewContent(CtLiteralInt);
        Ct($$)->value = $1;
    }
    | FLOAT {
        $$ = NewContent(CtLiteralFloat);
        Ct($$)->value = $1;
    }
    | CHAR {
        $$ = NewContent(CtLiteralChar);
        Ct($$)->value = $1;
    }
%%

///////////////////////////////////////////////////////////////////////////////

void Indent(int x) { for(int i=0; i<x; i++) printf("  "); }
void DFSOut(Content* x, int d, int cont)
{
    Indent(d); printf("{\n");
    Indent(d+1); printf("\"type\": \"%s\",\n", GetDisc(x->type));
    Indent(d+1); printf("\"value\": \"%s\",\n", x->value ? x->value : "");
    Indent(d+1); printf("\"line\": %d,\n", x->line);
    Indent(d+1); printf("\"column\": %d,\n", x->column);
    if(x->sub)
    {
        Indent(d+1); printf("\"subs\":\n");
        Indent(d+1); printf("[\n");
        ForSubs(e, x)
        {
            int cc = 0;
            if(e->next) cc = 1;
            DFSOut(e, d+2, cc);
        }
        printf("\n");
        Indent(d+1); printf("]");
    }
    else
    {
        Indent(d+1); printf("\"subs\": []");
    }
    printf("\n");
    Indent(d);
    if(cont) printf("},\n");
    else printf("}");
}


///////////////////////////////////////////////////////////////////////////////

int valid = 1;

int main(int agrc, char **argv)
{
    freopen(argv[1], "r", stdin);
    yyparse();
    if(valid) DFSOut(root, 0, 0);
    return !valid;
}

int yyerror(const char *s)
{
    valid = 0;
    printf("%s\n",s);
    printf("line %d column %d\n", line, column);
}
