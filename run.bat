flex ./exp.l

bison -d -v ./exp.y

echo off

D:\MinGW\bin\gcc -std=c99 -g -o ./bin/molc.exe lex.yy.c exp.tab.c -L"D:\GnuWin32\lib" -I"D:\GnuWin32\include" -lfl -ly

del exp.tab.c
del exp.tab.h
del lex.yy.c

dotnet build

echo on
rem .\bin\molc.exe
.\bin\vm.exe ./x.mol
