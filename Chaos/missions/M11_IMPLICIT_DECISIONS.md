# M11 – Decisiones implícitas

El sistema funciona.
Más o menos.
Con matices.

No hay errores claros.
No hay conflictos abiertos.
No hay nada “obviamente mal”.

Y aun así, **el sistema se comporta de cierta manera sin que quede claro por qué**.

Eso suele ser señal de una cosa:
hay decisiones importantes…  
**que nunca se hicieron explícitas**.

---

## Contexto

A lo largo de su evolución, el sistema ha ido acumulando:

- comportamientos asumidos
- reglas no escritas
- suposiciones implícitas
- decisiones distribuidas en el código

Nadie las documentó.
Nadie las explicó.
Pero el sistema **se basa en ellas**.

Ahora te toca trabajar con eso.

---

## Tu objetivo

Identificar las decisiones implícitas del sistema y **hacerlas explícitas a través del código**, cumpliendo:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ El comportamiento del sistema es consistente
- ✅ Las suposiciones quedan claras en el código
- ✅ No se elimina funcionalidad existente

No se trata de cambiar decisiones.
Se trata de **hacer visibles las que ya existen**.

---

## Reglas importantes

- ❌ No elimines lógica “porque no la entiendes”
- ❌ No simplifiques para evitar decidir
- ❌ No ignores comportamientos “raros pero estables”
- ❌ No asumas que si nadie lo explicó es irrelevante

- ✅ Asume que el sistema tomó decisiones por una razón
- ✅ Descubre cuáles son esas decisiones
- ✅ Refuérzalas de forma explícita

---

## Qué son decisiones implícitas (ejemplos)

Pueden existir cosas como:

- Estados que se interpretan de una forma concreta sin estar definido
- Valores que solo tienen sentido bajo ciertas condiciones
- Comportamientos que solo funcionan si “se usa bien”
- Suposiciones no protegidas por validaciones
- Reglas que solo existen porque “así siempre ha sido”

Nada de esto es un bug por sí solo.
El problema es **que no está claro**.

---

## Cómo afrontar esta misión

1. Observa el comportamiento real del sistema
2. Identifica qué cosas da por supuestas
3. Pregúntate:
   - ¿qué decisión se está tomando aquí?
   - ¿dónde se refleja esa decisión?
   - ¿qué pasaría si no se cumple?

Después:
- refuerza esas decisiones
- protege las suposiciones
- deja claro en el código qué se espera y por qué

Esto puede implicar:
- validaciones explícitas
- nombres más claros
- flujos más definidos
- decisiones centralizadas

---

## Concepto clave

Los sistemas grandes no fallan porque tengan malas decisiones.
Fallan porque **no saben qué decisiones están tomando**.

Este ejercicio simula algo muy común:
> El sistema funciona… pero solo si sabes cómo usarlo.

Ahora te toca hacer que **el propio sistema lo diga**.

---

## Entrega

Cuando termines:

1. El sistema debe comportarse de forma predecible
2. Las decisiones implícitas deben notarse en el código
3. El comportamiento debe poder explicarse sin suposiciones externas
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué decisiones implícitas encontraste
- explica cómo las hiciste visibles

---

## Aviso final

Aquí no se evalúa:
- si te gustaron las decisiones
- si las habrías tomado tú

Se evalúa si supiste **detectarlas, respetarlas y hacerlas explícitas**.

Eso es mantenimiento real.