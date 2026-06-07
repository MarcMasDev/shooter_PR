# Proyecto – Survival Zombie Urbano

### Marc Mas Vidal, Programació en Unity 3D

https://github.com/MarcMasDev/shooter\_PR



Juego de acción y supervivencia zombie en un entorno urbano abierto. El jugador debe sobrevivir en una ciudad infestada, gestionando recursos (vida y munición), conduciendo vehículos y enfrentándose a oleadas de enemigos. El proyecto incluye sistemas complejos de IA para zombies y peatones, tráfico autónomo, mecánicas de conducción, y un sistema de progresión tipo survival.

\---

## 1\. Instrucciones para ejecutar el proyecto

### Requisitos previos

* **Versión de Unity:** Unity Editor 6000.0.38f1 LTS.
* **Git LFS:** Recomendado, dado que el proyecto puede contener assets pesados (modelos 3D, audios).

### Pasos para abrir el proyecto

1. Clona el repositorio desde GitHub: https://github.com/MarcMasDev/shooter\\\\\\\_PR
2. Abre Unity Hub, haz clic en "Add project from disk" y selecciona la carpeta del proyecto.
3. Espera a que Unity importe los assets y resuelva las dependencias.

### Abrir la escena principal

1. En la ventana **Project**, navega hasta Assets/Scenes/.
2. Abre la escena "SampleScene".

\---

## 2\. Controles del juego

### Movimiento y Cámara

|Tecla / Input|Acción|
|-|-|
|W A S D|Mover al personaje|
|Ratón|Orientar la cámara|
|Espacio|Saltar|
|Shift|Correr|

### Combate

|Tecla / Input|Acción|
|-|-|
|Botón Izquierdo del Ratón|Disparar arma actual / Atacar cuerpo a cuerpo|
|Botón Derecho del Ratón|Apuntar|
|1, 2, 3 / rueda del ratón|Cambiar arma (Pistola, Escopeta, Cuchillo/Espada, Granadas)|

### Interacción y Vehículos

|Tecla / Input|Acción|
|-|-|
|E|Interactuar (Robar/Entrar a un coche, abrir puertas)|
|W A S D|Conducir (Acelerar, Frenar/Atrás, Girar) cuando se está en un vehículo|
|Espacio|Turbo|

\---

## 3\. Estructura de código e intercomunicación

Todo el código original se encuentra en la carpeta **\\Assets\\Scripts\\MyScripts**.



### Filosofía de diseño e Intercomunicación

* **Patrón Observer (Eventos):** El HUD, los efectos de sonido, las animaciones y los sistemas de partículas reaccionan a eventos de C#. Esto permite que los scripts de UI o Sonido no dependan directamente de la lógica interna del jugador o del enemigo (character blackboard).
* **ScriptableObjects:** Utilizados para la configuración de armas y estadísticas base de los diferentes tipos de zombies, facilitando el balanceo desde el Inspector.
* **Managers Centralizados:** Scripts como GameManager y AudioManager gestionan el flujo global de la partida y la sonorización de forma persistente.

### Scripts Principales y su Importancia

#### 1\. Sistema del Jugador

* **PlayerController.cs**: Gestiona el input del usuario y el movimiento basado en físicas o CharacterController. Se comunica con el Animator para transicionar entre estados (Idle, Caminar, Correr, Saltar).
* **WeaponSystem.cs / WeaponAiming.cs**: Controla el disparo de proyectiles (Raycast o físicas según el arma). Incluye la lógica del disparo recto (básico) y el apuntado manual (opcional). Descuenta munición e informa a la UI.
* **PlayerRiggingController.cs**: Controla el sistema de *Animation Rigging* para que la cabeza y torso miren dinámicamente hacia dónde el jugador apunta.



#### 2\. Inteligencia Artificial (Zombies y Peatones)

Ambas IAs utilizan el componente nativo `NavMeshAgent`.

* **ZombieFSM.cs**: Cerebro del enemigo. Implementa una máquina de estados:

  * *Patrol:* Deambula aleatoriamente por la ciudad buscando *waypoints*.
  * *Chase:* Al detectar al jugador por distancia, el NavMeshAgent calcula la ruta hacia él. En caso de ser disparado, el agente cambia su estado al ataque.
  * *Attack:* Al llegar al rango de cuerpo a cuerpo, se detiene el movimiento y lanza el trigger de la animación de ataque.
* **PedestrianFSM.cs**: IA de los civiles. Deambulan pacíficamente, pero incluyen un sensor de proximidad. Si detectan la capa Zombie, cambian al estado *Flee* (Huir), calculando un vector de escape en dirección opuesta al enemigo.
* **InfectionSystem.cs**: Lógica que detecta cuando un zombie alcanza a un peatón, instanciando un nuevo prefab de zombie en el lugar del peatón y destruyendo el original.

