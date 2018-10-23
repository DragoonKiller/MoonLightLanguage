## 编译原理课程实验
#### Moonlight Programming Language

#### 定义
1. 值(value)/对象(object)
   语言内置的基本类型有三种, 分别是**整数**,**浮点数**和**字符**. 另外有**数组**类型和**函数**类型. 这些的类型所对应的实例被称为**值**或**对象**.
   * 形如 `1`, `100000` 的数字是整数, 大小是32位.
   * 形如 `1.2` , `1.` 的数字是浮点数, 大小是64位. 注意不支持类似于 `.01` 的语法.
   * 使用单引号括起来的一个字符是字符, 通常是 16 位 Unicode, 例如 `'a'`.
   * 形如 `{数字元素的初始化内容}[数组长度]` 的对象是数组, 例如 `{2}[5]` 表示长度为 5 , 全部元素初始化为 2 的数组. 数组中的所有元素均为引用, 它们可以指向到不同类型的值(对象).
   * 形如 `[参数列表]{函数体}` 或 `{函数体}` 的对象是函数. 其它细节会在 5.函数 中单独描述.
   * 空值 `None`. 它不能参与任何运算. 你不能显式地写出值 `None`, 但是你可以使用函数 `{ =>.; }` 的返回值来获取空值.
    
1. 标识符(identifier)  
   合法的标识符由大写字母, 小写字母或下划线开头, 后接若干个(可以没有)大写字母, 小写字母, 数字或下划线组成, 例如 `Identifier`, `func`, `__A`, `Add125`.
   在该语言中, 所有的标识符均为指向某一个值(对象)的引用, 并且允许链式引用.
   
1. 表达式(expression)  
   表达式是计算式. 任何表达式**一定**会有一个返回值.
   * 运算表达式, 包括加 `a + b`, 减 `a - b`, 乘 `a * b`, 除 `a / b`, 取模 `a % b`, 取负 `-a`.
   * 逻辑表达式, 包括与 `a && b`, 或 `a || b`, 非 `!a`.
   * 比较表达式, 包括大于 `a > b`, 小于 `a < b`, 不等于 `a != b`, 等于 `a == b`, 小于等于 `a <= b`, 大于等于 `a >= b`.
   * 取数组元素表达式, 形如 `a.b` , 该表达式返回数组 `a` 中的下标为 `b` 的元素, 下标从 0 开始.
   * 赋值表达式, 形如 `a = b`. 当出现赋值表达式时, 如果 `a` 是一个已经存在于上下文中的引用, 那么会计算表达式 `b` 值并赋给该引用; 如果 `a` 不存在于上下文中, 符号 `a` 会首先被加入上下文并拥有值 `None`, 再计算表达式 `b` 并将其值赋给表达式 `a`.
   * 条件表达式. 形如 `a ? b : c` 的表达式是条件表达式. 
   * 函数执行表达式. 形如 `Identifier(parameters)` 的表达式是函数执行表达式, 它会执行 `Identifier` 中引用的函数对象. 注意由于函数定义本身也是一个对象, 它可以直接被执行, 此时语法形如 `[a]{ => a + 1; } (2)`.
   * 值表达式, 所有直接写出了值(value)的表达式都是值表达式, 例如 `125`. 函数本身也是值, 因而形如 `[a]{ => a; }` 的函数定义也是值表达式.


1. 陈述(statement)  
   陈述语句是程序的基本执行单元. 整个程序由若干条陈述语句组成, 每条陈述语句必定由分号结尾.
   * 空陈述, 是只带一个分号 `;` 的陈述.
   * 表达式陈述, 是由一个表达式和一个分号组成的陈述, 例如 `a + 1;`, `a = 3 + b`;
   * 循环陈述, 即循环语句, 是形如 `a ::: b;` 的陈述, 其中 `a` 应当是一个返回整数类型的表达式, 若 `a` 为 0 则结束该陈述语句的执行, 否则计算表达式 `b`.
   * 返回陈述, 形如 `=> a;` 以及 `=> .;` 的陈述是返回陈述. 前者会跳出当前函数的执行(或结束主程序)并返回一个值; 后者跳出函数执行并返回值 `None`.
  
1. 函数(function)  
   函数是该语言的核心. 一个函数由参数表, 捕获表和函数体构成, 形如 `[参数表]{函数体}`. 函数体是若干陈述的顺序排列. 参数表是若干标识符的顺序排列. 函数在其被定义时会自动**捕获**其执行代码中需要的上下文, 并且所有捕获均为引用捕获. 所有捕获仍然会以引用形式保存在函数定义所返回的函数对象中.  
   函数结束其执行或返回时, 会释放其参数表. 函数对象不再被引用时, 会释放其捕获表(及该函数对象的所有相关内容). 但是, 如果这些内容被其它函数对象捕获, 它们则会由于其它函数的存在而被保留.

1. 垃圾回收(Garbage Collection)
   该语言不需要使用者显示管理内存. 全局(最顶层)上下文符号表会被保留至程序运行结束, 但是在该符号被创建以前不能被使用. 申请和释放内存均由编译器或运行时自动管理, 在某一时刻, 对于一些不可能被使用到的内存, 程序应当有能力将其及时回收.

1. 内置函数(Builtin Function)  
   以下函数在程序执行以前被加入符号表, 并完成一些内置功能:
   * `ReadInt` 函数从控制台读入一个整数.
   * `ReadChar` 函数从控制台读取一个字符.
   * `ReadFloat` 函数从控制台读取一个浮点数.
   * `ToChar` 函数将整数转换为字符. 高位截断. 注意你不能直接将浮点数转换为字符.
   * `ToInt` 函数将字符或浮点数转换为整数. 浮点转换整数时下取整.
   * `ToFloat` 函数将整数转换为64位浮点数. 注意你不能直接将字符转换为浮点数.
   * `Write` 函数将参数内容输出到控制台.
   

#### 参考实现
参考实现是C#写的解释器. 源代码存在这个repo里.
