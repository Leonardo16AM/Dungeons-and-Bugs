## C+-: A Language to Rule Them All

C+- is the coding language utilized to program powers, games, and much more in Dungeons & Bugs. It's a Turing-complete language that gets its name due to its resemblance to C++. The language operates by cells, similar to Jupyter notebooks. When a game is run and separate C+- scripts are executed, these scripts can communicate with each other and access global variables, functions, and other types of declarations.

## Core Components of the Language:

- Declaration of integers:
    ```c++ 
        int n=16;
    ```
-  Declaration of character strings:
    ```c++ 
        str s="Hello there!";
    ```
 
-  Notify all players in a match:
    ```c++ 
        notify("General Kenobi!");
    ```
 
-  Notify all players in a match with an image and a subtitle:
    ```c++ 
        notipic("https://disruptivo.tv/wp-content/uploads/2015/12/Untitled.png","Not the droids you are looking for");
    ```
 - Conditional 'if':
    ```c++ 
        if(5>3){
            notify("It is obvious");
        }
    ```
 
- Conditional 'if-else':
    ```c++ 
        if(534134>3488353){
            notify("It is obvious");
        }else{
            notify("Not so obvious");    
        }
    ```
- 'While' loop:
    ```c++
        int cnt=5;
        while(cnt!=0){
            notify(cnt);
            cnt=cnt-5;
        }
    ```
- 'For' loop:
    ```c++
        for(int i=0;i<5){
            notify(i);
            i++;
        }
    ```
- Sleep:
    ```c++
        sleep(1000);
    ```

 - End turn after code execution:
    ```c++
        end_turn();
    ```

 - Add or remove powers to/from players:

    ```c++
        add_power("Gandalf","scream","Scream at the enemy to prevent them from passing",'notify("You shall not pass!");');
        del_power("Gandalf","scream");
    ```
 - Random numbers:
     ```c++
        int r=random(500);
    ```
- Change the turn order:
     ```c++
        change_turn_order();
    ```

- Check if a variable or function exists in the current scope:
     ```c++
        if(exist("n")){
            notify("N exists");
        }
    ```
- Algebraic operations:
     ```c++
        int n=1;
        n++;
        n--;
        n=n+16;
        n=n+7;
        n=n*8;
        n=n/8;
        n=n%2;
    ```
- Function definitions:
     ```c++
        def int fib(int n){
            if(n==1|n==0)return 1;
            return fib(n-1)+fib(n-2);
        }
        def void say_hello(str s){
            notify("Hello "+s);
        }
        def str gk(str s){
            return s+" There!";
        }

    ```
- Using functions:
    ```c++
        notify(fib(5));
        /*Output: 8*/
        say_hello("Joe");
        /*Output: Hello Joe*/
        notify(gk("Hello"));
        /*Output: Hello There!*/
    ```

The script will always possess global variables that represent the current state of the game, such as `Villain.life`. With these basic structures of the language, you can create more complex operations and a fully functional turn-based game.

---