#### 3\. Sistema de Vehículos y Tráfico

* **Se usa la tecnología de la PEC 1.**
* Los coches manejados por la IA siguen un circuito de *waypoints* o *splines* por la carretera, respetando la velocidad y deteniéndose ante obstáculos.
* **DamageZone.cs**: Detecta colisiones a alta velocidad. Si el impacto es contra la capa de un zombie o peatón, invoca su muerte inmediata y activa un sistema de partículas explosivo.

#### 4\. Spawner

* **Spawner.cs**: Clase padre. Instancia zombies y peatones en diferentes puntos de la ciudad.

  * ZombieSpawner: Aumenta la frecuencia de aparición y mezcla diferentes tipos de zombies (fuertes/rápidos/lentos) a medida que el contador de bajas del jugador aumenta.
  * PedestrianSpawner: controla también su transformación a zombies.



\---

## 4\. Descripción del trabajo realizado

A continuación se detalla cómo se han cumplido los requerimientos del enunciado:

### Puntos Básicos

1. **Entorno Urbano Abierto:** Se ha creado una ciudad explorable con calles, edificios y aceras. Se ha configurado correctamente el **NavMesh** para que los agentes distingan entre zonas caminables y obstáculos. Hay algunos espacios interiores y un segundo nivel de altura (vías del tren).
2. **Sistema de Disparo Básico:** Completamente integrado usando la tecnología de la PEC 2 adaptada a la tercera persona y a su controlador. Se ha agregado un sistema de rigging para evitar que el personaje apunte siempre hacía dónde apunta el jugador.
3. **Animaciones del Jugador:** Todas las animaciones han sido implementadas de forma fácil, gracias al script Character Blackboard ya diseñada en la PEC2. Las animaciones son de mixamo. Los personajes (personaje principal y peatones) son de mixamo también. Los zombies son de la Asset Store de unity.
4. **HUD Completo:** El Canvas se muestra de forma permanente. Incluye:

   1. Kills: contador de kills totales. Incluye las que hace el jugador o las de los coches autónomos (atropellando a zombies) ya que ambas modifican la dificultad del juego.
   2. Score: los items disponen de un sistema de puntos necesarios para consumirlos. Estos se obtienen mediante el impacto (10 puntos) y la muerta de los enemigos (50 puntos). Las bajas con el coche no dan puntos.
   3. Información básica PEC 2 (escudo, vida, munición, nombre del arma).
   4. El HUD incluye también el HUD de los coches de la PEC 1. Además de un indicador para los objectos de interacción, los cuales disponen de texto personalizado por cada objeto.
5. **IA de Zombies:** Comportamiento autónomo. Pasean y transicionan a persecución y ataque al detectar al jugador.

   1. Los zombies y todos los agentes (menos el personaje) incluyen RAGDOLLS. Al morir se hacen ragdolls, lo que ofrece físicas realistas en su muerte.
6. **Animaciones de Enemigos:** Integración completa de todas las animaciones que se piden, menos de la muerte que se ha sustituido por el ragdoll, aunque el animator y la tecnologia están preparados, solo se tendría que crear la transición.
7. **IA de Peatones:** Sistema de huida. Al morir se transforman en zombies si hay algun zombie cerca comiendo su cadáver. El jugador los puede matar, no dan puntos.
8. **Tráfico y Conducción:** Coches autónomos y aparcados. El jugador puede acercarse, interactuar para robarlos y conducirlos.
9. **Sistemas de Partículas:** Las partículas que se han hecho de 0 y no són extras son:

   1. Las partículas de spawn.
   2. Las partículas de turbo.
   3. Las partículas de muzzle flash.
   4. Partícula de explosión y sangre (al chocar con el coche).
10. **Ítems Recolectables:** Pickups de vida y munición distribuidos por la ciudad. Estos disponen del sistema nuevo descrito de puntos. Todo se controla con una script padre (Purchase Interactable).
11. **Menú y Flujo de Partida:** Todos los menús comentados están implementados. El menú principal dispone de un extra de navegación, en el que el jugador puede usar un arma para manegar los menús.

### Puntos Opcionales (Extras Implementados)

