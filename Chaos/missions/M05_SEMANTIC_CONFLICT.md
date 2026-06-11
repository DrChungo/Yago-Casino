# M05 – Conflicto semántico

El proyecto compila.
Los endpoints responden.
No hay errores visibles.

Y aun así, **el sistema no tiene un significado claro**.

Esta misión no va de arreglar errores técnicos.
Va de enfrentarte a una situación mucho más real y peligrosa:
un sistema que hace cosas… pero **no queda claro qué pretende**.

---

## Contexto

Tras múltiples cambios:

- El modelo tiene sentido por sí solo
- Los endpoints parecen correctos individualmente
- El sistema responde sin fallos

Pero cuando miras el conjunto:
- distintas partes expresan ideas distintas
- hay comportamientos que se contradicen
- el significado global no está claro

Nadie lo hizo “mal”.
Simplemente nadie lo diseñó todo a la vez.

---

## Tu objetivo

Resolver los conflictos de significado del sistema.

Al finalizar esta misión:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ El sistema expresa **una única idea coherente**
- ✅ No se han eliminado funcionalidades
- ✅ No se ha vuelto atrás en la evolución

El objetivo no es perfección.
El objetivo es que **el sistema se entienda**.

---

## Reglas importantes

- ❌ No elimines código “porque estorba”
- ❌ No devuelvas siempre valores por defecto para “quedar bien”
- ❌ No hagas que todo sea opcional para evitar decidir
- ❌ No preguntes qué quería hacer otra persona

- ✅ Asume que todo lo existente tenía un motivo
- ✅ Decide cuál es el significado correcto ahora
- ✅ Haz explícitas las decisiones en el código

---

## Ejemplos de conflictos semánticos (orientativos)

Pueden ocurrir situaciones como:

- El sistema dice estar en un estado, pero los datos indican otro
- Dos endpoints representan el mismo concepto de forma distinta
- El nombre de una propiedad no encaja con su uso real
- Los valores devueltos son técnicamente válidos pero conceptualmente confusos
- El sistema “funciona”, pero nadie sabría explicar qué representa su estado

No todos son errores.
Pero **no todos pueden ser verdad a la vez**.

---

## Cómo afrontar esta misión

1. Mira el sistema como un todo
2. Olvídate momentáneamente del código
3. Pregúntate:
   - ¿Qué representa realmente este sistema?
   - ¿Qué información es clave?
   - ¿Qué decisiones parecen incompatibles?

Después:
- adapta el comportamiento
- alinea los significados
- haz que todas las partes cuenten la misma historia

Tomar una decisión es parte del trabajo.

---

## Concepto clave

Un sistema incoherente no es uno que falla.
Es uno que **confunde**.

Y los sistemas que confunden son los más difíciles de mantener.

Este ejercicio simula algo muy común:
> El sistema creció… pero nadie aclaró qué debía significar al final.

Ahora te toca a ti.

---

## Entrega

Cuando termines:

1. El sistema debe comportarse de forma coherente
2. El significado global debe poder explicarse
3. Las decisiones deben reflejarse en el código
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué conflicto de significado detectaste
- explica qué decisión tomaste y por qué

---

## Aviso final

Aquí no hay respuestas “correctas” absolutas.
Hay decisiones **mejor o peor razonadas**.

A partir de este punto,
no decidir también es una decisión.
``