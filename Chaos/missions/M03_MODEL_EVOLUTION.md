# M03 – Evolución del modelo

Hasta ahora, los problemas podían parecer “de Git” o “de merge”.

A partir de este momento, los problemas empiezan a ser **de significado**.

El proyecto ha seguido evolucionando.
Distintas personas han modificado el **modelo de datos** en distintos momentos,
con distintas suposiciones y sin una decisión global.

No hay una única versión “correcta” del modelo.
Hay **restos de varias decisiones válidas**.

---

## Contexto

El modelo del sistema (`StatusResponse` y lo que lo rodea):

- Ha cambiado más de una vez
- Ha sido ampliado de distintas formas
- Se usa en varios puntos del código
- No todos los cambios asumieron lo mismo

Nada de esto es accidental.
Todo el código existente **fue escrito con intención**.

Ahora te toca trabajar con ese modelo **tal como está**.

---

## Tu objetivo

Hacer que el sistema funcione de forma coherente **con el modelo actual**, cumpliendo todas estas condiciones:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ Todas las propiedades existentes del modelo tienen sentido
- ✅ No se elimina información “porque no encaja”
- ✅ El comportamiento del sistema es explicable

No se trata de volver al modelo original.
Se trata de **asumir que el modelo ha evolucionado** y actuar en consecuencia.

---

## Reglas clave

- ❌ No elimines propiedades del modelo
- ❌ No vuelvas al modelo del baseline
- ❌ No “simplifiques” borrando información
- ❌ No preguntes a nadie qué quiso hacer

- ✅ Asume que cada cambio tenía un motivo
- ✅ Adapta el comportamiento del sistema al modelo actual
- ✅ Toma decisiones explícitas en el código

---

## Qué puede estar ocurriendo

Algunas situaciones posibles (no exhaustivas):

- Propiedades nuevas que no se usan en todos los sitios
- Campos que significan cosas distintas según el contexto
- Valores por defecto inconsistentes
- Endpoints que asumen una versión antigua del modelo
- Código que “funciona”, pero devuelve información confusa

No todas están mal.
Pero **todas deben encajar juntas**.

---

## Cómo afrontar esta misión

1. Lee el modelo actual con atención
2. Busca dónde se usa
3. Observa qué asume cada parte del código
4. Decide:
   - qué representa ahora el modelo
   - qué información es obligatoria
   - qué comportamiento tiene sentido

Después, ajusta el sistema para que:
- use el modelo de forma coherente
- no contradiga decisiones anteriores
- no pierda información

---

## Muy importante

Si algo no encaja a la primera:
- el problema **no es el modelo**
- el problema **es cómo se está usando**

Eliminar propiedades o ignorarlas **no es una solución válida**.

Este ejercicio simula una realidad muy común:
> El modelo cambió… y el sistema creció alrededor de él.

Ahora te toca **hacerlo consistente**.

---

## Entrega

Cuando termines:

1. El proyecto debe arrancar correctamente
2. El comportamiento debe ser razonable y coherente
3. El modelo debe tener un significado claro
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué crees que representa ahora el modelo
- explica qué decisiones tuviste que asumir

---

## Aviso final

En este punto:
- compilar ya no es suficiente
- responder 200 ya no es suficiente

A partir de ahora, “funcionar” significa:
> *Hacer lo correcto con los datos que existen.*
