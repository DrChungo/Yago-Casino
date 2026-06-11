# M09 – Un merge antiguo mal resuelto

El sistema ha pasado por muchos cambios.
El código actual no parece tener conflictos abiertos.
Todo compila.
Todo responde.

Pero algo no termina de cuadrar.

El problema **no es reciente**.
El problema viene de **una integración pasada que se resolvió “rápido”**.

---

## Contexto

En algún momento anterior:

- hubo un conflicto de merge
- alguien eligió una solución rápida
- el código “pasó”
- nadie volvió a revisarlo

Desde entonces:
- el sistema funciona
- pero ciertos comportamientos son extraños
- hay incoherencias difíciles de explicar

No hay marcas de conflicto.
No hay errores evidentes.
Pero **hay consecuencias**.

---

## Tu objetivo

Identificar los efectos de una mala resolución de merge en el pasado
y corregirlos **sin eliminar funcionalidades**, cumpliendo:

- ✅ El proyecto compila
- ✅ `/health` funciona
- ✅ `/status` funciona
- ✅ Se respeta la evolución del sistema
- ✅ Se corrigen comportamientos incoherentes
- ✅ No se reescribe la historia compartida

Este ejercicio no va de arreglar un conflicto.
Va de **arreglar una mala decisión histórica**.

---

## Reglas importantes

- ❌ No borres código porque “sobra”
- ❌ No rehagas el pasado con resets destructivos
- ❌ No resuelvas volviendo al baseline
- ❌ No asumas que el merge se hizo “bien”

- ✅ Asume que el merge fue apresurado
- ✅ Analiza qué se perdió o se mezcló mal
- ✅ Corrige el sistema actual, no el pasado

---

## Señales típicas de un mal merge

Puede que encuentres cosas como:

- Lógica duplicada con diferencias sutiles
- Comportamientos que solo funcionan en ciertos casos
- Valores mezclados de dos versiones incompatibles
- Código que parece correcto… pero incompleto
- Decisiones contradictorias coexistiendo

Nada de esto genera errores claros.
Pero **el sistema ya no es honesto consigo mismo**.

---

## Cómo afrontar esta misión

1. Revisa el historial de commits
2. Busca integraciones antiguas relevantes
3. Observa qué decisiones chocan entre sí
4. Decide:
   - qué comportamientos deben mantenerse
   - qué partes quedaron mal integradas
   - cómo reconciliar ambas intenciones

Después:
- adapta el código actual
- refuerza la coherencia
- deja el sistema en un estado comprensible

---

## Concepto clave

Los merges rápidos:
- suelen “funcionar”
- no rompen builds
- introducen errores silenciosos

Este ejercicio simula exactamente eso.

No estás arreglando Git.
Estás arreglando las consecuencias humanas del pasado.

---

## Entrega

Cuando termines:

1. El sistema debe comportarse de forma coherente
2. Los efectos del mal merge deben desaparecer
3. El código debe reflejar una decisión clara
4. Haz commit de tus cambios en tu rama

Después:
- completa `POSTMORTEM.md`
- explica qué decisión del merge crees que fue problemática
- explica cómo la corregiste

---

## Aviso final

Este tipo de problema:

- no aparece en el momento del merge
- no viene acompañado de errores claros
- suele estallar cuando el sistema ya es grande

Aprende a detectarlo aquí,
antes de que te toque hacerlo con clientes.
