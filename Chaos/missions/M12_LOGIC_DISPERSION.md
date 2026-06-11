# M12 – Dispersión de la lógica

El sistema funciona.
Tiene decisiones claras.
El comportamiento es coherente.

Pero hay un problema silencioso:

👉 **La misma lógica aparece en varios sitios.**
👉 **Y no es exactamente igual en todos.**

Nada se rompe ahora.
Pero el sistema se está volviendo frágil.

---

## Contexto

A lo largo del tiempo, distintas personas han resuelto
problemas similares en distintos momentos.

El resultado es que ahora existen:

- reglas repetidas en varios puntos
- condiciones muy parecidas con pequeñas diferencias
- decisiones implementadas más de una vez
- lógica distribuida sin coordinación

Nadie lo hizo “mal”.
Simplemente nadie centralizó esas decisiones.

---

## Tu objetivo

Reducir la dispersión de la lógica del sistema,
dejando una versión **clara, consistente y mantenible**,
cumpliendo:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ La lógica del sistema es consistente
- ✅ Las reglas existen en un único sitio lógico
- ✅ No se elimina funcionalidad existente

No se trata de “limpiar”.
Se trata de **evitar que la misma decisión viva en varios sitios**.

---

## Reglas importantes

- ❌ No dejes duplicación “porque ahora funciona”
- ❌ No borres comportamiento para simplificar
- ❌ No copies y pegues lógica para resolver casos nuevos
- ❌ No ocultes el problema con condiciones especiales

- ✅ Identifica reglas comunes
- ✅ Decide dónde deben vivir
- ✅ Haz que todas las partes usen esa decisión

---

## Qué puede estar ocurriendo (ejemplos orientativos)

Puedes encontrar situaciones como:

- La misma condición evaluada en endpoints distintos
- Reglas similares aplicadas con pequeños matices
- Valores calculados varias veces de forma parecida
- Lógica de negocio mezclada con lógica de infraestructura
- Cambios futuros que requerirían modificar muchos sitios

Todo funciona.
Pero **cada cambio cuesta demasiado**.

---

## Cómo afrontar esta misión

1. Observa el comportamiento del sistema
2. Identifica reglas que se repiten
3. Comprueba si todas significan lo mismo
4. Decide:
   - qué lógica es realmente común
   - dónde debería vivir
   - cómo deben usarla las distintas partes

Después:
- centraliza decisiones
- reduce duplicación
- deja claro qué reglas gobiernan el sistema

El objetivo no es hacer el código “bonito”.
Es hacerlo **robusto frente a cambios**.

---

## Concepto clave

La duplicación no siempre rompe cosas hoy.
Rompe cosas mañana.

Este ejercicio simula una realidad muy común:
> “Cada vez que tocamos esto, se rompe otra cosa.”

Ahora te toca reducir ese riesgo.

---

## Entrega

Cuando termines:

1. El sistema debe comportarse igual que antes
2. La lógica común debe estar claramente localizada
3. El código debe ser más fácil de evolucionar
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué lógica estaba dispersa
- explica cómo la unificaste

---

## Aviso final

Un sistema con lógica dispersa:
- es difícil de cambiar
- es fácil de romper
- castiga a quien llega nuevo

Aprende a detectar este problema aquí,
antes de sufrirlo durante años.