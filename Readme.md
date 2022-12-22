# Dungeons and Bugs

Dungeons and Bugs es un motor de juegos con historia del tipo Dungeons and Dragons. Permite juegos multijugador en línea, con modo cooperativo o player vs player. Posee su propio  lenguaje de programación para crear aventuras, heroes, poderes y mucho más. ¿Estás listo para vivir aventuras y crear las tuyas propias?

## ¿Cómo jugar?

Dungeons and Bugs tiene un cliente gráfico para jugar en Telegram llamada Code Dungeon, para acceder a ella entre a [@code_dungeon_bot](https://t.me/code_dungeon_bot) (El servidor debe estar corriendo para poder usarlo).   

Para crear una nueva aventura use el comando `/new_adventure`, seguidamente recibira una mensaje donde aparezcan todas las aventuras disponibles, marque en la que quiera jugar, el tipo de juego dependerá de la  aventura seleccionada, y puede variar mucho.  

Una vez que selecciones una aventura recibiras un mensaje con el número de la partida, este número se lo puedes compartir a tus amigos para que se unan a la partida, la  cantidad de jugadores que pueden jugar depende de la aventura seleccionada. Para que tus amigos se unan a la partida usen el comando `/join 1E6F12B`, donde el último número exadecimal es el número de tu partida. Una vez estén todos adentro puedes usar `/start_adventure` para iniciar  la aventura.  

Ya estás dentro de la aventura, deberás seguidamente elegir tu héroe, una vez que todos estén elegidos empezará la historia.  

El juego se jugará por turnos, y cada personaje podrá usar acciones predeterminadas en el inicio, es probable que  durante el transcurso de la aventura obtengan nuevas acciones, y además podrás programar tus propias acciones, para ver las acciones disponibles podrás usar `/actions`. Durante el transcurso de la aventura podrán morir jugadores, luchar con monstruos, resolver puzles y muchas cosas más.


## Creando tu propia aventura:

Las avneturas estarán guardadas en archivos `JSON` en la carpeta `adventures`. Para crear aventuras nuevas deberás seguir el formato siguiente:

```json
{
    "name":"Nombre extenso de la aventura",
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
            "beg_story":"Historia del inicio del stage",
            "beg_pic":"imagen_de_fin_de_stage.jpg",
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
            ,"end_story":"Historia del fin del stage",
            "end_pic":"imagen_de_fin_de_stage.png"
        }
    ]
}
```