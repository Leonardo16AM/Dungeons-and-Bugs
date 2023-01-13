<div align="center">
    <img src="images/D%26B-14.jpg" alt="Dungeons and Bugs" title="Dungeons and Bugs Logo" width="500" height="500" /> 
</div>



Dungeons & Bugs
Dungeons and Bugs es un motor de juegos con historias con dinámicas de
turnos. Soporta multijugador en línea, con modo cooperativo o player vs
player, y la clásica campaña en solitario. Para que no se pierda la
dinámica, dispone de un chat interno en modo multijugador, asi en todo
momento, todos saben lo que piensan los del equipo y de esa forma, trazar
una estrategia.

Posee su propio lenguaje de programación, para crear aventuras, héroes,
poderes y mucho más, de forma intuitiva, llamado C mas o menos (C+-) en
la que puedes lanzarte y liberar tu creatividad para crear tus propios
universos, ademas que constituye la base de las acciones ejecutables en
cada turno.

¿Estás listo para vivir aventuras y crear las tuyas propias?

---
## Instalación:
Es necesario tener `dotnet v6` o una versión más moderna.
```c# 
dotnet add package Newtonsoft.Json --version 13.0.1
``` 
Para ejecutar el programa ejecute el siguiente comando en la carpeta principal:
```
dotnet run
```
En caso de que este corriendo el código desde un release debe tener una carpeta llamada `/adventures` con los archivos `JSON` de los juegos que desee ejecutar.  

---
## ¿Cómo jugar?    

Dungeons and Bugs posee un cliente gráfico para jugar en Telegram llamada Code Dungeon, para acceder a ella entre a [@code_dungeon_bot](https://t.me/code_dungeon_bot). Para mas información revisar el [Manual de Juego](). 

---


## Creando tu propia aventura:

Los juegos estarán guardadas en archivos `JSON` en la carpeta `/adventures`, deberá programarlos en C+-. Para más informacion revise el [Manual de Programacion]().  

### Ejemplo de codigo en C+-:
```c++
    def int fib(int n){
        if(n==1|n==0)return 1;
        return fib(n-1)+fib(n-2);
    }
    notify("Sucesion de fibonacci:");
    for(int i=0;i<7){
        notify(i+" :"+fib(i));
        i++;
        sleep(1000);
    }  
    int m=16;
    if( m!=5 & ( m==1| !m!=16 ) ){
        notify("Done");
    }
```


