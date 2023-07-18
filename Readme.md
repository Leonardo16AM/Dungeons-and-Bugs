<div align="center">
    <img src="images/D%26B-14.jpg" alt="Dungeons and Bugs" title="Dungeons and Bugs Logo" width="500" height="500" /> 
</div>

# Dungeons & Bugs
Welcome to Dungeons & Bugs, a unique game engine tailored to deliver dynamic turn-based storytelling experiences. This engine supports both online multiplayer modes, including cooperative and player-versus-player, as well as the traditional solo campaign. 

To keep the dynamics vibrant, we have integrated a real-time chat feature for multiplayer modes. This allows seamless team communication, thereby enabling effective strategy planning and execution.

One of the most exciting features of Dungeons & Bugs is its proprietary programming language, referred to as 'C+-' (C more or less). This intuitive language allows you to create adventures, characters, powers, and much more, paving the way for you to unleash your creativity and design your own gaming universes. C+- also forms the foundation for executable actions in each turn.

Are you ready to embark on an exciting journey and create your own adventures?

---

## Installation:

To ensure smooth operation, please install `dotnet v6` or a later version. The following package is also required:

```c# 
dotnet add package Newtonsoft.Json --version 13.0.1
``` 
To run the program, execute the following command in the root directory:
```
dotnet run
```
If you are running the code from a release, ensure you have a folder named `/adventures` containing the `JSON` files of the games you wish to execute.  

---

## How to Play?    

Dungeons and Bugs offers a graphical client to play on Telegram called Code Dungeon. To access this, visit [@code_dungeon_bot](https://t.me/code_dungeon_bot). For more detailed instructions, please refer to the [Gameplay Manual](). 

---

## Crafting Your Own Adventure:

The games are stored in `JSON` files located in the `/adventures` directory. You will need to code these using C+-. For further guidance, please refer to the [Programming Manual]().  

### Example of C+- Code:
```c++
    def int fib(int n){
        if(n==1|n==0)return 1;
        return fib(n-1)+fib(n-2);
    }
    notify("Fibonacci sequence:");
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
This brief code snippet showcases how you can define functions, utilize control structures, and perform operations in our C+- language. Immerse yourself in the vast possibilities with Dungeons & Bugs, and let your creativity run wild!
