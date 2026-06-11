# M01 – Cambios individuales

Hasta ahora, el proyecto funcionaba correctamente.

A partir de este momento, **va a empezar a cambiar**.

Vas a trabajar **tú solo**, en tu propia rama.
No sabes qué harán los demás.
Ellos no saben qué harás tú.

Esto es intencional.

---

## Contexto

El proyecto funciona, pero **no está terminado**.
Hay cosas que podrían mejorarse, ampliarse o ajustarse.

Nadie te ha dado un diseño global.
Nadie te ha dicho qué cambiar.
Tienes libertad de acción.

Trabaja como si estuvieras en un equipo real y te hubieran dicho:

> “Haz alguna mejora razonable tú solo.”

---

## Tu objetivo

Introducir **uno o varios cambios razonables** en el sistema.

Cambios que:
- tengan sentido técnico
- parezcan una mejora legítima
- no busquen romper nada a propósito
- tú consideres correctos

Al final:
- ✅ El proyecto debe compilar
- ✅ `/health` debe funcionar
- ✅ `/status` debe funcionar
- ✅ Tu rama debe parecer válida para integrar

---

## Reglas MUY importantes

- ✅ Trabaja **solo en tu rama**
- ❌ No hables con nadie
- ❌ No mires ramas ajenas
- ❌ No coordines qué toca cada uno
- ❌ No te preocupes por si encaja con lo que harán otros

Asume que:
- Lo que tú hagas es correcto
- Los demás se adaptarán después

---

## Qué tipo de cambios puedes hacer  
*(elige uno o varios)*

No es una lista cerrada, solo ejemplos:

- Añadir un campo nuevo a `StatusResponse`
- Cambiar valores por defecto
- Modificar el contenido de `/status`
- Añadir alguna lógica adicional en `/health`
- Ajustar nombres, formatos o estructura
- Tocar configuración si lo consideras necesario

No intentes anticiparte a los demás.
Actúa como si el sistema fuera solo tuyo.

---

## Qué NO debes hacer

- ❌ No introduzcas errores a propósito
- ❌ No comentes “esto lo arreglo luego”
- ❌ No dejes el sistema medio roto
- ❌ No hagas commits gigantes sin sentido
- ❌ No borres funcionalidad existente

Tu cambio debe ser **defendible técnicamente**.

---

## Entrega

Cuando termines:

1. Asegúrate de que el proyecto arranca
2. Haz commit de tu trabajo en **tu rama**
3. El commit debe reflejar tu intención

No hagas merge.
No intentes integrar nada.

---

## Importante

Este ejercicio **NO va de coordinarse**.
Va de algo mucho más real:

> Personas haciendo cambios correctos  
> sin saber qué cambiarán los demás.

Los problemas no aparecen ahora.
Aparecen **cuando alguien más tenga que integrar lo que hiciste**.

Eso vendrá después.