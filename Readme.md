# Dungeons and Bugs

Dungeons and Bugs es un motor de juegos con historia del tipo Dungeons and Dragons. Permite juegos multijugador en línea, con modo cooperativo o player vs player. Posee su propio  lenguaje de programación para crear aventuras, heroes, poderes y mucho más. ¿Estás listo para vivir aventuras y crear las tuyas propias?

---
## ¿Cómo jugar?

Dungeons and Bugs tiene un cliente gráfico para jugar en Telegram llamada Code Dungeon, para acceder a ella entre a [@code_dungeon_bot](https://t.me/code_dungeon_bot) (El servidor debe estar corriendo para poder usarlo).   

Para crear una nueva aventura use el comando `/new_adventure`, seguidamente recibira una mensaje donde aparezcan todas las aventuras disponibles, marque en la que quiera jugar, el tipo de juego dependerá de la  aventura seleccionada, y puede variar mucho.  

Una vez que selecciones una aventura recibiras un mensaje con el número de la partida, este número se lo puedes compartir a tus amigos para que se unan a la partida, la  cantidad de jugadores que pueden jugar depende de la aventura seleccionada. Para que tus amigos se unan a la partida usen el comando `/join 1E6F12B`, donde el último número exadecimal es el número de tu partida. Una vez estén todos adentro puedes usar `/start_adventure` para iniciar  la aventura.  

Ya estás dentro de la aventura, deberás seguidamente elegir tu héroe, una vez que todos estén elegidos empezará la historia.  

El juego se jugará por turnos, y cada personaje podrá usar acciones predeterminadas en el inicio, es probable que  durante el transcurso de la aventura obtengan nuevas acciones, y además podrás programar tus propias acciones, para ver las acciones disponibles podrás usar `/actions`. Durante el transcurso de la aventura podrán morir jugadores, luchar con monstruos, resolver puzles y muchas cosas más.

---
## Creando tu propia aventura:

Las avnenturas estarán guardadas en archivos `JSON` en la carpeta `adventures`. Para crear aventuras nuevas se debera usar el formato siguiente:

```json
{
    "name":"Nombre completo de la aventura",
    "token": "mini_name",
    "heroes":[
        {
            "token": "hero1",
            "name": "Name_hero1",
            "life": 100,
            "strength": 100,
            "agility": 70,
            "mana": 20,
            "desc": "Descripción del heroe1",
            "img": "foto_del_heroe_1.jpg",
            "powers": [
                ["nombre_del_poder","Descripción del poder",
                "Código en C+- del poder"
                ]
            ]
        },
        {
            "token": "hero2",
            "name": "Name_hero2",
            "life": 100,
            "strength": 100,
            "agility": 70,
            "mana": 20,
            "desc": "Descripción del heroe2",
            "img": "foto_del_heroe_2.jpg",
            "powers": [
                ["nombre_del_poder","Descripción del poder",
                "Código en C+- del poder"
                ]
            ]
        }
    ],
    "story":[
        {
            "villain": {"life":200,"strength":200,"agility":30,"mana":50},
            "beg_code":"Codigo en C+- que se correrá al inicio del stage"
            ,"events": {
                "hero1":[
                    "Codigo en C+- de posible encuentro de hero1"
                ],
                "hero2":[
                    "Codigo en C+- de posible encuentro de hero2"
                ]
            },        
            "end_code":"código que se correrá al fin del stage"
        }
    ]
}
```
Como se observa al inicio del archivo deberan crearse todos los posibles heroes, de estos se debe dar una descripcion del heroe, sus estadisticas y una imagen del mismo.
> ⚠ Los links a las imagenes de jugadores, stages, o imagenes usadas en C+- deben ser links en linea, se recomienda subir las imagenes a [imgur](imgur.com).

Las aventuras se divididen en distintos stages, donde cada stage es un objetivo adistinto a vencer, el stage dara por finalizado cuando `Villain.life` llegue a cero. Durante los stages los heroes jugaran por turnos para vencer el objetivo del stage actual. Al principio cada turno se ejecutara un encuentro, el cual sera un pedazo de codigo en C+-. Lo que  pase en cada encuentro puede variar dependiendo de la historia y de el codigo implementado en ellos, uno de los encuentros posibles puede ser que el jugador obtenga un poder nuevo, recupere vida, o simplemente reciba algun mensaje. Los encuentros se elegirtan aleatoriamente entre todos los posibles encuentros del stage actual. Tanto al iniciar un stage como el terminar se correra un codigo en C+-.

---
## C+- un lenguaje para gobernarlos a todos.
El lenguaje para progrmar poderes, encuentros y la historia en general es llamado C+-. Es un lenguaje Turing completo y su nombre surge debido a su parecido con el lenguaje C++.  
### Componentes principales del lenguaje:  

Declaracion de numeros enteros:   
 ```c++ 
    int n=16;
 ```
 Declaracion de cadenas de caracteres:   
 ```c++ 
    str s="Hello there!";
 ```
 
 Notificar a todos los jugadores en una partida:   
 ```c++ 
    notify("General Kenobi!");
 ```
 
 Notificar a todos los jugadores en una partida con una imagen y un subtexto:   
 ```c++ 
    notipic("https://disruptivo.tv/wp-content/uploads/2015/12/Untitled.png","Not the droids you are looking for");
 ```
 Condicional if:
  ```c++ 
    if(5>3){
        notify("It is obvius");
    }
 ```
 
 Condicional if-else:
  ```c++ 
    if(534134>3488353){
        notify("It is obvius");
    }else{
        notify("Not so obvius");    
    }
 ```
 Ciclo while:
 ```c++
    int cnt=5;
    while(cnt!=0){
        notify(cnt);
        cnt=cnt-5;
    }
 ```
 Sleep:
 ```c++
    sleep(1000);
 ```

 Añadir nuevos poderes a los jugadores  o quitárselos

 ```c++
    add_power("Gandalf","scream","Grita al enemigo para que no pueda pasar",'notify("You shall not pass!");');
    del_power("Gandalf","scream");
 ```

 El script siempre va a poseer variables globales tales que van a representar el estado acutal del juego, tales como `Villain.life`, ademas se le pueden adicionar mas variables al estado del juego para luego ser usadas en turnos posteriores. Con estas estructuras basicas del lenguaje se pueden crear operaciones mas complicadas: 
 ```c++
    while( Gandalf.life!=0 ){
        if( Villain.life>0 &  Gandalf.strength>50 ){
            notify("Gandalf says: You shall not pass!");
        }else{
            notify("Gandalf and the balrog fall into the abyss while fighting");
            sleep(1000);
            Gandalf.life=Gandalf.life-(Villain.strength-Gandlaf.strength/10);
            Villain.life=Villain.life-random();
            notify("Gandalf's life is now"+ Gandalf.life +" and the Balrog's life is"+Villain.life);
        }
    }
 ```

 ---
