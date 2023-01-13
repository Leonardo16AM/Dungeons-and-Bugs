## C+- un lenguaje para gobernarlos a todos.
C+- es el lenguaje para programar poderes, juegos, y mucho mas. Es un lenguaje Turing completo y su nombre surge debido a su parecido con el lenguaje C++. El lenguaje funciona por celdas (como los cuadernos Jupyter), cuando se ejecute un juego y se ejecuten por separado scripts en C+-, estos scripts podran comunicarse los unos con los otros y acceder a las variables globales, funciones y otros tipos de declaraciones.
## Componentes principales del lenguaje:  

- Declaración de numeros enteros:   
    ```c++ 
        int n=16;
    ```
-  Declaración de cadenas de caracteres:   
    ```c++ 
        str s="Hello there!";
    ```
 
-  Notificar a todos los jugadores en una partida:   
    ```c++ 
        notify("General Kenobi!");
    ```
 
-  Notificar a todos los jugadores en una partida con una imagen y un subtexto:   
    ```c++ 
        notipic("https://disruptivo.tv/wp-content/uploads/2015/12/Untitled.png","Not the droids you are looking for");
    ```
 - Condicional if:
    ```c++ 
        if(5>3){
            notify("It is obvius");
        }
    ```
 
- Condicional if-else:
    ```c++ 
        if(534134>3488353){
            notify("It is obvius");
        }else{
            notify("Not so obvius");    
        }
    ```
- Ciclo while:
    ```c++
        int cnt=5;
        while(cnt!=0){
            notify(cnt);
            cnt=cnt-5;
        }
    ```
- Ciclo for:
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

 - Terminar turno luego de que se ejecuteel codigo:
    ```c++
        end_turn();
    ```

 - Añadir nuevos poderes a los jugadores  o quitárselos

    ```c++
        add_power("Gandalf","scream","Grita al enemigo para que no pueda pasar",'notify("You shall not pass!");');
        del_power("Gandalf","scream");
    ```
 - Numeros aleatorios
     ```c++
        int r=random(500);
    ```
- Cambiar el orden de los turnos
     ```c++
        change_turn_order();
    ```

- Ver si una variable o funcion existe en el scope actual
     ```c++
        if(exist("n")){
            notify("N exist");
        }
    ```
- Operaciones algebraicas
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
- Definicion de funciones
     ```c++
        def int fib(int n){
            if(n==1|n==0)return 1;
            return fib(n-1)+fib(n-2);
        }
        def void say_hello(str s){
            notify("Hello"+s);
        }
        def str gk(str s){
            return s+" There!";
        }

    ```
- Uso de funciones:
    ```c++
        notify(fib(5));
        /*Output: 8*/
        say_hello("Joe");
        /*Output: Hello Joe*/
        notify(gk("Hello"););
        /*Output: Hello There!*/
    ```




 El script siempre va a poseer variables globales tales que van a representar el estado acutal del juego, tales como `Villain.life`. Con estas estructuras basicas del lenguaje se pueden crear operaciones mas complicadas, y crear un juego por turnos completamente funcional.

 ---