1. **Nuevas Armas:** Se implementó un sistema de inventario (weapon invetroy) y cambio de armas que incluye: **Pistola, Fusil de asalto** y Bate de béisbol.
2. **Sonorización Completa:** A través del AudioManager y otros audio source independientes, se incluyeron sonidos ambientales de ciudad, gruñidos de zombies, pasos, disparos, motores de coche...
3. **Variedad de Zombies:** Creación de dos arquetipos:

   * *Normal:* Velocidad y daño estándar.
   * *Tanque/Boss:* Lento, más robusto, daño masivo y tamaño escalado.
   * Aún así, ambos disponen de una velocidad variable, pueden ser rápidos o lentos dependiendo de la dificultad controlada por el ZombieSpawner.
4. **Apuntado Manual:** Al mantener presionado el botón derecho, la cámara cambia a un FOV más cerrado (tipo *Aim Down Sights*), reduciendo la velocidad del controller y cambiando el crosshair.
5. **Jefe Final y Puerta:** Integración de zonas cerradas con una puerta cerrada con llave. El zombie tipo *Boss* (situado en la parte superior, en los railes) deja caer la "Llave del refugio" al morir, abriendo una sala de munición.
6. **Combate Cuerpo a Cuerpo:** Opción de equipar un bate. Usa la tecnología de weapon melee.
7. **Loot Dinámico:** Sistema de *Drop* probabilístico; al morir los enemigos pueden instanciar munición o curas para mantener el bucle de juego del jugador.
8. **Infección Viral:** Cuando un zombie está comiendo el cadáver de un peatón, se hace *Spawn* de un nuevo zombie.
9. **Atropellos Explosivos:** Conducir a cierta velocidad permite atroppellar a zombies y peatones El enemigo recibe daño masivo y explota (partículas de sangre/explosión).
10. **Supervivencia (Spawner Progresivo):** El `SpawnerManager` aumenta la dificultad gradualmente como ya se ha explicado anteriormente.
11. **Otros Extra:**

    * Crosshair de impacto (mediante animator y el struct Impact).
    * Menú de pausa con opciones.
    * Turbo en los coches.
    * Sistema de daño dependiendo de la zona de impacto (en la cabeza se hace más daño y en las extremidades menos).
    * Implementación de los damage numbers para las kills y los puntos.

\---

## 5\. Assets Utilizados

*Todos los assets visuales y de sonido utilizados pertenecen a licencias libres (CC0) o han sido descargados de forma legítima de la Unity Asset Store.*

*Fat Zombie (Low Poly): https://assetstore.unity.com/packages/3d/characters/humanoids/fat-zombie-low-poly-296216*

*AllSky Free - 10 Sky / Skybox Set: https://assetstore.unity.com/packages/2d/textures-materials/sky/allsky-free-10-sky-skybox-set-146014*

*Game Makers Sound Effects Kit Part 1: https://assetstore.unity.com/packages/audio/sound-fx/game-makers-sound-effects-kit-part-1-220084*

*Damage Sounds (Male) - NPC/Player Audio Pack: https://assetstore.unity.com/packages/audio/sound-fx/voices/damage-sounds-male-npc-player-audio-pack-285385*

*GUI - The Stone: https://assetstore.unity.com/packages/2d/gui/gui-the-stone-116852*

*Damage Numbers Pro: https://assetstore.unity.com/packages/2d/gui/damage-numbers-pro-186447*

*Blood Gush: https://assetstore.unity.com/packages/vfx/particles/blood-gush-73426*

*Infected Zombies Bundle: https://assetstore.unity.com/packages/3d/characters/humanoids/infected-zombies-bundle-232768*

*NaughtyAttributes: https://assetstore.unity.com/packages/tools/utilities/naughtyattributes-129996*

*FPS Engine (solo sonidos y algunos VFX): https://assetstore.unity.com/packages/templates/systems/fps-engine-218594*

*Zombie Voice Audio Pack - Free: https://assetstore.unity.com/packages/audio/sound-fx/creatures/zombie-voice-audio-pack-free-196645*

*Zombie Sound Pack - Free Version: https://assetstore.unity.com/packages/audio/sound-fx/zombie-sound-pack-free-version-124430*

*War FX (solo para el impacto): https://assetstore.unity.com/packages/vfx/particles/war-fx-5669*

*Crosshairs - Occa Software: https://assetstore.unity.com/packages/2d/gui/icons/crosshairs-216732*

*Baseball Bats – Pack: https://assetstore.unity.com/packages/3d/props/weapons/baseball-bats-pack-102171*

*FPS AKM - Model \& Textures: https://assetstore.unity.com/packages/3d/fps-akm-model-textures-63654*

*Pistol 92: https://assetstore.unity.com/packages/p/pistol-92-175490*

*Starter Assets - ThirdPerson | URP*

*1900s Industrial Environment: https://assetstore.unity.com/packages/p/1900s-industrial-environment-252380*

*Assets (audio) de Freesound*

*Assets (animaciones y modelos) de Mixamo*

